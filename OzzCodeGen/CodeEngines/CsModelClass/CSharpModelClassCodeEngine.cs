using OzzCodeGen.CodeEngines.CsModelClass.Templates;
using OzzCodeGen.CodeEngines.CsModelClass.UI;
using OzzCodeGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsModelClass;

[XmlInclude(typeof(ModelClassEntitySetting))]
public class CSharpModelClassCodeEngine : BaseModelClassCodeEngine
{
    public override string EngineId { get { return EngineTypes.CsModelClassCodeEngineId; } }

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName { get { return "CsModelClassCodeEngine.settings"; } }

    public override string ProjectTypeName { get { return "C# Model Class Generator"; } }

    public readonly string EnumExtensionName = "EnumExtension";

    public readonly string ExtensionsFolderName = "Extensions";

    public string ExtensionsNamespaceName => $"{Project.NamespaceName}.Extensions";


    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public override string GetDefaultTargetFolder()
    {
        return "Models";
    }

    public override List<string> GetTemplateList()
    {
        return new List<string> { modelClass };
    }
    private const string modelClass = "Model Class";

    protected override BaseEntitySetting CreateEntitySetting()
    {
        return new ModelClassEntitySetting();
    }

    protected override BaseModelClassPropertySetting CreatePropertySetting()
    {
        return new ModelPropertySetting();
    }

