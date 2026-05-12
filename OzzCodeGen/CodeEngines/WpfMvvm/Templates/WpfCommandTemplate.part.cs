using OzzCodeGen.CodeEngines.Mvvm;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class WpfCommandTemplate
    {
        public WpfCommandTemplate(WpfMvvmEntitySetting entitySetting, MvvmTemplate templateType)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, templateType: templateType)
        {

        }



        public string BaseCommandName => CodeEngine.BaseCommandName;

        public string GetClassName() => EntitySetting.GetCommandName(TemplateType);

        public override string GetDefaultFileName()
        {
            if (string.IsNullOrWhiteSpace(EntitySetting.SubFolder))
                return $"{GetClassName()}.cs";

            return $"{EntitySetting.SubFolder}\\{GetClassName()}.cs";
        }

        public string GetNamespace() => EntitySetting.GetCommandsNamespaceName();

        public bool IsCreateOrEdit => TemplateType == MvvmTemplate.Create || TemplateType == MvvmTemplate.Edit;

        public bool HasDlgService => IsCreateOrEdit; // || TemplateType == MvvmTemplate.Collection;

        public string GetConstructorParameters()
        {
            var sb = new StringBuilder();
            sb.Append(EntitySetting.CommandVmTypeName);
            sb.Append(" viewModel");
            if (HasDlgService)
            {
                sb.Append(", ");
                sb.Append(CodeEngine.DialogServiceContract);
                sb.Append(' ');
                sb.Append(CodeEngine.DialogServiceClassName.ToCamelCase());
            }
            if (!IsCreateOrEdit)
                return sb.ToString();

            var foreignLookupEntities = GetForeignLookupEntities();
            int i = 0;
            if (foreignLookupEntities != null && foreignLookupEntities.Any())
            {
                sb.Append(",\r\n");
                sb.Append(' ', GetClassName().Length + 16);
                foreach (var lookupEntity in foreignLookupEntities)
                {
                    if (i > 0 && i % 2 == 0)
                    {
                        sb.Append(",\r\n");
                        sb.Append(' ', GetClassName().Length + 16);
                    }
                    else if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(lookupEntity.GetLookupName(LookupTemplate.Interface));
                    sb.Append(' ');
                    sb.Append(lookupEntity.GetLookupName(LookupTemplate.RunTimeClass).ToCamelCase());
                    i++;
                }
            }
            return sb.ToString();
        }

        public string GetSvcShowParameters()
        {
            var sb = new StringBuilder();
            sb.Append("owner");
            if (IsEdit)
            {
                sb.Append(", _viewModel.Selected");
                sb.Append(EntitySetting.Name);
            }
            if (!IsCreateOrEdit)
                return sb.ToString();

            var foreignLookupEntities = GetForeignLookupEntities();
            foreach (var lookupEntity in foreignLookupEntities)
            {
                sb.Append(", _");
                sb.Append(lookupEntity.GetLookupName(LookupTemplate.RunTimeClass).ToCamelCase());
            }

            if (TemplateType == MvvmTemplate.Create)
            {
                var preselectProperties = GetPreselectProperties();
                foreach (var property in preselectProperties)
                {
                    sb.Append(", Preselected");
                    sb.Append(property.Name);
                }
            }

            return sb.ToString();
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>
            {
                "System.Windows",
                CodeEngine.ServicesNamespaceName
            };

            var resxEngine = CodeEngine.ResxEngine;
            if (resxEngine != null)
                namespaces.Add(resxEngine.NamespaceName);

            if (!string.IsNullOrEmpty(EntitySetting.CommandVmNamespace))
                namespaces.Add(EntitySetting.CommandVmNamespace);

            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
                namespaces.Add(CodeEngine.BaseCommandNamespaceName);

            if (TemplateType == MvvmTemplate.Create
                && !namespaces.Contains(CodeEngine.LookupNamespaceName)
                && GetForeignLookupEntities().Any())
            {
                namespaces.Add(CodeEngine.LookupNamespaceName);
            }

            if (TemplateType == MvvmTemplate.Create && GetPreselectProperties().Any())
            {
                var modelClassEngine = CodeEngine.ModelClassCodeEngine;
                if (modelClassEngine != null)
                {
                    namespaces.Add(modelClassEngine.NamespaceName);
                }
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
