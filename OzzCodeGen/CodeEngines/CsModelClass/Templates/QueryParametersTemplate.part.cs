namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class QueryParametersTemplate
    {
        public QueryParametersTemplate(CSharpModelClassCodeEngine codeEngine)
        {
            _codeEngine = codeEngine;
            NamespaceName = CodeEngine.ValidatorNamespaceName;
        }

        public override CSharpModelClassCodeEngine CodeEngine => _codeEngine;
        CSharpModelClassCodeEngine _codeEngine;

        public override string GetDefaultFileName()
        {
            return "QueryParameters.cs";
        }
    }
}
