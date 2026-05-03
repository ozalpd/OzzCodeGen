using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return CodeEngine.ViewModelNamespaceName;
            }
            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ViewModelFolder)}";
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
