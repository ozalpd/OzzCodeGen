using System;
using System.Collections.Generic;
using System.Linq;
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
                _controllerName = value != null ? value.ToPascalCase() : string.Empty;
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
                _dataSetName = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("DataSetName");
            }
        }
        private string _dataSetName;


        public string StrongDependedForeignKey
        {
            get { return _strongDependedForeignKey; }
            set
            {
                _strongDependedForeignKey = value;
                RaisePropertyChanged("StrongDependedForeignKey");
                RaisePropertyChanged("StrongForeignKeyProperty");
                RaisePropertyChanged("StrongForeignEntity");
            }
        }
        private string _strongDependedForeignKey;

        [XmlIgnore]
        public AspNetMvcPropertySetting StrongForeignKeyProperty
        {
            get
            {
                if (string.IsNullOrEmpty(StrongDependedForeignKey))
                    return null;

                return Properties.FirstOrDefault(p => p.Name.Equals(StrongDependedForeignKey));
            }
        }

        [XmlIgnore]
        public AspNetMvcEntitySetting StrongForeignEntity
        {
            get
            {
                return CodeEngine.GetForeignKeyEntity(StrongForeignKeyProperty);
            }
        }

        /// <summary>
        /// Retuns entity list those are set ForeignEntityForCreate = this instance 
        /// </summary>
        /// <returns></returns>
        public ICollection<AspNetMvcEntitySetting> GetEntitiesStrongForThis()
        {
            var entities = new List<AspNetMvcEntitySetting>();
            foreach (var item in CodeEngine.Entities)
            {
                if (!entities.Contains(item) && IsThisStrongForeign(item))
                    entities.Add(item);
            }
            return entities;
        }

        /// <summary>
        /// Returns true if this instance or any decendant is ForeignEntityForCreate of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsThisStrongForeign(AspNetMvcEntitySetting entity)
        {
            if (entity.StrongForeignEntity == this)
                return true;
            var baseEntity = (AspNetMvcEntitySetting)this.GetBaseEntitySetting();
            return baseEntity == null ? false : baseEntity.IsThisStrongForeign(entity);
        }

        /// <summary>
        /// Parameter for Save and SaveAsync methods of data context
        /// </summary>
        public string SaveParameter
        {
            get { return _saveParameter; }
            set
            {
                _saveParameter = value;
                RaisePropertyChanged("SaveParameter");
            }
        }
        private string _saveParameter;

        /// <summary>
        /// Model or ViewModel class name for Create action
        /// </summary>
        public string ModelForCreate
        {
            get
            {
                if (string.IsNullOrEmpty(_modelForCreate))
                {
                    _modelForCreate = Name;
                }
                return _modelForCreate;
            }
            set
            {
                _modelForCreate = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ModelForCreate");
            }
        }
        private string _modelForCreate;

        /// <summary>
        /// Model or ViewModel class name for Edit action
        /// </summary>
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
                _modelForEdit = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ModelForEdit");
            }
        }
        private string _modelForEdit;


        public string RolesCanView
        {
            get { return _canView; }
            set
            {
                _canView = value != null ? value.ToLowerInvariant() : string.Empty;
                RaisePropertyChanged("RolesCanView");
            }
        }
        private string _canView;

        public string RolesCanCreate
        {
            get { return _canCreate; }
            set
            {
                _canCreate = value != null ? value.ToLowerInvariant() : string.Empty;
                RaisePropertyChanged("RolesCanCreate");
            }
        }
        private string _canCreate;

        public string RolesCanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value != null ? value.ToLowerInvariant() : string.Empty;
                RaisePropertyChanged("RolesCanEdit");
            }
        }
        private string _canEdit;

        public string RolesCanDelete
        {
            get { return _canDelete; }
            set
            {
                _canDelete = value != null ? value.ToLowerInvariant() : string.Empty;
                RaisePropertyChanged("RolesCanDelete");
            }
        }
        private string _canDelete;



        public string[] RolesCanViewToArray()
        {
            if (string.IsNullOrEmpty(RolesCanView))
            {
                return null;
            }
            else
            {
                return RolesCanView.Replace(" ", "").Split(',');
            }
        }

        public string[] RolesCanCreateToArray()
        {
            if (string.IsNullOrEmpty(RolesCanCreate))
            {
                return null;
            }
            else
            {
                return RolesCanCreate.Replace(" ", "").Split(',');
            }
        }

        public string[] RolesCanEditToArray()
        {
            if (string.IsNullOrEmpty(RolesCanEdit))
            {
                return null;
            }
            else
            {
                return RolesCanEdit.Replace(" ", "").Split(',');
            }
        }

        public string[] RolesCanDeleteToArray()
        {
            if (string.IsNullOrEmpty(RolesCanDelete))
            {
                return null;
            }
            else
            {
                return RolesCanDelete.Replace(" ", "").Split(',');
            }
        }


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


        public bool GenerateGetQueryMethod
        {
            get
            {
                if (_generateGetQueryMethod == null)
                    _generateGetQueryMethod = true;
                return _generateGetQueryMethod.Value;
            }
            set
            {
                _generateGetQueryMethod = value;
                RaisePropertyChanged("GenerateGetQueryMethod");
            }
        }
        private bool? _generateGetQueryMethod;

        public bool GenerateIndexAction
        {
            get
            {
                if (_generateIndexAction == null)
                    _generateIndexAction = true;
                return _generateIndexAction.Value;
            }
            set
            {
                _generateIndexAction = value;
                RaisePropertyChanged("GenerateIndexAction");
            }
        }
        private bool? _generateIndexAction;

        public bool GenerateCreateAction
        {
            get
            {
                if (_generateCreateAction == null)
                    _generateCreateAction = true;
                return _generateCreateAction.Value;
            }
            set
            {
                _generateCreateAction = value;
                RaisePropertyChanged("GenerateCreateAction");
            }
        }
        private bool? _generateCreateAction;

        public bool GenerateDeleteAction
        {
            get
            {
                if (_generateDeleteAction == null)
                    _generateDeleteAction = true;
                return _generateDeleteAction.Value;
            }
            set
            {
                _generateDeleteAction = value;
                RaisePropertyChanged("GenerateDeleteAction");
            }
        }
        private bool? _generateDeleteAction;

        public bool GenerateEditAction
        {
            get
            {
                if (_generateEditAction == null)
                    _generateEditAction = true;
                return _generateEditAction.Value;
            }
            set
            {
                _generateEditAction = value;
                RaisePropertyChanged("GenerateEditAction");
            }
        }
        private bool? _generateEditAction;

        public bool GenerateDetailsAction
        {
            get
            {
                if (_generateDetailsAction == null)
                    _generateDetailsAction = true;
                return _generateDetailsAction.Value;
            }
            set
            {
                _generateDetailsAction = value;
                RaisePropertyChanged("GenerateDetailsAction");
            }
        }
        private bool? _generateDetailsAction;

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

        /// <summary>
        /// After Saving in Create Actions RedirectToAction method parameter
        /// </summary>
        public string CreateRedirect
        {
            get
            {
                if (string.IsNullOrEmpty(_createRedirect))
                    _createRedirect = "\"Index\"";
                return _createRedirect;
            }
            set
            {
                _createRedirect = value;
                RaisePropertyChanged("CreateRedirect");
            }
        }
        private string _createRedirect;

        /// <summary>
        /// After Saving in Edit Action RedirectToAction method parameter
        /// </summary>
        public string EditRedirect
        {
            get
            {
                if (string.IsNullOrEmpty(_editRedirect))
                    _editRedirect = "\"Index\"";
                return _editRedirect;
            }
            set
            {
                _editRedirect = value;
                RaisePropertyChanged("EditRedirect");
            }
        }
        private string _editRedirect;


        public string FillSelectListsMethod
        {
            get
            {
                if (string.IsNullOrEmpty(_fillSelectListsMethod))
                    _fillSelectListsMethod = AspNetMvcEngine.FillSelectListsMethodDefaultName;
                return _fillSelectListsMethod;
            }
            set
            {
                _fillSelectListsMethod = value;
                RaisePropertyChanged("FillSelectListsMethod");
            }
        }
        private string _fillSelectListsMethod;

        /// <summary>
        /// Base controller class for MVC Controller
        /// </summary>
        public string BaseControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(_baseControllerName))
                {
                    _baseControllerName = CodeEngine.BaseControllerName;
                }
                return _baseControllerName;
            }
            set
            {
                _baseControllerName = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("BaseControllerName");
            }
        }
        private string _baseControllerName;


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

        public string[] GetFillSelectListsMethods()
        {
            var methods = FillSelectListsMethod.Split(';');
            return (from m in methods
                    select m.Trim())
                   .ToArray();
        }

        public string GetMethodCall(string methodName)
        {
            string method = methodName.Trim();
            if (string.IsNullOrEmpty(method))
                return string.Empty;

            if (method.Contains("(") && method.EndsWith(")"))
            {
                return method + ";";
            }
            else
            {
                return string.Format("{0}({1});", method, Name.ToCamelCase());
            }
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
        public MvcFilterType FilterViewType
        {
            get { return _filterViewType; }
            set
            {
                _filterViewType = value;
                RaisePropertyChanged("FilterViewType");
                RaisePropertyChanged("FilteredDisplayColumns");
            }
        }
        private MvcFilterType _filterViewType;

        [XmlIgnore]
        public IEnumerable<AspNetMvcPropertySetting> FilteredDisplayColumns
        {
            get
            {
                switch (FilterViewType)
                {
                    case MvcFilterType.AllProperties:
                        return GetInheritedIncludedProperties();
                    case MvcFilterType.IndexViewProperties:
                        return GetInheritedIncludedProperties().Where(c => c.InIndexView);
                    case MvcFilterType.DetailsViewProperties:
                        return GetInheritedIncludedProperties().Where(c => c.InDetailsView);
                    case MvcFilterType.CreateViewProperties:
                        return GetInheritedIncludedProperties().Where(c => c.InCreateView);
                    case MvcFilterType.EditViewProperties:
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
