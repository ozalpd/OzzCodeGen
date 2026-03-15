using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.ModelClass;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataEntitySetting : AbstractEntitySetting<MetadataPropertySetting>
    {
        [XmlIgnore]
        [JsonIgnore]
        public ModelClassCodeEngineBase CodeEngine { get; set; }

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

        public override AbstractEntitySetting<MetadataPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return CodeEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }
    }
}