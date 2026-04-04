using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.Storage;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryPropertySetting : BaseCSharpPropertySetting
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



    public SqliteRepositoryPropertySetting? GetForeignKeyProperty()
    {
        string fkeyName = GetForeignKeyName();
        if (string.IsNullOrEmpty(fkeyName))
            return null;

        var entitySetting = EntitySetting as SqliteRepositoryEntitySetting;
        return entitySetting?.Properties.FirstOrDefault(p => p.Name.Equals(fkeyName));
    }

    [XmlIgnore]
    [JsonIgnore]
    public string ClrTypeName => GetTypeName();

    [XmlIgnore]
    [JsonIgnore]
    public bool IsRepositoryColumn => IsSimpleOrString;

    [XmlIgnore]
    [JsonIgnore]
    public bool IsUniqueIndexed => StorageColumnSetting?.Indexed == true && StorageColumnSetting?.Unique == true;

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

    protected override BaseCodeEngine GetCodeEngine()
    {
        SqliteRepositoryEntitySetting setting = (SqliteRepositoryEntitySetting)EntitySetting;
        return setting?.CodeEngine;
    }
}
