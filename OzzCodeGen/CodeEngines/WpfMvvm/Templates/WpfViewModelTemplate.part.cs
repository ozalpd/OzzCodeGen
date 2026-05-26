using OzzCodeGen.CodeEngines.Mvvm;
using OzzUtils;
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
            var result = base.GetIncludedProperties().OfType<WpfMvvmPropertySetting>();

            if (TemplateType == MvvmTemplate.Edit)
            {
                result = result.Where(p => p.EditViewMode != ViewFieldMode.Exclude);
            }
            else if (TemplateType == MvvmTemplate.Create)
            {
                result = result.Where(p => p.CreateViewMode != ViewFieldMode.Exclude);
            }
            else if (TemplateType == MvvmTemplate.Collection)
            {
                result = result.Where(p => p.ShowInCollection);
            }
            else if (TemplateType == MvvmTemplate.Detail)
            {
                result = result.Where(p => p.ShowInDetail);
            }


            return result.ToList();
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{CodeEngine.ViewModelFolder.FolderPathToNamespace()}");
            }

            if (EntitySetting.GetForeignLookupEntities(IsEdit).Any())
            {
                namespaces.Add(CodeEngine.LookupNamespaceName);
                namespaces.Add("System.Collections.ObjectModel");
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
                }
            }

            if (TemplateType == MvvmTemplate.Collection)
            {
                namespaces.Add(CodeEngine.RepoContractNamespaceName);
                //namespaces.Add(CodeEngine.RepositoryNamespaceName);
                namespaces.Add(CodeEngine.ServicesNamespaceName);
                namespaces.Add(EntitySetting.GetCommandsNamespaceName());
                if (EntitySetting.GenerateGetPaged && EntitySetting.GenerateQueryParam
                    && !namespaces.Contains(EntitySetting.GetQueryParamNamespace()))
                {
                    namespaces.Add(EntitySetting.GetQueryParamNamespace());
                }
            }

            namespaces = namespaces.OrderBy(ns => ns).ToList();
            if (modelClassEngine != null && GetEnumTypeNames().Any())
            {
                namespaces.Add($"static {modelClassEngine.ExtensionsNamespaceName}.{modelClassEngine.EnumExtensionClassName}");
            }

            return namespaces;
        }
    }
}
