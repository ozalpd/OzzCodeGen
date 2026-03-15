using OzzCodeGen.CodeEngines.ModelClass;
using System.Linq;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataEntitySetting : BaseModelClassEntitySetting<MetadataPropertySetting>
    {
        public override AbstractEntitySetting<MetadataPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            var codeEngine = (MetadataCodeEngine)CodeEngine;

            return codeEngine.Entities
                    .OfType<MetadataEntitySetting>()
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }
    }
}