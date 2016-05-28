using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.AspNetMvc.UI;
using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.Definitions;
using System.Collections.ObjectModel;
using OzzUtils;
using OzzCodeGen.CodeEngines.Storage;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    [XmlInclude(typeof(AspNetMvcEntitySetting))]
    public partial class AspNetMvcEngine : BaseCodeEngine
    {
        public override string EngineId
        {
            get { return EngineTypes.AspNetMvcEngineId; }
        }
        public override string ProjectTypeName
        {
            get { return "ASP.NET MVC Controller & View Generator"; }
        }

        public static string FillSelectListsMethodDefaultName = "SetSelectLists";

        [XmlIgnore]
        public static string DefaultFileName { get { return "AspNetMvcGen.settings"; } }
        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new AspNetMvcEntitySetting()
            {
                Name = entity.Name,
                DataModel = this.Project.DataModel,
                CodeEngine = this,
                Exclude = entity.Abstract,
                GenerateController = !entity.Abstract,
                IndexView = !entity.Abstract,
                DetailsView = !entity.Abstract,
                CreateView = !entity.Abstract,
                EditView = !entity.Abstract,
                RolesCanView = RolesCanView,
                RolesCanEdit = RolesCanEdit,
                RolesCanDelete = RolesCanDelete,
                RolesCanCreate = RolesCanCreate,
                SaveParameter = SaveParameter,
                BaseControllerName = BaseControllerName
            };

            foreach (var property in entity.Properties)
            {
                if (property.DefinitionType != DefinitionType.Collection)
                {
                    var ps = GetDefaultPropertySetting(property, setting);
                }
            }

            return setting;
        }

        protected AspNetMvcPropertySetting GetDefaultPropertySetting(BaseProperty property, AspNetMvcEntitySetting setting)
        {
            var ps = new AspNetMvcPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting
            };
            setting.Properties.Add(ps);

            return ps;
        }

        protected override void OnSearchStringChanged()
        {
            RaisePropertyChanged("Entities");
        }

        [XmlIgnore]
        public ObservableCollection<AspNetMvcEntitySetting> Entities
        {
            get
            {
                if (Project == null || string.IsNullOrEmpty(Project.SearchString))
                {
                    return _entities;
                }
                else
                {
                    var result = _entities
                        .Where(e => e.Name.StartsWith(Project.SearchString, System.StringComparison.InvariantCultureIgnoreCase) ||
                            e.Properties.Where(p => p.Name.StartsWith(Project.SearchString, System.StringComparison.InvariantCultureIgnoreCase)).Any());
                    return new ObservableCollection<AspNetMvcEntitySetting>(result);
                }
            }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged("Entities");
            }
        }
        private ObservableCollection<AspNetMvcEntitySetting> _entities;

        protected override void OnEntitySettingsChanged()
        {
            var entities = new ObservableCollection<AspNetMvcEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (AspNetMvcEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            Entities = entities;
        }

        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var entitySetting = (AspNetMvcEntitySetting)setting;
            entitySetting.DataModel = Project.DataModel;
            entitySetting.CodeEngine = this;

            List<AspNetMvcPropertySetting> remvProp = new List<AspNetMvcPropertySetting>();
            foreach (var propSetting in entitySetting.Properties)
            {
                if (entity.Properties.FirstOrDefault(p => p.Name == propSetting.Name) == null)
                {
                    remvProp.Add(propSetting);
                }
            }

            foreach (var propSetting in remvProp)
            {
                entitySetting.Properties.Remove(propSetting);
            }

            foreach (var propSetting in entity.Properties)
            {
                var ps = entitySetting.Properties.FirstOrDefault(p => p.Name == propSetting.Name);
                if (ps == null)// && property.DefinitionType != DefinitionType.Collection
                {
                    ps = GetDefaultPropertySetting(propSetting, entitySetting);
                }
                else //if (ps != null)
                {
                    ps.EntitySetting = setting;
                }
            }
            entitySetting.Properties = entitySetting
                .Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder)
                .ToList();

            RefreshSecurityRoles();
        }

        protected override System.Windows.Controls.UserControl GetUiControl()
        {
            if (_engineUI == null)
            {
                _engineUI = new MvcEngineUI()
                {
                    CodeEngine = this
                };
            }
            return _engineUI;
        }
        MvcEngineUI _engineUI;

        public override System.Windows.Controls.UserControl GetSettingsDlgUI()
        {
            return new MvcEngineSettingsUI();
        }

        public string TargetViewsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_targetViewsFolder))
                {
                    _targetViewsFolder = "Views";
                }
                return _targetViewsFolder;
            }
            set
            {
                _targetViewsFolder = value;
                RaisePropertyChanged("TargetViewsFolder");
                RaisePropertyChanged("TargetViewsDir");
            }
        }
        private string _targetViewsFolder;

        [XmlIgnore]
        public string TargetViewsDir
        {
            get
            {
                if (string.IsNullOrEmpty(TargetDirectory))
                {
                    return string.Empty;
                }
                else
                {
                    return Path.GetFullPath(Path.Combine(TargetDirectory, TargetViewsFolder));
                }
            }
        }


        public string TargetModelsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_targetModelsFolder))
                {
                    _targetModelsFolder = "Models";
                }
                return _targetModelsFolder;
            }
            set
            {
                _targetModelsFolder = value;
                RaisePropertyChanged("TargetModelsFolder");
                RaisePropertyChanged("TargetModelsDir");
            }
        }
        private string _targetModelsFolder;

        [XmlIgnore]
        public string TargetModelsDir
        {
            get
            {
                if (string.IsNullOrEmpty(TargetDirectory))
                {
                    return string.Empty;
                }
                else
                {
                    return Path.GetFullPath(Path.Combine(TargetDirectory, TargetModelsFolder));
                }
            }
        }


        public string SnippetsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_SnippetsFolder))
                {
                    _SnippetsFolder = "MVC-CodeSnippets";
                }
                return _SnippetsFolder;
            }
            set
            {
                _SnippetsFolder = value;
                RaisePropertyChanged("SnippetsFolder");
                RaisePropertyChanged("SnippetsDir");
            }
        }
        private string _SnippetsFolder;

        [XmlIgnore]
        public string SnippetsDir
        {
            get
            {
                if (string.IsNullOrEmpty(TargetDirectory))
                {
                    return string.Empty;
                }
                else
                {
                    return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, SnippetsFolder));
                }
            }
        }


        public string TargetControllersFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_targetControllersFolder))
                {
                    _targetControllersFolder = "Controllers";
                }
                return _targetControllersFolder;
            }
            set
            {
                _targetControllersFolder = value;
                RaisePropertyChanged("TargetControllersFolder");
                RaisePropertyChanged("TargetControllersDir");
            }
        }
        private string _targetControllersFolder;

        [XmlIgnore]
        public string TargetControllersDir
        {
            get
            {
                if (string.IsNullOrEmpty(TargetDirectory))
                {
                    return string.Empty;
                }
                else
                {
                    return Path.GetFullPath(Path.Combine(TargetDirectory, TargetControllersFolder));
                }
            }
        }

        public string ControllersNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_controllersNamespace) && Project != null)
                {
                    _controllersNamespace = GetDefaultControllersNamespace(Project.NamespaceName);
                }
                return _controllersNamespace;
            }
            set
            {
                _controllersNamespace = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ControllersNamespace");
            }
        }
        private string _controllersNamespace;

        protected string GetDefaultControllersNamespace(string projectNamespaceName)
        {
            return projectNamespaceName + "." + "Controllers";
        }

        public string ModelsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_modelsNamespace) && Project != null)
                {
                    _modelsNamespace = GetDefaultModelsNamespace(Project.NamespaceName);
                }
                return _modelsNamespace;
            }
            set
            {
                _modelsNamespace = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ModelsNamespace");
            }
        }
        private string _modelsNamespace;

        protected string GetDefaultModelsNamespace(string projectNamespaceName)
        {
            return projectNamespaceName + "." + "Models";
        }

        public string DataModelsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_dataModelsNamespace) && Project != null)
                {
                    _dataModelsNamespace = GetDefaultDataModelsNamespace(Project.NamespaceName);
                }
                return _dataModelsNamespace;
            }
            set
            {
                _dataModelsNamespace = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("DataModelsNamespace");
            }
        }
        private string _dataModelsNamespace;
        protected string GetDefaultDataModelsNamespace(string projectNamespaceName)
        {
            return projectNamespaceName + "." + "Models";
        }

        public string ViewModelsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_viewModelsNamespace) && Project != null)
                {
                    _viewModelsNamespace = GetDefaultViewModelsNamespace(Project.NamespaceName);
                }
                return _viewModelsNamespace;
            }
            set
            {
                _viewModelsNamespace = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ViewModelsNamespace");
            }
        }
        private string _viewModelsNamespace;

        protected string GetDefaultViewModelsNamespace(string projectNamespaceName)
        {
            return projectNamespaceName + "." + "ViewModels";
        }

        public string ViewsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_viewsNamespace) && Project != null)
                {
                    _viewsNamespace = GetDefaultViewsNamespace(Project.NamespaceName);
                }
                return _viewsNamespace;
            }
            set
            {
                _viewsNamespace = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("ViewsNamespace");
            }
        }
        private string _viewsNamespace;

        protected string GetDefaultViewsNamespace(string projectNamespaceName)
        {
            return projectNamespaceName + "." + "Views";
        }

        /// <summary>
        /// Default base class for MVC Controller
        /// </summary>
        public string BaseControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(_baseControllerName))
                {
                    _baseControllerName = "AbstractController";
                }
                return _baseControllerName;
            }
            set
            {
                string oldValue = _baseControllerName;
                onBaseControllerNameChanging(value);
                onBaseControllerNameChanged(oldValue);
            }
        }
        protected virtual void onBaseControllerNameChanging(string newValue)
        {
            _baseControllerName = newValue.ToPascalCase();
        }
        protected virtual void onBaseControllerNameChanged(string oldValue)
        {
            RaisePropertyChanged("BaseControllerName");
            foreach (var item in Entities)
            {
                if (item.BaseControllerName.Equals(oldValue))
                {
                    item.BaseControllerName = BaseControllerName;
                }
            }
        }
        private string _baseControllerName;


        public string BaseOfBaseController
        {
            get
            {
                if (string.IsNullOrEmpty(_baseOfBaseController))
                    _baseOfBaseController = "Controller";
                return _baseOfBaseController;
            }
            set
            {
                _baseOfBaseController = value != null ? value.ToPascalCase() : string.Empty;
                RaisePropertyChanged("BaseOfBaseController");
            }
        }
        private string _baseOfBaseController;

        /// <summary>
        /// Class name for repository or data context
        /// </summary>
        public string DataContextClass
        {
            get
            {
                if (string.IsNullOrEmpty(_dataContextClass))
                {
                    _dataContextClass = GetDefaultDataContextClass(Project.Name);
                }
                return _dataContextClass;
            }
            set
            {
                string oldValue = _dataContextClass;
                onDataContextClassChanging(value);
                onDataContextClassChanged(oldValue);
            }
        }
        protected virtual void onDataContextClassChanging(string newValue)
        {
            _dataContextClass = newValue.ToPascalCase();
        }
        protected virtual void onDataContextClassChanged(string oldValue)
        {
            RaisePropertyChanged("DataContextClass");
        }
        private string _dataContextClass;

        protected virtual string GetDefaultDataContextClass(string projectName)
        {
            return string.Format("{0}Context", projectName.ToPascalCase());
        }

        /// <summary>
        /// Default instance name for controllers' DataContext or Repository
        /// </summary>
        public string DataContextInstance
        {
            get
            {
                if (string.IsNullOrEmpty(_dataContextInstance))
                    _dataContextInstance = "DataContext";
                return _dataContextInstance;
            }
            set
            {
                string oldValue = _dataContextInstance;
                onDataContextInstanceChanging(value);
                onDataContextInstanceChanged(oldValue);
            }
        }
        public virtual void onDataContextInstanceChanging(string newValue)
        {
            _dataContextInstance = newValue.ToPascalCase();
        }
        public virtual void onDataContextInstanceChanged(string oldValue)
        {
            RaisePropertyChanged("DataContextInstance");
            //foreach (var item in Entities)
            //{
            //    if (item.DataContextInstance.Equals(oldValue))
            //    {
            //        item.DataContextInstance = DataContextInstance;
            //    }
            //}
        }
        private string _dataContextInstance;

        /// <summary>
        /// Parameter for Save and SaveAsync methods of controllers' data context
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
        /// Generate DataContext property in controllers
        /// </summary>
        public bool GenerateDataContext
        {
            get { return _generateDataContext; }
            set
            {
                if (_generateDataContext == value) return;
                _generateDataContext = value;
                RaisePropertyChanged("GenerateDataContext");
            }
        }
        private bool _generateDataContext;

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

        public string RolesCanSeeRestricted
        {
            get { return _canSeeRestricted; }
            set
            {
                _canSeeRestricted = value != null ? value.ToLowerInvariant() : string.Empty;
                RaisePropertyChanged("RolesCanSeeRestricted");
            }
        }
        private string _canSeeRestricted;

        public bool AreCanUserMethodsPublic
        {
            get { return _areCanUserMethodsPublic; }
            set
            {
                _areCanUserMethodsPublic = value;
                RaisePropertyChanged("AreCanUserMethodsPublic");
            }
        }
        bool _areCanUserMethodsPublic;

        [XmlIgnore]
        public ResxEngine ResxEngine
        {
            get
            {
                if (Project != null && _resxEngine == null)
                {
                    var engine = Project.GetCodeEngine(EngineTypes.LocalizationResxGenId);
                    _resxEngine = engine != null ? (ResxEngine)engine : null;
                }
                return _resxEngine;
            }
        }
        private ResxEngine _resxEngine;


        [XmlIgnore]
        public StorageCodeEngine StorageEngine
        {
            get
            {
                if (Project != null && _storageEngine== null)
                {
                    //TODO: If a new kind server side StorageCodeEngine will be added, it should be caught here
                    //for now only StorageCodeEngine is EngineTypes.TSqlScriptsId
                    var engine = Project.GetCodeEngine(EngineTypes.TSqlScriptsId);
                    _storageEngine = engine == null ? null : (TSqlScriptsEngine)engine;
                }
                return _storageEngine;
            }
        }
        StorageCodeEngine _storageEngine;

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static AspNetMvcEngine OpenFile(string fileName)
        {
            AspNetMvcEngine instance = GetInstanceFromFile(
                fileName, 
                typeof(AspNetMvcEngine)) as AspNetMvcEngine;
            return instance;
        }
    }
}
