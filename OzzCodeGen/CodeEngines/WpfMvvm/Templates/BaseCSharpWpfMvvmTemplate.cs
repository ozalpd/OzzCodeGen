using OzzCodeGen.CodeEngines.Mvvm;
using OzzCodeGen.CodeEngines.Mvvm.Templates;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public abstract class BaseCSharpWpfMvvmTemplate : BaseCSharpMvvmTemplate
    {
        protected BaseCSharpWpfMvvmTemplate(WpfMvvmCodeEngine codeEngine, WpfMvvmEntitySetting entitySetting = null, MvvmTemplate templateType = MvvmTemplate.Create, bool isInterface = false)
        {
            CodeEngine = codeEngine;
            EntitySetting = entitySetting;
            IsInterface = isInterface;
            TemplateType = templateType;
        }

        public WpfMvvmCodeEngine CodeEngine { get; }

        public WpfMvvmEntitySetting EntitySetting { get; }

        /// <summary>
        /// Gets a value indicating whether the current template represents a create operation. This can be used to determine the class name and included properties.
        /// </summary>
        public bool IsCreate => TemplateType == MvvmTemplate.Create;

        /// <summary>
        /// Gets a value indicating whether the current template represents an edit operation. This can be used to determine the class name and included properties.
        /// </summary>
        public bool IsEdit => TemplateType == MvvmTemplate.Edit;

        public MvvmTemplate TemplateType { get; }

        /// <summary>
        /// If true, it indicates the template is for an interface, otherwise it's for a class.
        /// </summary>
        public bool IsInterface { get; }

        public virtual IEnumerable<WpfMvvmPropertySetting> GetIncludedProperties()
        {
            if (EntitySetting == null)
                return Enumerable.Empty<WpfMvvmPropertySetting>();

            if (_includedProperties != null)
                return _includedProperties;

            _includedProperties = EntitySetting.GetInheritedIncludedProperties()
                                               .OfType<WpfMvvmPropertySetting>()
                                               .ToList();

            return _includedProperties;
        }
        private IEnumerable<WpfMvvmPropertySetting> _includedProperties;

        public string GetLookupContractNamespace()
        {
            if (string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
                return CodeEngine.LookupNamespaceName;

            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.LookupFolder)}";
        }

        public IEnumerable<WpfMvvmEntitySetting> GetForeignLookupEntities(MvvmTemplate? templateType = null)
        {
            if (templateType == null)
                templateType = TemplateType;

            if ((templateType != MvvmTemplate.Create && templateType != MvvmTemplate.Edit)
                || EntitySetting == null)
                return Enumerable.Empty<WpfMvvmEntitySetting>();


            if (templateType == MvvmTemplate.Create && _flookupEntities == null)
            {
                _flookupEntities = EntitySetting.GetForeignLookupEntities(isForEdit: false);
                return _flookupEntities;
            }

            if (templateType == MvvmTemplate.Edit && _flookupEntitiesForEdit == null)
            {
                _flookupEntitiesForEdit = EntitySetting.GetForeignLookupEntities(isForEdit: true);
                return _flookupEntitiesForEdit;
            }

            return templateType == MvvmTemplate.Edit ? _flookupEntitiesForEdit : _flookupEntities;
        }
        private IEnumerable<WpfMvvmEntitySetting> _flookupEntities;
        private IEnumerable<WpfMvvmEntitySetting> _flookupEntitiesForEdit;

        public List<WpfMvvmPropertySetting> GetPreselectProperties()
        {
            if (_preselectProperties == null)
            {
                _preselectProperties = EntitySetting.GetPreselectProperties();
            }

            return _preselectProperties;
        }
        private List<WpfMvvmPropertySetting> _preselectProperties;

        public bool HasLookupOrPreselectProperties(MvvmTemplate? templateType = null)
        {
            var foreignLookupEntities = GetForeignLookupEntities(templateType);
            var preselectProperties = GetPreselectProperties();
            return (foreignLookupEntities != null && foreignLookupEntities.Any())
                || (preselectProperties != null && preselectProperties.Count > 0);
        }

        protected virtual WpfMvvmPropertySetting GetPrimaryKey()
        {
            if (EntitySetting == null)
                return null;

            return EntitySetting.GetInheritedSimpleProperties()
                                .FirstOrDefault(p => p.IsKey);
        }

        public string GetShowViewParams(bool isDeclaration = false, bool isDlgSvc = false, bool isDesignTime = false)
        {
            return GetShowViewParams(EntitySetting, isDeclaration, isEdit: IsEdit, isDlgSvc: isDlgSvc, isDesignTime: isDesignTime);
        }

        public string GetShowViewParams(WpfMvvmEntitySetting entitySetting, bool isDeclaration = false, bool isEdit = false, bool isDlgSvc = false, bool isDesignTime = false)
        {
            var sb = new StringBuilder();
            int idx = 0;
            int lineBrkInterval = isDeclaration
                                ? 2
                                : isDesignTime ? 8 : 4;
            int indent = (3 * entitySetting.Name.Length) + (isEdit ? 2 : 0) + (isDesignTime ? 24 : 48) + (IsInterface ? 0 : 7);
            if (isDlgSvc)
            {
                if (isDeclaration)
                    sb.Append("Window ");
                sb.Append("owner");
                idx++;
            }

            if (isEdit)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                if (isDeclaration)
                {
                    sb.Append(entitySetting.Name);
                    sb.Append(' ');
                }
                sb.Append(entitySetting.Name.ToCamelCase());
                idx++;
            }

            var foreignLookupEntities = entitySetting.GetForeignLookupEntities(isForEdit: isEdit);
            foreach (var lookupEntity in foreignLookupEntities)
            {
                AddCommaOrSpace(sb, idx, lineBrkInterval, indent);
                idx++;

                if (isDesignTime)
                {
                    sb.Append("new "); // instantiate the design time class for the view designer
                    sb.Append(lookupEntity.GetLookupName(LookupTemplate.DesignTimeClass));
                    sb.Append("()");
                    continue; // For design time there must be no declaration
                }


                if (isDeclaration)
                {
                    sb.Append(lookupEntity.GetLookupName(LookupTemplate.Interface));
                    sb.Append(' ');
                }

                sb.Append(lookupEntity.GetLookupName(LookupTemplate.RunTimeClass).ToCamelCase());
            }

            if (isEdit)
                return sb.ToString();

            var preselectProperties = entitySetting.GetPreselectProperties();
            foreach (var property in preselectProperties)
            {
                AddCommaOrSpace(sb, idx, lineBrkInterval, indent);
                idx++;
                if (isDesignTime)
                {
                    sb.Append("null");
                    continue; // For design time there must be no declaration
                }

                if (isDeclaration)
                {
                    sb.Append(property.GetTypeName(getReturnType: true));
                    sb.Append("? ");
                }
                sb.Append("preselected");
                sb.Append(property.Name);
            }

            return sb.ToString();
        }

        public string GetVmConstructorParams(bool isDeclaration = false)
        {
            var sb = new System.Text.StringBuilder();
            if (IsEdit)
            {
                if (isDeclaration)
                {
                    sb.Append(EntitySetting.Name);
                    sb.Append(' ');
                }
                sb.Append(EntitySetting.Name.ToCamelCase());
            }

            var foreignLookupEntities = EntitySetting.GetForeignLookupEntities(IsEdit);
            foreach (var lookupEntity in foreignLookupEntities)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                if (isDeclaration)
                {
                    sb.Append(lookupEntity.GetLookupName(LookupTemplate.Interface));
                    sb.Append(' ');
                }
                sb.Append(lookupEntity.GetLookupName(LookupTemplate.RunTimeClass).ToCamelCase());
            }

            return sb.ToString();
        }

        static void AddCommaOrSpace(StringBuilder sb, int paramIdx, int lineBrkInterval, int indentWidth)
        {
            if (paramIdx > 0 && paramIdx % lineBrkInterval == 0)
            {
                sb.Append(",\r\n");
                sb.Append(' ', indentWidth);
            }
            else if (sb.Length > 0)
            {
                sb.Append(", ");
            }
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
