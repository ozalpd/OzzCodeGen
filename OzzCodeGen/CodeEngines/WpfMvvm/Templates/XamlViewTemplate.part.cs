using OzzCodeGen.CodeEngines.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string GetClassName() => EntitySetting.GetViewName(TemplateType);

        public string GetFullClassName() => $"{GetNamespace()}.{GetClassName()}";

        public override string GetDefaultFileName()
        {
            if (string.IsNullOrWhiteSpace(EntitySetting.SubFolder))
                return $"{GetClassName()}.xaml";

            return $"{EntitySetting.SubFolder}\\{GetClassName()}.xaml";
        }

        public string GetNamespace()
        {
            if (string.IsNullOrWhiteSpace(_namespaceName))
            {
                _namespaceName = EntitySetting.GetViewsNamespaceName();
            }
            return _namespaceName;
        }
        private string _namespaceName;

        public string GetActionString(string key)
        {
            bool hasLocalization = CodeEngine.ResxEngine != null;
            return hasLocalization ? GetLocalizedString(key, "ActionStrings") : key;
        }

        public string GetLocalizedString(string key, string resxName = "LocalizedStrings")
        {
            var sb = new StringBuilder();
            sb.Append("{x:Static ");
            if (CodeEngine.LocalizationNamespaceName != GetNamespace())
                sb.Append("i18n:");

            sb.Append(resxName);
            sb.Append('.');
            sb.Append(key);
            sb.Append('}');

            return sb.ToString();
        }

        public IEnumerable<WpfMvvmPropertySetting> GetProperties()
        {
            return EntitySetting.PropertiesInCreateEditOrder
                                .OfType<WpfMvvmPropertySetting>()
                                .Where(p => p.IncludeInView
                                         && p.IsSimpleOrString)
                                .ToList();
        }

        public WpfMvvmPropertySetting GetFocusProperty()
        {
            if (!_focusPropertyInitialized)
            {
                var prop = GetProperties().FirstOrDefault();
                bool isReadOnly = IsEdit
                                ? prop.IsReadOnlyInEdit
                                : prop.IsReadOnlyInCreate;
                _focusProperty = isReadOnly || prop.IsBoolean ? null : prop;
                _focusPropertyInitialized = true;
            }

            return _focusProperty;
        }
        private WpfMvvmPropertySetting _focusProperty;
        private bool _focusPropertyInitialized = false;

        public string GetElementName(WpfMvvmPropertySetting propertySetting, bool includeNameTag = false)
        {
            string enumName = propertySetting.GetEnumTypeName();
            var lookupEntity = propertySetting.GetLookupEntity();
            var sb = new StringBuilder();
            if (includeNameTag)
                sb.Append("x:Name=\"");

            sb.Append(propertySetting.Name);
            if (propertySetting.IsBoolean)
            {
                sb.Append("CheckBox");
            }
            else if (propertySetting.IsDateTime)
            {
                sb.Append("DatePicker");
            }
            else if (!string.IsNullOrEmpty(enumName) || lookupEntity != null)
            {
                sb.Append("ComboBox");
            }
            else if (propertySetting.IsSimpleOrString)
            {
                sb.Append("TextBox");
            }
            else
            {
                sb.Append("Control");
            }

            if (includeNameTag)
                sb.Append("\" ");

            return sb.ToString();
        }

        public int GetMinHeight()
        {
            int baseHeight = 160;
            int propertyHeight = 32;
            int propertiesCount = GetProperties().Count();
            int multilinePropertiesCount = GetProperties().Count(p => p.IsMultiLine);
            int minHeight = baseHeight + propertyHeight * propertiesCount + multilinePropertiesCount * 48;
            return (int)(Math.Ceiling(minHeight * 0.10m) * 10); // round up to the nearest 10
        }

        public string GetTitle()
        {
            bool hasLocalization = CodeEngine.ResxEngine != null;
            switch (TemplateType)
            {
                case MvvmTemplate.Create:
                    return hasLocalization ? GetLocalizedString($"Create{EntitySetting.Name}") : $"Create {EntitySetting.Name}";

                case MvvmTemplate.Edit:
                    return hasLocalization ? GetLocalizedString($"Edit{EntitySetting.Name}") : $"Edit {EntitySetting.Name}";

                case MvvmTemplate.Delete:
                    return hasLocalization ? GetLocalizedString($"Delete{EntitySetting.Name}") : $"Delete {EntitySetting.Name}";

                case MvvmTemplate.Collection:
                    return hasLocalization ? GetLocalizedString($"{EntitySetting.Name}List") : $"{EntitySetting.Name} Collection";

                default:
                    break;
            }

            return EntitySetting.Name;
        }
        public WpfMvvmCodeEngine CodeEngine => EntitySetting.CodeEngine as WpfMvvmCodeEngine;

        public WpfMvvmEntitySetting EntitySetting { get; }

        /// <summary>
        /// If true, it indicates the template is for edit view, otherwise it's for create view. This can be used to determine the class name and included properties.
        /// </summary>
        public bool IsEdit => TemplateType == MvvmTemplate.Edit;

        /// <summary>
        /// If true, it indicates the template is for edit view, otherwise it's for create view. This can be used to determine the class name and included properties.
        /// </summary>
        public bool IsCreateOrEdit => TemplateType == MvvmTemplate.Create || TemplateType == MvvmTemplate.Edit;

        public MvvmTemplate TemplateType { get; }


        public string BaseClassDeclaration
        {
            get
            {
                if (_baseClassDeclaration != null)
                    return _baseClassDeclaration;

                if (TemplateType != MvvmTemplate.Edit)
                {
                    _baseClassDeclaration = "Window";
                    return _baseClassDeclaration;
                }

                if (CodeEngine.ViewNamespaceName != GetNamespace())
                {
                    _baseClassDeclaration = $"base:{GetBaseClassName()}";
                }
                else
                {
                    _baseClassDeclaration = $"local:{GetBaseClassName()}";
                }

                return _baseClassDeclaration;
            }
        }
        private string _baseClassDeclaration;


        public string BaseClassNamespaceDeclaration
        {
            get
            {
                if (_baseClassNamespaceDeclaration != null)
                    return _baseClassNamespaceDeclaration;

                if (TemplateType == MvvmTemplate.Edit && CodeEngine.ViewNamespaceName != GetNamespace())
                    _baseClassNamespaceDeclaration = $"clr-namespace:{CodeEngine.ViewNamespaceName}";
                else
                    _baseClassNamespaceDeclaration = string.Empty;

                return _baseClassNamespaceDeclaration;
            }
        }
        private string _baseClassNamespaceDeclaration;

        public string LocalizationNamespaceDeclaration
        {
            get
            {
                if (CodeEngine.LocalizationNamespaceName == GetNamespace())
                    _localizationNamespaceDeclaration = string.Empty;

                if (_localizationNamespaceDeclaration != null)
                    return _localizationNamespaceDeclaration;

                var sb = new StringBuilder();
                sb.Append("clr-namespace:");
                sb.Append(CodeEngine.LocalizationNamespaceName);

                if (!string.IsNullOrWhiteSpace(CodeEngine.LocalizationAssemblyName))
                {
                    sb.Append(";assembly=");
                    sb.Append(CodeEngine.LocalizationAssemblyName);
                }

                _localizationNamespaceDeclaration = sb.ToString();
                return _localizationNamespaceDeclaration;
            }
        }
        private string _localizationNamespaceDeclaration;
    }
}
