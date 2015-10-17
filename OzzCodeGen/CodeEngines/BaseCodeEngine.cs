using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using OzzUtils.Savables;

namespace OzzCodeGen.CodeEngines
{
    public abstract class BaseCodeEngine : SavableObject, INotifyPropertyChanged
    {
        [XmlIgnore]
        public abstract string EngineId { get; }
        [XmlIgnore]
        public abstract string ProjectTypeName { get; }
        public abstract string GetDefaultFileName();

        [XmlIgnore]
        public CodeGenProject Project
        {
            get { return _project; }
            set
            {
                if (_project != null)
                {
                    _project.PropertyChanged -= Project_PropertyChanged;
                }
                _project = value;
                if (_project != null)
                {
                    _project.PropertyChanged += Project_PropertyChanged;
                    RefreshFromProject(true);
                }
            }
        }
        CodeGenProject _project;

        protected virtual void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("TargetFolder"))
            {
                OnTargetDirectoryChanged();
            }
        }

        public virtual void OnProjectNameChanging(string newValue) { }
        public virtual void OnProjectNameChanged(string oldValue) { }

        public virtual void OnProjectNamespaceChanging(string newValue)
        {
            isNamespaceDefault = NamespaceName.Equals(GetDefaultNamespace());
        }
        public virtual void OnProjectNamespaceChanged(string oldValue)
        {
            if (isNamespaceDefault)
            {
                NamespaceName = GetDefaultNamespace();
            }
        }
        bool isNamespaceDefault;

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

        public string MergeWithNamespace(string name)
        {
            return string.Format("{0}.{1}", NamespaceName, name);
        }

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

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_targetFolder))
                {
                    _targetFolder = GetDefaultTargetFolder();
                }
                return _targetFolder;
            }
            set
            {
                if (_targetFolder == value) return;
                OnTargetDirectoryChanging();
                _targetFolder = value;
                RaisePropertyChanged("TargetFolder");
                OnTargetDirectoryChanged();
            }
        }
        string _targetFolder;
        protected virtual void OnTargetDirectoryChanging() { }
        protected virtual void OnTargetDirectoryChanged()
        {
            RaisePropertyChanged("TargetDirectory");
        }
        public abstract string GetDefaultTargetFolder();

        [XmlIgnore]
        public string TargetDirectory
        {
            get
            {
                if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
                {
                    return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, TargetFolder));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

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
                CanRender = (RenderAllEntities || CurrentEntitySetting != null) && !string.IsNullOrEmpty(SelectedTemplate);
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
                CanRender = (RenderAllEntities || CurrentEntitySetting != null) && !string.IsNullOrEmpty(SelectedTemplate);
            }
        }
        private bool _renderAll;


        [XmlIgnore]
        public bool CanRender
        {
            get { return _canRender; }
            set
            {
                _canRender = value;
                RaisePropertyChanged("CanRender");
            }
        }
        private bool _canRender;


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
            EntitySettings = entities.OrderBy(e => e.EntityDefinition.DisplayOrder).ToList();
        }

        protected abstract BaseEntitySetting GetDefaultSetting(EntityDefinition entity);
        protected abstract void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems);
        public abstract bool RenderSelectedTemplate();
        public abstract bool RenderAllTemplates();

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
