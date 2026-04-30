using OzzCodeGen.CodeEngines.CSharp;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Mvvm;

public abstract class BaseMvvmEntitySetting<TPropertySetting> : BaseCSharpEntitySetting<TPropertySetting>, IMvvmEntitySetting
    where TPropertySetting : BaseMvvmPropertySetting
{
    [XmlIgnore]
    [JsonIgnore]
    public virtual BaseMvvmCodeEngine CodeEngine { get; set; }

    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }

    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<BaseMvvmPropertySetting> MvvmProperties => Properties.Cast<BaseMvvmPropertySetting>();


    public bool GenerateCreateCommand
    {
        get { return _generateCreateCommand ?? true; }
        set
        {
            if (_generateCreateCommand == value) return;
            _generateCreateCommand = value;
            RaisePropertyChanged(nameof(GenerateCreateCommand));
        }
    }
    private bool? _generateCreateCommand;

    public bool GenerateCreateView
    {
        get { return _generateCreateView ?? true; }
        set
        {
            if (_generateCreateView == value) return;
            _generateCreateView = value;
            RaisePropertyChanged(nameof(GenerateCreateView));
        }
    }
    private bool? _generateCreateView;

    public bool GenerateEditView
    {
        get { return _generateEditView ?? true; }
        set
        {
            if (_generateEditView == value) return;
            _generateEditView = value;
            RaisePropertyChanged(nameof(GenerateEditView));
        }
    }
    private bool? _generateEditView;

    public bool GenerateCreateViewModel
    {
        get { return _generateCreateViewModel ?? true; }
        set
        {
            if (_generateCreateViewModel == value) return;
            _generateCreateViewModel = value;
            RaisePropertyChanged(nameof(GenerateCreateViewModel));
        }
    }
    private bool? _generateCreateViewModel;

    public bool GenerateDeleteCommand
    {
        get { return _generateDeleteCommand ?? true; }
        set
        {
            if (_generateDeleteCommand == value) return;
            _generateDeleteCommand = value;
            RaisePropertyChanged(nameof(GenerateDeleteCommand));
        }
    }
    private bool? _generateDeleteCommand;

    public bool GenerateEditCommand
    {
        get { return _generateEditCommand ?? true; }
        set
        {
            if (_generateEditCommand == value) return;
            _generateEditCommand = value;
            RaisePropertyChanged(nameof(GenerateEditCommand));
        }
    }
    private bool? _generateEditCommand;

    public bool GenerateEditViewModel
    {
        get { return _generateEditViewModel ?? true; }
        set
        {
            if (_generateEditViewModel == value) return;
            _generateEditViewModel = value;
            RaisePropertyChanged(nameof(GenerateEditViewModel));
        }
    }
    private bool? _generateEditViewModel;

    public bool GenerateLookupService
    {
        get { return _generateLookupService ?? false; }
        set
        {
            if (_generateLookupService == value) return;
            _generateLookupService = value;
            RaisePropertyChanged(nameof(GenerateLookupService));
        }
    }
    private bool? _generateLookupService;


    [XmlIgnore]
    [JsonIgnore]
    public bool IsInCreateDialogService => GenerateCreateView && GenerateCreateCommand;

    [XmlIgnore]
    [JsonIgnore]
    public bool IsInEditDialogService => GenerateEditView && GenerateEditCommand;


    public string GetCommandsNamespaceName()
    {
        if (string.IsNullOrEmpty(SubFolder))
            return CodeEngine.CommandNamespaceName;

        return $"{CodeEngine.CommandNamespaceName}.{SubFolder}";
    }

    public string GetViewsNamespaceName()
    {
        if (string.IsNullOrEmpty(SubFolder))
            return CodeEngine.ViewNamespaceName;

        return $"{CodeEngine.ViewNamespaceName}.{SubFolder}";
    }

    public string GetViewModelsNamespaceName()
    {
        if (string.IsNullOrEmpty(SubFolder))
            return CodeEngine.ViewModelNamespaceName;

        return $"{CodeEngine.ViewModelNamespaceName}.{SubFolder}";
    }

    [XmlIgnore]
    [JsonIgnore]
    public string RepositoryInstanceName
    {
        get
        {
            if (RepositoryName.StartsWith("I"))
                return RepositoryName.Substring(1).ToCamelCase();

            return RepositoryName.ToCamelCase();
        }
    }

    /// <summary>
    /// Name of the repository interface or repository class for this entity.
    /// </summary>
    public string RepositoryName
    {
        get { return _repositoryName ?? $"I{Name}Repository"; }
        set
        {
            if (_repositoryName == value) return;
            _repositoryName = value;
            RaisePropertyChanged(nameof(RepositoryName));
        }
    }
    private string _repositoryName;

    /// <summary>
    /// Gets or sets the subfolder name associated with this entity. This is used to determine where generated files for this entity should be placed within the project structure.
    /// </summary>
    /// <remarks>If not explicitly set, the subfolder name is automatically generated by pluralizing the entity's name.</remarks>
    public string SubFolder
    {
        get
        {
            if (_subFolder == null)
                _subFolder = Name.Pluralize();

            return _subFolder;
        }
        set
        {
            if (_subFolder == value) return;
            _subFolder = value;
            RaisePropertyChanged(nameof(SubFolder));
        }
    }
    private string _subFolder;


    public BaseMvvmPropertySetting FindProperty(string propertyName)
    {
        return Properties.FirstOrDefault(p => p.Name == propertyName);
    }

    public void AddProperty(BaseMvvmPropertySetting propertySetting)
    {
        Properties.Add((TPropertySetting)propertySetting);
    }

    public IEnumerable<TPropertySetting> GetDefaultValuedProperties()
    {
        return GetInheritedIncludedProperties().Where(p => !string.IsNullOrEmpty(p.DefaultValue));
    }

    public string GetLookupName(LookupServiceTemplateType templateType)
    {
        switch (templateType)
        {
            case LookupServiceTemplateType.Interface:
                return $"I{Name}LookupService";

            case LookupServiceTemplateType.DesignTimeClass:
                return $"{Name}MockLookupService";

            case LookupServiceTemplateType.RunTimeClass:
                return $"{Name}LookupService";

            default:
                return $"{Name}LookupService";
        }
    }

    public IEnumerable<TPropertySetting> GetStringProperties()
    {
        return GetInheritedIncludedProperties().Where(p => p.IsString);
    }

    public bool RemoveProperty(BaseMvvmPropertySetting propertySetting)
    {
        return Properties.Remove((TPropertySetting)propertySetting);
    }

    public void SortProperties()
    {
        Properties = Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder).ToList();
    }
}
