using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.AspNetMvc.Templates;
using OzzCodeGen.CodeEngines.AspNetMvc.UI;
using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.Definitions;
using System.Collections.ObjectModel;
using OzzUtils;
using OzzCodeGen.CodeEngines.Storage;
using System;
using System.ComponentModel;

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
                DataContextClass = DataContextClass,
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

        [XmlIgnore]
        public ObservableCollection<AspNetMvcEntitySetting> Entities
        {
            get { return _entities; }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged("Entities");
            }
        }
        private ObservableCollection<AspNetMvcEntitySetting> _entities;


        public string AdminRole
        {
            get { return _adminRole; }
            set
            {
                string oldValue = _adminRole;
                onAdminRoleChanging(value);
                onAdminRoleChanged(oldValue);
            }
        }
        private string _adminRole;


        public ObservableCollection<string> SecurityRoles
        {
            get
            {
                if (_securityRoles == null)
                    _securityRoles = new ObservableCollection<string>();
                return _securityRoles;
            }
            set
            {
                _securityRoles = value;
                RaisePropertyChanged("SecurityRoles");
            }
        }
        private ObservableCollection<string> _securityRoles;

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

        public bool RenderTemplate(string templateName, bool multiTemplates)
        {
            bool rendered = true;
            if (RenderAllEntities)
            {
                foreach (var item in Entities)
                {
                    rendered = RenderTemplate(item, templateName, true) & rendered;
                }

            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                rendered = RenderTemplate((AspNetMvcEntitySetting)CurrentEntitySetting, templateName, multiTemplates) & rendered;
            }

            if (templateName.Equals(controllerClass))
            {
                rendered = RenderBaseController() & rendered;
            }

            return rendered;
        }

        public bool RenderTemplate(AspNetMvcEntitySetting entity, string templateName, bool multiTemplates)
        {
            bool notValid = (!entity.GenerateController && templateName.Equals(controllerClass)) || 
                            (!entity.IndexView && templateName.Equals(mvcIndexView)) || 
                            (!entity.DetailsView && templateName.Equals(mvcDetailsView)) || 
                            (!entity.CreateView && templateName.Equals(mvcCreateView)) || 
                            (!entity.EditView && templateName.Equals(mvcEditView));
            if (notValid)
            {
                if (!multiTemplates)
                {
                    System.Windows.Forms.MessageBox.Show(templateName + " is not valid.");
                }
                return false;
            }

            if (entity.Exclude &&
                System.Windows.Forms.MessageBox.Show(
                "Entity is excluded! Do you still want to generate it's file(s)?\r\nTemplate: " + templateName, 
                "Entity is excluded!", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return false;
            }
            var template = GetTemplateFile(entity, templateName);

            string fileName;
            if (template is MvcController)
            {
                fileName = Path.Combine(TargetControllersDir, template.GetDefaultFileName());
            }
            else if (template is AbstractMvcView)
            {
                fileName = ((AbstractMvcView)template).GetDefaultFilePath();
            }
            else
            {
                fileName = string.Empty;
            }

            return template.WriteToFile(fileName, OverwriteExisting || entity.OverwriteExisting);
        }

        public override bool RenderSelectedTemplate()
        {
            if (SelectedTemplate.Equals(mvcAllViews))
            {
                var templates = GetViewTemplates();
                return RenderTemplates(templates);
            }
            else
            {
                return RenderTemplate(SelectedTemplate, false);
            }
        }

        public override bool RenderAllTemplates()
        {
            var templates = GetViewTemplates();
            templates.Add(controllerClass);

            return RenderTemplates(templates);
        }

        private bool RenderTemplates(List<string> templates)
        {
            bool rendered = true;
            foreach (var template in templates)
            {
                rendered = RenderTemplate(template, true);
            }
            return rendered;
        }

        public bool RenderSecurityRoles()
        {
            var rolesTmpl = new MvcSecurityRoles(this);
            return rolesTmpl.WriteToFile(rolesTmpl.GetDefaultFilePath(), true);
        }

        public bool RenderBaseController()
        {
            var baseCtrlTmpl = new MvcBaseController(this);
            return baseCtrlTmpl.WriteToFile(baseCtrlTmpl.GetDefaultFilePath(), true);
        }

        private List<string> GetViewTemplates()
        {
            return new List<string>()
            {
                mvcIndexView,
                mvcDetailsView,
                mvcCreateView,
                mvcEditView,
            };
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>()
            { 
                controllerClass,
                mvcAllViews,
                mvcIndexView,
                mvcDetailsView,
                mvcCreateView,
                mvcEditView,
            };
        }
        private const string controllerClass = "Controller for ASP.NET MVC";
        private const string mvcAllViews = "All Views for ASP.NET MVC";
        private const string mvcIndexView = "Index View for ASP.NET MVC";
        private const string mvcDetailsView = "Details View for ASP.NET MVC";
        private const string mvcEditView = "Edit View for ASP.NET MVC";
        private const string mvcCreateView = "Create View for ASP.NET MVC";

        protected AbstractTemplate GetTemplateFile(AspNetMvcEntitySetting entity, string templateName)
        {
            AbstractTemplate tmp = null;
            switch (templateName)
            {
                case controllerClass:
                    tmp = new MvcController(entity);
                    break;

                case mvcIndexView:
                    tmp = new MvcIndexView(entity);
                    break;

                case mvcDetailsView:
                    tmp = new MvcDetailsView(entity);
                    break;

                case mvcEditView:
                    tmp = new MvcEditView(entity);
                    break;

                case mvcCreateView:
                    tmp = new MvcEditView(entity, true);
                    break;

                case mvcAllViews:
                    tmp = null;
                    break;

                default:
                    break;
            }

            return tmp;
        }


        public override string GetDefaultTargetFolder()
        {
            return Project != null ? Project.Name : "MVC Files";
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

        protected override void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.Project_PropertyChanged(sender, e);
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
        public virtual void onBaseControllerNameChanging(string newValue)
        {
            _baseControllerName = newValue.ToPascalCase();
        }
        public virtual void onBaseControllerNameChanged(string oldValue)
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
        public virtual void onDataContextClassChanging(string newValue)
        {
            _dataContextClass = newValue.ToPascalCase();
        }
        public virtual void onDataContextClassChanged(string oldValue)
        {
            RaisePropertyChanged("DataContextClass");
            foreach (var item in Entities)
            {
                item.DataContextClass = DataContextClass;
            }
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

        public void SetRolesToControllers()
        {   
            foreach (var item in Entities)
            {
                item.RolesCanDelete = RolesCanDelete;
                item.RolesCanEdit = RolesCanEdit;
                item.RolesCanView = RolesCanView;
                item.RolesCanCreate = RolesCanCreate;
            }

            RefreshSecurityRoles();
        }

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
        /// Returns [Authorize(Roles = \"roleName\")] attribute or empty string depending on roleName parameter.
        /// </summary>
        /// <param name="roleName">If value is 'users' returns [Authorize]. If value is empty string or 'everyone' returns empty string.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets AspNetMvcEntitySetting instance of property's object type
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public AspNetMvcEntitySetting GetForeignKeyEntity(AspNetMvcPropertySetting property)
        {
            if (!property.IsForeignKey())
            {
                return null;
            }

            var entity = (AspNetMvcEntitySetting)property.EntitySetting;
            var complexProp = entity.GetInheritedComplexProperties()
                                .FirstOrDefault(p => ((ComplexProperty)p.PropertyDefinition).DependentPropertyName == property.Name);

            AspNetMvcEntitySetting result = null;

            if (complexProp != null)
            {
                result = Entities
                        .FirstOrDefault(e => e.EntityDefinition.Name.Equals(complexProp.PropertyDefinition.TypeName));
            }
            var pkey = result.GetPrimeryKey();
            string s = result.EntityDefinition.DisplayMember;

            return result;
        }

        public void SetSaveParameterToControllers()
        {
            foreach (var item in Entities)
            {
                item.SaveParameter = SaveParameter;
            }
        }

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
