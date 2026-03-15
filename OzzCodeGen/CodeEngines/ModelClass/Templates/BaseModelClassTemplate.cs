using OzzCodeGen.Definitions;
using OzzCodeGen.Templates.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.ModelClass.Templates
{
    public abstract class BaseModelClassTemplate : CsClassBase
    {
        public string ClassName
        {
            get
            {
                if (MetadataForDTO)
                {
                    return EntitySetting.Name + "DTO";
                }
                else
                {
                    return EntitySetting.Name;
                }
            }
        }

        public abstract BaseModelClassCodeEngine CodeEngine { get; }

        public virtual IModelClassEntitySetting EntitySetting { get; protected set; }

        public bool MetadataForDTO { get; protected set; }


        private static void AddDataTypeAttrib(BaseModelClassPropertySetting property, List<string> attributes)
        {
            if (!string.IsNullOrEmpty(property.DataType) &&
                !property.DataType.Equals(DataTypes.NotSpecial, StringComparison.InvariantCultureIgnoreCase) &&
                (property.PropertyDefinition is StringProperty || property.PropertyDefinition is SimpleProperty))
            {
                if (property.DataType.Equals(DataTypes.EmailAddress))
                {
                    attributes.Add("[EmailAddress]");
                }
                else
                {
                    attributes.Add("[DataType(DataType." + property.DataType + ")]");
                }
            }
        }

        private static void AddDisplayFormat(BaseModelClassPropertySetting property, List<string> attributes)
        {
            if (property.HtmlEncode == null && string.IsNullOrEmpty(property.DataFormatString))
                return;

            bool hasAttrib = false;
            var sb = new StringBuilder();
            sb.Append("[DisplayFormat(");
            if (property.HtmlEncode.HasValue)
            {
                sb.Append("HtmlEncode = ");
                sb.Append(property.HtmlEncode.Value.ToString().ToLowerInvariant());

                hasAttrib = true;
            }


            if (!string.IsNullOrEmpty(property.DataFormatString))
            {
                if (hasAttrib)
                    sb.Append(", ");

                sb.Append("DataFormatString = \"{");
                sb.Append(property.DataFormatString);
                sb.Append("}\", ApplyFormatInEditMode = true");

                hasAttrib = true;
            }

            sb.Append(")]");

            attributes.Add(sb.ToString());
        }

        private void AddKeyAttrib(BaseModelClassPropertySetting property, List<string> attributes)
        {
            if (property.PropertyDefinition is SimpleProperty)
            {
                SimpleProperty definition = (SimpleProperty)property.PropertyDefinition;
                if (definition.IsKey) attributes.Add("[Key]");
            }
        }

        private void AddMaxLengthAttrib(BaseModelClassPropertySetting property, List<string> attributes)
        {
            if (property.PropertyDefinition is StringProperty
                && ((StringProperty)property.PropertyDefinition).MaxLength > 0)
            {
                StringProperty definition = (StringProperty)property.PropertyDefinition;
                var resxEngine = CodeEngine.ResxEngine;

                if (CodeEngine.UseResourceFiles && resxEngine != null &&
                    !string.IsNullOrEmpty(resxEngine.ErrorResxFilename))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[StringLength(");
                    sb.Append(definition.MaxLength);
                    sb.Append(", ErrorMessageResourceType = typeof(");
                    //sb.Append(resxEngine.NamespaceName);
                    //sb.Append('.');
                    sb.Append(resxEngine.ErrorResxFilename);
                    sb.Append("), ErrorMessageResourceName = \"MaxLength\")]");
                    attributes.Add(sb.ToString());
                }
                else
                {
                    attributes.Add(string.Format("[StringLength({0})]", definition.MaxLength));
                }
            }
        }

        private static void AddRequiredAttrib(BaseModelClassPropertySetting property, List<string> attributes)
        {
            if (!string.IsNullOrEmpty(property.Required))
            {
                if (property.Required.Equals("[Required]", StringComparison.InvariantCultureIgnoreCase))
                {
                    attributes.Add("[Required]");
                }
                else
                {
                    attributes.Add("[Required(" + property.Required + ")]");
                }
            }
        }

        public IEnumerable<BaseModelClassPropertySetting> GetInheritedIncludedProperties()
        {
            var inheritedProperties = EntitySetting.GetInheritedProperties();
            if (MetadataForDTO)
            {
                return inheritedProperties
                    .Where(p => p.IsSimpleOrString && !p.DTOExclusion && !p.Exclude)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
            }
            else
            {
                return inheritedProperties
                    .Where(c => !c.Exclude)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
            }
        }

        protected List<string> GetPropertyAttributes(BaseModelClassPropertySetting property)
        {
            var attributes = new List<string>();
            if (!string.IsNullOrEmpty(property.CustomAttributes))
            {
                var custAttribs = property.CustomAttributes.Split(';');
                foreach (var cust in custAttribs)
                {
                    if (!string.IsNullOrWhiteSpace(cust))
                        attributes.Add($"[{cust}]");
                }
            }

            AddKeyAttrib(property, attributes);
            AddDisplayFormat(property, attributes);
            AddMaxLengthAttrib(property, attributes);
            AddRequiredAttrib(property, attributes);
            AddDataTypeAttrib(property, attributes);

            if (!string.IsNullOrEmpty(property.UIHint))
                attributes.Add("[UIHint(\"" + property.UIHint + "\")]");

            if (!string.IsNullOrEmpty(property.RegularExpression))
                attributes.Add("[RegularExpression(" + property.RegularExpression + ")]");

            if (!string.IsNullOrEmpty(property.Range))
                attributes.Add("[Range(" + property.Range + ")]");

            if (!string.IsNullOrEmpty(property.DefaultValue))
                attributes.Add("[DefaultValue(" + property.DefaultValue + ")]");

            string displayAttrib = property.GetDisplayAttrib();
            if (!string.IsNullOrEmpty(displayAttrib))
            {
                attributes.Add(displayAttrib);
            }

            return attributes;
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            NamespaceName = EntitySetting.CodeEngine.NamespaceName;
            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
