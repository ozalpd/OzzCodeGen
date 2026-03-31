using OzzCodeGen.CodeEngines.CsModelClass;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataPropertySetting : BaseModelClassPropertySetting
    {
        protected override BaseCodeEngine GetCodeEngine()
        {
            var entitySetting = (MetadataEntitySetting)EntitySetting;
            return entitySetting?.CodeEngine;
        }
    }
}
