using OzzCodeGen.CodeEngines.CSharp;
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

    public BaseMvvmPropertySetting FindProperty(string propertyName)
    {
        return Properties.FirstOrDefault(p => p.Name == propertyName);
    }

    public void AddProperty(BaseMvvmPropertySetting propertySetting)
    {
        Properties.Add((TPropertySetting)propertySetting);
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
