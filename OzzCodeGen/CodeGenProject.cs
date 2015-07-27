using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines;
using OzzCodeGen.Definitions;
using OzzCodeGen.Providers;

namespace OzzCodeGen
{
    public class CodeGenProject : BaseSavable, INotifyPropertyChanged
    {
        protected List<BaseAppEngine> _appEngines;

        public CodeGenProject()
        {
            //this.PropertyChanged += (o, e) =>
            //{
            //    if (e.PropertyName != "HasProjectChanges") HasProjectChanges = true;
            //};

            _appEngines = new List<BaseAppEngine>();
        }

        public Guid ProjectId
        {
            set { _projectId = value; }
            get
            {
                if (!_projectId.HasValue) _projectId = Guid.NewGuid();
                return _projectId.Value;
            }
        }
        Guid? _projectId;

        public string Name { get; set; }

        public string NamespaceName
        {
            get { return _namespaceName; }
            set
            {
                if (_namespaceName == value) return;
                _namespaceName = value;
                RaisePropertyChanged("NamespaceName");
            }
        }
        private string _namespaceName;

        public string ModelProviderId
        {
            get { return _modelProviderId; }
            set
            {
                if (_modelProviderId == value) return;
                _modelProviderId = value;
                RaisePropertyChanged("ModelProviderId");
            }
        }
        private string _modelProviderId;

        public string TargetSolutionDir
        {
            get { return _targetSolutionDir; }
            set
            {
                if (_targetSolutionDir == value) return;
                _targetSolutionDir = value;
                RaisePropertyChanged("TargetSolutionDir");
            }
        }
        private string _targetSolutionDir;


        public string ModelSource
        {
            get { return _modelSource; }
            set
            {
                if (_modelSource == value) return;
                _modelSource = value;
                RaisePropertyChanged("ModelSource");
            }
        }
        private string _modelSource;

        public string EntityTrackEnum
        {
            get
            {
                if (string.IsNullOrEmpty(_entityTrackEnum))
                {
                    _entityTrackEnum = "EntityType";
                }
                return _entityTrackEnum;
            }
            set
            {
                _entityTrackEnum = value;
                RaisePropertyChanged("EntityTrackEnum");
            }
        }
        private string _entityTrackEnum;
        
        public EnumDefinitionList EnumDefinitions
        {
            get { return _enumDefinitions; }
            set
            {
                _enumDefinitions = value;
                RaisePropertyChanged("EnumDefinitions");
            }
        }
        EnumDefinitionList _enumDefinitions;

        public DataModel DataModel
        {
            get { return _dataModel; }
            set
            {
                if (_dataModel == value) return;
                _dataModel = value;
                RaisePropertyChanged("DataModel");
            }
        }
        DataModel _dataModel;

        public bool RemoveEntity(EntityDefinition item)
        {
            foreach (var engine in _appEngines)
            {
                var entity = engine.EntitySettings.FirstOrDefault(e => e.EntityDefinition == item);
                if (entity != null)
                {
                    engine.EntitySettings.Remove(entity);
                }
            }

            bool result = DataModel.Remove(item);

            foreach (var engine in _appEngines)
            {
                engine.RefreshFromProject(true);
            }

            return result;
        }

        public bool StripSourcePrefixes
        {
            get { return _stripSourcePrefixes; }
            set
            {
                if (_stripSourcePrefixes == value) return;
                _stripSourcePrefixes = value;
                RaisePropertyChanged("StripSourcePrefixes");
            }
        }
        bool _stripSourcePrefixes;

        public string SourceClassPrefix
        {
            get { return _sourceClassPrefix; }
            set
            {
                if (_sourceClassPrefix == value) return;
                _sourceClassPrefix = value;
                RaisePropertyChanged("SourceClassPrefix");
            }
        }
        string _sourceClassPrefix = string.Empty;

        [XmlIgnore]
        public bool HasProjectChanges
        {
            get { return _hasProjectChanges; }
            set
            {
                if (_hasProjectChanges == value) return;
                _hasProjectChanges = value;
                RaisePropertyChanged("HasProjectChanges");
            }
        }
        private bool _hasProjectChanges;

        
        public List<string> AppEngineList
        {
            get
            {
                if (_appEngineList == null) _appEngineList = new List<string>();
                return _appEngineList;
            }
            set
            {
                _appEngineList = value;
                RaisePropertyChanged("AppEngineList");
            }
        }
        private List<string> _appEngineList;

