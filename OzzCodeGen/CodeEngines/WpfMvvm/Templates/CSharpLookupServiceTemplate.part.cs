using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpLookupServiceTemplate
    {
        public CSharpLookupServiceTemplate(WpfMvvmEntitySetting entitySetting, bool isInterface)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, isInterface: isInterface)
        {

        }

        public string GetClassName()
        {
            return $"{(IsInterface ? "I" : "")}{EntitySetting.Name}LookupService";
        }

        public string GetDeclaration()
        {
            return $"{(IsInterface ? "interface" : "class")} {GetClassName()}{(IsInterface ? "" : $" : I{GetClassName()}")}";
        }

        public override string GetDefaultFileName()
        {
            return $"{GetClassName()}.cs";
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }
            if (!IsInterface)
            {
                namespaces.Add(CodeEngine.RepositoryNamespaceName);
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
