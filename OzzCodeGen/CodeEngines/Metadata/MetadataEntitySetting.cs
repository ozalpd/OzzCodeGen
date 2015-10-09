using System;
using System.Linq;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataEntitySetting : AbstractEntitySetting<MetadataPropertySetting>
    {
        [XmlIgnore]
        public MetadataCodeEngine CodeEngine { get; set; }

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