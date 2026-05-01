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

    /// <summary>
    /// Gets or sets the namespace of the command ViewModel associated with this entity.
    /// </summary>
    public string CommandVmNamespace
    {
        get { return _commandVmNamespace ?? string.Empty; }
        set
        {
            if (_commandVmNamespace == value) return;
            _commandVmNamespace = value;
            RaisePropertyChanged(nameof(CommandVmNamespace));
        }
    }
    private string _commandVmNamespace;

    /// <summary>
    /// ViewModel type name to be used in generated command classes for this entity. Which ViewModel type must have
    /// selected item property for the entity and save, load, and delete methods for the command classes to call.
    /// </summary>
    /// <remarks>If the entity is abstract, this property returns an empty string. Otherwise, it returns a
    /// namespace in the format "I{Name}VM" by default, where {Name} is the entity's name.</remarks>
    public string CommandVmTypeName
    {
        get
        {
            if (_commandVmTypeName == null)
                _commandVmTypeName = EntityDefinition.Abstract
                                   ? string.Empty
                                   : $"I{Name.Pluralize()}VM";

            return _commandVmTypeName;
        }
        set
        {
            if (_commandVmTypeName == value) return;
            _commandVmTypeName = value;
            RaisePropertyChanged(nameof(CommandVmTypeName));
        }
    }
    private string _commandVmTypeName;

    public bool GenerateCreateCommand
    {
        get { return _generateCreateCommand ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateCreateCommand == value) return;
            _generateCreateCommand = value;
            RaisePropertyChanged(nameof(GenerateCreateCommand));
        }
    }
    private bool? _generateCreateCommand;

    public bool GenerateEditCommand
    {
        get { return _generateEditCommand ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateEditCommand == value) return;
            _generateEditCommand = value;
            RaisePropertyChanged(nameof(GenerateEditCommand));
        }
    }
    private bool? _generateEditCommand;

    public bool GenerateDeleteCommand
    {
        get { return _generateDeleteCommand ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateDeleteCommand == value) return;
            _generateDeleteCommand = value;
            RaisePropertyChanged(nameof(GenerateDeleteCommand));
        }
    }
    private bool? _generateDeleteCommand;

    public bool GenerateCreateVM
    {
        get { return _generateCreateVM ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateCreateVM == value) return;
            _generateCreateVM = value;
            RaisePropertyChanged(nameof(GenerateCreateVM));
        }
    }
    private bool? _generateCreateVM;

    public bool GenerateEditVM
    {
        get { return _generateEditVM ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateEditVM == value) return;
            _generateEditVM = value;
            RaisePropertyChanged(nameof(GenerateEditVM));
        }
    }
    private bool? _generateEditVM;

    public bool GenerateCollectionVM
    {
        get { return _generateCollectionVM ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateCollectionVM == value) return;
            _generateCollectionVM = value;
            RaisePropertyChanged(nameof(GenerateCollectionVM));
        }
    }
    private bool? _generateCollectionVM;

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

    public bool GenerateCreateView
    {
        get { return _generateCreateView ?? !EntityDefinition.Abstract; }
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
        get { return _generateEditView ?? !EntityDefinition.Abstract; }
        set
        {
            if (_generateEditView == value) return;
            _generateEditView = value;
            RaisePropertyChanged(nameof(GenerateEditView));
        }
    }
    private bool? _generateEditView;


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
        get
        {
            if (_repositoryName == null)
                _repositoryName = EntityDefinition.Abstract
                                ? string.Empty
                                : $"I{Name}Repository";

            return _repositoryName;
        }
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
                _subFolder = EntityDefinition.Abstract
                           ? string.Empty
                           : Name.Pluralize();

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

    public string GetLookupName(LookupTemplate templateType)
    {
        switch (templateType)
        {
            case LookupTemplate.Interface:
                return $"I{Name}LookupService";

            case LookupTemplate.DesignTimeClass:
                return $"{Name}MockLookupService";

            case LookupTemplate.RunTimeClass:
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
