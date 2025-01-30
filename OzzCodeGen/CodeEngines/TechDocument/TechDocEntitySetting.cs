using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.TechDocument
{
    public class TechDocEntitySetting : AbstractEntitySetting<TechDocPropertySetting>
    {
        [XmlIgnore]
        public TechDocumentEngine CodeEngine { get; set; }

        public override AbstractEntitySetting<TechDocPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return CodeEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }
    }
}
