namespace OzzCodeGen.CodeEngines.ModelClass.Templates
{
    public partial class ValidatorTemplate
    {
        public ValidatorTemplate(ModelClassEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
            NamespaceName = CodeEngine.ValidatorNamespaceName;
        }

        public override ModelClassCodeEngine CodeEngine => (ModelClassCodeEngine)EntitySetting.CodeEngine;

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.ValidatorClassName}.cs";
        }
    }
}
