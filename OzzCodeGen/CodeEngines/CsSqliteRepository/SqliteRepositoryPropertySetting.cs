using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;
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
