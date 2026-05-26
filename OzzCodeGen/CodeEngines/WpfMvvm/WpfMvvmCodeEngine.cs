using OzzCodeGen.CodeEngines.Mvvm;
using OzzCodeGen.CodeEngines.WpfMvvm.Templates;
using OzzCodeGen.CodeEngines.WpfMvvm.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

[XmlInclude(typeof(WpfMvvmEntitySetting))]
public class WpfMvvmCodeEngine : BaseMvvmCodeEngine
{
    public override string EngineId => EngineTypes.WpfMvvmCodeEngineId;

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName => "WpfMvVmCodeEngine.settings";

    public override string ProjectTypeName => "WPF MVVM Code Generator";

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public override string GetDefaultTargetFolder()
    {
        return "WpfMvvm";
    }

    public override List<string> GetTemplateList()
    {
        SelectedTemplate = wpfAllTemplates;
        return new List<string>
        {
            wpfCommandsTemplate,
            wpfViewModelsTemplate,
            wpfViewsTemplate,
            wpfViewAndVmTemplates,
            wpfAllTemplates
        };
    }
    private const string wpfCommandsTemplate = "WPF Commands";
    private const string wpfViewModelsTemplate = "WPF ViewModels";
    private const string wpfViewsTemplate = "WPF Views";
    private const string wpfViewAndVmTemplates = "WPF Views + ViewModels";
    private const string wpfAllTemplates = "WPF Views + ViewModels + Commands";

    protected override BaseEntitySetting CreateEntitySetting()
    {
        return new WpfMvvmEntitySetting();
    }

    protected override BaseMvvmPropertySetting CreatePropertySetting()
    {
        return new WpfMvvmPropertySetting();
    }

    protected override void OnSearchStringChanged()
    {
        RaisePropertyChanged(nameof(Entities));
    }

    [XmlIgnore]
    [JsonIgnore]
    public List<WpfMvvmEntitySetting> Entities
    {
        get
        {
            if (Project == null || string.IsNullOrEmpty(Project.SearchString))
            {
                return _entities;
            }

            return _entities
                .Where(e => e.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)
                    || e.Properties.Any(p => p.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }
        set
        {
            if (_entities == value) return;
            _entities = value;
            RaisePropertyChanged(nameof(Entities));
        }
    }
    private List<WpfMvvmEntitySetting> _entities;

    protected override void OnEntitySettingsChanged()
    {
        Entities = EntitySettings?.OfType<WpfMvvmEntitySetting>().ToList() ?? new List<WpfMvvmEntitySetting>();
    }

    private bool RenderEntity(WpfMvvmEntitySetting entitySetting, bool renderAll)
    {
        if (entitySetting == null)
            return false;

        bool allWritten = true;
        renderAll = renderAll || SelectedTemplate == wpfAllTemplates;

        if (renderAll || SelectedTemplate == wpfViewModelsTemplate || SelectedTemplate == wpfViewAndVmTemplates)
        {
            allWritten &= RenderBaseViewModels();
            allWritten &= RenderViewModels(entitySetting);
        }

        if (renderAll || SelectedTemplate == wpfViewsTemplate || SelectedTemplate == wpfViewAndVmTemplates)
        {
            allWritten &= RenderViews(entitySetting);
        }

        if (renderAll || SelectedTemplate == wpfCommandsTemplate)
        {
            allWritten &= RenderCommands(entitySetting);
        }

        return allWritten;
    }

    private bool RenderBaseViewModels()
    {
        bool allWritten = true;
        allWritten &= RenderBaseVM(new WpfBaseVmTemplate(this, BaseVM.BaseViewModel));
        allWritten &= RenderBaseVM(new WpfBaseVmTemplate(this, BaseVM.DataErrorInfoVM));
        allWritten &= RenderBaseVM(new WpfBaseVmTemplate(this, BaseVM.CreateEditVM));
        allWritten &= RenderBaseVM(new WpfBaseVmTemplate(this, BaseVM.CollectionVM));

        return allWritten;
    }

    private bool RenderBaseVM(WpfBaseVmTemplate baseVmTemplate)
    {
        bool hasInfra = !string.IsNullOrEmpty(InfrastructureFolder);
        string targetInfraDir = hasInfra
                              ? TargetInfrastructureDirectory
                              : TargetDirectory;
        var baseVmFileName = Path.Combine(targetInfraDir, ViewModelFolder, baseVmTemplate.GetDefaultFileName());
        return baseVmTemplate.WriteToFile(baseVmFileName, OverwriteExisting);
    }

    private bool RenderCommands(WpfMvvmEntitySetting entitySetting)
    {
        if (entitySetting == null || entitySetting.GenerateAnyCommand == false)
            return true;

        bool allWritten = RenderDialogSvcTemplate();

        bool hasInfra = !string.IsNullOrEmpty(InfrastructureFolder);
        string targetInfraDir = hasInfra
                              ? TargetInfrastructureDirectory
                              : TargetDirectory;
        var baseCmdTemplate = new WpfBaseCommandTemplate(this);
        allWritten &= RenderTemplate(baseCmdTemplate, targetInfraDir, CommandFolder);

        WpfCommandTemplate template = null;
        if (entitySetting.GenModeCreateCommand > FileGenerationMode.SkipManuallyEdited)
        {
            template = new WpfCommandTemplate(entitySetting, MvvmTemplate.Create);
            allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
        }

        if (entitySetting.GenModeEditCommand > FileGenerationMode.SkipManuallyEdited)
        {
            template = new WpfCommandTemplate(entitySetting, MvvmTemplate.Edit);
            allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
        }

        if (entitySetting.GenModeDeleteCommand > FileGenerationMode.SkipManuallyEdited)
        {
            template = new WpfCommandTemplate(entitySetting, MvvmTemplate.Delete);
            allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
        }

        if (entitySetting.GenModeDetailCommand > FileGenerationMode.SkipManuallyEdited)
        {
            template = new WpfCommandTemplate(entitySetting, MvvmTemplate.Detail);
            allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
        }

        if (entitySetting.GenModeLoadCommand > FileGenerationMode.SkipManuallyEdited)
        {
            template = new WpfCommandTemplate(entitySetting, PageCommand.LoadCommand);
            allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);

            if (entitySetting.GenerateGetPaged)
            {
                template = new WpfCommandTemplate(entitySetting, PageCommand.PrevPageCommand);
                allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
                template = new WpfCommandTemplate(entitySetting, PageCommand.NextPageCommand);
                allWritten &= RenderTemplate(template, TargetDirectory, CommandFolder);
            }
        }

        return allWritten;
    }