        public string ServiceUrl
        {
            get { return _serverUrl; }
            set
            {
                _serverUrl = value;
                RaisePropertyChanged("ServerUrl");
            }
        }
        private string _serverUrl;


        [XmlIgnore]
        public BaseAppEngine CurrentAppEngine
        {
            get { return _currentAppEngine; }
            set
            {
                if (_currentAppEngine == value) return;
                _currentAppEngine = value;
                RaisePropertyChanged("CurrentAppEngine");
            }
        }
        private BaseAppEngine _currentAppEngine;


        [XmlIgnore]
        public PropertyDefaultSettingList DefaultPropertySettings
        {
            get
            {
                if (_defaultPropertySettings == null) _defaultPropertySettings = new PropertyDefaultSettingList();
                return _defaultPropertySettings;
            }
            set
            {
                _defaultPropertySettings = value;
                RaisePropertyChanged("DefaultPropertySettings");
            }
        }
        PropertyDefaultSettingList _defaultPropertySettings;

        [XmlIgnore]
        public string DefaultPropertySettingsFile
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(SavedFileName), "DefaultProperties.xml");
            }
        }

        public bool AddToDefaultProperties(BaseProperty p)
        {
            if (DefaultPropertySettings.FirstOrDefault(dp => dp.Name == p.Name) != null) return false;
            var defaultProp = p.GetDefaultSetting();
            DefaultPropertySettings.Add(defaultProp);
            RaisePropertyChanged("DefaultPropertySettings");
            return true;
        }

        public bool ApplyDisplayNameToOthers(BaseProperty property)
        {
            foreach (var entity in DataModel)
            {
                var found = entity.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (found != null && found != property) found.DisplayName = property.DisplayName;
            }
            return true;
        }

        public bool ApplyToAllProperties(PropertyDefaultSetting setting)
        {
            foreach (var entity in DataModel)
            {
                var property = entity.Properties.FirstOrDefault(p => p.Name == setting.Name);
                if (property != null) property.ApplySetting(setting);
            }
            return true;
        }

        public BaseAppEngine GetAppEngine(string appEngineId)
        {
            return _appEngines.FirstOrDefault(t => t.EngineId == appEngineId);
        }

        public void AddEngine(BaseAppEngine appEngine)
        {
            _appEngines.Add(appEngine);
            appEngine.Project = this;
            AddEngine(appEngine.EngineId);
            CurrentAppEngine = appEngine;
        }

        public void AddEngine(string appEngine)
        {
            if (AppEngineList.Contains(appEngine)) return;
            AppEngineList.Insert(0, appEngine);
            RaisePropertyChanged("AppEngineList");
            AddEngine(EngineTypes.GetInstance(appEngine));
        }

        public void RefreshDataModel(IModelProvider modelProvider, bool cleanRemovedItems)
        {
            modelProvider.RefreshDataModel(ModelSource, DataModel, cleanRemovedItems);
            foreach (var engine in this._appEngines)
            {
                engine.RefreshFromProject(true);
            }
        }

        public override void SaveToFile()
        {
            base.SaveToFile();
            SaveBoundFiles();
            HasProjectChanges = false;
        }

        public override void SaveToFile(string FileName)
        {
            base.SaveToFile(FileName);
            SaveBoundFiles();
            HasProjectChanges = false;
        }

        protected void SaveBoundFiles()
        {
            DefaultPropertySettings.SaveToFile(DefaultPropertySettingsFile);

            foreach (var engine in _appEngines)
            {
                engine.SaveToFile();
            }
        }

        public void LoadBoundFiles()
        {
            DefaultPropertySettings = PropertyDefaultSettingList.OpenFile(DefaultPropertySettingsFile);

            string dir = Path.GetDirectoryName(SavedFileName);
            foreach (string targetId in AppEngineList)
            {
                AddEngine(EngineTypes.OpenFile(dir, targetId));
            }
            if (_appEngines.FirstOrDefault() != null)
            {
                CurrentAppEngine = _appEngines.FirstOrDefault();
            }
        }

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static CodeGenProject OpenFile(string fileName)
        {
            //StreamReader reader = new StreamReader(fileName);
            //XmlSerializer x = new XmlSerializer(typeof(CodeGenProject));
            //CodeGenProject instance = x.Deserialize(reader) as CodeGenProject;
            //reader.Close();
            //instance.SavedFileName = fileName;

            CodeGenProject instance = GetInstanceFromFile(
                fileName, 
                typeof(CodeGenProject)) as CodeGenProject;
            instance.LoadBoundFiles();
            return instance;
        }

        public virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
