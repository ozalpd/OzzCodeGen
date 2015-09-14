using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines
{
    public class EntitySetting : BaseEntitySetting
    {
        public List<PropertySetting> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<PropertySetting>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<PropertySetting> _properties;
    }
}