    private bool RenderDialogSvcTemplate()
    {
        bool allWritten = true;
        var svcTmplate = new WpfDialogServcTemplate(this, isInterface: true);
        allWritten &= RenderTemplate(svcTmplate, TargetDirectory, ServicesFolder);

        svcTmplate = new WpfDialogServcTemplate(this, isInterface: false);
        allWritten &= RenderTemplate(svcTmplate, TargetDirectory, ServicesFolder);
        return allWritten;
    }

    private bool RenderTemplate(BaseCSharpWpfMvvmTemplate template, string targetDir, string subFolder)
    {
        bool overwrite = OverwriteExisting || (template.EntitySetting != null && template.EntitySetting.OverwriteExisting);
        var fileName = Path.Combine(targetDir, subFolder, template.GetDefaultFileName());
        return template.WriteToFile(fileName, overwrite);
    }

    private bool RenderViewModels(WpfMvvmEntitySetting entitySetting)
    {
        if (entitySetting.GenerateAnyViewModel == false)
            return true;

        bool allWritten = true;

        if (entitySetting.GenModeCreateVM > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new WpfViewModelTemplate(entitySetting, MvvmTemplate.Create);
            allWritten &= RenderTemplate(template, TargetDirectory, ViewModelFolder);
        }

        if (entitySetting.GenModeEditVM > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new WpfViewModelTemplate(entitySetting, MvvmTemplate.Edit);
            allWritten &= RenderTemplate(template, TargetDirectory, ViewModelFolder);
        }

        if (entitySetting.GenModeCollectionVM > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new WpfViewModelTemplate(entitySetting, MvvmTemplate.Collection);
            allWritten &= RenderTemplate(template, TargetDirectory, ViewModelFolder);

            if (entitySetting.GenerateGetPaged)
            {
                CSharpQueryParamsVmTemplate qparamTemplate = null;
                bool hasInfra = !string.IsNullOrEmpty(InfrastructureFolder);
                string targetInfraDir = hasInfra
                                      ? TargetInfrastructureDirectory
                                      : TargetDirectory;

                if (entitySetting.GenerateQueryParam)
                {
                    qparamTemplate = new CSharpQueryParamsVmTemplate(this, entitySetting);
                    allWritten &= RenderTemplate(qparamTemplate, targetInfraDir, ViewModelFolder);
                }

                // For GetPaged or the base query param class of the entitySetting's query param class.
                // We need the file even if GenerateQueryParam is false, as it can be used as a base class for the entity's query param class
                qparamTemplate = new CSharpQueryParamsVmTemplate(this);
                allWritten &= RenderTemplate(qparamTemplate, targetInfraDir, ViewModelFolder);
            }
        }

        if (entitySetting.GenModeLookupService > FileGenerationMode.SkipManuallyEdited)
        {
            bool hasInfra = !string.IsNullOrEmpty(InfrastructureFolder);
            string targetInfraDir = hasInfra & PutLookupInInfra
                                  ? TargetInfrastructureDirectory
                                  : TargetDirectory;
            var template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.Interface);
            allWritten &= RenderTemplate(template, targetInfraDir, LookupFolder);

            template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.DesignTimeClass);
            allWritten &= RenderTemplate(template, targetInfraDir, DesignTimeFolder);

