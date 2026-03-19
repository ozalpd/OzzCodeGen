using OzzCodeGen.Definitions;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.ModelClass
{
    public abstract class BaseModelClassPropertySetting : BasePropertySetting
    {
        [XmlIgnore]
        [JsonIgnore]
        public IModelClassEntitySetting ModelClassEntitySetting
        {
            get { return (IModelClassEntitySetting)EntitySetting; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public BaseModelClassCodeEngine CodeEngine
        {
            get { return ModelClassEntitySetting.CodeEngine; }
        }

        public string CustomAttributes
        {
            get { return _custValid; }
            set
            {
                _custValid = value;
                RaisePropertyChanged("CustomAttributes");
            }
        }
        private string _custValid;

        public string DataFormatString
        {
            get { return _dataFormatString; }
            set
            {
                _dataFormatString = value;
                RaisePropertyChanged("DataFormatString");
            }
        }
        private string _dataFormatString;

        public string DataType
        {
            get
            {
                if (string.IsNullOrEmpty(_dataType) && IsSimpleOrString)
                {
                    _dataType = GetDefaultDataType();
                }
                return _dataType;
            }
            set
            {
                _dataType = value;
                RaisePropertyChanged("DataType");
            }
        }
        private string _dataType;

        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
                RaisePropertyChanged("DefaultValue");
            }
        }
        private string _defaultValue;

        public bool DTOExclusion
        {
            get { return _dtoExclusion; }
            set
            {
                if (_dtoExclusion == value) return;
                _dtoExclusion = value;
                RaisePropertyChanged("DTOExclusion");
            }
        }
        private bool _dtoExclusion;

        private string GetDefaultDataType()
        {
            if (EntitySetting?.DataModel == null)
                return string.Empty;

            var lowerName = Name.ToLowerInvariant();
            if (lowerName.Contains("phone"))
                return DataTypes.PhoneNumber;
            else if (lowerName.Contains("email") || lowerName.Contains("e-mail"))
                return DataTypes.EmailAddress;
            else if (lowerName.Equals("postalcode") || lowerName.Equals("zip") || lowerName.Equals("zipcode"))
                return DataTypes.PostalCode;
            else if (lowerName.Equals("description") || lowerName.Contains("notes"))
                return DataTypes.MultilineText;
            else if (lowerName.Equals("password"))
                return DataTypes.Password;
            else if (lowerName.Contains("imageurl"))
                return DataTypes.ImageUrl;
            else if (lowerName.EndsWith("url"))
                return DataTypes.Url;
            else if (lowerName.EndsWith("date"))
                return DataTypes.Date;
            else if (lowerName.Contains("datetime"))
                return DataTypes.DateTime;
            else if (lowerName.Contains("date"))
                return DataTypes.Date;

            return string.Empty;
        }

        public string GetDisplayAttrib()
        {
            if (!string.IsNullOrEmpty(ResourceName))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[Display(ResourceType = typeof(");
                sb.Append(ResourceName);
                sb.Append("), Name = \"");
                sb.Append(Name);
                sb.Append("\")]");

                return sb.ToString();
            }
            if (!string.IsNullOrEmpty(PropertyDefinition.DisplayName))
            {
                return "[DisplayName(\"" + PropertyDefinition.DisplayName + "\")]";
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetRangeResource()
        {
            if (string.IsNullOrEmpty(Range))
                return string.Empty;

            if (Range.Contains(".MaxValue"))
            {
                return "ValueMin";
            }

            var propDef = PropertyDefinition as SimpleProperty;
            if (propDef == null || string.IsNullOrEmpty(ResourceName))
                return string.Empty;

            if (propDef.IsTypeIntNumeric())
            {
                return "RangeInt";
            }
            else if (propDef.IsTypeDecimalNumeric())
            {
                return "RangeDecimal";
            }
            else if (propDef.IsTypeDateTime())
            {
                return "RangeDateTime";
            }

            return string.Empty;
        }

        public string GetTypeName()
        {
            StringBuilder sb = new StringBuilder();
            if (PropertyDefinition.IsTypeNumeric() && PropertyDefinition is SimpleProperty)
            {
                sb.Append(((SimpleProperty)PropertyDefinition).EnumTypeName);
            }
            if (sb.Length == 0)
            {
                sb.Append(PropertyDefinition.TypeName);
            }
            if (PropertyDefinition is SimpleProperty && !PropertyDefinition.IsTypeString() && ((SimpleProperty)PropertyDefinition).IsNullable)
            {
                sb.Append("?");
                return sb.ToString();
            }

            bool isClassic = CodeEngine.Project.TargetPlatform == TargetDotNetPlatform.DotNetFramework;
            if (isClassic || PropertyDefinition is CollectionProperty)
                return sb.ToString();

            if (PropertyDefinition is ComplexProperty && ((ComplexProperty)PropertyDefinition).Dependency?.IsNullable == true)
            {
                sb.Append("?");
                return sb.ToString();
            }

            if (PropertyDefinition is StringProperty && ((SimpleProperty)PropertyDefinition).IsNullable)
            {
                sb.Append("?");
            }

            return sb.ToString();
        }

        protected string GetDefaultRequiredMsg(BaseProperty PropertyDefinition)
        {
            if (PropertyDefinition == null || PropertyDefinition.IsTypeBoolean())
            {
                return string.Empty;
            }

            if (PropertyDefinition is SimpleProperty)
            {
                var simp = PropertyDefinition as SimpleProperty;
                if (simp.IsKey || simp.IsNullable || simp.IsServerComputed || simp.IsStoreGenerated)
                {
                    return string.Empty;
                }
                if (CodeEngine.UseResourceFiles && CodeEngine.ResxEngine != null &&
                    !string.IsNullOrEmpty(CodeEngine.ResxEngine.ErrorResxFilename))
                {
                    var resxEngine = CodeEngine.ResxEngine;
                    var reqString = "Required";
                    var errorResx = resxEngine.ErrorResxFilename;
                    return string.Format("ErrorMessageResourceType = typeof({0}), ErrorMessageResourceName = \"Required\"",
                               errorResx, reqString);
                }
                else
                {
                    return "[Required]";
                }
            }

            return string.Empty;
        }

        public bool? HtmlEncode
        {
            get
            {
                return _htmlEncode;
            }
            set
            {
                _htmlEncode = value;
                RaisePropertyChanged("HtmlEncode");
            }
        }
        private bool? _htmlEncode;

        public string Range
        {
            get { return _range; }
            set
            {
                if (_range == value) return;
                _range = value;
                RaisePropertyChanged("Range");
            }
        }
        private string _range;

        public string RegularExpression
        {
            get { return _regularExpression; }
            set
            {
                if (_regularExpression == value) return;
                _regularExpression = value;
                RaisePropertyChanged("RegularExpression");
            }
        }
        private string _regularExpression;

        public string Required
        {
            get
            {
                if (string.IsNullOrEmpty(_required))
                {
                    _required = GetDefaultRequiredMsg(PropertyDefinition);
                }
                return _required;
            }
            set
            {
                if (_required == value) return;
                _required = value;
                RaisePropertyChanged("Required");
            }
        }
        private string _required;

        /// <summary>
        /// Gets the name of the resource file associated with the current model entity, if resource files are used.
        /// </summary>
        /// <remarks>If resource files are not enabled, this property returns an empty string. When
        /// resource files are enabled, the returned value depends on whether a single resource file is used or a
        /// per-entity resource file is configured. This property is intended for use by derived classes that need to
        /// access the resolved resource file name for localization or code generation purposes.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        protected string ResourceName
        {
            get
            {
                if (!CodeEngine.UseResourceFiles)
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(_resourceName))
                {
                    var resxEngine = CodeEngine.ResxEngine;

                    if (resxEngine != null && resxEngine.SingleResx)
                    {
                        _resourceName = resxEngine.SingleResxFilename;
                    }
                    else if (resxEngine != null)
                    {
                        var resxEntity = resxEngine
                                        .Entities
                                        .FirstOrDefault(e => e.Name.Equals(ModelClassEntitySetting.Name));
                        _resourceName = resxEngine.GetDefaultTargetFile(resxEntity);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return _resourceName;
            }
        }
        string _resourceName;

        public string UIHint
        {
            get { return _uiHint; }
            set
            {
                if (_uiHint == value) return;
                _uiHint = value;
                RaisePropertyChanged("UIHint");
            }
        }
        private string _uiHint;
    }
}
