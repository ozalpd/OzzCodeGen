using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class WpfBaseCommandTemplate
    {
        public WpfBaseCommandTemplate(WpfMvvmCodeEngine codeEngine) : base(codeEngine) { }


        public string GetClassDeclaration() => $"public abstract class {GetClassName()} : ICommand";

        public string GetClassName() => CodeEngine.BaseCommandName;

        public override string GetDefaultFileName() => $"{GetClassName()}.cs";

        public string GetNamespace()
        {
            if (string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                return CodeEngine.CommandNamespaceName;
            }
            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.BaseCommandNamespaceName)}";
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>() {
                "System",
                "System.Windows.Input"
            };

            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
