using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.AppEngines.Localization
{
    public class LocalizationPropertySetting : BasePropertySetting
    {
        public bool LocalizeRequiredMsg
        {
            get
            {
                if (!(_requiredMsg.HasValue) && PropertyDefinition is SimpleProperty)
                {
                    var simple = (SimpleProperty)PropertyDefinition;
                    bool isBool = simple.IsTypeBoolean();
                    _requiredMsg = !isBool & !simple.IsNullable;
                }
                else if (!(_requiredMsg.HasValue))
                {
                    _requiredMsg = false;
                }
                return _requiredMsg.Value;
            }
            set
            {
                _requiredMsg = value;
                RaisePropertyChanged("LocalizeRequiredMsg");
            }
        }
        private bool? _requiredMsg;

        public void ResetLocalizeRequiredMsg()
        {
            if (PropertyDefinition is SimpleProperty && (!(_requiredMsg ?? false)))
            {
                _requiredMsg = null;
                if (LocalizeRequiredMsg)
                {
                    RaisePropertyChanged("LocalizeRequiredMsg");
                }
            }
        }
        
        public bool LocalizeValidationMsg
        {
            get { return _validationMsg; }
            set
            {
                _validationMsg = value;
                RaisePropertyChanged("LocalizeValidationMsg");
            }
        }
        private bool _validationMsg;
    }
}
