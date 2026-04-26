using OzzCodeGen.CodeEngines.CsDbRepository;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

public class SqliteRepositoryEntitySetting : BaseCsDbRepositoryEntitySetting<SqliteRepositoryPropertySetting>
{
    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }

    /// <summary>
    /// Gets or sets the name of the database table associated with the entity.
    /// </summary>
    /// <remarks>If not explicitly set, the table name is determined by the associated storage entity setting.
    /// If no storage entity setting is available, the pluralized form of the entity's name is used.</remarks>
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

    /// <summary>
    /// Gets or sets the SQL ORDER BY clause used to determine the sorting of query results.
    /// </summary>
    /// <remarks>When setting this property, the value is automatically normalized to ensure it begins with
    /// "ORDER BY". If the value does not start with "ORDER BY", it is prepended automatically. Leading and trailing
    /// whitespace is trimmed. If the value is null, it is treated as an empty string.</remarks>
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

    /// <summary>
    /// Time stamp property name for creation time of the entity
    /// </summary>
    public string CreatedAtName
    {
        get
        {
            if (string.IsNullOrEmpty(_createdAtProptyName)
                && GetInheritedIncludedProperties().Any(p => p.PropertyDefinition.Name == _defaultCreatedAtProptyName))
                _createdAtProptyName = _defaultCreatedAtProptyName;

            return _createdAtProptyName;
        }
        set
        {
            if (_createdAtProptyName == value) return;
            _createdAtProptyName = value != null ? value.Replace(" ", "") : string.Empty;
            RaisePropertyChanged("CreatedAtName");
        }
    }
    private string _createdAtProptyName;
    // Default time stamp property name for creation time of the entity
    // CreateDate name is used in TSqlScriptsEngine for same puprpose to generate table and set value in insert stored procedure,
    // for preventing complications we use CreatedAt as default value for CreatedAtProperty.
    private static readonly string _defaultCreatedAtProptyName = "CreatedAt";

    /// <summary>
    /// Property name for update time of the entity
    /// </summary>
    public string UpdatedAtName
    {
        get
        {
            if (string.IsNullOrEmpty(_updatedAtProptyName)
                && GetInheritedIncludedProperties().Any(p => p.PropertyDefinition.Name == _defaultModifiedAtProptyName))
                _updatedAtProptyName = _defaultModifiedAtProptyName;

            return _updatedAtProptyName;
        }
        set
        {
            if (_updatedAtProptyName == value) return;
            _updatedAtProptyName = value != null ? value.Replace(" ", "") : string.Empty;
            RaisePropertyChanged("UpdatedAtName");
        }
    }
    private string _updatedAtProptyName;
    // Default time stamp property name for update time of the entity
    // ModifyDate name is used in TSqlScriptsEngine for same puprpose to generate table and set value in update stored procedure,
    // for preventing complications we use UpdatedAt as default value for UpdatedAtName.
    private static readonly string _defaultModifiedAtProptyName = "UpdatedAt";


    public override AbstractEntitySetting<SqliteRepositoryPropertySetting> GetBaseEntitySetting()
    {
        if (string.IsNullOrEmpty(EntityDefinition?.BaseTypeName))
            return null;

        var codeEngine = (CSharpSqliteRepositoryEngine)CodeEngine;
        return codeEngine.Entities
                         .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
    }

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
}
