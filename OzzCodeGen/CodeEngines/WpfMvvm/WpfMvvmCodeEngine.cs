using OzzCodeGen.CodeEngines.CsSqliteRepository;
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
        SelectedTemplate = wpfViewModelsTemplate;
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
            allWritten &= RenderViewModels(entitySetting);
            allWritten &= RenderBaseViewModels();
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
        var baseVmFileName = Path.Combine(TargetInfrastructureDirectory, ViewModelFolder, baseVmTemplate.GetDefaultFileName());
        return baseVmTemplate.WriteToFile(baseVmFileName, OverwriteExisting);
    }

    private bool RenderCommands(WpfMvvmEntitySetting entitySetting)
    {
        if (!entitySetting.GenerateEditCommand)
            return true;
        bool allWritten = true;

        var template = new WpfDialogServcTemplate(this, isInterface: true);
        allWritten &= RenderTemplate(template, TargetDirectory, ServicesFolder);
        template = new WpfDialogServcTemplate(this, isInterface: false);
        allWritten &= RenderTemplate(template, TargetDirectory, ServicesFolder);

        //TODO: Add command templates here
        //CreateEditCommandTemplate, DeleteCommandTemplate, etc.


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
        bool allWritten = true;

        if (entitySetting.GenerateCreateVM)
        {
            var template = new WpfViewModelTemplate(entitySetting, isEdit: false);
            allWritten &= RenderTemplate(template, TargetDirectory, ViewModelFolder);
        }

        if (entitySetting.GenerateEditVM)
        {
            var template = new WpfViewModelTemplate(entitySetting, isEdit: true);
            allWritten &= RenderTemplate(template, TargetDirectory, ViewModelFolder);
        }

        if (entitySetting.GenerateLookupService)
        {
            var template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.Interface);
            allWritten &= RenderTemplate(template, TargetInfrastructureDirectory, LookupFolder);
            template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.DesignTimeClass);
            allWritten &= RenderTemplate(template, TargetInfrastructureDirectory, LookupFolder);

            string targetDir = PutLookupInInfra ? TargetInfrastructureDirectory : TargetDirectory;
            template = new CSharpLookupServiceTemplate(entitySetting, LookupTemplate.RunTimeClass);
            allWritten &= RenderTemplate(template, targetDir, LookupFolder);
        }

        return allWritten;
    }

    private bool RenderViews(WpfMvvmEntitySetting entitySetting)
    {
        bool allWritten = true;

        if (entitySetting.GenerateCreateView)
        {
            //var template = new WpfMvvmViewXamlTemplate(entitySetting, isEdit: false);
            //var fileName = Path.Combine(TargetDirectory, ViewFolder, template.GetDefaultFileName());
            //allWritten &= template.WriteToFile(fileName, OverwriteExisting || entitySetting.OverwriteExisting);
        }
        if (entitySetting.GenerateEditView)
        {
            //var template = new WpfMvvmViewXamlTemplate(entitySetting, isEdit: true);
            //var fileName = Path.Combine(TargetDirectory, ViewFolder, template.GetDefaultFileName());
            //allWritten &= template.WriteToFile(fileName, OverwriteExisting || entitySetting.OverwriteExisting);
        }
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
