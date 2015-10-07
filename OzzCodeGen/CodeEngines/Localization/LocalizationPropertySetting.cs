using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.Localization
{
    public class LocalizationPropertySetting : BasePropertySetting
    {
        public bool LocalizeRequiredMsg
        {
            get { return _requiredMsg; }
            set
            {
                _requiredMsg = value;
                RaisePropertyChanged("LocalizeRequiredMsg");
            }
        }
        private bool _requiredMsg;
        
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
