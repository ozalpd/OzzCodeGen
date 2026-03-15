using System.Linq;

namespace OzzCodeGen.CodeEngines.ModelClass
{
    public class ModelClassEntitySetting : BaseModelClassEntitySetting<ModelPropertySetting>
    {
        public override AbstractEntitySetting<ModelPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;
            var codeEngine = (ModelClassCodeEngine)CodeEngine;

            return codeEngine.Entities
                    .OfType<ModelClassEntitySetting>()
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }

    }
}
