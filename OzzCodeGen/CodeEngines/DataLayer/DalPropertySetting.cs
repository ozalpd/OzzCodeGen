using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using OzzCodeGen.Templates;

namespace OzzCodeGen.CodeEngines.DataLayer
{
    public class DalPropertySetting : BasePropertySetting
    {
        public bool InMetadata
        {
            get
            {
                if (!_inMetadata.HasValue)
                {
                    _inMetadata = true;
                }
                return _inMetadata.Value;
            }
            set
            {
                _inMetadata = value;
                RaisePropertyChanged("InMetadata");
            }
        }
        private bool? _inMetadata;

        public bool InViewModel
        {
            get
            {
                if (!_inViewModel.HasValue)
                {
                    _inViewModel = true;
                }
                return _inViewModel.Value;
            }
            set
            {
                _inViewModel = value;
                RaisePropertyChanged("InViewModel");
            }
        }
        private bool? _inViewModel;

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

        /// <summary>
        /// DataType of the Property
        /// </summary>
        public string DataType
        {
            get { return _DataType; }
            set
            {
                if (_DataType == value) return;
                _DataType = value;
                RaisePropertyChanged("DataType");
            }
        }
        private string _DataType;

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

        public string GetDisplayAttrib()
        {
            if (!string.IsNullOrEmpty(PropertyDefinition.DisplayName))
            {
                return "[DisplayName(\"" + PropertyDefinition.DisplayName + "\")]";
            }
            else
            {
                return string.Empty;
            }
        }

        [XmlIgnore]
        protected string ResourceName
        {
            get
            {
                if (string.IsNullOrEmpty(_resourceName))
                {
                    var entity = (DalEntitySetting)EntitySetting;
                    if (entity.CodeEngine.ResxEngine != null)
                    {
                        _resourceName = entity.CodeEngine.ResxEngine.SingleResx ? entity.CodeEngine.ResxEngine.SingleResxFilename :
                                            string.Format("Resources.{0}String", PropertyDefinition.EntityDefinition.Name);
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

        protected string GetDefaultRequiredMsg(BaseProperty PropertyDefinition)
        {
            if (PropertyDefinition == null || PropertyDefinition.IsTypeBoolean())
            {
                return string.Empty;
            }

            if (PropertyDefinition is SimpleProperty)
            {
                if (((SimpleProperty)PropertyDefinition).IsKey || ((SimpleProperty)PropertyDefinition).IsNullable)
                {
                    return string.Empty;
                }
                if (!string.IsNullOrEmpty(ResourceName))
                {
                    return string.Format("ErrorMessageResourceType = typeof({0}), ErrorMessageResourceName = \"{1}RequiredMsg\"",
                                   ResourceName, PropertyDefinition.Name);
                }
                else
                {
                    return "ErrorMessage = \"Please fill " + PropertyDefinition.DisplayName + " field!\"";
                }
            }

            return string.Empty;
        }


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
    }
}
