using System.Text;

namespace OzzCodeGen.CodeEngines.ModelClass.Templates
{
    public partial class ModelClassTemplate
    {
        public ModelClassTemplate(ModelClassEntitySetting entitySetting, bool metadataForDTO = false)
        {
            EntitySetting = entitySetting;
            MetadataForDTO = metadataForDTO;
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

            sb.Append(".cs");

            return sb.ToString();
        }
    }
}
