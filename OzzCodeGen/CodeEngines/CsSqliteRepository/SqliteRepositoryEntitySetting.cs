using System.Linq;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryEntitySetting : CsModelClass.BaseModelClassEntitySetting<SqliteRepositoryPropertySetting>
{
    public string TableName
    {
        get
        {
            if (string.IsNullOrEmpty(_tableName))
                _tableName = Name;
            return _tableName;
        }
        set
        {
            if (_tableName == value) return;
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
