using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using System.ComponentModel;

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
                    _useInSearch = PropertyDefinition.IsTypeString() &&
                        ((StringProperty)PropertyDefinition).MaxLenght < 257;
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

        /// <summary>
        /// Is visible in IndexView
        /// </summary>
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
                if (MvcEntity != null)
                    MvcEntity.FilterViewType = MvcEntity.FilterViewType;
            }
        }
        private bool? _inIndexView;

        /// <summary>
        /// Is visible in DetailsView
        /// </summary>
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
                if (MvcEntity != null)
                    MvcEntity.FilterViewType = MvcEntity.FilterViewType;
            }
        }
        private bool? _inDetailsView;

        /// <summary>
        /// Is visible in CreateView
        /// </summary>
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
                if (MvcEntity != null)
                    MvcEntity.FilterViewType = MvcEntity.FilterViewType;
            }
        }
        private bool? _inCreateView;

        /// <summary>
        /// Is visible in EditView
        /// </summary>
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
                if (MvcEntity != null)
                    MvcEntity.FilterViewType = MvcEntity.FilterViewType;
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
        

        public string UseForCreate
        {
            get
            {
                if (string.IsNullOrEmpty(_useForCreate))
                {
                    _useForCreate = GetUseForEdit();
                }
                return _useForCreate;
            }
            set
            {
                _useForCreate = value;
                RaisePropertyChanged("UseForCreate");
            }
        }
        private string _useForCreate;

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

        [XmlIgnore]
        public bool IsSensitiveData
        {
            get
            {
                return PropertyDefinition?.RestrictedData ?? false;
            }
        }

        [XmlIgnore]
        public AspNetMvcEntitySetting MvcEntity
        {
            get { return (AspNetMvcEntitySetting)EntitySetting; }
        }

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
