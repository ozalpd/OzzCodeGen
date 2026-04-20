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

    private bool RenderCommands(WpfMvvmEntitySetting entitySetting)
    {
        if (!entitySetting.GenerateCommands)
            return true;
        var template = new WpfMvvmCommandsTemplate(entitySetting);
        var fileName = Path.Combine(TargetDirectory, CommandFolder, template.GetDefaultFileName());
        return template.WriteToFile(fileName, OverwriteExisting || entitySetting.OverwriteExisting);
    }

    private bool RenderBaseViewModels()
    {
        bool allWritten = true;
        allWritten &= RenderBaseVM(new CSharpWpfBaseVmTemplate(this, BaseViewModelTypes.BaseViewModel));
        allWritten &= RenderBaseVM(new CSharpWpfBaseVmTemplate(this, BaseViewModelTypes.DataErrorInfoVM));
        allWritten &= RenderBaseVM(new CSharpWpfBaseVmTemplate(this, BaseViewModelTypes.CreateEditVM));
        allWritten &= RenderBaseVM(new CSharpWpfBaseVmTemplate(this, BaseViewModelTypes.CollectionVM));

        return allWritten;
    }

    private bool RenderBaseVM(CSharpWpfBaseVmTemplate baseVmTemplate)
    {
        string baseVmFolder = string.IsNullOrWhiteSpace(InfrastructureFolder) ? ViewModelFolder : InfrastructureFolder;
        var baseVmFileName = Path.Combine(TargetDirectory, baseVmFolder, baseVmTemplate.GetDefaultFileName());
        return baseVmTemplate.WriteToFile(baseVmFileName, OverwriteExisting);
    }

    private bool RenderViewModels(WpfMvvmEntitySetting entitySetting)
    {
        bool allWritten = true;

        if (entitySetting.GenerateCreateViewModel)
        {
            var template = new CSharpWpfViewModelTemplate(entitySetting, isEdit: false);
            var fileName = Path.Combine(TargetDirectory, ViewModelFolder, template.GetDefaultFileName());
            allWritten &= template.WriteToFile(fileName, OverwriteExisting || entitySetting.OverwriteExisting);
        }

        if (entitySetting.GenerateEditViewModel)
        {
            var template = new CSharpWpfViewModelTemplate(entitySetting, isEdit: true);
            var fileName = Path.Combine(TargetDirectory, ViewModelFolder, template.GetDefaultFileName());
            allWritten &= template.WriteToFile(fileName, OverwriteExisting || entitySetting.OverwriteExisting);
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

    /// <summary>
    /// Renders shared MVVM infrastructure output for the engine.
    /// </summary>
    /// <remarks>
    /// Infrastructure is rendered independently from per-entity files because these files are cross-cutting and reused by
    /// all generated views/viewmodels/commands. This also keeps future platform engines (for example MAUI) aligned on the
    /// same contracts and base classes.
    /// </remarks>
    private bool RenderInfrastructure()
    {
        bool allWritten = true;

        // Shared contract interfaces consumed by generated ViewModels and commands.
        var contractTemplate = new MvvmContractsTemplate(this);
        var contractFileName = Path.Combine(TargetDirectory, InfrastructureFolder, "Contracts", contractTemplate.GetDefaultFileName());
        allWritten &= contractTemplate.WriteToFile(contractFileName, OverwriteExisting);

        // Shared base MVVM implementation (ViewModelBase + RelayCommand + AsyncRelayCommand).
        var baseTemplate = new MvvmBaseClassesTemplate(this);
        var baseFileName = Path.Combine(TargetDirectory, InfrastructureFolder, baseTemplate.GetDefaultFileName());
        allWritten &= baseTemplate.WriteToFile(baseFileName, OverwriteExisting);

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
