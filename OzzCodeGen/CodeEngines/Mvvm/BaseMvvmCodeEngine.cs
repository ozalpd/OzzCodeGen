using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.Definitions;
using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Mvvm;

public abstract class BaseMvvmCodeEngine : BaseAppInfraCodeEngine
{
    protected abstract BaseMvvmPropertySetting CreatePropertySetting();

    public readonly string AbstractCreateEditViewModelName = "AbstractCreateEditVM";

    public string CommandFolder
    {
        get { return _commandFolder ?? "Commands"; }
        set
        {
            if (_commandFolder == value) return;
            _commandFolder = value;
            RaisePropertyChanged(nameof(CommandFolder));
            RaisePropertyChanged(nameof(TargetCommandDirectory));
        }
    }
    private string _commandFolder;

    public string CommandNamespaceName
    {
        get { return _commandNamespaceName ?? $"{Project.NamespaceName}.Commands"; }
        set
        {
            if (_commandNamespaceName == value) return;
            _commandNamespaceName = value;
            RaisePropertyChanged(nameof(CommandNamespaceName));
        }
    }
    private string _commandNamespaceName;


    public string RepositoryNamespaceName
    {
        get { return _repositoryNamespaceName ?? $"{Project.NamespaceName}.Repositories"; }
        set
        {
            if (_repositoryNamespaceName == value) return;
            _repositoryNamespaceName = value;
            RaisePropertyChanged(nameof(RepositoryNamespaceName));
        }
    }
    private string _repositoryNamespaceName;

    public string ServiceFolder
    {
        get { return _ServiceFolder ?? "Services"; }
        set
        {
            if (_ServiceFolder == value) return;
            _ServiceFolder = value;
            RaisePropertyChanged(nameof(ServiceFolder));
            RaisePropertyChanged(nameof(TargetServiceDirectory));
        }
    }
    private string _ServiceFolder;

    public string ServiceNamespaceName
    {
        get { return _ServiceNamespaceName ?? $"{Project.NamespaceName}.Services"; }
        set
        {
            if (_ServiceNamespaceName == value) return;
            _ServiceNamespaceName = value;
            RaisePropertyChanged(nameof(ServiceNamespaceName));
        }
    }
    private string _ServiceNamespaceName;


    [XmlIgnore]
    [JsonIgnore]
    public string TargetCommandDirectory => Path.GetFullPath(Path.Combine(TargetDirectory, CommandFolder));


