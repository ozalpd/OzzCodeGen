namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class QueryParametersTemplate
    {
        public QueryParametersTemplate(CSharpModelClassCodeEngine codeEngine, ModelClassEntitySetting entitySetting = null)
        {
            _codeEngine = codeEngine;
            NamespaceName = CodeEngine.QueryParamNamespaceName;

            _entitySetting = entitySetting;
        }

        public override ModelClassEntitySetting EntitySetting => _entitySetting;
        ModelClassEntitySetting? _entitySetting;

        public override CSharpModelClassCodeEngine CodeEngine => _codeEngine;
        CSharpModelClassCodeEngine _codeEngine;

        public override string GetDefaultFileName()
        {
            return EntitySetting != null
                ? $"{EntitySetting.Name}QueryParameters.cs"
                : "QueryParameters.cs";
        }
    }
}
