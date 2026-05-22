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

        public string GetBaseClassName()
        {
            switch (TemplateType)
            {
                case MvvmTemplate.Create:
                    return CodeEngine.BaseCreateEditViewModelName;

                case MvvmTemplate.Edit:
                    return CodeEngine.BaseCreateEditViewModelName;

                case MvvmTemplate.Delete:
                    return CodeEngine.BaseViewModelName;

                case MvvmTemplate.Collection:
                    return $"AbstractCollectionVM<{EntitySetting.Name}>";

                default:
                    return $"{EntitySetting.Name}BaseViewModel";
            }
        }

        public string GetClassName()
        {
            return EntitySetting.GetViewModelName(TemplateType);
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
                namespaces.Add(CodeEngine.LookupNamespaceName);
                namespaces.Add("System.Collections.ObjectModel");
            }

            if (TemplateType == MvvmTemplate.Collection)
            {
                namespaces.Add(CodeEngine.RepoContractNamespaceName);
                namespaces.Add(CodeEngine.RepositoryNamespaceName);
                namespaces.Add(CodeEngine.ServicesNamespaceName);
                namespaces.Add(EntitySetting.GetCommandsNamespaceName());
            }

            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
                if (EntitySetting.GenerateGetPaged && TemplateType == MvvmTemplate.Collection)
                    namespaces.Add(modelClassEngine.QueryParamNamespaceName);

                if (GetEnumTypeNames().Any())
                {
                    namespaces.Add(modelClassEngine.ExtensionsNamespaceName);
                    namespaces = namespaces.OrderBy(ns => ns).ToList();
                    namespaces.Add($"static {modelClassEngine.ExtensionsNamespaceName}.{modelClassEngine.EnumExtensionClassName}");
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
