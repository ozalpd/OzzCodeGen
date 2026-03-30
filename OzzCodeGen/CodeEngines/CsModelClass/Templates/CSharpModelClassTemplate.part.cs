using System.Text;

namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class CSharpModelClassTemplate
    {
        public CSharpModelClassTemplate(ModelClassEntitySetting entitySetting, bool metadataForDTO = false)
        {
            EntitySetting = entitySetting;
            MetadataForDTO = metadataForDTO;
            NamespaceName = EntitySetting.CodeEngine.NamespaceName;
        }


        public override CSharpModelClassCodeEngine CodeEngine => (CSharpModelClassCodeEngine)EntitySetting.CodeEngine;


        public override string GetDefaultFileName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EntitySetting.Name);

            if (MetadataForDTO)
            {
                sb.Append("DTO");
            }

            sb.Append(".cs");

            return sb.ToString();
        }
    }
}
