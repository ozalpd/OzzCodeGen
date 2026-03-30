using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsModelClass;

public abstract class BaseModelClassCodeEngine : BaseCodeEngine
{
    protected abstract BaseEntitySetting CreateEntitySetting();
    protected abstract BaseModelClassPropertySetting CreatePropertySetting();

    protected virtual BaseModelClassPropertySetting GetDefaultPropertySetting(BaseProperty property, BaseEntitySetting setting)
    {
        var modelClassSetting = setting as IModelClassEntitySetting;
        if (modelClassSetting == null)
        {
            throw new InvalidOperationException($"{setting?.GetType().Name} must implement {nameof(IModelClassEntitySetting)}.");
        }

        var ps = CreatePropertySetting();
        ps.Name = property.Name;
        ps.EntitySetting = setting;
        modelClassSetting.AddProperty(ps);

        return ps;
    }

    protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
    {
        var setting = CreateEntitySetting();
        var modelClassSetting = setting as IModelClassEntitySetting;
        if (modelClassSetting == null)
        {
            throw new InvalidOperationException($"{setting?.GetType().Name} must implement {nameof(IModelClassEntitySetting)}.");
        }

        setting.DataModel = Project.DataModel;
        setting.Name = entity.Name;
        modelClassSetting.CodeEngine = this;

        foreach (var property in entity.Properties)
        {
            if (property.DefinitionType != DefinitionType.Collection)
            {
                _ = GetDefaultPropertySetting(property, setting);
            }
        }
        return setting;
    }

    protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
    {
        var modelClassSetting = setting as IModelClassEntitySetting;
        if (modelClassSetting == null)
        {
            throw new InvalidOperationException($"{setting?.GetType().Name} must implement {nameof(IModelClassEntitySetting)}.");
        }

        setting.DataModel = Project.DataModel;
        modelClassSetting.CodeEngine = this;

        var remvProp = modelClassSetting
            .ModelProperties
            .Where(p => entity.Properties.FirstOrDefault(ep => ep.Name == p.Name) == null)
            .ToList();

        foreach (var dalProp in remvProp)
        {
            modelClassSetting.RemoveProperty(dalProp);
        }

        foreach (var property in entity.Properties)
        {
            var ps = modelClassSetting.FindProperty(property.Name);
            if (ps == null)
            {
                _ = GetDefaultPropertySetting(property, setting);
            }
            else
            {
                ps.EntitySetting = setting;
            }
        }

        modelClassSetting.SortProperties();
    }

    public override UserControl GetSettingsDlgUI()
    {
        return null;
    }

    protected override void OnSearchStringChanged()
    {
        RaisePropertyChanged("Entities");
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
                _customAttribNamespace = $"{Project.NamespaceName}.Validate";
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
