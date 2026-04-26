using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpWpfViewModelTemplate
    {
        public CSharpWpfViewModelTemplate(WpfMvvmEntitySetting entitySetting, bool isEdit)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, isEdit: isEdit)
        {

        }

        public string GetClassName()
        {
            return $"{EntitySetting.Name}{(IsEdit ? "Edit" : "Create")}VM";
        }

        public override string GetDefaultFileName()
        {
            if (string.IsNullOrWhiteSpace(EntitySetting.SubFolder))
                return $"{GetClassName()}.cs";

            return $"{EntitySetting.SubFolder}\\{GetClassName()}.cs";
        }

        public override IEnumerable<WpfMvvmPropertySetting> GetIncludedProperties()
        {
            return base.GetIncludedProperties().Where(p => p.IncludeInViewModel);
        }


        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>()
            {
                "System",
                //"System.Linq",
                //"System.Collections",
                //"System.Collections.Generic",
                //"System.Collections.ObjectModel",
                //"System.ComponentModel",
                //"System.ComponentModel.DataAnnotations",
                CodeEngine.RepositoryNamespaceName
            };
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }
            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ViewModelFolder)}");
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
