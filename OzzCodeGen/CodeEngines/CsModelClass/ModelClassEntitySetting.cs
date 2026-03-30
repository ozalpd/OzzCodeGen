using System.Linq;

namespace OzzCodeGen.CodeEngines.CsModelClass
{
    public class ModelClassEntitySetting : BaseModelClassEntitySetting<ModelPropertySetting>
    {
        public override AbstractEntitySetting<ModelPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;
            var codeEngine = (CSharpModelClassCodeEngine)CodeEngine;

            return codeEngine.Entities
                    .OfType<ModelClassEntitySetting>()
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }

    }
}
