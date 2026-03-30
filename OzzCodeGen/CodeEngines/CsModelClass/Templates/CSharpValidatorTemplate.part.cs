namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class CSharpValidatorTemplate
    {
        public CSharpValidatorTemplate(ModelClassEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
            NamespaceName = CodeEngine.ValidatorNamespaceName;
        }

        public override CSharpModelClassCodeEngine CodeEngine => (CSharpModelClassCodeEngine)EntitySetting.CodeEngine;

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.ValidatorClassName}.cs";
        }
    }
}
