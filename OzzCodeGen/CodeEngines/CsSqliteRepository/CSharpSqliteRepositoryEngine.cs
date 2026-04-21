using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;
using OzzCodeGen.CodeEngines.CsSqliteRepository.UI;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;
using OzzCodeGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

[XmlInclude(typeof(SqliteRepositoryEntitySetting))]
public class CSharpSqliteRepositoryEngine : BaseAppInfraCodeEngine
{
    public override string EngineId => EngineTypes.CSharpSqliteRepositoryEngineId;

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName => "CsSqliteRepositoryEngine.settings";

    public override string ProjectTypeName => "C# SQLite Repository Generator";

    public readonly string SqliteExtensionsClassName = "SqliteExtensions";

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public override string GetDefaultTargetFolder()
    {
        return "Repositories\\Sqlite";
    }

    protected override string GetDefaultNamespace()
    {
        if (Project == null)
            return string.Empty;

        return Project.NamespaceName.EndsWith(".Repositories")
            ? Project.NamespaceName
            : $"{Project.NamespaceName}.Repositories.SQLite";
    }

    public override List<string> GetTemplateList()
    {
        return new List<string> { repoClass };
    }
    private const string repoClass = "Repository Class";

    public override UserControl GetSettingsDlgUI()
    {
        return null;
    }

    protected override BaseEntitySetting CreateEntitySetting()
    {
        return new SqliteRepositoryEntitySetting();
    }

    protected SqliteRepositoryPropertySetting GetDefaultPropertySetting(BaseProperty property, SqliteRepositoryEntitySetting setting)
    {
        var defType = property.DefinitionType;
        var repositoryProperty = new SqliteRepositoryPropertySetting()
        {
            Name = property.Name,
            EntitySetting = setting,
            Exclude = defType == DefinitionType.Complex || defType == DefinitionType.Collection,
            IsLoadingFromFile = false
        };
        setting.Properties.Add(repositoryProperty);

        return repositoryProperty;
    }

    protected string GetRepositoryName(string entityName)
    {
        return $"{entityName}Repository";
    }

