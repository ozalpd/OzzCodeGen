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

    public bool GenerateAnyCommand => GenModeCreateCommand > FileGenerationMode.DoNotGenerate
                                   || GenModeEditCommand > FileGenerationMode.DoNotGenerate
                                   || GenModeDeleteCommand > FileGenerationMode.DoNotGenerate;

    public bool GenerateAnyView => GenModeCreateView > FileGenerationMode.DoNotGenerate
                                   || GenModeEditView > FileGenerationMode.DoNotGenerate;

    public bool GenerateAnyViewModel => GenModeCreateVM > FileGenerationMode.DoNotGenerate
                                   || GenModeEditVM > FileGenerationMode.DoNotGenerate
                                   || GenModeCollectionVM > FileGenerationMode.DoNotGenerate;
    public FileGenerationMode GenModeCreateCommand
    {
        get
        {
            if (_genModeCreateCommand == null)
                _genModeCreateCommand = EntityDefinition.Abstract
                                       ? FileGenerationMode.DoNotGenerate
                                       : FileGenerationMode.GenerateIfNotExists;

            return _genModeCreateCommand.Value;
        }
        set
        {
            if (_genModeCreateCommand == value) return;
            _genModeCreateCommand = value;
            RaisePropertyChanged(nameof(GenModeCreateCommand));
        }
    }
    private FileGenerationMode? _genModeCreateCommand;

    public FileGenerationMode GenModeEditCommand
    {
        get
        {
            if (_genModeEditCommand == null)
                _genModeEditCommand = EntityDefinition.Abstract
                                     ? FileGenerationMode.DoNotGenerate
                                     : FileGenerationMode.GenerateIfNotExists;

            return _genModeEditCommand.Value;
        }
        set
        {
            if (_genModeEditCommand == value) return;
            _genModeEditCommand = value;
            RaisePropertyChanged(nameof(GenModeEditCommand));
        }
    }
    private FileGenerationMode? _genModeEditCommand;

    public FileGenerationMode GenModeDeleteCommand
    {
        get
        {
            if (_genModeDeleteCommand == null)
                _genModeDeleteCommand = EntityDefinition.Abstract
                                       ? FileGenerationMode.DoNotGenerate
                                       : FileGenerationMode.GenerateIfNotExists;
            return _genModeDeleteCommand.Value;
        }
        set
        {
            if (_genModeDeleteCommand == value) return;
            _genModeDeleteCommand = value;
            RaisePropertyChanged(nameof(GenModeDeleteCommand));
        }
    }
    private FileGenerationMode? _genModeDeleteCommand;

    public FileGenerationMode GenModeCreateVM
    {
        get
        {
            if (_genModeCreateVM == null)
                _genModeCreateVM = EntityDefinition.Abstract
                                 ? FileGenerationMode.DoNotGenerate
                                 : FileGenerationMode.GenerateIfNotExists;
            return _genModeCreateVM.Value;
        }
        set
        {
            if (_genModeCreateVM == value) return;
            _genModeCreateVM = value;
            RaisePropertyChanged(nameof(GenModeCreateVM));
        }
    }
    private FileGenerationMode? _genModeCreateVM;

    public FileGenerationMode GenModeEditVM
    {
        get
        {
            if (_genModeEditVM == null)
                _genModeEditVM = EntityDefinition.Abstract
                               ? FileGenerationMode.DoNotGenerate
                               : FileGenerationMode.GenerateIfNotExists;
            return _genModeEditVM.Value;
        }
        set
        {
            if (_genModeEditVM == value) return;
            _genModeEditVM = value;
            RaisePropertyChanged(nameof(GenModeEditVM));
        }
    }
    private FileGenerationMode? _genModeEditVM;

    public FileGenerationMode GenModeCollectionVM
    {
        get
        {
            if (_genModeCollectionVM == null)
                _genModeCollectionVM = EntityDefinition.Abstract
                                     ? FileGenerationMode.DoNotGenerate
                                     : FileGenerationMode.GenerateIfNotExists;

            return _genModeCollectionVM.Value;
        }
        set
        {
            if (_genModeCollectionVM == value) return;
            _genModeCollectionVM = value;
            RaisePropertyChanged(nameof(GenModeCollectionVM));
        }
    }
    private FileGenerationMode? _genModeCollectionVM;


    public FileGenerationMode GenModeLookupService
    {
        get { return _genModeLookupService ?? FileGenerationMode.DoNotGenerate; }
        set
        {
            if (_genModeLookupService == value) return;
            _genModeLookupService = value;
            RaisePropertyChanged(nameof(GenModeLookupService));
        }
    }
    private FileGenerationMode? _genModeLookupService;

    public FileGenerationMode GenModeCreateView
    {
        get
        {
            if (_genModeCreateView == null)
                _genModeCreateView = EntityDefinition.Abstract
                                   ? FileGenerationMode.DoNotGenerate
                                   : FileGenerationMode.GenerateIfNotExists;

            return _genModeCreateView.Value;
        }
        set
        {
            if (_genModeCreateView == value) return;
            _genModeCreateView = value;
            RaisePropertyChanged(nameof(GenModeCreateView));
        }
    }
    private FileGenerationMode? _genModeCreateView;

    public FileGenerationMode GenModeEditView
    {
        get
        {
            if (_genModeEditView == null)
                _genModeEditView = EntityDefinition.Abstract
                                 ? FileGenerationMode.DoNotGenerate
                                 : FileGenerationMode.GenerateIfNotExists;

            return _genModeEditView.Value;
        }
        set
        {
            if (_genModeEditView == value) return;
            _genModeEditView = value;
            RaisePropertyChanged(nameof(GenModeEditView));
        }
    }
    private FileGenerationMode? _genModeEditView;


    [XmlIgnore]
    [JsonIgnore]
    public bool IsInCreateDialogService => GenModeCreateView > FileGenerationMode.DoNotGenerate && GenModeCreateCommand > FileGenerationMode.DoNotGenerate;

    [XmlIgnore]
    [JsonIgnore]
    public bool IsInEditDialogService => GenModeEditView > FileGenerationMode.DoNotGenerate && GenModeEditCommand > FileGenerationMode.DoNotGenerate;


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

    public string GetCommandName(MvvmTemplate templateType)
    {
        switch (templateType)
        {
            case MvvmTemplate.Create:
                return $"{Name}CreateCommand";

            case MvvmTemplate.Edit:
                return $"{Name}EditCommand";

            case MvvmTemplate.Delete:
                return $"{Name}DeleteCommand";

            case MvvmTemplate.Collection:
                return $"{Name.Pluralize()}ManageCommand";

            default:
                return $"{Name}Command";
        }
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

    public string GetViewModelName(MvvmTemplate templateType)
    {
        switch (templateType)
        {
            case MvvmTemplate.Create:
                return $"{Name}CreateVM";

            case MvvmTemplate.Edit:
                return $"{Name}EditVM";

            case MvvmTemplate.Collection:
                return $"{Name}CollectionVM";
            default:
                return $"{Name}ViewModel";
        }
    }

    public string GetViewName(MvvmTemplate templateType)
    {
        switch (templateType)
        {
            case MvvmTemplate.Create:
                // This will propbably be a dialog view, so we use the entity name followed by 'CreateView' to indicate that it's a view for creating an instance of the entity.
                return $"{Name}CreateView";

            case MvvmTemplate.Edit:
                // This will propbably be a dialog view, so we use the entity name followed by 'EditView' to indicate that it's a view for editing an instance of the entity.
                return $"{Name}EditView";

            case MvvmTemplate.Delete:
                // If we use a view for delete operations, we should mostly use a dialog view, so the entity name followed by 'DeleteView' indicates that it's a view for confirming the deletion of an instance of the entity.
                return $"{Name}DeleteView";

            case MvvmTemplate.Collection:
                // For collection views, we typically use the pluralized form of the entity name to indicate that the view is for managing multiple instances of the entity.
                // Name ends with 'Window' to indicate that it's a window view, which is common for collection views in MVVM applications.
                return $"{Name.Pluralize()}Window";

            default:
                return $"{Name}Window";
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
