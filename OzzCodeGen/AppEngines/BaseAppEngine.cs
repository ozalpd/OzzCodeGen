using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using OzzCodeGen.Templates;

namespace OzzCodeGen.AppEngines
{
    public abstract class BaseAppEngine : BaseSavable, INotifyPropertyChanged
    {
        [XmlIgnore]
        public abstract string EngineId { get; }
        [XmlIgnore]
        public abstract string ProjectTypeName { get; }
        public abstract string GetDefaultFileName();

        public Guid ProjectId { get; set; }

        [XmlIgnore]
        public CodeGenProject Project
        {
            get { return _project; }
            set
            {
                _project = value;
                ProjectId = Project.ProjectId; //TODO: put a check point here
                RefreshFromProject(true);
            }
        }
        CodeGenProject _project;

        /// <summary>
        /// Name of the entity
        /// </summary>
        public TargetLanguage Language
        {
            get { return _language; }
            set
            {
                if (_language == value) return;
                _language = value;
                RaisePropertyChanged("Language");
            }
        }
        private TargetLanguage _language;

        public string NamespaceName
        {
            get
            {
                if (string.IsNullOrEmpty(_namespaceName))
                    _namespaceName = GetDefaultNamespace();
                return _namespaceName;
            }
            set
            {
                if (_namespaceName == value) return;
                _namespaceName = value;
                RaisePropertyChanged("NamespaceName");
            }
        }
        private string _namespaceName;

        public string TargetDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_targetDirectory) && Project != null)
                {
                    _targetDirectory = GetDefaultTargetDir(Project.TargetSolutionDir);
                }
                return _targetDirectory;
            }
            set
            {
                if (_targetDirectory == value) return;
                _targetDirectory = value;
                RaisePropertyChanged("TargetDirectory");
            }
        }
        private string _targetDirectory;

        [XmlIgnore]
        public bool OverwriteExisting
        {
            get { return _overwriteExisting; }
            set
            {
                if (_overwriteExisting == value) return;
                _overwriteExisting = value;
                RaisePropertyChanged("OverwriteExisting");
            }
        }
        private bool _overwriteExisting;


        [XmlIgnore]
        public List<string> Templates
        {
            get
            {
                if (_templates == null)
                {
                    _templates = GetTemplateList();
                    SelectedTemplate = _templates.FirstOrDefault();
                }
                return _templates;
            }
            set
            {
                _templates = value;
                RaisePropertyChanged("Templates");
            }
        }
        private List<string> _templates;

        public List<BaseEntitySetting> EntitySettings
        {
            get { return _entitySettings; }
            set
            {
                if (_entitySettings == value) return;
                OnEntitySettingsChanging();
                _entitySettings = value;
                RaisePropertyChanged("EntitySettings");
                OnEntitySettingsChanged();
            }
        }
        private List<BaseEntitySetting> _entitySettings;
        protected virtual void OnEntitySettingsChanging() { }
        protected virtual void OnEntitySettingsChanged() { }

        [XmlIgnore]
        public BaseEntitySetting CurrentEntitySetting
        {
            get
            {
                return _currentEntitySetting;
            }
            set
            {
                if (_currentEntitySetting == value) return;
                OnCurrentEntitySettingChanging();
                _currentEntitySetting = value;
                RaisePropertyChanged("CurrentEntitySetting");
                OnCurrentEntitySettingChanged();
            }
        }
        private BaseEntitySetting _currentEntitySetting;
        protected virtual void OnCurrentEntitySettingChanging() { }
        protected virtual void OnCurrentEntitySettingChanged() { }

        public bool RenderAllEntities
        {
            get { return _renderAll; }
            set
            {
                _renderAll = value;
                RaisePropertyChanged("RenderAllEntities");
            }
        }
        private bool _renderAll;

        public virtual void RefreshFromProject(bool cleanRemovedItems)
        {
            List<BaseEntitySetting> entities;
            if (EntitySettings == null) entities = new List<BaseEntitySetting>();
            else
            {
                entities = EntitySettings;
                EntitySettings = null;
            }

            List<BaseEntitySetting> remvEntities = new List<BaseEntitySetting>();
            foreach (var item in entities)
            {
                var modelEntity = Project.DataModel.FirstOrDefault(e => e.Name == item.Name);
                if (modelEntity == null)
                {
                    remvEntities.Add(item);
                }
            }
            foreach (var item in remvEntities)
            {
                entities.Remove(item);
            }

            foreach (var e in Project.DataModel)
            {
                var target = entities.FirstOrDefault(t => t.Name == e.Name);
                if (target == null)
                {
                    target = GetDefaultSetting(e);
                    entities.Add(target);
                }
                RefreshSetting(target, e, cleanRemovedItems);
            }
            EntitySettings = entities.OrderBy(e => e.Name).ToList();
        }

        protected abstract BaseEntitySetting GetDefaultSetting(EntityDefinition entity);
        protected abstract void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems);
        public abstract bool RenderSelectedTemplate();
        public abstract bool RenderAllTemplates();
        public abstract string GetDefaultTargetDir(string targetSolutionDir);

        protected abstract UserControl GetUiControl();
        public abstract UserControl GetSettingsDlgUI();

        public abstract List<string> GetTemplateList();
        [XmlIgnore]
        public string SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                if (_selectedTemplate == value) return;
                _selectedTemplate = value;
                RaisePropertyChanged("SelectedTemplate");
            }
        }
        private string _selectedTemplate;

        [XmlIgnore]
        public UserControl UiControl
        {
            get
            {
                if (_uiControl == null) _uiControl = GetUiControl();
                return _uiControl;
            }
        }
        private UserControl _uiControl;
        

        public override void SaveToFile()
        {
            SavedFileName = Path.Combine(
                       Path.GetDirectoryName(Project.SavedFileName),
                       GetDefaultFileName());
            
            base.SaveToFile();
        }

        protected virtual string GetDefaultNamespace()
        {
            if (Project == null)
                return string.Empty;

            return Project.NamespaceName.EndsWith(".Models") ?
                        Project.NamespaceName :
                        Project.NamespaceName + ".Models";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return this.EngineId;
        }
    }
}
