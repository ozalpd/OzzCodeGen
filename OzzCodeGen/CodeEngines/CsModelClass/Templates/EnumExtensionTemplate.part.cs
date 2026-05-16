using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsModelClass.Templates
{
    public partial class EnumExtensionTemplate
    {
        public EnumExtensionTemplate(CSharpModelClassCodeEngine codeEngine)
        {
            _codeEngine = codeEngine;
            NamespaceName = CodeEngine.ExtensionsNamespaceName;
        }
        public override CSharpModelClassCodeEngine CodeEngine => _codeEngine;
        CSharpModelClassCodeEngine _codeEngine;

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.EnumExtensionClassName}.cs";
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>()
            {
                "System.ComponentModel.DataAnnotations",
                "System.Reflection",
                "System.Resources",
            };

            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