    [XmlIgnore]
    [JsonIgnore]
    public List<ModelClassEntitySetting> Entities
    {
        get
        {
            if (Project == null || string.IsNullOrEmpty(Project.SearchString))
            {
                return _entities;
            }
            else
            {
                var result = _entities
                    .Where(e => e.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                        e.Properties.Where(p => p.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)).Any());
                return result.ToList();
            }
        }
        set
        {
            if (_entities == value) return;
            _entities = value;
            RaisePropertyChanged(nameof(Entities));
        }
    }
    private List<ModelClassEntitySetting> _entities;

    public bool GenAnnotations
    {
        get { return _genAnnotations; }
        set
        {
            _genAnnotations = value;
            RaisePropertyChanged(nameof(GenAnnotations));
        }
    }
    private bool _genAnnotations;

    /// <summary>
    /// Generates a validator class for all the entities when enabled.
    /// </summary>
    public bool GenerateValidator
    {
        set
        {
            _generateValidator = value;
            RaisePropertyChanged(nameof(GenerateValidator));
        }
        get
        {
            return _generateValidator;
        }
    }
    private bool _generateValidator;


    public bool GenerateXmlDoc
    {
        get { return _generateXmlDoc; }
        set
        {
            _generateXmlDoc = value;
            RaisePropertyChanged(nameof(GenerateXmlDoc));
        }
    }
    private bool _generateXmlDoc;

    [XmlIgnore]
    [JsonIgnore]
    public string TargetQueryParamDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, QueryParamFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }


    [XmlIgnore]
    [JsonIgnore]
    public string TargetValidatorDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, ValidatorFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Target folder for QueryParameters class which is relative to the solution directory.
    /// </summary>
    public string QueryParamFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_queryParamFolder))
                _queryParamFolder = "Helpers";
            return _queryParamFolder;
        }
        set
        {
            _queryParamFolder = value;
            RaisePropertyChanged(nameof(QueryParamFolder));
            RaisePropertyChanged(nameof(TargetQueryParamDirectory));
        }
    }
    private string _queryParamFolder;

    public string QueryParamNamespaceName
    {
        get
        {
            if (string.IsNullOrEmpty(_queryParamNamespaceName))
                _queryParamNamespaceName = $"{Project.NamespaceName}.Helpers";
            return _queryParamNamespaceName;
        }
        set
        {
            _queryParamNamespaceName = value;
            RaisePropertyChanged(nameof(QueryParamNamespaceName));
        }
    }
    private string _queryParamNamespaceName;

    /// <summary>
    /// Target folder for validator class which is relative to the solution directory.
    /// </summary>
    public string ValidatorFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_validatorFolder))
                _validatorFolder = "Validation";
            return _validatorFolder;
        }
        set
        {
            _validatorFolder = value;
            RaisePropertyChanged(nameof(ValidatorFolder));
            RaisePropertyChanged(nameof(TargetValidatorDirectory));
        }
    }
    private string _validatorFolder;

    public readonly string QueryParamClassName = "QueryParameters";
    public readonly string ValidatorClassName = "ModelValidator";

    public string ValidatorNamespaceName
    {
        get
        {
            if (string.IsNullOrEmpty(_validatorNamespaceName))
                _validatorNamespaceName = $"{Project.NamespaceName}.Validation";
            return _validatorNamespaceName;
        }
        set
        {
            _validatorNamespaceName = value;
            RaisePropertyChanged(nameof(ValidatorNamespaceName));
        }
    }
    private string _validatorNamespaceName;


    protected override void OnEntitySettingsChanged()
    {
        var entities = new List<ModelClassEntitySetting>();
        if (EntitySettings != null)
        {
            foreach (ModelClassEntitySetting item in EntitySettings)
            {
                entities.Add(item);
            }
        }
        Entities = entities;
    }


    /// <summary>
    /// Reads a project settings file and creates a ProjectSettings instance
    /// </summary>
    /// <param name="fileName">An XML file's path that contains project settings</param>
    /// <returns></returns>
    public static CSharpModelClassCodeEngine OpenFile(string fileName)
    {
        CSharpModelClassCodeEngine instance = GetInstanceFromFile(
           fileName,
           typeof(CSharpModelClassCodeEngine)) as CSharpModelClassCodeEngine;

        foreach (var setting in instance.EntitySettings.OfType<ModelClassEntitySetting>())
        {
            foreach (var prop in setting.Properties)
            {
                prop.IsLoadingFromFile = false;
            }
        }
        return instance;
    }

    protected bool RenderTemplate(ModelClassEntitySetting entitySettings)
    {
        if (entitySettings == null)
            return false;

        var template = new CSharpModelClassTemplate(entitySettings);
        var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
        bool allWritten = template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting);

        if (GenerateForDTO)
        {
            var dtoTemplate = new CSharpModelClassTemplate(entitySettings, true);
            fileName = Path.Combine(TargetDirectory, dtoTemplate.GetDefaultFileName());
            allWritten = allWritten && dtoTemplate.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting);
        }

        if (entitySettings.GenerateQueryParam)
        {
            var queryParamTemplate = new QueryParametersTemplate(this, entitySettings);
            fileName = Path.Combine(TargetQueryParamDirectory, queryParamTemplate.GetDefaultFileName());
            allWritten = queryParamTemplate.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting) && allWritten;
        }

        return allWritten;
    }

    public override bool RenderSelectedTemplate()
    {
        if (!string.IsNullOrEmpty(Project.SearchString))
        {
            Project.SearchString = string.Empty;
        }

        var entitySet = EntitySettings
                            .OfType<ModelClassEntitySetting>()
                            .ToList();
        bool allWritten = true;
        var entity = GetSelectedEntity();
        if (RenderAllEntities)
        {
            foreach (var setting in entitySet.Where(e => e.Exclude == false))
            {
                allWritten = RenderTemplate(setting) & allWritten;
            }
        }
        else
        {
            allWritten = RenderTemplate(entity);
        }

        if (GenerateValidator)
        {
            var validatorTemplate = new CSharpValidatorTemplate(this);
            var fileName = Path.Combine(TargetValidatorDirectory, validatorTemplate.GetDefaultFileName());
            allWritten = validatorTemplate.WriteToFile(fileName, OverwriteExisting) & allWritten;
        }

        if (entitySet.Any(e => e.GenerateQueryParam))
        {
            var queryParamTemplate = new QueryParametersTemplate(this);
            var fileName = Path.Combine(TargetQueryParamDirectory, queryParamTemplate.GetDefaultFileName());
            allWritten = queryParamTemplate.WriteToFile(fileName, OverwriteExisting) & allWritten;
        }

        return allWritten;
    }

    public override bool RenderAllTemplates()
    {
        throw new NotImplementedException("There is only one template in this code engine!");
    }

    protected override UserControl GetUiControl()
    {
        if (_engineUI == null)
        {
            _engineUI = SetNewUI();
        }
        return _engineUI;
    }
    ModelClassEngineUI _engineUI = null;

    private ModelClassEngineUI SetNewUI()
    {
        var control = new ModelClassEngineUI()
        {
            CodeEngine = this
        };
        MenuItem mnuExclude = UiTools.CreateMenuItem("Exclude", "Exclude selected property");
        MenuItem mnuSetUiHint = UiTools
            .CreateMenuItem("Set UIHint to Same", "Set UIHint to same named properties in other entities");
        MenuItem mnuClearReq = UiTools.CreateMenuItem("Clear Required Settings from Entity", "Clear all Required Settings in this entity");
        MenuItem mnuClearAllReq = UiTools.CreateMenuItem("Clear Required Settings from All Entities", "Clear all Required Settings in all entities");

        mnuExclude.Click += ExcludeProperty;
        mnuClearReq.Click += ClearReq;
        mnuClearAllReq.Click += ClearAllReq;
        mnuSetUiHint.Click += SetUIHintToOthers;

        control.grdEntitySettings.ContextMenu.Items.Add(mnuClearReq);
        control.grdEntitySettings.ContextMenu.Items.Add(mnuClearAllReq);
        control.grdPropertySettings.ContextMenu.Items.Add(mnuExclude);
        control.grdPropertySettings.ContextMenu.Items.Add(mnuSetUiHint);

        return control;
    }

    private void ClearReq(object o, RoutedEventArgs ea)
    {
        var entity = GetSelectedEntity();
        ClearReq(entity);
    }

    private void ClearAllReq(object o, RoutedEventArgs ea)
    {
        foreach (var entity in Entities)
        {
            ClearReq((ModelClassEntitySetting)entity);
        }
    }

    private static void ClearReq(ModelClassEntitySetting entity)
    {
        if (entity == null) return;
        foreach (var property in entity.Properties)
        {
            property.Required = string.Empty;
        }
    }

    private void ExcludeProperty(object o, RoutedEventArgs ea)
    {
        var property = GetSelectedProperty();
        if (property == null) return;
        property.Exclude = true;
    }

    private void SetUIHintToOthers(object o, RoutedEventArgs ea)
    {
        var selected = GetSelectedProperty();
        if (selected == null) return;

        foreach (var entity in Entities)
        {
            var property = entity.Properties.FirstOrDefault(p => p.Name.Equals(selected.Name));
            if (property != null)
            {
                property.UIHint = selected.UIHint;
            }
        }
    }

    private ModelPropertySetting GetSelectedProperty()
    {
        if (_engineUI.grdPropertySettings.SelectedItem == null) return null;
        return (ModelPropertySetting)_engineUI.grdPropertySettings.SelectedItem;
    }

    protected ModelClassEntitySetting GetSelectedEntity()
    {
        if (_engineUI.grdEntitySettings.SelectedItem == null) return null;
        return (ModelClassEntitySetting)_engineUI.grdEntitySettings.SelectedItem;
    }

    public ModelClassEntitySetting GetEntitySettingByName(string name)
    {
        return Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}
