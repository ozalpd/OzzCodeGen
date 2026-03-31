using OzzCodeGen.CodeEngines.Storage;
using OzzUtils;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryEntitySetting : AbstractEntitySetting<SqliteRepositoryPropertySetting>
{
    [XmlIgnore]
    [JsonIgnore]
    public CSharpSqliteRepositoryEngine CodeEngine { get; set; }

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

    public string TableName
    {
        get
        {
            if (string.IsNullOrEmpty(_tableName) && StorageEntitySetting != null)
            {
                _tableName = StorageEntitySetting.TableName;
            }
            else if (string.IsNullOrEmpty(_tableName))
            {
                _tableName = Name.Pluralize();
            }

            return _tableName;
        }
        set
        {
            if (_tableName == value)
                return;

            _tableName = value;
            RaisePropertyChanged(nameof(TableName));
        }
    }
    private string _tableName;

    public override AbstractEntitySetting<SqliteRepositoryPropertySetting> GetBaseEntitySetting()
    {
        if (string.IsNullOrEmpty(EntityDefinition?.BaseTypeName))
            return null;

        var codeEngine = (CSharpSqliteRepositoryEngine)CodeEngine;
        return codeEngine.Entities.FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
    }
}
