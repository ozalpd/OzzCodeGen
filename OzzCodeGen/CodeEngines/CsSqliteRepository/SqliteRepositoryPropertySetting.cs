using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.Storage;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryPropertySetting : BaseCSharpPropertySetting
{
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
            RaisePropertyChanged("CheckIfAltered");
        }
    }
    private bool? _checkIfAltered;


    /// <summary>
    /// Generate a dedicated update method for this column.
    /// </summary>
    public bool SingleColumnUpdate
    {
        get { return _singleColumnUpdate; }
        set
        {
            _singleColumnUpdate = value;
            RaisePropertyChanged("SingleColumnUpdate");
        }
    }
    private bool _singleColumnUpdate;


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
