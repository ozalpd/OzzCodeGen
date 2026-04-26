using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpLookupServiceTemplate
    {
        public CSharpLookupServiceTemplate(WpfMvvmEntitySetting entitySetting, LookupServiceTemplateType templateType)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, isInterface: templateType == LookupServiceTemplateType.Interface)
        {
            TemplateType = templateType;
        }

        public string GetClassName(LookupServiceTemplateType? templateType = null)
        {
            var templType = templateType ?? TemplateType;
            switch (templType)
            {
                case LookupServiceTemplateType.Interface:
                    return $"I{EntitySetting.Name}LookupService";

                case LookupServiceTemplateType.DesignTimeClass:
                    return $"{EntitySetting.Name}MockLookupService";

                case LookupServiceTemplateType.RunTimeClass:
                    return $"{EntitySetting.Name}LookupService";

                default:
                    return $"{EntitySetting.Name}LookupService";
            }
        }

        public string GetDeclaration()
        {
            return $"{(IsInterface ? "interface" : "class")} {GetClassName()}{(IsInterface ? "" : $" : {GetClassName(LookupServiceTemplateType.Interface)}")}";
        }

        public override string GetDefaultFileName()
        {
            return $"{GetClassName()}.cs";
        }

        public string GetNamespace()
        {
            if (TemplateType == LookupServiceTemplateType.RunTimeClass || string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                return CodeEngine.ServiceNamespaceName;
            }
            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ServiceFolder)}";
        }


        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }
            if (TemplateType == LookupServiceTemplateType.RunTimeClass)
            {
                // For runtime class, we need the repository namespace for the repository interface reference
                namespaces.Add(CodeEngine.RepositoryNamespaceName);

                // If infrastructure folder is specified, then we need to add the service namespace for the interface reference
                if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
                    namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ServiceFolder)}");
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }

        public LookupServiceTemplateType TemplateType { get; }

        public enum LookupServiceTemplateType
        {
            Interface,
            DesignTimeClass,
            RunTimeClass
        }
    }
}
