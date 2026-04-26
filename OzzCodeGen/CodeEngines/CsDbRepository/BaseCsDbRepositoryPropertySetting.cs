using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.CsSqliteRepository;
using OzzCodeGen.CodeEngines.Storage;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsDbRepository
{
    public abstract class BaseCsDbRepositoryPropertySetting : BaseCSharpPropertySetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether the property should be automatically loaded when its parent entity is
        /// loaded.
        /// </summary>
        /// <remarks>This property is only applicable to complex or collection properties. Setting this value to
        /// <see langword="true"/> for other property types has no effect.</remarks>
        public bool AutoLoad
        {
            get { return _autoLoad; }
            set
            {
                bool newValue = value && (IsLoadingFromFile || IsComplex || IsCollection);
                if (newValue != _autoLoad)
                {
                    _autoLoad = newValue;
                    RaisePropertyChanged(nameof(AutoLoad));
                }
            }
        }
        private bool _autoLoad = false;

        /// <summary>
        /// Check if this column altered in update stored procedure by where clause
        /// </summary>
        public bool CheckIfAltered
        {
            get
            {
                if (_checkIfAltered == null)
                {
                    _checkIfAltered = StorageColumnSetting?.CheckIfAltered == true;
                }

                return _checkIfAltered.Value;
            }
            set
            {
                _checkIfAltered = value;
                RaisePropertyChanged(nameof(CheckIfAltered));
            }
        }
        private bool? _checkIfAltered;

        /// <summary>
        /// Gets or sets a value that is table column name for this property. If not set, it defaults to the same as property name.
        /// </summary>
        public string ColumnName
        {
            get
            {
                if (string.IsNullOrEmpty(_columnName))
                    _columnName = Name;
                return _columnName;
            }
            set
            {
                if (_columnName == value) return;
                _columnName = value;
                RaisePropertyChanged(nameof(ColumnName));
            }
        }
        private string _columnName;

        /// <summary>
        /// Generate a dedicated update method for this column.
        /// </summary>
        public bool SingleColumnUpdate
        {
            get { return _singleColumnUpdate; }
            set
            {
                _singleColumnUpdate = value;
                RaisePropertyChanged(nameof(SingleColumnUpdate));
            }
        }
        private bool _singleColumnUpdate;



        /// <summary>
        /// Gets the fully qualified name of the corresponding Common Language Runtime (CLR) type for this model property.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string ClrTypeName => GetTypeName();

        /// <summary>
        /// Gets a value indicating whether the property is included as a column in the generated repository.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsRepositoryColumn => IsSimpleOrString;

        /// <summary>
        /// Gets a value indicating whether the associated storage column is treated as an integer type in the repository.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsRepositoryIntegerColumn => StorageColumnSetting?.IsInteger == true;

        /// <summary>
        /// Gets a value indicating whether the associated storage column is treated as a text column in the repository.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsRepositoryTextColumn => StorageColumnSetting?.IsText == true;

        /// <summary>
        /// Gets a value indicating whether the column is configured as a unique index.
        /// </summary>
        /// <remarks>A column is considered uniquely indexed if both the index and unique constraints are set in
        /// the associated storage column settings. This property is typically used to determine whether the column enforces
        /// uniqueness at the database level.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsUniqueIndexed => StorageColumnSetting?.Indexed == true && StorageColumnSetting?.Unique == true;

        /// <summary>
        /// Gets the storage-specific column settings associated with this property, if available.
        /// </summary>
        /// <remarks>This property returns the corresponding storage column settings when the property is a simple
        /// or string type and a matching storage entity property exists. The value is determined based on the current
        /// entity and property context and may be null if no storage mapping is found.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public StorageColumnSetting? StorageColumnSetting
        {
            get
            {
                if (_storageColumnSetting == null && IsSimpleOrString)
                {
                    var entitySetting = (SqliteRepositoryEntitySetting)EntitySetting;
                    var storageEntity = entitySetting?.StorageEntitySetting;
                    if (storageEntity != null)
                    {
                        _storageColumnSetting = storageEntity.Properties.FirstOrDefault(c => c.Name.Equals(Name));
                    }
                }
                return _storageColumnSetting;
            }
        }
        private StorageColumnSetting? _storageColumnSetting;

        /// <summary>
        /// Gets the property setting that represents the foreign key for this repository property, if one exists.
        /// </summary>
        /// <remarks>This method searches for a property within the associated entity settings whose name matches
        /// the foreign key name. If no matching property is found or the foreign key name is not defined, the method
        /// returns <see langword="null"/>.</remarks>
        /// <returns>A <see cref="SqliteRepositoryPropertySetting"/> representing the foreign key property if found; otherwise, <see
        /// langword="null"/>.</returns>
        public BaseCsDbRepositoryPropertySetting? GetForeignKeyProperty()
        {
            string fkeyName = GetForeignKeyName();
            if (string.IsNullOrEmpty(fkeyName))
                return null;

            var entitySetting = EntitySetting as SqliteRepositoryEntitySetting;
            return entitySetting?.Properties.FirstOrDefault(p => p.Name.Equals(fkeyName));
        }

    }
}
