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
    }
}
