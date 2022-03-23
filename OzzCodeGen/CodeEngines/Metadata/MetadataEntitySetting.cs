using System;
using System.Linq;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataEntitySetting : AbstractEntitySetting<MetadataPropertySetting>
    {
        [XmlIgnore]
        public MetadataCodeEngine CodeEngine { get; set; }

        [XmlIgnore]
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