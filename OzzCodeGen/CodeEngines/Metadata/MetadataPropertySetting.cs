using OzzCodeGen.Definitions;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public class MetadataPropertySetting : BasePropertySetting
    {
        [XmlIgnore]
        public MetadataEntitySetting MetadataEntitySetting
        {
            get { return (MetadataEntitySetting)EntitySetting; }
        }

        [XmlIgnore]
        public MetadataCodeEngine CodeEngine
        {
            get { return MetadataEntitySetting.CodeEngine; }
        }


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


        /// <summary>
        /// DataType of the Property
        /// </summary>
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
            //else if (lowerName.Equals("price") || lowerName.Contains("cost"))
            //    return DataTypes.Currency;
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
                    /*
                    var resxEntity = resxEngine
                                    .Entities
                                    .FirstOrDefault(e => e.Name.Equals(MetadataEntitySetting.Name));
                    var resxProperty = resxEntity == null ? null :
                                        resxEntity.Properties.FirstOrDefault(p => p.Name.Equals(Name));
                                        */
                    //TODO: get reqString from resxProperty
                    var reqString = "Required";
                    var errorResx = resxEngine.ErrorResxFilename; //resxEngine.MergeWithNamespace(resxEngine.ErrorResxFilename);
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
                //if (_htmlEncode == null)
                //    _htmlEncode = true;
                return _htmlEncode;
            }
            set
            {
                _htmlEncode = value;
                RaisePropertyChanged("HtmlEncode");
            }
        }
        private bool? _htmlEncode;


        /// <summary>
        /// Range of the Property
        /// </summary>
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

        /// <summary>
        /// RegularExpression of the Property
        /// </summary>
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


        /// <summary>
        /// Required of the Property
        /// </summary>
        public string Required
        {
            get
            {
                if (string.IsNullOrEmpty(_required))
                {
                    bool fl = this.EntitySetting.EntityDefinition == null;
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


        [XmlIgnore]
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
                        _resourceName = resxEngine.SingleResxFilename; //resxEngine.MergeWithNamespace(resxEngine.SingleResxFilename);
                    }
                    else if (resxEngine != null)
                    {
                        var resxEntity = resxEngine
                                        .Entities
                                        .FirstOrDefault(e => e.Name.Equals(MetadataEntitySetting.Name));
                        _resourceName = resxEngine.GetDefaultTargetFile(resxEntity); //resxEngine.MergeWithNamespace(resxEngine.GetDefaultTargetFile(resxEntity));
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


        /// <summary>
        /// UIHint of the Property
        /// </summary>
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
