using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.Metadata;
using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.ModelClass;

[XmlInclude(typeof(MetadataEntitySetting))]
public abstract class BaseModelClassCodeEngine : BaseCodeEngine
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
