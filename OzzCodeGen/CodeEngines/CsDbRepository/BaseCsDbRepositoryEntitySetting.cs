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
            return GetInheritedIncludedProperties()
                            .Where(p => p.AutoLoad)
                            .OrderBy(p => p.PropertyDefinition.DisplayOrder)
                            .ToList();
        }

        /// <summary>
        /// Returns a collection of properties that reference the current entity within the model.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="ModelPropertySetting"/> objects that reference this entity. If there
        /// are no referencing properties, returns an empty collection.</returns>
        public IEnumerable<ModelPropertySetting> GetReferencingProperties()
        {
            if (ModelClassEntitySetting != null)
                return ModelClassEntitySetting.GetReferencingProperties();

            return Enumerable.Empty<ModelPropertySetting>();
        }

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
                if (_storageEntitySetting == null && CodeEngine?.SqliteStorageCodeEngine != null)
                {
                    _storageEntitySetting = CodeEngine.SqliteStorageCodeEngine.Entities.FirstOrDefault(e => e.Name == Name);
                }

                return _storageEntitySetting;
            }
        }
        private StorageEntitySetting _storageEntitySetting;
    }
}
