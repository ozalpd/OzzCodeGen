using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines;
using OzzCodeGen.Definitions;
using OzzCodeGen.Providers;
using OzzUtils.Savables;

namespace OzzCodeGen
{
    public class CodeGenProject : SavableObject, INotifyPropertyChanged
    {
        protected List<BaseCodeEngine> _codeEngines;

        public CodeGenProject()
        {
            //this.PropertyChanged += (o, e) =>
            //{
            //    if (e.PropertyName != "HasProjectChanges") HasProjectChanges = true;
            //};

            _codeEngines = new List<BaseCodeEngine>();
        }


        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = "New Project";
                return _name;
            }
            set
            {
                if (_namespaceName == value)
                    return;
                string oldValue = _name;
                onNameChanging(value);
                onNameChanged(oldValue);
            }
        }
        protected virtual void onNameChanging(string newValue)
        {
            foreach (var item in this._codeEngines)
            {
                item.OnProjectNameChanging(newValue);
            }
            _name = newValue;
        }
        protected virtual void onNameChanged(string oldValue)
        {
            RaisePropertyChanged("Name");
            foreach (var item in this._codeEngines)
            {
                item.OnProjectNameChanged(oldValue);
            }
        }
        private string _name;

        public string NamespaceName
        {
            get { return _namespaceName; }
            set
            {
                if (_namespaceName == value)
                    return;
                string oldValue = _namespaceName;
                onNamespaceNameChanging(value);
                onNamespaceNameChanged(oldValue);
            }
        }
        protected virtual void onNamespaceNameChanging(string newValue)
        {
            foreach (var item in this._codeEngines)
            {
                item.OnProjectNamespaceChanging(newValue);
            }
            _namespaceName = newValue;
        }
        protected virtual void onNamespaceNameChanged(string oldValue)
        {
            RaisePropertyChanged("NamespaceName");
            foreach (var item in this._codeEngines)
            {
                item.OnProjectNamespaceChanged(oldValue);
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

        [XmlIgnore]
        public IModelProvider ModelProvider
        {
            get { return _modelProvider; }
            set
            {
                _modelProvider = value;
                _modelProvider.Project = this;
                RaisePropertyChanged("ModelProvider");
            }
        }
        IModelProvider _modelProvider;

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_targetFolder))
                {
                    _targetFolder = "..\\Generated Codes";
                }
                return _targetFolder;
            }
            set
            {
                if (_targetFolder == value) return;
                _targetFolder = value;
                RaisePropertyChanged("TargetFolder");
            }
        }
        string _targetFolder;

        [XmlIgnore]
        public string TargetSolutionDir
        {
            get
            {
                if(string.IsNullOrEmpty(TargetFolder) || string.IsNullOrEmpty(SavedFileName))
                {
                    return string.Empty;
                }
                else
                {
                    return Path.GetFullPath(
                            Path.Combine(
                                Path.GetDirectoryName(SavedFileName), TargetFolder));
                }
            }
        }


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
            foreach (var engine in _codeEngines)
            {
                var entity = engine.EntitySettings.FirstOrDefault(e => e.EntityDefinition == item);
                if (entity != null)
                {
                    engine.EntitySettings.Remove(entity);
                }
            }

            bool result = DataModel.Remove(item);

            foreach (var engine in _codeEngines)
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

        
        public List<string> CodeEngineList
        {
            get
            {
                if (_codeEngineList == null) _codeEngineList = new List<string>();
                return _codeEngineList;
            }
            set
            {
                _codeEngineList = value;
                RaisePropertyChanged("CodeEngineList");
            }
        }
        private List<string> _codeEngineList;

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
        public BaseCodeEngine CurrentCodeEngine
        {
            get { return _currentCodeEngine; }
            set
            {
                if (_currentCodeEngine == value) return;
                _currentCodeEngine = value;
                RaisePropertyChanged("CurrentCodeEngine");
            }
        }
        private BaseCodeEngine _currentCodeEngine;

        public bool ApplyDisplayNameToOthers(BaseProperty property)
        {
            foreach (var entity in DataModel)
            {
                var found = entity.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (found != null && found != property) found.DisplayName = property.DisplayName;
            }
            return true;
        }

        public BaseCodeEngine GetCodeEngine(string codeEngineId)
        {
            return _codeEngines.FirstOrDefault(t => t.EngineId == codeEngineId);
        }

        public void AddEngine(BaseCodeEngine codeEngine)
        {
            _codeEngines.Add(codeEngine);
            codeEngine.Project = this;
            AddEngine(codeEngine.EngineId);
            CurrentCodeEngine = codeEngine;
        }

        public void AddEngine(string codeEngine)
        {
            if (CodeEngineList.Contains(codeEngine)) return;
            CodeEngineList.Insert(0, codeEngine);
            RaisePropertyChanged("CodeEngineList");
            AddEngine(EngineTypes.GetInstance(codeEngine));
        }

        public void RefreshDataModel(bool cleanRemovedItems)
        {
            ModelProvider.RefreshDataModel(ModelSource, DataModel, cleanRemovedItems);
            foreach (var engine in this._codeEngines)
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
            RaisePropertyChanged("TargetSolutionDir");
            RaisePropertyChanged("TargetFolder");
        }

        protected void SaveBoundFiles()
        {
            foreach (var engine in _codeEngines)
            {
                engine.SaveToFile();
            }
        }

        public void LoadBoundFiles()
        {
            string dir = Path.GetDirectoryName(SavedFileName);
            foreach (string targetId in CodeEngineList)
            {
                AddEngine(EngineTypes.OpenFile(dir, targetId));
            }
            if (_codeEngines.FirstOrDefault() != null)
            {
                CurrentCodeEngine = _codeEngines.FirstOrDefault();
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
