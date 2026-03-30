using OzzCodeGen.CodeEngines.Storage;
using OzzUtils;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryEntitySetting : CsModelClass.BaseModelClassEntitySetting<SqliteRepositoryPropertySetting>
{
    public StorageEntitySetting StorageEntitySetting
    {
        get
        {
            CSharpSqliteRepositoryEngine codeEngine = CodeEngine as CSharpSqliteRepositoryEngine;
            if (_storageEntitySetting == null && codeEngine?.StorageCodeEngine != null)
            {
                _storageEntitySetting = codeEngine.StorageCodeEngine.Entities.FirstOrDefault(e => e.Name == Name);
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
