using OzzUtils;
using OzzUtils.Wpf;

namespace OzzLocalization
{
    public class Vocab : AbstractViewModel
    {
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
                RaisePropertyChanged("IsTranslated");
            }
        }
        string _name;

        public string Translation
        {
            get
            {
                if (string.IsNullOrEmpty(_translation))
                {
                    _translation = GetDefaultTranslation();
                }
                return _translation;
            }
            set
            {
                _translation = value;
                RaisePropertyChanged("Translation");
                RaisePropertyChanged("IsTranslated");
            }
        }
        string _translation;

        public string GetDefaultTranslation()
        {
            return Name.PascalCaseToTitleCase();
        }

        public bool IsTranslated
        {
            get
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Translation))
                {
                    return false;
                }
                return !Translation.Equals(GetDefaultTranslation());
            }
        }

        public string RequiredMsg
        {
            get { return _requiredMsg; }
            set
            {
                _requiredMsg = value;
                RaisePropertyChanged("RequiredMsg");
            }
        }
        string _requiredMsg;

        public string ValidationMsg
        {
            get { return _validationMsg; }
            set
            {
                _validationMsg = value;
                RaisePropertyChanged("ValidationMsg");
            }
        }
        string _validationMsg;

        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                RaisePropertyChanged("ToolTip");
            }
        }
        string _toolTip;


        public override string ToString()
        {
            return Name;
        }
    }
}
