using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.DataLayer.Templates;
using OzzCodeGen.AppEngines.DataLayer.UI;
using OzzCodeGen.AppEngines.Localization;
using OzzCodeGen.Definitions;
using OzzCodeGen.Templates;
using OzzCodeGen.UI;
using OzzCodeGen.Utilities;

namespace OzzCodeGen.AppEngines.DataLayer
{
    [XmlInclude(typeof(DalEntitySetting))]
    public class DataLayerEngine : BaseAppEngine
    {
        public override string EngineId { get { return EngineTypes.EfDbFirstDataLayerId; } }
        [XmlIgnore]
        public static string DefaultFileName { get { return "DataLayer.settings"; } }
        public override string ProjectTypeName { get { return "DataLayer for Database First"; } }

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        public string CustFilesDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_custFilesDirectory)) _custFilesDirectory = TargetDirectory;
                return _custFilesDirectory;
            }
            set
            {
                if (_custFilesDirectory == value) return;
                _custFilesDirectory = value;
                RaisePropertyChanged("CustFilesDirectory");
            }
        }
        private string _custFilesDirectory;

        public string ViewModelNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_viewModelNamespace)) _viewModelNamespace = this.Project.NamespaceName + ".ViewModels";
                return _viewModelNamespace;
            }
            set
            {
                if (_viewModelNamespace == value) return;
                _viewModelNamespace = value;
                RaisePropertyChanged("ViewModelNamespace");
            }
        }
        private string _viewModelNamespace;

        public string ViewModelFilesDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_viewModelFilesDirectory)) _viewModelFilesDirectory = TargetDirectory;
                return _viewModelFilesDirectory;
            }
            set
            {
                if (_viewModelFilesDirectory == value) return;
                _viewModelFilesDirectory = value;
                RaisePropertyChanged("ViewModelFilesDirectory");
            }
        }
        private string _viewModelFilesDirectory;

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new DalEntitySetting()
            {
                DataModel = this.Project.DataModel,
                AppEngine = this,
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

        [XmlIgnore]
        public List<DalEntitySetting> Entities
        {
            get { return _entities; }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged("Entities");
            }
        }
        private List<DalEntitySetting> _entities;

        protected override void OnEntitySettingsChanged()
        {
            var entities = new List<DalEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (DalEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            Entities = entities;
        }



        public bool UseDisplayName
        {
            get { return _useDisplayName; }
            set
            {
                _useDisplayName = value;
                RaisePropertyChanged("UseDisplayName");
            }
        }
        private bool _useDisplayName;
        

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


        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var dataLayerSetting = (DalEntitySetting)setting;
            dataLayerSetting.DataModel = Project.DataModel;
            ((DalEntitySetting)setting).AppEngine = this;

            List<DalPropertySetting> remvProp = new List<DalPropertySetting>();
            foreach (var dalProp in dataLayerSetting.Properties)
            {
                if (entity.Properties.FirstOrDefault(p => p.Name == dalProp.Name) == null)
                {
                    remvProp.Add(dalProp);
                }
            }
            foreach (var dalProp in remvProp)
            {
                dataLayerSetting.Properties.Remove(dalProp);
            }

            foreach (var property in entity.Properties)
            {
                var ps = dataLayerSetting.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (ps == null)// && property.DefinitionType != DefinitionType.Collection
                {
                    ps = GetDefaultPropertySetting(property, dataLayerSetting);
                }
                else //if (ps != null)
                {
                    ps.EntitySetting = setting;
                }
            }
        }

        protected DalPropertySetting GetDefaultPropertySetting(BaseProperty property, DalEntitySetting setting)
        {
            var ps = new DalPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting
            };
            setting.Properties.Add(ps);

            return ps;
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>() { 
                metadataClass,
                mvcViewModel
            };
        }
        private const string customPartial = "Custom Partial Entity File";
        private const string metadataClass = "Metadata Class";
        private const string mvcViewModel = "ViewModel for ASP.NET MVC";

        protected BaseDalTemplate GetTemplateFile(DalEntitySetting entitySettings, string templateName)
        {
            BaseDalTemplate template = null;
            switch (templateName)
            {
                case mvcViewModel:
                    template = new MvcServerVM();
                    break;

                case metadataClass:
                    template = new MetadataClass();
                    break;

                case customPartial:
                    template = new CustomEntityFile();
                    break;

                default:
                    break;
            }

            template.EntitySetting = entitySettings;
            template.NamespaceName = NamespaceName;
            template.ViewModelNamespace = ViewModelNamespace;

            return template;
        }

        protected void RenderCustomFile(DalEntitySetting entitySettings)
        {
            if (!entitySettings.CreateCustomFile) return;

            var template = GetTemplateFile(entitySettings, customPartial);
            string fileName = Path.Combine(CustFilesDirectory, template.GetDefaultFileName());
            if (!File.Exists(fileName))
            {
                template.WriteToFile(fileName, false);
            }
        }

        protected bool RenderTemplate(DalEntitySetting entitySettings)
        {
            if (string.IsNullOrEmpty(SelectedTemplate))
                return false;

            RenderCustomFile(entitySettings);
            var template = GetTemplateFile(entitySettings, SelectedTemplate);
            string fileName;

            if (template is MvcServerVM)
            {
                fileName = Path.Combine(ViewModelFilesDirectory, template.GetDefaultFileName());
            }
            else
            {
                fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
            }

            return template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting);
        }

        public override bool RenderSelectedTemplate()
        {
            if (RenderAllEntities)
            {
                bool allWritten = true;
                foreach (DalEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
                {
                    allWritten = RenderTemplate(setting) & allWritten;
                }
                return allWritten;
            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                return RenderTemplate((DalEntitySetting)CurrentEntitySetting);
            }
        }

        public override bool RenderAllTemplates()
        {
            return false;
        }

        protected override UserControl GetUiControl()
        {
            if (_uiControl == null) SetNewUI();
            return _uiControl;
        }
        DataLayerEngineUI _uiControl = null;

        private void SetNewUI()
        {
            _uiControl = new DataLayerEngineUI();
            _uiControl.AppEngine = this;
            //var mnuOptions = _uiControl.mnuOptions;
            //mnuOptions.Visibility = Visibility.Visible;
            MenuItem mnuExclude = UiTools.CreateMenuItem("Exclude", "Exclude selected property");
            MenuItem mnuExcludeAll = UiTools.CreateMenuItem("Exclude Similar", "Exclude properties with same name");
            MenuItem mnuSetDataType = UiTools.CreateMenuItem("Set DataType to Same", "Set DataType to same named properties in other entities");
            MenuItem mnuSetUiHint = UiTools
                .CreateMenuItem("Set UIHint to Same", "Set UIHint to same named properties in other entities");
            MenuItem mnuClearReq = UiTools.CreateMenuItem("Clear Required Settings", "Clear all Required Settings in this entity");

            mnuExclude.Click += ExludeProperty;
            mnuExcludeAll.Click += ExludeSimilarProperties;
            mnuSetDataType.Click += SetDataType;
            mnuClearReq.Click += ClearReq;

            _uiControl.grdEntitySettings.ContextMenu.Items.Add(mnuClearReq);

            _uiControl.grdPropertySettings.ContextMenu.Items.Add(mnuExclude);
            _uiControl.grdPropertySettings.ContextMenu.Items.Add(mnuExcludeAll);
            _uiControl.grdPropertySettings.ContextMenu.Items.Add(mnuSetDataType);
            _uiControl.grdPropertySettings.ContextMenu.Items.Add(mnuSetUiHint);
        }

        private void ClearReq(object o, RoutedEventArgs ea)
        {
            var entity = GetSelectedEntity();
            if (entity == null) return;

            foreach (var property in entity.Properties)
            {
                property.Required = string.Empty;
            }
        }

        private void SetDataType(object o, RoutedEventArgs ea)
        {
            DalPropertySetting property = GetSelectedProperty();
            if (property == null) return;
            string dataType = property.DataType;

            int similarCount = 0;
            foreach (DalEntitySetting entity in EntitySettings)
            {
                var similar = entity.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (similar != null && similar != property)
                {
                    similarCount++;
                    similar.DataType = dataType;
                }
            }
            MessageBox.Show(string.Format("Found {0} similar properties.", similarCount));
        }

        private void ExludeSimilarProperties(object o, RoutedEventArgs ea)
        {
            DalPropertySetting property = GetSelectedProperty();
            if (property == null) return;
            int similarCount = 0;
            int excludeCount = 0;
            foreach (DalEntitySetting entity in EntitySettings)
            {
                var similar = entity.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (similar != null)
                {
                    similarCount++;
                    if (!similar.Exclude) excludeCount++;
                    similar.Exclude = true;
                }
            }
            MessageBox.Show(string.Format("Found {0} similar properties, {1} of them was not excluded!", similarCount, excludeCount));
        }

        private void ExludeProperty(object o, RoutedEventArgs ea)
        {
            DalPropertySetting property = GetSelectedProperty();
            if (property == null) return;
            property.Exclude = true;
        }

        private DalPropertySetting GetSelectedProperty()
        {
            if (_uiControl.grdPropertySettings.SelectedItem == null) return null;
            return (DalPropertySetting)_uiControl.grdPropertySettings.SelectedItem;
        }

        private DalEntitySetting GetSelectedEntity()
        {
            if (_uiControl.grdEntitySettings.SelectedItem == null) return null;
            return (DalEntitySetting)_uiControl.grdEntitySettings.SelectedItem;
        }

        public override UserControl GetSettingsDlgUI()
        {
            return null;
        }

        public override string GetDefaultTargetDir(string targetSolutionDir)
        {
            if (string.IsNullOrEmpty(targetSolutionDir))
                return string.Empty;
            return Path.Combine(targetSolutionDir, "Models");
        }


        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static DataLayerEngine OpenFile(string fileName)
        {
            DataLayerEngine instance = GetInstanceFromFile(
               fileName,
               typeof(DataLayerEngine)) as DataLayerEngine;

            return instance;
        }
    }
}
