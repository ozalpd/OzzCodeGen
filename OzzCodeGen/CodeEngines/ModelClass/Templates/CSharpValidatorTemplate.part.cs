namespace OzzCodeGen.CodeEngines.ModelClass.Templates
{
    public partial class CSharpValidatorTemplate
    {
        public CSharpValidatorTemplate(ModelClassEntitySetting entitySetting)
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
