using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.ModelClass
{
    public interface IModelClassEntitySetting
    {
        string Name { get; }
        BaseModelClassCodeEngine CodeEngine { get; set; }
        IEnumerable<BaseModelClassPropertySetting> ModelProperties { get; }
        IEnumerable<BaseModelClassPropertySetting> GetInheritedProperties();
        BaseModelClassPropertySetting FindProperty(string propertyName);
        IEnumerable<BaseModelClassPropertySetting> GetInheritedIncludedProperties();
        void AddProperty(BaseModelClassPropertySetting propertySetting);
        bool HasCustomAttributes { get; }
        bool RemoveProperty(BaseModelClassPropertySetting propertySetting);
        void SortProperties();
    }

    public abstract class BaseModelClassEntitySetting<TPropertySetting> : AbstractEntitySetting<TPropertySetting>, IModelClassEntitySetting
        where TPropertySetting : BaseModelClassPropertySetting
    {
        [XmlIgnore]
        [JsonIgnore]
        public bool HasCustomAttributes
        {
            get
            {
                return Properties != null
                    && Properties.Any(p => !string.IsNullOrEmpty(p.CustomAttributes));
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public virtual BaseModelClassCodeEngine CodeEngine { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<BaseModelClassPropertySetting> ModelProperties
        {
            get { return Properties.Cast<BaseModelClassPropertySetting>(); }
        }

        public BaseModelClassPropertySetting FindProperty(string propertyName)
        {
            return Properties.FirstOrDefault(p => p.Name == propertyName);
        }

        public void AddProperty(BaseModelClassPropertySetting propertySetting)
        {
            Properties.Add((TPropertySetting)propertySetting);
        }

        public bool RemoveProperty(BaseModelClassPropertySetting propertySetting)
        {
            return Properties.Remove((TPropertySetting)propertySetting);
        }

        public void SortProperties()
        {
            Properties = Properties
                .OrderBy(p => p.PropertyDefinition.DisplayOrder)
                .ToList();
        }

        public new IEnumerable<BaseModelClassPropertySetting> GetInheritedProperties()
        {
            return base.GetInheritedProperties().Cast<BaseModelClassPropertySetting>();
        }

        public new IEnumerable<BaseModelClassPropertySetting> GetInheritedIncludedProperties()
        {
            return base.GetInheritedIncludedProperties().Cast<BaseModelClassPropertySetting>();
        }
    }
}
