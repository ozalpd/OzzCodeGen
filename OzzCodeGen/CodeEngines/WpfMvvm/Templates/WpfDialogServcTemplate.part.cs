using OzzCodeGen.CodeEngines.Mvvm;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class WpfDialogServcTemplate
    {
        private string _interfaceName => CodeEngine.DialogServiceContract;
        private string _className => CodeEngine.DialogServiceClassName;

        public WpfDialogServcTemplate(WpfMvvmCodeEngine codeEngine, bool isInterface) : base(codeEngine, isInterface: isInterface) { }



        /// <summary>
        /// The generated class will be internal accessibility when IsPublic is false, and will have public accessibility when IsPublic is true.
        /// Intended use for C# WPF application so it is set to false by default.
        /// </summary>
        public bool IsPublic { get; set; } = false;

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>()
            {
                "System.Windows"
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

            var entitySettings = GetEntitySettings();
            if (entitySettings.Any(e => e.GetForeignLookupEntities().Any()))
            {
                namespaces.Add(GetLookupContractNamespace());
            }

            if (!IsInterface)
            {
                foreach (var item in entitySettings)
                {
                    string vwNamespace = item.GetViewsNamespaceName();
                    if (!string.IsNullOrWhiteSpace(vwNamespace) && !namespaces.Contains(vwNamespace))
                    {
                        namespaces.Add(vwNamespace);
                    }
                }
            }
            return namespaces.OrderBy(ns => ns).ToList();
        }

        public string GetAccessibility()
        {
            return IsPublic ? "public partial" : "internal partial";
        }

        public string GetDeclaration()
        {
            var sb = new StringBuilder();
            sb.Append(GetAccessibility());

            if (IsInterface)
            {
                sb.Append(" interface ");
                sb.Append(_interfaceName);
            }
            else
            {
                sb.Append(" class ");
                sb.Append(_className);
                sb.Append(" : ");
                sb.Append(_interfaceName);
            }
            return sb.ToString();
        }

        public override string GetDefaultFileName()
        {
            return IsInterface ? $"{_interfaceName}.cs" : $"{_className}.cs";
        }

        public string GetParamsDeclaration(WpfMvvmEntitySetting entitySetting, bool isEdit = false)
        {
            var sb = new StringBuilder();
            sb.Append("Window owner");
            if (isEdit)
            {
                sb.Append(", ");
                sb.Append(entitySetting.Name);
                sb.Append(' ');
                sb.Append(entitySetting.Name.ToCamelCase());
            }

            var foreignLookupEntities = entitySetting.GetForeignLookupEntities(isForEdit: isEdit);

            foreach (var lookupEntity in foreignLookupEntities)
            {
                sb.Append(", ");
                sb.Append(lookupEntity.GetLookupName(LookupServiceTemplateType.Interface));
                sb.Append(' ');
                sb.Append(lookupEntity.GetLookupName(LookupServiceTemplateType.RunTimeClass).ToCamelCase());
            }

            if (isEdit)
                return sb.ToString();

            var preselectProperties = entitySetting.GetPreselectProperties();
            foreach (var property in preselectProperties)
            {
                sb.Append(", ");
                sb.Append(property.GetTypeName(getReturnType: true));
                sb.Append("? preselected");
                sb.Append(property.Name);
            }

            return sb.ToString();
        }

        public IEnumerable<WpfMvvmEntitySetting> GetEntitySettings()
        {
            if (_entitySettings == null)
            {
                _entitySettings = CodeEngine.Entities
                                       .Where(e => !e.Exclude
                                                && (e.IsInEditDialogService || e.IsInCreateDialogService))
                                       .OrderBy(e => e.Name)
                                       .ToList();
            }
            return _entitySettings;
        }
        private List<WpfMvvmEntitySetting> _entitySettings;
    }
}
