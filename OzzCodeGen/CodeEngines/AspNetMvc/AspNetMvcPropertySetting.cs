using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public class AspNetMvcPropertySetting : BasePropertySetting
    {

        public bool UseInSearch
        {
            get
            {
                if (!_useInSearch.HasValue)
                {
                    _useInSearch = PropertyDefinition.IsTypeString();
                }
                return _useInSearch.Value;
            }
            set
            {
                _useInSearch = value;
                RaisePropertyChanged("UseInSearch");
            }
        }
        private bool? _useInSearch;

        public bool InIndexView
        {
            get
            {
                if (!_inIndexView.HasValue)
                {
                    _inIndexView = !(PropertyDefinition.IsClientComputed ||
                                      PropertyDefinition.IsStoreGenerated ||
                                      PropertyDefinition.IsServerComputed ||
                                      Name.EndsWith("Id"));
                }
                return _inIndexView.Value;
            }
            set
            {
                _inIndexView = value;
                RaisePropertyChanged("InIndexView");
            }
        }
        private bool? _inIndexView;

        public bool InDetailsView
        {
            get
            {
                if (!_inDetailsView.HasValue)
                {
                    _inDetailsView = !(Name.EndsWith("Id"));
                }
                return _inDetailsView.Value;
            }
            set
            {
                _inDetailsView = value;
                RaisePropertyChanged("InDetailsView");
            }
        }
        private bool? _inDetailsView;

        public bool InCreateView
        {
            get
            {
                if (!_inCreateView.HasValue)
                {
                    _inCreateView = !(PropertyDefinition.IsClientComputed ||
                                      PropertyDefinition.IsStoreGenerated ||
                                      PropertyDefinition.IsServerComputed ||
                                      PropertyDefinition is ComplexProperty);
                }
                return _inCreateView.Value;
            }
            set
            {
                _inCreateView = value;
                RaisePropertyChanged("InCreateView");
            }
        }
        private bool? _inCreateView;

        public bool InEditView
        {
            get
            {
                if (!_inEditView.HasValue)
                {
                    _inEditView = !(PropertyDefinition.IsClientComputed ||
                                      PropertyDefinition.IsStoreGenerated ||
                                      PropertyDefinition.IsServerComputed ||
                                      PropertyDefinition is ComplexProperty);
                }
                return _inEditView.Value;
            }
            set
            {
                _inEditView = value;
                RaisePropertyChanged("InEditView");
            }
        }
        private bool? _inEditView;

        public bool HasValidation
        {
            get
            {
                if (!_hasValidation.HasValue)
                {
                    _hasValidation = !PropertyDefinition.IsStoreGenerated & !PropertyDefinition.IsServerComputed;
                }
                return _hasValidation.Value;
            }
            set
            {
                _hasValidation = value;
                RaisePropertyChanged("HasValidation");
            }
        }
        private bool? _hasValidation;
        

        public string UseForEdit
        {
            get
            {
                if (string.IsNullOrEmpty(_useForEdit))
                {
                    _useForEdit = GetUseForEdit();
                }
                return _useForEdit;
            }
            set
            {
                _useForEdit = value;
                RaisePropertyChanged("UseForEdit");
            }
        }
        private string _useForEdit;

        private string GetUseForEdit()
        {
            if (Name.StartsWith("Image") && PropertyDefinition is StringProperty)
            {
                return "ImageUpload";
            }
            else if (PropertyDefinition is SimpleProperty &&
                ((SimpleProperty)PropertyDefinition).IsForeignKey)
            {
                return "DropDownList";
            }
            else if (PropertyDefinition is StringProperty)
            {
                return "TextBox";
            }
            return "Standart";
        }

        [XmlIgnore]
        public string[] UseForEditList
        {
            get
            {
                return _useForEditList;
            }
        }
        static string[] _useForEditList = {
                    "Standart",
                    "TextBox",
                    "TextArea",
                    "ImageUpload",
                    "DropDownList",
                    "ReadOnlyDisplay",
                    "HiddenInput"
                };

        public bool IsNullable()
        {
            if (PropertyDefinition is SimpleProperty)
            {
                return ((SimpleProperty)PropertyDefinition).IsNullable;
            }
            else
            {
                return false;
            }
        }
    }
}
