using OzzCodeGen.Definitions;
using OzzCodeGen.Templates.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.Metadata.Templates
{
    public partial class MetadataClassTemplate : CsClassBase
    {
        public MetadataClassTemplate(MetadataEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
        }

        public MetadataClassTemplate(MetadataEntitySetting entitySetting, bool forDTO)
            : this(entitySetting)
        {
            MetadataForDTO = forDTO;
        }

        public MetadataEntitySetting EntitySetting { get; private set; }
        public bool MetadataForDTO { get; private set; }

        protected List<string> GetPropertyAttributes(MetadataPropertySetting property)
        {
            var attributes = new List<string>();
            AddKeyAttrib(property, attributes);
            AddMaxLeghtAttrib(property, attributes);
            AddRequiredAttrib(property, attributes);
            AddDataTypeAttrib(property, attributes);

            if (!string.IsNullOrEmpty(property.UIHint))
                attributes.Add("[UIHint(\"" + property.UIHint + "\")]");

            if (!string.IsNullOrEmpty(property.RegularExpression))
                attributes.Add("[RegularExpression(" + property.RegularExpression + ")]");

            if (!string.IsNullOrEmpty(property.Range))
                attributes.Add("[Range(" + property.Range + ")]");


            string displayAttrib = property.GetDisplayAttrib();
            if (!string.IsNullOrEmpty(displayAttrib))
            {
                attributes.Add(displayAttrib);
            }

            return attributes;
        }

        private static void AddDataTypeAttrib(MetadataPropertySetting property, List<string> attributes)
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

        private static void AddRequiredAttrib(MetadataPropertySetting property, List<string> attributes)
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

        private void AddMaxLeghtAttrib(MetadataPropertySetting property, List<string> attributes)
        {
            if (property.PropertyDefinition is StringProperty && ((StringProperty)property.PropertyDefinition).MaxLenght > 0)
            {
                StringProperty definition = (StringProperty)property.PropertyDefinition;
                var resxEngine = EntitySetting.CodeEngine.ResxEngine;

                if (EntitySetting.CodeEngine.UseResourceFiles && resxEngine != null && 
                    !string.IsNullOrEmpty(resxEngine.ErrorResxFilename))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[StringLength(");
                    sb.Append(definition.MaxLenght);
                    sb.Append(", ErrorMessageResourceType = typeof(");
                    //sb.Append(resxEngine.NamespaceName);
                    //sb.Append('.');
                    sb.Append(resxEngine.ErrorResxFilename);
                    sb.Append("), ErrorMessageResourceName = \"MaxLeght\")]");
                    attributes.Add(sb.ToString());
                }
                else
                {
                    attributes.Add(string.Format("[StringLength({0})]", definition.MaxLenght));
                }
            }
        }

        private void AddKeyAttrib(MetadataPropertySetting property, List<string> attributes)
        {
            if (property.PropertyDefinition is SimpleProperty)
            {
                SimpleProperty definition = (SimpleProperty)property.PropertyDefinition;
                if (definition.IsKey) attributes.Add("[Key]");
            }
        }

        public IEnumerable<MetadataPropertySetting> GetInheritedIncludedProperties()
        {
            if (MetadataForDTO)
            {
                return EntitySetting
                            .GetInheritedIncludedProperties()
                            .Where(p => p.IsSimpleOrString && !p.DTOExclusion);
            }
            else
            {
                return EntitySetting.GetInheritedIncludedProperties();
            }
        }

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
        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            NamespaceName = EntitySetting.CodeEngine.NamespaceName;
            return base.WriteToFile(FilePath, overwriteExisting);
        }

        public override string GetDefaultFileName()
        {
            if (MetadataForDTO)
            {
                return EntitySetting.Name + "DTO.meta.cs";
            }
            else
            {
                return EntitySetting.Name + ".meta.cs";
            }
        }
    }
}
