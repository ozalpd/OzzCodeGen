using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.AspNetMvc.Templates;
using OzzCodeGen.AppEngines.AspNetMvc.UI;
using OzzCodeGen.AppEngines.DataLayer;
using OzzCodeGen.AppEngines.Localization;
using OzzCodeGen.Definitions;
using System.Collections.ObjectModel;

namespace OzzCodeGen.AppEngines.AspNetMvc
{
    [XmlInclude(typeof(AspNetMvcEntitySetting))]
    public class AspNetMvcEngine : BaseAppEngine
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
                DataModel = this.Project.DataModel,
                AppEngine = this,
                Exclude = entity.Abstract,
                GenerateController = !entity.Abstract,
                IndexView = !entity.Abstract,
                DetailsView = !entity.Abstract,
                CreateView = !entity.Abstract,
                EditView = !entity.Abstract,
                Name = entity.Name
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
            entitySetting.AppEngine = this;

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
        }

        protected override System.Windows.Controls.UserControl GetUiControl()
        {
            if (_engineUI == null)
            {
                _engineUI = new MvcEngineUI()
                {
                    AppEngine = this
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
            if (RenderAllEntities)
            {
                bool rendered = true;
                foreach (var item in Entities)
                {
                    rendered = RenderTemplate(item, templateName, true) & rendered;
                }

                return rendered;
            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                return RenderTemplate((AspNetMvcEntitySetting)CurrentEntitySetting, templateName, multiTemplates);
            }
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
        private const string customPartial = "Custom partial file for ASP.NET MVC Controller";
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
            return "MVC Files";
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

        protected override void OnTargetDirectoryChanged()
        {
            base.OnTargetDirectoryChanged();
            RaisePropertyChanged("TargetControllersDir");
            RaisePropertyChanged("TargetViewsDir");
        }

        public string ControllersNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_controllersNamespace) && Project != null)
                {
                    _controllersNamespace = Project.NamespaceName + "." + "Controllers";
                }
                return _controllersNamespace;
            }
            set
            {
                _controllersNamespace = value;
                RaisePropertyChanged("ControllersNamespace");
            }
        }
        private string _controllersNamespace;

        public string ModelsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_modelsNamespace) && DataLayerEngine != null)
                {
                    _modelsNamespace = DataLayerEngine.NamespaceName;
                }
                else if (string.IsNullOrEmpty(_modelsNamespace) && Project != null)
                {
                    _modelsNamespace = Project.NamespaceName + "." + "Models";
                }
                return _modelsNamespace;
            }
            set
            {
                _modelsNamespace = value;
                RaisePropertyChanged("ModelsNamespace");
            }
        }
        private string _modelsNamespace;

        public string ViewModelsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_viewModelsNamespace) && Project != null)
                {
                    _viewModelsNamespace = Project.NamespaceName + "." + "ViewModels";
                }
                return _viewModelsNamespace;
            }
            set
            {
                _viewModelsNamespace = value;
                RaisePropertyChanged("ViewModelsNamespace");
            }
        }
        private string _viewModelsNamespace;

        public string ViewsNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_viewsNamespace) && Project != null)
                {
                    _viewsNamespace = Project.NamespaceName + "." + "Views";
                }
                return _viewsNamespace;
            }
            set
            {
                _viewsNamespace = value;
                RaisePropertyChanged("ViewsNamespace");
            }
        }
        private string _viewsNamespace;


        [XmlIgnore]
        public DataLayerEngine DataLayerEngine
        {
            get
            {
                if (_dataLayerEngine == null && Project != null)
                {
                    var engine = Project.GetAppEngine(EngineTypes.EfDbFirstDataLayerId);
                    _dataLayerEngine = engine != null ? (DataLayerEngine)engine : null;
                }
                return _dataLayerEngine;
            }
        }
        private DataLayerEngine _dataLayerEngine;

        [XmlIgnore]
        public ResxEngine ResxEngine
        {
            get
            {
                if (_resxEngine == null && Project != null)
                {
                    var engine = Project.GetAppEngine(EngineTypes.LocalizationResxGenId);
                    _resxEngine = engine != null ? (ResxEngine)engine : null;
                }
                return _resxEngine;
            }
        }
        private ResxEngine _resxEngine;



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
