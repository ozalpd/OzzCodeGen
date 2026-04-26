using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsDbRepository
{
    public abstract class BaseCsDbRepositoryEntitySetting<T> : BaseCSharpEntitySetting<T> where T : BaseCsDbRepositoryPropertySetting
    {
        [XmlIgnore]
        [JsonIgnore]
        public BaseCsDbRepositoryEngine<T> CodeEngine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paged query methods should be generated.
        /// </summary>
        /// <remarks>When enabled, the code generator will include methods that support retrieving data in pages,
        /// which is useful for large datasets or implementing pagination in user interfaces.</remarks>
        public bool GenerateGetPaged
        {
            set
            {
                _generateGetPaged = value;
                RaisePropertyChanged(nameof(GenerateGetPaged));
            }
            get
            {
                return _generateGetPaged;
            }
        }
        private bool _generateGetPaged;


        public IEnumerable<T> GetAutoLoadProperties()
        {
            if (_autoLoadProperties == null)
                _autoLoadProperties = GetInheritedIncludedProperties()
                                            .Where(p => p.AutoLoad)
                                            .OrderBy(p => p.PropertyDefinition.DisplayOrder)
                                            .ToList();
            return _autoLoadProperties;
        }
        IEnumerable<T> _autoLoadProperties;

        public string GetRepositoryName()
        {
            return GetRepositoryName(Name);
        }

        public string GetRepositoryName(string entityName)
        {
            string fixedName = FixEntityTypeName(entityName);

            return $"{fixedName}Repository";
        }

        private static string FixEntityTypeName(string entityName)
        {
            string fixedName = entityName.EndsWith("Dto") ? entityName[..^3] : entityName;
            if (fixedName.StartsWith("ICollection<"))
                fixedName = fixedName.Substring(12, fixedName.Length - 13);
            return fixedName;
        }

        public bool HasThisKindOfRepository(string entityName)
        {
            string fixedName = FixEntityTypeName(entityName);

            var entity = CodeEngine.Entities.FirstOrDefault(c => c.Name.Equals(fixedName, StringComparison.InvariantCultureIgnoreCase));
            if (entity == null)
                return false;

            var autoLoadProperties = entity.GetAutoLoadProperties();
            return autoLoadProperties.Any(p => p.PropertyDefinition.TypeName.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
        }


        /// <summary>
        /// Gets the model class entity settings associated with this entity, if available.
        /// </summary>
        /// <remarks>This property returns the corresponding model class entity settings from the model class code engine,
        /// based on the entity's name. If the settings are not available or the code engine is not initialized, the property
        /// returns null.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public ModelClassEntitySetting ModelClassEntitySetting
        {
            get
            {
                if (_modelClassEntity == null && CodeEngine?.ModelClassCodeEngine != null)
                {
                    _modelClassEntity = CodeEngine.ModelClassCodeEngine.Entities.FirstOrDefault(e => e.Name == Name);
                }

                return _modelClassEntity;
            }
        }
        private ModelClassEntitySetting _modelClassEntity;

        /// <summary>
        /// Gets a value indicating whether query parameter classes should be generated for the current entity based on its
        /// settings and searchable properties.
        /// </summary>
        /// <remarks>Query parameter generation is enabled when the entity's model class settings specify that
        /// query parameters should be generated and there is at least one searchable property, either as a simple search
        /// field or as a range field. This property is typically used by code generation engines to determine whether to
        /// emit query parameter classes for filtering or searching scenarios.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public bool HasSearchableProperties
        {
            get
            {
                return ModelClassEntitySetting?.GenerateQueryParam == true
                    && (SearchableNonRangeProperties.Any() || SearchableRangeProperties.Any());
            }
        }

        /// <summary>
        /// Gets an enumerable collection of model properties that are searchable and not used for range-based filtering.
        /// </summary>
        /// <remarks>This property returns only those properties that are considered searchable but do not
        /// represent minimum or maximum values for range queries. If the associated model class entity setting is not
        /// available, the collection will be empty.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<ModelPropertySetting> SearchableNonRangeProperties
        {
            get
            {
                if (ModelClassEntitySetting != null)
                    return ModelClassEntitySetting.SearchableNonRangeProperties;
                else
                    return Enumerable.Empty<ModelPropertySetting>();
            }
        }

        /// <summary>
        /// Gets the collection of model properties that support range-based search operations.
        /// </summary>
        /// <remarks>This property returns only those properties that are configured to allow searching by a
        /// range, such as minimum and maximum values. If no such properties are defined, the collection is empty.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<ModelPropertySetting> SearchableRangeProperties
        {
            get
            {
                if (ModelClassEntitySetting != null)
                    return ModelClassEntitySetting.SearchableRangeProperties;
                else
                    return Enumerable.Empty<ModelPropertySetting>();
            }
        }

        /// <summary>
        /// Gets the storage-specific settings associated with this entity, if available.
        /// </summary>
        /// <remarks>This property returns the corresponding storage entity settings from the storage code engine,
        /// based on the entity's name. If no matching settings are found or the storage code engine is not available, the
        /// property returns null.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public StorageEntitySetting StorageEntitySetting
        {
            get
            {
                if (_storageEntitySetting == null && CodeEngine?.StorageCodeEngine != null)
                {
                    _storageEntitySetting = CodeEngine.StorageCodeEngine.Entities.FirstOrDefault(e => e.Name == Name);
                }

                return _storageEntitySetting;
            }
        }
        private StorageEntitySetting _storageEntitySetting;

    }
}
