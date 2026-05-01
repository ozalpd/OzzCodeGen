using OzzCodeGen.CodeEngines.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpLookupServiceTemplate
    {
        public CSharpLookupServiceTemplate(WpfMvvmEntitySetting entitySetting, LookupTemplate templateType)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, isInterface: templateType == LookupTemplate.Interface)
        {
            TemplateType = templateType;
        }

        public string GetClassName(LookupTemplate? templateType = null)
        {
            var templType = templateType ?? TemplateType;
            return EntitySetting.GetLookupName(templType);
        }

        public string GetDeclaration()
        {
            return $"{(IsInterface ? "interface" : "class")} {GetClassName()}{(IsInterface ? "" : $" : {GetClassName(LookupTemplate.Interface)}")}";
        }

        public override string GetDefaultFileName()
        {
            return $"{GetClassName()}.cs";
        }

        public string GetNamespace()
        {
            if (TemplateType == LookupTemplate.RunTimeClass || string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                return CodeEngine.LookupNamespaceName;
            }
            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.LookupFolder)}";
        }


        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }
            if (TemplateType == LookupTemplate.RunTimeClass)
            {
                namespaces.Add(CodeEngine.RepoContractNamespaceName);

                // If infrastructure folder is specified, then we need to add the service namespace for the interface reference
                if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder) && !CodeEngine.PutLookupInInfra)
                    namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.LookupFolder)}");
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }

        public LookupTemplate TemplateType { get; }
    }
}