            template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.RunTimeClass);
            allWritten &= RenderTemplate(template, targetInfraDir, LookupFolder);
        }

        return allWritten;
    }

    private bool RenderViews(WpfMvvmEntitySetting entitySetting)
    {
        if (entitySetting.GenerateAnyView == false)
            return true;

        bool allWritten = RenderDialogSvcTemplate();

        var stylesTmplate = new XamlStylesTemplate();
        var fileName = Path.Combine(TargetDirectory, XamlResourcesFolder, stylesTmplate.GetDefaultFileName());
        allWritten &= stylesTmplate.WriteToFile(fileName, OverwriteExisting);

        if (entitySetting.GenModeCreateView > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new CSharpViewTemplate(entitySetting, MvvmTemplate.Create);
            allWritten &= RenderViewTemplate(template);
        }

        if (entitySetting.GenModeEditView > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new CSharpViewTemplate(entitySetting, MvvmTemplate.Edit);
            allWritten &= RenderViewTemplate(template);
        }

        if (entitySetting.GenModeDetailView > FileGenerationMode.SkipManuallyEdited)
        {
            var template = new CSharpViewTemplate(entitySetting, MvvmTemplate.Detail);
            allWritten &= RenderViewTemplate(template);
        }

        return allWritten;
    }

    public bool RenderViewTemplate(CSharpViewTemplate template)
    {
        bool allWritten = true;
        allWritten &= RenderTemplate(template, TargetDirectory, ViewFolder);
        var xamlTemplate = template.XamlViewTemplate; //new XamlViewTemplate(entitySetting, MvvmTemplate.Create);
        string fileName = Path.Combine(TargetDirectory, ViewFolder, xamlTemplate.GetDefaultFileName());
        allWritten &= xamlTemplate.WriteToFile(fileName, OverwriteExisting);

        return allWritten;
    }

    private bool RenderSelectedOrAll(bool renderAll)
    {
        if (!string.IsNullOrEmpty(Project?.SearchString))
        {
            Project.SearchString = string.Empty;
        }

        bool allWritten = true;
        //  allWritten &= RenderInfrastructure();

        if (RenderAllEntities)
        {
            foreach (var setting in EntitySettings.OfType<WpfMvvmEntitySetting>().Where(e => !e.Exclude))
            {
                allWritten &= RenderEntity(setting, renderAll);
            }

            return allWritten;
        }

        return RenderEntity(CurrentEntitySetting as WpfMvvmEntitySetting, renderAll) && allWritten;
    }

    public override bool RenderSelectedTemplate()
    {
        return RenderSelectedOrAll(false);
    }

    public override bool RenderAllTemplates()
    {
        return RenderSelectedOrAll(true);
    }

    protected override UserControl GetUiControl()
    {
        if (_engineUI == null)
        {
            _engineUI = new WpfMvvmEngineUI
            {
                CodeEngine = this
            };
        }

        return _engineUI;
    }
    private WpfMvvmEngineUI _engineUI;

    public static WpfMvvmCodeEngine OpenFile(string fileName)
    {
        var instance = GetInstanceFromFile(fileName, typeof(WpfMvvmCodeEngine)) as WpfMvvmCodeEngine;
        foreach (var setting in instance.EntitySettings.OfType<WpfMvvmEntitySetting>())
        {
            foreach (var prop in setting.Properties)
            {
                prop.IsLoadingFromFile = false;
            }
        }

        return instance;
    }
}
