using OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;
using OzzCodeGen.CodeEngines.CsSqliteRepository.UI;
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
public class CSharpSqliteRepositoryEngine : CsModelClass.BaseModelClassCodeEngine
{
    public override string EngineId => EngineTypes.CSharpSqliteRepositoryEngineId;

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName => "CSharpSqliteRepositoryEngine.settings";

    public override string ProjectTypeName => "C# SQLite Repository Generator";

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public override string GetDefaultTargetFolder()
    {
        return "SqliteRepositories";
    }

    protected override string GetDefaultNamespace()
    {
        if (Project == null)
            return string.Empty;

        return Project.NamespaceName.EndsWith(".Repositories")
            ? Project.NamespaceName
            : $"{Project.NamespaceName}.Repositories";
    }

    protected override BaseEntitySetting CreateEntitySetting()
    {
        return new SqliteRepositoryEntitySetting();
    }

    protected override CsModelClass.BaseModelClassPropertySetting CreatePropertySetting()
    {
        return new SqliteRepositoryPropertySetting();
    }

    protected override CsModelClass.BaseModelClassPropertySetting GetDefaultPropertySetting(BaseProperty property, BaseEntitySetting setting)
    {
        var repositoryProperty = (SqliteRepositoryPropertySetting)base.GetDefaultPropertySetting(property, setting);
        repositoryProperty.ColumnName = property.Name;
        repositoryProperty.Exclude = property is ComplexProperty || property is CollectionProperty;
        return repositoryProperty;
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

    public string ModelNamespaceName
    {
        get
        {
            if (string.IsNullOrEmpty(_modelNamespaceName))
            {
                _modelNamespaceName = Project == null
                    ? string.Empty
                    : (Project.NamespaceName.EndsWith(".Models") ? Project.NamespaceName : $"{Project.NamespaceName}.Models");
            }
            return _modelNamespaceName;
        }
        set
        {
            if (_modelNamespaceName == value) return;
            _modelNamespaceName = value;
            RaisePropertyChanged(nameof(ModelNamespaceName));
        }
    }
    private string _modelNamespaceName;

    public string BaseRepositoryClassName
    {
        get
        {
            if (string.IsNullOrEmpty(_baseRepositoryClassName))
                _baseRepositoryClassName = "SqliteRepositoryBase";
            return _baseRepositoryClassName;
        }
        set
        {
            if (_baseRepositoryClassName == value) return;
            _baseRepositoryClassName = value;
            RaisePropertyChanged(nameof(BaseRepositoryClassName));
        }
    }
    private string _baseRepositoryClassName;

    public bool GenerateBaseRepository
    {
        get
        {
            return _generateBaseRepository ?? true;
        }
        set
        {
            if (_generateBaseRepository == value) return;
            _generateBaseRepository = value;
            RaisePropertyChanged(nameof(GenerateBaseRepository));
        }
    }
    private bool? _generateBaseRepository;

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
        base.RefreshSetting(setting, entity, cleanRemovedItems);

        var repositorySetting = (SqliteRepositoryEntitySetting)setting;
        if (string.IsNullOrEmpty(repositorySetting.TableName))
            repositorySetting.TableName = repositorySetting.Name;

        foreach (var property in repositorySetting.Properties)
        {
            if (string.IsNullOrEmpty(property.ColumnName))
                property.ColumnName = property.Name;

            if (property.PropertyDefinition is ComplexProperty)
                property.Exclude = true;
        }
    }

    public static CSharpSqliteRepositoryEngine OpenFile(string fileName)
    {
        var instance = GetInstanceFromFile(fileName, typeof(CSharpSqliteRepositoryEngine)) as CSharpSqliteRepositoryEngine;
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

    protected bool RenderBaseTemplate()
    {
        var template = new CSharpSqliteRepositoryBaseTemplate(this);
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
        if (GenerateBaseRepository)
        {
            allWritten = RenderBaseTemplate() & allWritten;
        }

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
}
