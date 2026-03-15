using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.ModelClass
{
    public interface IModelClassEntitySetting
    {
        string Name { get; }
        BaseModelClassCodeEngine CodeEngine { get; }
    }

    public abstract class BaseModelClassEntitySetting<TPropertySetting> : AbstractEntitySetting<TPropertySetting>, IModelClassEntitySetting
        where TPropertySetting : BaseModelClassPropertySetting
    {
        [XmlIgnore]
        [JsonIgnore]
        public BaseModelClassCodeEngine CodeEngine { get; set; }

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
    }
}
