using OzzCodeGen.CodeEngines.Mvvm.Templates;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public abstract class BaseCSharpWpfMvvmTemplate : BaseCSharpMvvmTemplate
    {
        protected BaseCSharpWpfMvvmTemplate(WpfMvvmCodeEngine codeEngine, WpfMvvmEntitySetting entitySetting = null, bool isEdit = false)
        {
            CodeEngine = codeEngine;
            EntitySetting = entitySetting;
            IsEdit = isEdit;
        }

        public WpfMvvmCodeEngine CodeEngine { get; }

        public WpfMvvmEntitySetting EntitySetting { get; }

        public bool IsEdit { get; }

        public virtual IEnumerable<WpfMvvmPropertySetting> GetIncludedProperties()
        {
            if (EntitySetting == null)
                return Enumerable.Empty<WpfMvvmPropertySetting>();

            var includedProperties = EntitySetting
                            .GetInheritedIncludedProperties()
                            .OfType<WpfMvvmPropertySetting>();

            return includedProperties;
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
                namespaces.Add(modelClassEngine.ValidatorNamespaceName);
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
