using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.AppEngines.Localization
{
    public class LocalizationEntitySetting : BaseEntitySetting
    {
        [XmlIgnore]
        public string BaseTypeName
        {
            get { return EntityDefinition.BaseTypeName; }
        }

        public List<LocalizationPropertySetting> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<LocalizationPropertySetting>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<LocalizationPropertySetting> _properties;
    }
}
