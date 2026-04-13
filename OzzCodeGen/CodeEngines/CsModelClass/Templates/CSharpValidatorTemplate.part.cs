using System.Linq;

namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class CSharpValidatorTemplate
    {
        public CSharpValidatorTemplate(CSharpModelClassCodeEngine codeEngine)
        {
            _codeEngine = codeEngine;
            NamespaceName = CodeEngine.ValidatorNamespaceName;
        }

        public override CSharpModelClassCodeEngine CodeEngine => _codeEngine;
        CSharpModelClassCodeEngine _codeEngine;

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.ValidatorClassName}.cs";
        }

        public bool HasCustomAttributes()
        {
            foreach (var entity in CodeEngine.EntitySettings.OfType<ModelClassEntitySetting>())
            {
                if (entity.Properties.Any(p => !string.IsNullOrWhiteSpace(p.CustomAttributes)))
                    return true;
            }
            return false;
        }
    }
}
