using OzzCodeGen.CodeEngines.ModelClass;
using OzzCodeGen.CodeEngines.ModelClass.Templates;
using System.Text;

namespace OzzCodeGen.CodeEngines.Metadata.Templates
{
    public partial class MetadataClassTemplate : BaseModelClassTemplate
    {
        public MetadataClassTemplate(MetadataEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
            NamespaceName = EntitySetting.CodeEngine.NamespaceName;
        }

        public MetadataClassTemplate(MetadataEntitySetting entitySetting, bool forDTO)
            : this(entitySetting)
        {
            MetadataForDTO = forDTO;
        }

        
        public override BaseModelClassCodeEngine CodeEngine => EntitySetting.CodeEngine;

        public override string GetDefaultFileName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EntitySetting.Name);

            if (MetadataForDTO)
            {
                sb.Append("DTO");
            }

            MetadataCodeEngine codeEngine = EntitySetting.CodeEngine as MetadataCodeEngine;
            if (codeEngine != null && codeEngine.SeperateMetaDataClass)
            {
                sb.Append(".meta");
            }

            sb.Append(".cs");

            return sb.ToString();
        }
    }
}
