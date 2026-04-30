using OzzCodeGen.CodeEngines.Mvvm;
using OzzUtils;
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

        public string GetParamsDeclaration()
        {
            var sb = new System.Text.StringBuilder();
            if (IsEdit)
            {
                sb.Append(EntitySetting.Name);
                sb.Append(' ');
                sb.Append(EntitySetting.Name.ToCamelCase());
            }

            var foreignLookupEntities = EntitySetting.GetForeignLookupEntities(IsEdit);
            foreach (var lookupEntity in foreignLookupEntities)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.Append(lookupEntity.GetLookupName(LookupServiceTemplateType.Interface));
                sb.Append(' ');
                sb.Append(lookupEntity.GetLookupName(LookupServiceTemplateType.RunTimeClass).ToCamelCase());
            }

            return sb.ToString();
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }
            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                namespaces.Add($"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ViewModelFolder)}");
            }
            if (EntitySetting.GetForeignLookupEntities(IsEdit).Any())
            {
                namespaces.Add(GetLookupContractNamespace());
                namespaces.Add("System.Collections.ObjectModel");
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
