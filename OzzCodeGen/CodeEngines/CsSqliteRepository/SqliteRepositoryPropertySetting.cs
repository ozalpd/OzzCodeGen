using OzzCodeGen.CodeEngines.CSharp;
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

    protected override BaseCodeEngine GetCodeEngine()
    {
        SqliteRepositoryEntitySetting setting = (SqliteRepositoryEntitySetting)EntitySetting;
        return setting?.CodeEngine;
    }
}
