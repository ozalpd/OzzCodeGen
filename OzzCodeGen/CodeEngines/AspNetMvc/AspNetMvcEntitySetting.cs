using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using OzzUtils;


namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public class AspNetMvcEntitySetting : AbstractEntitySetting<AspNetMvcPropertySetting>
    {
        [XmlIgnore]
        public AspNetMvcEngine CodeEngine { get; set; }

        public string ControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(_controllerName))
                {
                    _controllerName = Name.Pluralize();
                }
                return _controllerName;
            }
            set
            {
                _controllerName = value;
                RaisePropertyChanged("ControllerName");
            }
        }
        private string _controllerName;


        public string DataSetName
        {
            get
            {
                if (string.IsNullOrEmpty(_dataSetName))
                {
                    _dataSetName = GetDataSetName();
                }
                return _dataSetName;
            }
            set
            {
                _dataSetName = value;
                RaisePropertyChanged("DataSetName");
            }
        }
        private string _dataSetName;

        public string BaseControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(_baseControllerName))
                {
                    _baseControllerName = ((AspNetMvcEngine)CodeEngine).BaseControllerName;
                }
                return _baseControllerName;
            }
            set
            {
                _baseControllerName = value;
                RaisePropertyChanged("BaseControllerName");
            }
        }
        private string _baseControllerName;

        public string ModelForEdit
        {
            get
            {
                if (string.IsNullOrEmpty(_modelForEdit))
                {
                    _modelForEdit = Name;
                }
                return _modelForEdit;
            }
            set
            {
                _modelForEdit = value;
                RaisePropertyChanged("ModelForEdit");
            }
        }
        private string _modelForEdit;


        public string WhoCanView
        {
            get
            {
                if (string.IsNullOrEmpty(_canView))
                {
                    _canView = "Users";
                }
                return _canView;
            }
            set
            {
                _canView = value;
                RaisePropertyChanged("WhoCanView");
            }
        }
        private string _canView;

        public string WhoCanEdit
        {
            get
            {
                if (string.IsNullOrEmpty(_canEdit))
                {
                    _canEdit = "Admin, Editor";
                }
                return _canEdit;
            }
            set
            {
                _canEdit = value;
                RaisePropertyChanged("WhoCanEdit");
            }
        }
        private string _canEdit;

        public string WhoCanDelete
        {
            get
            {
                if (string.IsNullOrEmpty(_canDelete))
                {
                    _canDelete = "Admin";
                }
                return _canDelete;
            }
            set
            {
                _canDelete = value;
                RaisePropertyChanged("WhoCanDelete");
            }
        }
        private string _canDelete;


        public bool GenerateController
        {
            get
            {
                if (!_generateController.HasValue) _generateController = true;
                return _generateController.Value;
            }
            set
            {
                if (_generateController == value) return;
                _generateController = value;
                RaisePropertyChanged("GenerateController");
            }
        }
        private bool? _generateController;

        /// <summary>
        /// Generate IndexView
        /// </summary>
        public bool IndexView
        {
            get
            {
                if (!_indexView.HasValue) _indexView = true;
                return _indexView.Value;
            }
            set
            {
                if (_indexView == value) return;
                _indexView = value;
                RaisePropertyChanged("IndexView");
            }
        }
        private bool? _indexView;

        /// <summary>
        /// Generate DetailsView
        /// </summary>
        public bool DetailsView
        {
            get
            {
                if (!_detailsView.HasValue) _detailsView = true;
                return _detailsView.Value;
            }
            set
            {
                if (_detailsView == value) return;
                _detailsView = value;
                RaisePropertyChanged("DetailsView");
            }
        }
        private bool? _detailsView;

        /// <summary>
        /// Generate IndexView
        /// </summary>
        public bool CreateView
        {
            get
            {
                if (!_createView.HasValue) _createView = true;
                return _createView.Value;
            }
            set
            {
                if (_createView == value) return;
                _createView = value;
                RaisePropertyChanged("CreateView");
            }
        }
        private bool? _createView;

        /// <summary>
        /// Generate IndexView
        /// </summary>
        public bool EditView
        {
            get
            {
                if (!_editView.HasValue) _editView = true;
                return _editView.Value;
            }
            set
            {
                if (_editView == value) return;
                _editView = value;
                RaisePropertyChanged("EditView");
            }
        }
        private bool? _editView;


        public string GetAuthorizeAttrib(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) || roleName.Equals("everyone", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            else if (roleName.Equals("users", StringComparison.InvariantCultureIgnoreCase))
            {
                return "[Authorize]";
            }
            else
            {
                return string.Format("[Authorize(Roles = \"{0}\")]", roleName);
            }
        }

        private string GetDataSetName()
        {
            var baseEntity = GetBaseEntitySetting();
            if (baseEntity != null && !baseEntity.EntityDefinition.Abstract)
            {
                return ((AspNetMvcEntitySetting)baseEntity).DataSetName;
            }
            else
            {
                return Name.Pluralize();
            }
        }

        public string GetDataSetQuery()
        {
            var baseEntity = GetBaseEntitySetting();
            if (baseEntity == null)
            {
                return DataSetName;
            }
            if (((AspNetMvcEntitySetting)baseEntity).DataSetName == DataSetName)
            {
                return DataSetName + ".OfType<" + Name + ">()";
            }
            else
            {
                return DataSetName;
            }
        }

        public AspNetMvcEntitySetting GetForeignKeyEntity(AspNetMvcPropertySetting property)
        {
            if (!property.IsForeignKey())
            {
                return null;
            }

            var complexProp = GetInheritedComplexProperties()
                                .FirstOrDefault(p => ((ComplexProperty)p.PropertyDefinition).DependentPropertyName == property.Name);

            AspNetMvcEntitySetting result = null;

            if (complexProp != null)
            {
                result = CodeEngine
                        .Entities
                        .FirstOrDefault(e => e.EntityDefinition.Name.Equals(complexProp.PropertyDefinition.TypeName));
            }
            var pkey = result.GetPrimeryKey();
            string s = result.EntityDefinition.DisplayMember;

            return result;
        }

        public override AbstractEntitySetting<AspNetMvcPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return CodeEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }


        [XmlIgnore]
        public MvcViewType FilterViewType
        {
            get { return _filterViewType; }
            set
            {
                _filterViewType = value;
                RaisePropertyChanged("FilterViewType");
                RaisePropertyChanged("FilteredDisplayColumns");
            }
        }
        private MvcViewType _filterViewType;

        [XmlIgnore]
        public IEnumerable<AspNetMvcPropertySetting> FilteredDisplayColumns
        {
            get
            {
                switch (FilterViewType)
                {
                    case MvcViewType.AllViews:
                        return GetInheritedIncludedProperties();
                    case MvcViewType.IndexView:
                        return GetInheritedIncludedProperties().Where(c => c.InIndexView);
                    case MvcViewType.DetailsView:
                        return GetInheritedIncludedProperties().Where(c => c.InDetailsView);
                    case MvcViewType.CreateView:
                        return GetInheritedIncludedProperties().Where(c => c.InCreateView);
                    case MvcViewType.EditView:
                        return GetInheritedIncludedProperties().Where(c => c.InEditView);
                    default:
                        return GetInheritedIncludedProperties();
                }
            }
        }
        
        [XmlIgnore]
        public IEnumerable<AspNetMvcPropertySetting> DisplayColumns
        {
            get
            {
                return GetInheritedIncludedProperties();
            }
        }
    }
}
