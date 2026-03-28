using OzzCodeGen.Definitions;
using OzzCodeGen.Templates.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.ModelClass.Templates
{
    public abstract class BaseCSharpModelClassTemplate : CsClassBase
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


                if (!string.IsNullOrEmpty(ErrorResxFilename))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[StringLength(");
                    sb.Append(definition.MaxLength);
                    sb.Append(", ErrorMessageResourceType = typeof(");
                    //sb.Append(resxEngine.NamespaceName);
                    //sb.Append('.');
                    sb.Append(ErrorResxFilename);
                    sb.Append("), ErrorMessageResourceName = \"MaxStringLength\")]");
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

        protected string ErrorResxFilename
        {
            get
            {
                var resxEngine = CodeEngine.UseResourceFiles
                               ? CodeEngine.ResxEngine : null;

                return resxEngine != null
                     ? resxEngine.ErrorResxFilename
                     : string.Empty;
            }
        }

        /// <summary>
        /// Retrieves all property settings that represent collection properties for the current model class,
        /// including inherited properties.
        /// </summary>
        /// <remarks>Collection properties are identified by their property definition type. This method
        /// includes both directly declared and inherited collection properties.</remarks>
        /// <returns>An enumerable collection of property settings corresponding to collection properties defined or inherited by
        /// the model class.</returns>
        public IEnumerable<BaseModelClassPropertySetting> GetCollectionProperties()
        {
            if (_collectionProperties != null)
                return _collectionProperties;

            var properties = GetInheritedIncludedProperties();
            _collectionProperties = properties.Where(p => p.PropertyDefinition is CollectionProperty);
            return _collectionProperties;
        }
        IEnumerable<BaseModelClassPropertySetting> _collectionProperties = null;


        /// <summary>
        /// Retrieves all navigation-like properties that point to another entity type,
        /// including inherited properties.
        /// </summary>
        /// <returns>An enumerable collection of property settings corresponding to navigation-like properties defined or inherited by
        /// the model class.</returns>
        public IEnumerable<BaseModelClassPropertySetting> GetComplexProperties()
        {
            if (_complexProperties != null)
                return _complexProperties;

            var properties = GetInheritedIncludedProperties();
            _complexProperties = properties.Where(p => p.PropertyDefinition is ComplexProperty);
            return _complexProperties;
        }
        IEnumerable<BaseModelClassPropertySetting> _complexProperties = null;

        public IEnumerable<BaseModelClassPropertySetting> GetInheritedIncludedProperties()
        {
            if (_inheritedProperties != null)
                return _inheritedProperties;

            var inheritedProperties = EntitySetting.GetInheritedProperties();
            if (MetadataForDTO)
            {
                _inheritedProperties = inheritedProperties
                    .Where(p => p.IsSimpleOrString && !p.DTOExclusion && !p.Exclude)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
            }
            else
            {
                _inheritedProperties = inheritedProperties
                    .Where(c => !c.Exclude)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
            }
            return _inheritedProperties;
        }
        IEnumerable<BaseModelClassPropertySetting> _inheritedProperties = null;

        /// <summary>
        /// Retrieves all SimpleProperty and StringProperty settings for the current model class, including inherited properties.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{BaseModelClassPropertySetting}"/> containing the property settings for all simple
        /// and string properties. The collection is empty if no simple or string properties are present.</returns>
        public IEnumerable<BaseModelClassPropertySetting> GetSimpleProperties()
        {
            if (_simpleProperties != null)
                return _simpleProperties;

            var properties = GetInheritedIncludedProperties();
            _simpleProperties = properties.Where(p => p.PropertyDefinition is SimpleProperty);
            return _simpleProperties;
        }
        IEnumerable<BaseModelClassPropertySetting> _simpleProperties = null;

        /// <summary>
        /// Returns an enumerable collection of property settings that represent string properties for the current model
        /// class.
        /// </summary>
        /// <remarks>The returned collection includes only those properties whose definitions are of type
        /// <see cref="StringProperty"/>. The result is cached for subsequent calls.</remarks>
        /// <returns>An <see cref="IEnumerable{BaseModelClassPropertySetting}"/> containing the property settings for all string
        /// properties. The collection is empty if no string properties are present.</returns>
        public IEnumerable<BaseModelClassPropertySetting> GetStringProperties()
        {
            if (_stringProperties != null)
                return _stringProperties;

            var properties = GetInheritedIncludedProperties();
            _stringProperties = properties.Where(p => p.PropertyDefinition is StringProperty);
            return _stringProperties;
        }
        IEnumerable<BaseModelClassPropertySetting> _stringProperties = null;

        /// <summary>
        /// Generates a list of attribute strings to be applied to a property based on its settings.
        /// </summary>
        /// <remarks>The returned attribute strings are formatted for direct inclusion in generated C#
        /// code. Custom attributes, validation attributes, and display-related attributes are included based on the
        /// values present in the provided property settings.</remarks>
        /// <param name="property">The property settings used to determine which attributes to generate.</param>
        /// <returns>A list of strings, each representing an attribute to be applied to the property. The list may be empty if no
        /// attributes are applicable.</returns>
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

            string rangeString = property.Range;
            if (!string.IsNullOrEmpty(property.Range) && !string.IsNullOrEmpty(ErrorResxFilename))
            {
                string rangeResource = property.GetRangeResource();
                if (!string.IsNullOrEmpty(rangeResource))
                {
                    rangeString = $"{property.Range}, ErrorMessageResourceType = typeof({ErrorResxFilename}), ErrorMessageResourceName =\"{rangeResource}\"";
                }
            }

            if (!string.IsNullOrEmpty(property.Range))
            {
                attributes.Add($"[Range({rangeString})]");
            }

            if (!string.IsNullOrEmpty(property.DefaultValue))
            {
                attributes.Add("[DefaultValue(" + property.DefaultValue + ")]");
            }

            string displayAttrib = property.GetDisplayAttrib();
            if (!string.IsNullOrEmpty(displayAttrib))
            {
                attributes.Add(displayAttrib);
            }

            return attributes;
        }


        public TargetDotNetPlatform TargetPlatform => CodeEngine.Project.TargetPlatform;

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
