using OzzCodeGen.CodeEngines.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpViewTemplate
    {
        public CSharpViewTemplate(WpfMvvmEntitySetting entitySetting, MvvmTemplate templateType, FileGenerationMode fileGenerationMode)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, templateType: templateType, fileGenerationMode: fileGenerationMode)
        {
            XamlViewTemplate = new XamlViewTemplate(entitySetting, templateType);
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>()
            {
                "System.Windows",
                EntitySetting.GetViewModelsNamespaceName()
            };

            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
            }

            if (IsCreateOrEdit && (EntitySetting.GetForeignLookupEntities(IsEdit).Any()))
            {
                namespaces.Add(CodeEngine.LookupNamespaceName);
                namespaces.Add(CodeEngine.DesignTimeNamespaceName);
                namespaces.Add("System.Collections.ObjectModel");
            }

            if (IsCreateOrEdit && (HasAnyDecimalNumeric))
            {
                namespaces.Add("System.Globalization");
                namespaces.Add("System.Text.RegularExpressions");
            }

            if (IsCreateOrEdit && (HasAnyDecimalNumeric || HasAnyIntegerNumeric))
            {
                namespaces.Add("System.Windows.Controls");
                namespaces.Add("System.Windows.Input");
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }

        public string GetBaseClassName() => XamlViewTemplate.GetBaseClassName();

        public string GetClassName() => EntitySetting.GetViewName(TemplateType);

        public override string GetDefaultFileName() => $"{XamlViewTemplate.GetDefaultFileName()}.cs";

        public string GetNamespace() => XamlViewTemplate.GetNamespace();

        public string GetViewModelClassName() => EntitySetting.GetViewModelName(TemplateType);

        public XamlViewTemplate XamlViewTemplate { get; }
    }
}
