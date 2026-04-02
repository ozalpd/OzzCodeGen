using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryEntitySetting : BaseCSharpEntitySetting<SqliteRepositoryPropertySetting>
{
    [XmlIgnore]
    [JsonIgnore]
    public CSharpSqliteRepositoryEngine CodeEngine { get; set; }

    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }

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

    /// <summary>
    /// DDL file name for initializing the table for entity. If not set, it defaults to "{EntityName}.sql".
    /// </summary>
    public string FileNameDDL
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_fileNameDDL))
                _fileNameDDL = $"{Name}.sql";

            return _fileNameDDL;
        }
        set
        {
            if (_fileNameDDL == value)
                return;

            _fileNameDDL = value;
            RaisePropertyChanged(nameof(FileNameDDL));
        }
    }
    private string _fileNameDDL;

    /// <summary>
    /// Gets or sets a file name to data seed after initializing the table for entity.
    /// </summary>
    public string FileNameSeed
    {
        get
        {
            return _fileNameSeed;
        }
        set
        {
            if (_fileNameSeed == value)
                return;

            _fileNameSeed = value;
            RaisePropertyChanged(nameof(FileNameDDL));
        }
    }
    private string _fileNameSeed;



    private string GetDefaultOrderByClause()
    {
        var displayOrderProperty = GetInheritedIncludedProperties()
                                                .FirstOrDefault(p => p.Name.Equals("DisplayOrder", StringComparison.InvariantCultureIgnoreCase));
        var displayMemberName = EntityDefinition.DisplayMember;
        var sb = new StringBuilder();
        sb.Append("ORDER BY ");

        bool hasOrdeProperty = false;
        if (displayOrderProperty != null)
        {
            sb.Append(displayOrderProperty.ColumnName);
            hasOrdeProperty = true;
        }

        if (!string.IsNullOrEmpty(displayMemberName))
        {
            sb.Append(hasOrdeProperty ? ", " : string.Empty);
            sb.Append(displayMemberName);
            hasOrdeProperty = true;
        }

        if (hasOrdeProperty)
            return sb.ToString();

        var pk = GetPrimaryKey();
        if (pk == null)
            return string.Empty;
        return $"ORDER BY [{pk.ColumnName}]";
    }


    public string OrderByClause
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_orderByClause))
                _orderByClause = GetDefaultOrderByClause();

            return _orderByClause;
        }
        set
        {
            string newValue = value?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(_orderByClause) && _orderByClause.Equals(newValue, StringComparison.InvariantCultureIgnoreCase))
                return;

            if (newValue.StartsWith("ORDER BY "))
            {
                _orderByClause = newValue;
            }
            else
            {
                _orderByClause = $"ORDER BY {newValue.Replace("ORDER BY", "")}";
            }

            RaisePropertyChanged(nameof(OrderByClause));
        }
    }
    private string _orderByClause;

    public override AbstractEntitySetting<SqliteRepositoryPropertySetting> GetBaseEntitySetting()
    {
        if (string.IsNullOrEmpty(EntityDefinition?.BaseTypeName))
            return null;

        var codeEngine = (CSharpSqliteRepositoryEngine)CodeEngine;
        return codeEngine.Entities
                         .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
    }

    public new IEnumerable<SqliteRepositoryPropertySetting> GetInheritedProperties()
    {
        if (_inhertitedProperties == null)
            _inhertitedProperties = base.GetInheritedProperties()
                                        .Cast<SqliteRepositoryPropertySetting>()
                                        .OrderBy(c => c.PropertyDefinition.DisplayOrder)
                                        .ToList();

        return _inhertitedProperties;
    }
    IEnumerable<SqliteRepositoryPropertySetting> _inhertitedProperties;

    public IEnumerable<SqliteRepositoryPropertySetting> GetRepositoryColumns()
    {
        if (_repositoryColumns == null)
            _repositoryColumns = GetInheritedProperties()
                                    .Where(p => p.IsRepositoryColumn)
                                    .OrderBy(c => c.PropertyDefinition.DisplayOrder)
                                    .ToList();

        return _repositoryColumns;
    }
    IEnumerable<SqliteRepositoryPropertySetting> _repositoryColumns;

    public IEnumerable<SqliteRepositoryPropertySetting> GetForeignKeyColumns()
    {
        if (_fkeyColumns == null)
            _fkeyColumns = GetRepositoryColumns()
                                .Where(p => p.PropertyDefinition is SimpleProperty
                                         && ((SimpleProperty)p.PropertyDefinition).IsForeignKey)
                                .OrderBy(c => c.PropertyDefinition.DisplayOrder)
                                .ToList();

        return _fkeyColumns;
    }
    IEnumerable<SqliteRepositoryPropertySetting> _fkeyColumns;
}
