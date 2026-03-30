using OzzCodeGen.Definitions;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryPropertySetting : CsModelClass.BaseModelClassPropertySetting
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
    public bool IsRepositoryColumn => PropertyDefinition is SimpleProperty || PropertyDefinition is StringProperty;
}
