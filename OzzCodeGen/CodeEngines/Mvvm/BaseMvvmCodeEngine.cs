using OzzCodeGen.Definitions;
using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Mvvm;

public abstract class BaseMvvmCodeEngine : BaseCodeEngine
{
    protected abstract BaseMvvmPropertySetting CreatePropertySetting();


    [XmlIgnore]
    [JsonIgnore]
    public string TargetCommandDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, CommandFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }

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


    [XmlIgnore]
    [JsonIgnore]
    public string TargetInfrastructureDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, InfrastructureFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }


    /// <summary>
    /// Relative folder (under <see cref="BaseCodeEngine.TargetDirectory"/>) where shared MVVM infrastructure files are generated.
    /// </summary>
    /// <remarks>
    /// This folder is intentionally platform-agnostic so the generated base/contracts can be reused by future engines
    /// (for example, MAUI) with minimal duplication.
    /// </remarks>
    public string InfrastructureFolder
    {
        get { return _infrastructureFolder ?? "Mvvm"; }
        set
        {
            if (_infrastructureFolder == value) return;
            _infrastructureFolder = value;
            RaisePropertyChanged(nameof(InfrastructureFolder));
            RaisePropertyChanged(nameof(TargetInfrastructureDirectory));
        }
    }
    private string _infrastructureFolder;

    /// <summary>
    /// Namespace for generated MVVM contract interfaces (for example <c>IViewModel</c>, <c>IAsyncCommand</c>, <c>INavigationService</c>).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>{MvvmNamespaceName}.Contracts</c>.
    /// </remarks>
    public string MvvmContractsNamespaceName
    {
        get { return _mvvmContractsNamespaceName ?? $"{MvvmNamespaceName}.Contracts"; }
        set
        {
            if (_mvvmContractsNamespaceName == value) return;
            _mvvmContractsNamespaceName = value;
            RaisePropertyChanged(nameof(MvvmContractsNamespaceName));
        }
    }
    private string _mvvmContractsNamespaceName;

    /// <summary>
    /// Namespace for generated shared MVVM infrastructure types (for example <c>ViewModelBase</c>, <c>RelayCommand</c>, <c>AsyncRelayCommand</c>).
    /// </summary>
    public string MvvmNamespaceName
    {
        get { return _mvvmNamespaceName ?? $"{Project.NamespaceName}.Mvvm"; }
        set
        {
            if (_mvvmNamespaceName == value) return;
            _mvvmNamespaceName = value;
            RaisePropertyChanged(nameof(MvvmNamespaceName));
        }
    }
    private string _mvvmNamespaceName;

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


    [XmlIgnore]
    [JsonIgnore]
    public string TargetViewDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, ViewFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }

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
        }
    }
    private string _viewModelFolder;


    [XmlIgnore]
    [JsonIgnore]
    public string TargetViewModelDirectory
    {
        get
        {
            if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
            {
                return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, ViewModelFolder));
            }
            else
            {
                return string.Empty;
            }
        }
    }

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
            RaisePropertyChanged(nameof(TargetViewModelDirectory));
        }
    }
    private string _viewNamespaceName;

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