    [XmlIgnore]
    [JsonIgnore]
    public List<SqliteRepositoryEntitySetting> Entities
    {
        get
        {
            if (Project == null || string.IsNullOrEmpty(Project.SearchString))
                return _entities;

            var result = _entities
                .Where(e => e.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)
                    || e.Properties.Any(p => p.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)));
            return result.ToList();
        }
        set
        {
            if (_entities == value) return;
            _entities = value;
            RaisePropertyChanged(nameof(Entities));
        }
    }
    private List<SqliteRepositoryEntitySetting> _entities;

    /// <summary>
    /// Storage code engine is used to get information about the storage, such as table and column names, which can be used in the repository templates.
    /// It is not serialized because it is retrieved from the project when needed.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public SqliteScriptsEngine StorageCodeEngine
    {
        get
        {
            if (_storageCodeEngine == null && Project != null)
            {
                _storageCodeEngine = Project.GetCodeEngine(EngineTypes.SqliteScriptsId) as SqliteScriptsEngine;
            }
            return _storageCodeEngine;
        }
    }
    private SqliteScriptsEngine _storageCodeEngine;

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

    [XmlIgnore]
    [JsonIgnore]
    public string QueryParamNamespaceName
    {
        get { return ModelClassCodeEngine?.QueryParamNamespaceName ?? string.Empty; }
    }

    public string MetadataRepositoryName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_metadataName))
            {
                _metadataName = "MetadataRepository";
            }
            return _metadataName;
        }
        set
        {
            if (_metadataName == value) return;
            _metadataName = value;
            RaisePropertyChanged(nameof(MetadataRepositoryName));
        }
    }
    private string _metadataName;


    public string BaseRepositoryClassName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_baseRepositoryClassName))
                _baseRepositoryClassName = "BaseDatabaseRepository";
            return _baseRepositoryClassName;
        }
        set
        {
            if (_baseRepositoryClassName == value) return;
            _baseRepositoryClassName = value;
            RaisePropertyChanged(nameof(BaseRepositoryClassName));
        }
    }

    [XmlIgnore]
    [JsonIgnore]
    public override string TargetInfrastructureDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(InfrastructureFolder))
            {
                return $"If this is empty or whitespace, generated base/contracts will be placed in the target folder,\r\n{TargetDirectory}";
            }
            else
            {
                return Path.GetFullPath(Path.Combine(TargetDirectory, InfrastructureFolder));
            }
        }
    }
    private string _baseRepositoryClassName;


    protected override void OnEntitySettingsChanged()
    {
        var entities = new List<SqliteRepositoryEntitySetting>();
        if (EntitySettings != null)
        {
            foreach (SqliteRepositoryEntitySetting item in EntitySettings)
            {
                entities.Add(item);
            }
        }
        Entities = entities;
    }

    protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
    {
        var repositorySetting = (SqliteRepositoryEntitySetting)setting;
        repositorySetting.DataModel = Project.DataModel;
        repositorySetting.CodeEngine = this;


        List<SqliteRepositoryPropertySetting> remvProp = new List<SqliteRepositoryPropertySetting>();
        foreach (var dalProp in repositorySetting.Properties)
        {
            if (entity.Properties.FirstOrDefault(p => p.Name == dalProp.Name) == null)
            {
                remvProp.Add(dalProp);
            }
        }
        foreach (var dalProp in remvProp)
        {
            repositorySetting.Properties.Remove(dalProp);
        }

        foreach (var property in entity.Properties)
        {
            var ps = repositorySetting.Properties.FirstOrDefault(p => p.Name == property.Name);
            if (ps == null)// && property.DefinitionType != DefinitionType.Collection
            {
                ps = GetDefaultPropertySetting(property, repositorySetting);
            }
            else //if (ps != null)
            {
                ps.EntitySetting = setting;
            }
        }
        repositorySetting.Properties = new List<SqliteRepositoryPropertySetting>(
                                        repositorySetting
                                        .Properties
                                        .OrderBy(p => p.PropertyDefinition.DisplayOrder));
    }

    public static CSharpSqliteRepositoryEngine OpenFile(string fileName)
    {
        var instance = GetInstanceFromFile(fileName, typeof(CSharpSqliteRepositoryEngine)) as CSharpSqliteRepositoryEngine;
        foreach (var setting in instance.EntitySettings.OfType<SqliteRepositoryEntitySetting>())
        {
            foreach (var prop in setting.Properties)
            {
                prop.IsLoadingFromFile = false;
            }
        }
        return instance;
    }

    protected bool RenderTemplate(SqliteRepositoryEntitySetting entitySettings)
    {
        if (entitySettings == null)
            return false;

        var template = new CSharpSqliteRepositoryTemplate(this, entitySettings);
        var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
        return template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting);
    }

    protected bool RenderTemplate(BaseCSharpSqliteRepositoryTemplate template)
    {
        var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
        return template.WriteToFile(fileName, OverwriteExisting);
    }

    public override bool RenderSelectedTemplate()
    {
        if (!string.IsNullOrEmpty(Project.SearchString))
        {
            Project.SearchString = string.Empty;
        }

        var allWritten = true;
        allWritten = RenderTemplate(new CSharpSqliteExtensionsTemplate(this)) & allWritten;
        allWritten = RenderTemplate(new CSharpSqliteBaseRepositoryTemplate(this)) & allWritten;
        allWritten = RenderTemplate(new CSharpSqliteMetadataRepositoryTemplate(this)) & allWritten;

        var entity = GetSelectedEntity();
        if (RenderAllEntities)
        {
            foreach (var setting in EntitySettings.Cast<SqliteRepositoryEntitySetting>().Where(e => e.Exclude == false))
            {
                allWritten = RenderTemplate(setting) & allWritten;
            }
        }
        else
        {
            allWritten = RenderTemplate(entity) & allWritten;
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
    private SqliteRepositoryEngineUI _engineUI;

    private SqliteRepositoryEngineUI SetNewUI()
    {
        var control = new SqliteRepositoryEngineUI
        {
            CodeEngine = this,
            DataContext = this
        };

        var mnuExclude = UiTools.CreateMenuItem("Exclude", "Exclude selected property");
        mnuExclude.Click += ExcludeProperty;
        control.grdPropertySettings.ContextMenu.Items.Add(mnuExclude);

        return control;
    }

    private void ExcludeProperty(object o, RoutedEventArgs ea)
    {
        var property = GetSelectedProperty();
        if (property == null) return;
        property.Exclude = true;
    }

    private SqliteRepositoryPropertySetting GetSelectedProperty()
    {
        if (_engineUI.grdPropertySettings.SelectedItem == null) return null;
        return (SqliteRepositoryPropertySetting)_engineUI.grdPropertySettings.SelectedItem;
    }

    protected SqliteRepositoryEntitySetting GetSelectedEntity()
    {
        if (_engineUI?.grdEntitySettings.SelectedItem == null) return null;
        return (SqliteRepositoryEntitySetting)_engineUI.grdEntitySettings.SelectedItem;
    }

    protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
    {
        var entitySetting = (SqliteRepositoryEntitySetting)CreateEntitySetting();

        entitySetting.DataModel = Project.DataModel;
        entitySetting.Name = entity.Name;
        entitySetting.CodeEngine = this;

        foreach (var property in entity.Properties)
        {
            if (property.DefinitionType != DefinitionType.Collection)
            {
                _ = GetDefaultPropertySetting(property, entitySetting);
            }
        }
        return entitySetting;
    }
}
