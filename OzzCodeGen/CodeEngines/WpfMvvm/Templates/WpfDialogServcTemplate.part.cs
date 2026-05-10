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

        public WpfDialogServcTemplate(WpfMvvmCodeEngine codeEngine, bool isInterface) : base(codeEngine, isInterface: isInterface)
        {

        }

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
