using OzzCodeGen.CodeEngines.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class WpfViewModelTemplate
    {
        public WpfViewModelTemplate(WpfMvvmEntitySetting entitySetting, MvvmTemplate templateType)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, templateType: templateType)
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
            var namespaces = new List<string>();
            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ViewModelFolder)}");
            }
            if (EntitySetting.GetForeignLookupEntities(IsEdit).Any())
            {
                namespaces.Add(GetLookupContractNamespace());
                namespaces.Add("System.Collections.ObjectModel");
            }
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
                if (GetEnumTypeNames().Any())
                {
                    namespaces.Add(modelClassEngine.ExtensionsNamespaceName);
                    namespaces = namespaces.OrderBy(ns => ns).ToList();
                    namespaces.Add($"static {modelClassEngine.ExtensionsNamespaceName}.{modelClassEngine.EnumExtensionName}");
                }
                else
                {
                    namespaces = namespaces.OrderBy(ns => ns).ToList();
                }
            }
            else
            {
                namespaces = namespaces.OrderBy(ns => ns).ToList();
            }
            return namespaces;
        }
    }
}
