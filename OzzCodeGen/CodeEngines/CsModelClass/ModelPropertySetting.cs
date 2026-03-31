
namespace OzzCodeGen.CodeEngines.CsModelClass
{
    public class ModelPropertySetting : BaseModelClassPropertySetting
    {
        protected override BaseCodeEngine GetCodeEngine()
        {
            var entitySetting = (ModelClassEntitySetting)EntitySetting;
            return entitySetting?.CodeEngine;
        }
    }
}
