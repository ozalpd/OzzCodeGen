using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.CsModelClass;
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

    /// <summary>
    /// Gets the model class entity settings associated with this entity, if available.
    /// </summary>
    /// <remarks>This property returns the corresponding model class entity settings from the model class code engine,
    /// based on the entity's name. If the settings are not available or the code engine is not initialized, the property
    /// returns null.</remarks>
    [XmlIgnore]
    [JsonIgnore]
    public ModelClassEntitySetting ModelClassEntitySetting
    {
        get
        {
            if (_modelClassEntity == null && CodeEngine?.ModelClassCodeEngine != null)
            {
                _modelClassEntity = CodeEngine.ModelClassCodeEngine.Entities.FirstOrDefault(e => e.Name == Name);
            }

            return _modelClassEntity;
        }
    }
    private ModelClassEntitySetting _modelClassEntity;

    /// <summary>
    /// Gets or sets a value indicating whether paged query methods should be generated.
    /// </summary>
    /// <remarks>When enabled, the code generator will include methods that support retrieving data in pages,
    /// which is useful for large datasets or implementing pagination in user interfaces.</remarks>
    public bool GenerateGetPaged
    {
        set
        {
            _generateGetPaged = value;
            RaisePropertyChanged(nameof(GenerateGetPaged));
        }
        get
        {
            return _generateGetPaged;
        }
    }
    private bool _generateGetPaged;

    /// <summary>
    /// Gets a value indicating whether query parameter classes should be generated for the current entity based on its
    /// settings and searchable properties.
    /// </summary>
    /// <remarks>Query parameter generation is enabled when the entity's model class settings specify that
    /// query parameters should be generated and there is at least one searchable property, either as a simple search
    /// field or as a range field. This property is typically used by code generation engines to determine whether to
    /// emit query parameter classes for filtering or searching scenarios.</remarks>
    [XmlIgnore]
    [JsonIgnore]
    public bool HasSearchableProperties
    {
        get
        {
            return ModelClassEntitySetting?.GenerateQueryParam == true
                && (SearchableNonRangeProperties.Any() || SearchableRangeProperties.Any());
        }
    }

    /// <summary>
    /// Gets an enumerable collection of model properties that are searchable and not used for range-based filtering.
    /// </summary>
    /// <remarks>This property returns only those properties that are considered searchable but do not
    /// represent minimum or maximum values for range queries. If the associated model class entity setting is not
    /// available, the collection will be empty.</remarks>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<ModelPropertySetting> SearchableNonRangeProperties
    {
        get
        {
            if (ModelClassEntitySetting != null)
                return ModelClassEntitySetting.SearchableNonRangeProperties;
            else
                return Enumerable.Empty<ModelPropertySetting>();
        }
    }

    /// <summary>
    /// Gets the collection of model properties that support range-based search operations.
    /// </summary>
    /// <remarks>This property returns only those properties that are configured to allow searching by a
    /// range, such as minimum and maximum values. If no such properties are defined, the collection is empty.</remarks>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<ModelPropertySetting> SearchableRangeProperties
    {
        get
        {
            if (ModelClassEntitySetting != null)
                return ModelClassEntitySetting.SearchableRangeProperties;
            else
                return Enumerable.Empty<ModelPropertySetting>();
        }
    }

    /// <summary>
    /// Gets the storage-specific settings associated with this entity, if available.
    /// </summary>
    /// <remarks>This property returns the corresponding storage entity settings from the storage code engine,
    /// based on the entity's name. If no matching settings are found or the storage code engine is not available, the
    /// property returns null.</remarks>
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


    public IEnumerable<SqliteRepositoryPropertySetting> GetAutoLoadProperties()
    {
        if (_autoLoadProperties == null)
            _autoLoadProperties = GetInheritedIncludedProperties()
                                        .Where(p => p.AutoLoad)
                                        .OrderBy(p => p.PropertyDefinition.DisplayOrder)
                                        .ToList();
        return _autoLoadProperties;
    }
    IEnumerable<SqliteRepositoryPropertySetting> _autoLoadProperties;

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

    public string GetRepositoryName()
    {
        return GetRepositoryName(Name);
    }

    public string GetRepositoryName(string entityName)
    {
        string fixedName = FixEntityTypeName(entityName);

        return $"{fixedName}Repository";
    }

    private static string FixEntityTypeName(string entityName)
    {
        string fixedName = entityName.EndsWith("Dto") ? entityName[..^3] : entityName;
        if (fixedName.StartsWith("ICollection<"))
            fixedName = fixedName.Substring(12, fixedName.Length - 13);
        return fixedName;
    }

    public bool HasThisKindOfRepository(string entityName)
    {
        string fixedName = FixEntityTypeName(entityName);

        var entity = CodeEngine.Entities.FirstOrDefault(c => c.Name.Equals(fixedName, StringComparison.InvariantCultureIgnoreCase));
        if (entity == null)
            return false;

        var autoLoadProperties = entity.GetAutoLoadProperties();
        return autoLoadProperties.Any(p => p.PropertyDefinition.TypeName.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
