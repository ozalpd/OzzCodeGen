using OzzCodeGen.CodeEngines.CsDbRepository;
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
public class CSharpSqliteRepositoryEngine : BaseCsDbRepositoryEngine<SqliteRepositoryPropertySetting>
{
    public CSharpSqliteRepositoryEngine()
    {
        //If SelectedTemplate is not set, CanRender is set to false and Render button is disabled.
        SelectedTemplate = repoClass;
    }

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

    protected override void OnEntitySettingsChanged()
    {
        var entities = new List<BaseCsDbRepositoryEntitySetting<SqliteRepositoryPropertySetting>>();
        if (EntitySettings != null)
        {
            foreach (SqliteRepositoryEntitySetting item in EntitySettings)
            {
                entities.Add(item);
            }
        }
        Entities = entities;
        CanRender = CurrentEntitySetting != null;
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