    [XmlIgnore]
    [JsonIgnore]
    public override string TargetInfrastructureDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(InfrastructureFolder))
            {
                return
                    $@"If this is empty or whitespace, generated base/contracts will be placed in the related folders,
for Views: {TargetViewDirectory},
for ViewModels: {TargetViewModelDirectory},
for Commands: {TargetCommandDirectory}";
            }
            else
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, InfrastructureFolder));
            }
        }
    }


    [XmlIgnore]
    [JsonIgnore]
    public string TargetServiceDirectory => Path.GetFullPath(Path.Combine(TargetDirectory, ServiceFolder));

    [XmlIgnore]
    [JsonIgnore]
    public string TargetViewDirectory => Path.GetFullPath(Path.Combine(TargetDirectory, ViewFolder));

    [XmlIgnore]
    [JsonIgnore]
    public string TargetViewModelDirectory => Path.GetFullPath(Path.Combine(TargetDirectory, ViewModelFolder));

    public bool UseResourceFiles
    {
        get { return _useResourceFiles ?? false; }
        set
        {
            if (_useResourceFiles == value) return;
            _useResourceFiles = value;
            RaisePropertyChanged(nameof(UseResourceFiles));
        }
    }
    private bool? _useResourceFiles;

    public string ViewFolder
    {
        get { return _viewFolder ?? "Views"; }
        set
        {
            if (_viewFolder == value) return;
            _viewFolder = value;
            RaisePropertyChanged(nameof(ViewFolder));
            RaisePropertyChanged(nameof(TargetViewDirectory));
        }
    }
    private string _viewFolder;

    public string ViewModelFolder
    {
        get { return _viewModelFolder ?? "ViewModels"; }
        set
        {
            if (_viewModelFolder == value) return;
            _viewModelFolder = value;
            RaisePropertyChanged(nameof(ViewModelFolder));
            RaisePropertyChanged(nameof(TargetViewModelDirectory));
        }
    }
    private string _viewModelFolder;

    public string ViewModelNamespaceName
    {
        get { return _viewModelNamespaceName ?? $"{Project.NamespaceName}.ViewModels"; }
        set
        {
            if (_viewModelNamespaceName == value) return;
            _viewModelNamespaceName = value;
            RaisePropertyChanged(nameof(ViewModelNamespaceName));
        }
    }
    private string _viewModelNamespaceName;

    public string ViewNamespaceName
    {
        get { return _viewNamespaceName ?? $"{Project.NamespaceName}.Views"; }
        set
        {
            if (_viewNamespaceName == value) return;
            _viewNamespaceName = value;
            RaisePropertyChanged(nameof(ViewNamespaceName));
        }
    }
    private string _viewNamespaceName;



    /// <summary>
    /// Model class code engine is used to get information about the model classes, such as validator class, which can be used in the repository templates.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public CSharpModelClassCodeEngine ModelClassCodeEngine
    {
        get
        {
            if (_modelClassEngine == null && Project != null)
            {
                _modelClassEngine = Project.GetCodeEngine(EngineTypes.CsModelClassCodeEngineId) as CSharpModelClassCodeEngine;
            }
            return _modelClassEngine;
        }
    }
    private CSharpModelClassCodeEngine _modelClassEngine;


    protected virtual BaseMvvmPropertySetting GetDefaultPropertySetting(BaseProperty property, BaseEntitySetting setting)
    {
        var mvvmEntitySetting = setting as IMvvmEntitySetting;
        if (mvvmEntitySetting == null)
        {
            throw new InvalidOperationException($"{setting?.GetType().Name} must implement {nameof(IMvvmEntitySetting)}.");
        }

        var ps = CreatePropertySetting();
        ps.Name = property.Name;
        ps.EntitySetting = setting;
        mvvmEntitySetting.AddProperty(ps);

        return ps;
    }

    protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
    {
        var entitySetting = CreateEntitySetting();
        var mvvmEntitySetting = entitySetting as IMvvmEntitySetting;
        if (mvvmEntitySetting == null)
        {
            throw new InvalidOperationException($"{entitySetting?.GetType().Name} must implement {nameof(IMvvmEntitySetting)}.");
        }

        entitySetting.DataModel = Project.DataModel;
        entitySetting.Name = entity.Name;
        mvvmEntitySetting.CodeEngine = this;

        foreach (var property in entity.Properties.Where(p => p.DefinitionType != DefinitionType.Collection))
        {
            _ = GetDefaultPropertySetting(property, entitySetting);
        }

        return entitySetting;
    }

    protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
    {
        var mvvmEntitySetting = setting as IMvvmEntitySetting;
        if (mvvmEntitySetting == null)
        {
            throw new InvalidOperationException($"{setting?.GetType().Name} must implement {nameof(IMvvmEntitySetting)}.");
        }

        setting.DataModel = Project.DataModel;
        mvvmEntitySetting.CodeEngine = this;

        var remvProp = mvvmEntitySetting
            .MvvmProperties
            .Where(p => entity.Properties.FirstOrDefault(ep => ep.Name == p.Name) == null)
            .ToList();

        foreach (var item in remvProp)
        {
            mvvmEntitySetting.RemoveProperty(item);
        }

        foreach (var property in entity.Properties.Where(p => p.DefinitionType != DefinitionType.Collection))
        {
            var ps = mvvmEntitySetting.FindProperty(property.Name);
            if (ps == null)
            {
                _ = GetDefaultPropertySetting(property, setting);
            }
            else
            {
                ps.EntitySetting = setting;
            }
        }

        mvvmEntitySetting.SortProperties();
    }

    public override UserControl GetSettingsDlgUI()
    {
        return null;
    }
}
