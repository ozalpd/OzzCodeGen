using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsDbRepository
{
    public abstract class BaseCsDbRepositoryEngine<T> : BaseAppInfraCodeEngine where T : BaseCsDbRepositoryPropertySetting
    {
        protected string GetRepositoryName(string entityName)
        {
            return $"{entityName}Repository";
        }
        public string MetadataRepositoryName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_metadataName))
                {
                    _metadataName = "MetadataRepository";
                }
                return _metadataName;
            }
            set
            {
                if (_metadataName == value) return;
                _metadataName = value;
                RaisePropertyChanged(nameof(MetadataRepositoryName));
            }
        }
        private string _metadataName;


        public string BaseRepositoryClassName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_baseRepositoryClassName))
                    _baseRepositoryClassName = "BaseDatabaseRepository";
                return _baseRepositoryClassName;
            }
            set
            {
                if (_baseRepositoryClassName == value) return;
                _baseRepositoryClassName = value;
                RaisePropertyChanged(nameof(BaseRepositoryClassName));
            }
        }
        private string _baseRepositoryClassName;

        [XmlIgnore]
        [JsonIgnore]
        public List<BaseCsDbRepositoryEntitySetting<T>> Entities
        {
            get
            {
                if (Project == null || string.IsNullOrEmpty(Project.SearchString))
                    return _entities;

                var result = _entities
                    .Where(e => e.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)
                        || e.Properties.Any(p => p.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)));
                return result.ToList();
            }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged(nameof(Entities));
            }
        }
        private List<BaseCsDbRepositoryEntitySetting<T>> _entities;


        /// <summary>
        /// Model class code engine is used to get information about the model classes, such as validator class, which can be used in the repository templates.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public CSharpModelClassCodeEngine ModelClassCodeEngine
        {
            get
            {
                if (_modelClassEngine == null && Project != null)
                {
                    _modelClassEngine = Project.GetCodeEngine(EngineTypes.CsModelClassCodeEngineId) as CSharpModelClassCodeEngine;
                }
                return _modelClassEngine;
            }
        }
        private CSharpModelClassCodeEngine _modelClassEngine;

        [XmlIgnore]
        [JsonIgnore]
        public string QueryParamNamespaceName
        {
            get { return ModelClassCodeEngine?.QueryParamNamespaceName ?? string.Empty; }
        }


        /// <summary>
        /// Storage code engine is used to get information about the storage, such as table and column names, which can be used in the repository templates.
        /// It is not serialized because it is retrieved from the project when needed.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public StorageCodeEngine StorageCodeEngine
        {
            get
            {
                if (_storageCodeEngine == null && Project != null)
                {
                    _storageCodeEngine = Project.GetCodeEngine(EngineTypes.SqliteScriptsId) as SqliteScriptsEngine;
                }
                return _storageCodeEngine;
            }
        }
        private StorageCodeEngine _storageCodeEngine;
    }
}
