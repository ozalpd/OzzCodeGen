using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.Metadata;
using OzzCodeGen.CodeEngines.Metadata.Templates;
using OzzCodeGen.CodeEngines.Metadata.UI;
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

namespace OzzCodeGen.CodeEngines.ModelClass;

[XmlInclude(typeof(MetadataEntitySetting))]
public abstract class ModelClassCodeEngineBase : BaseCodeEngine
{
    public override string GetDefaultTargetFolder()
    {
        return "Metadata";
    }

    protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
    {
        var setting = new MetadataEntitySetting()
        {
            DataModel = this.Project.DataModel,
            CodeEngine = this,
            Name = entity.Name
        };
        foreach (var property in entity.Properties)
        {
            if (property.DefinitionType != DefinitionType.Collection)
            {
                _ = GetDefaultPropertySetting(property, setting);
            }
        }
        return setting;
    }

    protected MetadataPropertySetting GetDefaultPropertySetting(BaseProperty property, MetadataEntitySetting setting)
    {
        var ps = new MetadataPropertySetting()
        {
            Name = property.Name,
            EntitySetting = setting
        };
        setting.Properties.Add(ps);

        return ps;
    }

    protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
    {
        var dataLayerSetting = (MetadataEntitySetting)setting;
        dataLayerSetting.DataModel = Project.DataModel;
        dataLayerSetting.CodeEngine = this;

        List<MetadataPropertySetting> remvProp = new List<MetadataPropertySetting>();
        foreach (var dalProp in dataLayerSetting.Properties)
        {
            if (entity.Properties.FirstOrDefault(p => p.Name == dalProp.Name) == null)
            {
                remvProp.Add(dalProp);
            }
        }
        foreach (var dalProp in remvProp)
        {
            dataLayerSetting.Properties.Remove(dalProp);
        }

        foreach (var property in entity.Properties)
        {
            var ps = dataLayerSetting.Properties.FirstOrDefault(p => p.Name == property.Name);
            if (ps == null)
            {
                ps = GetDefaultPropertySetting(property, dataLayerSetting);
            }
            else
            {
                ps.EntitySetting = setting;
            }
        }
        dataLayerSetting.Properties = dataLayerSetting
            .Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder)
            .ToList();
    }

    protected bool RenderTemplate(MetadataEntitySetting entitySettings)
    {
        if (entitySettings == null)
            return false;

        var template = new MetadataClassTemplate(entitySettings);
        var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
        bool allWritten = true;

        if (GenerateForDTO)
        {
            var dtoTemplate = new MetadataClassTemplate(entitySettings, true);
            var dtoFileName = Path.Combine(TargetDirectory, dtoTemplate.GetDefaultFileName());
            allWritten = dtoTemplate.WriteToFile(dtoFileName, OverwriteExisting || entitySettings.OverwriteExisting);
        }
        return template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting) & allWritten;
    }

    public override bool RenderSelectedTemplate()
    {
        if (!string.IsNullOrEmpty(Project.SearchString))
        {
            Project.SearchString = string.Empty;
        }

        if (RenderAllEntities)
        {
            bool allWritten = true;
            foreach (MetadataEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
            {
                allWritten = RenderTemplate(setting) & allWritten;
            }
            return allWritten;
        }
        else
        {
            return RenderTemplate(GetSelectedEntity());
        }
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
    MetadataEngineUI _engineUI = null;

    private MetadataEngineUI SetNewUI()
    {
        var control = new MetadataEngineUI()
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
            ClearReq(entity);
        }
    }

    private static void ClearReq(MetadataEntitySetting entity)
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

    private MetadataPropertySetting GetSelectedProperty()
    {
        if (_engineUI.grdPropertySettings.SelectedItem == null) return null;
        return (MetadataPropertySetting)_engineUI.grdPropertySettings.SelectedItem;
    }

    protected MetadataEntitySetting GetSelectedEntity()
    {
        if (_engineUI.grdEntitySettings.SelectedItem == null) return null;
        return (MetadataEntitySetting)_engineUI.grdEntitySettings.SelectedItem;
    }

    public override UserControl GetSettingsDlgUI()
    {
        return null;
    }

    protected override void OnSearchStringChanged()
    {
        RaisePropertyChanged("Entities");
    }

    [XmlIgnore]
    [JsonIgnore]
    public List<MetadataEntitySetting> Entities
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
            RaisePropertyChanged("Entities");
        }
    }
    private List<MetadataEntitySetting> _entities;

    protected override void OnEntitySettingsChanged()
    {
        var entities = new List<MetadataEntitySetting>();
        if (EntitySettings != null)
        {
            foreach (MetadataEntitySetting item in EntitySettings)
            {
                entities.Add(item);
            }
        }
        Entities = entities;
    }

    public override List<string> GetTemplateList()
    {
        return new List<string> { metadataClass };
    }
    private const string metadataClass = "Metadata Class";

    public string CustomAttribNamespace
    {
        get
        {
            if (string.IsNullOrEmpty(_customAttribNamespace))
                _customAttribNamespace = $"{NamespaceName}.Validate";
            return _customAttribNamespace;
        }
        set
        {
            _customAttribNamespace = value;
            RaisePropertyChanged("CustomAttribNamespace");
        }
    }
    private string _customAttribNamespace;

    public bool GenerateForDTO
    {
        set
        {
            _generateForDTO = value;
            RaisePropertyChanged("GenerateForDTO");
        }
        get
        {
            return _generateForDTO;
        }
    }
    private bool _generateForDTO;

    public bool GenerateXmlDoc
    {
        get { return _generateXmlDoc; }
        set
        {
            _generateXmlDoc = value;
            RaisePropertyChanged("GenerateXmlDoc");
        }
    }
    private bool _generateXmlDoc;

    [XmlIgnore]
    [JsonIgnore]
    public ResxEngine ResxEngine
    {
        get
        {
            if (_resxEngine == null && Project != null)
            {
                var engine = Project.GetCodeEngine(EngineTypes.LocalizationResxGenId);
                _resxEngine = engine != null ? (ResxEngine)engine : null;
            }
            return _resxEngine;
        }
    }
    private ResxEngine _resxEngine;

    public bool UseResourceFiles
    {
        set
        {
            _useResourceFiles = value;
            RaisePropertyChanged("UseResourceFiles");
        }
        get
        {
            if (_useResourceFiles == null)
            {
                _useResourceFiles = ResxEngine != null;
            }
            return _useResourceFiles.Value;
        }
    }
    private bool? _useResourceFiles;
}
