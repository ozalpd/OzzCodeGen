using OzzCodeGen.CodeEngines.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class XamlViewTemplate
    {
        public XamlViewTemplate(WpfMvvmEntitySetting entitySetting, MvvmTemplate templateType)
        {
            EntitySetting = entitySetting;
            TemplateType = templateType;
        }


        public string GetBaseClassName()
        {
            if (TemplateType == MvvmTemplate.Edit)
                return CodeEngine.BaseEditViewName;
            else
                return "Window";
        }

        public string GetBaseClassNamespaceDeclaration()
        {
            if (TemplateType == MvvmTemplate.Edit && CodeEngine.ViewNamespaceName != GetNamespace())
                return $"clr-namespace:{CodeEngine.ViewNamespaceName}";
            else
                return string.Empty;
        }

        public string GetClassName() => EntitySetting.GetViewName(TemplateType);

        public string GetFullClassName() => $"{GetNamespace()}.{GetClassName()}";

        public override string GetDefaultFileName()
        {
            if (string.IsNullOrWhiteSpace(EntitySetting.SubFolder))
                return $"{GetClassName()}.xaml";

            return $"{EntitySetting.SubFolder}\\{GetClassName()}.xaml";
        }

        public string GetNamespace() => EntitySetting.GetViewsNamespaceName();

        public WpfMvvmCodeEngine CodeEngine => EntitySetting.CodeEngine as WpfMvvmCodeEngine;

        public WpfMvvmEntitySetting EntitySetting { get; }

        public MvvmTemplate TemplateType { get; }

    }
}
