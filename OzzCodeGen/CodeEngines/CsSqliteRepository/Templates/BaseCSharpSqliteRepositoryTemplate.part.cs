using OzzCodeGen.CodeEngines.CsDbRepository;
using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.Definitions;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;

public abstract partial class BaseCSharpSqliteRepositoryTemplate
{
    protected BaseCSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting = null) : base(codeEngine, entitySetting) { }

    public override CSharpSqliteRepositoryEngine CodeEngine => (CSharpSqliteRepositoryEngine)base.CodeEngine;
    public override SqliteRepositoryEntitySetting EntitySetting => (SqliteRepositoryEntitySetting)base.EntitySetting;


    protected IEnumerable<SqliteRepositoryPropertySetting> GetRepositoryProperties()
    {
        if (EntitySetting == null)
            return Enumerable.Empty<SqliteRepositoryPropertySetting>();

        return EntitySetting
            .GetInheritedIncludedProperties()
            .OfType<SqliteRepositoryPropertySetting>()
            .Where(p => p.IsRepositoryColumn)
            .OrderBy(p => p.PropertyDefinition.DisplayOrder);
    }

    protected SqliteRepositoryPropertySetting GetCreatedAtColumn()
    {
        string createdAtColName = EntitySetting.CreatedAtName;
        return GetRepositoryProperties()
                    .FirstOrDefault(p => p.ColumnName.Equals(createdAtColName, StringComparison.InvariantCultureIgnoreCase));
    }

    protected SqliteRepositoryEntitySetting? GetEntityByName(string entityName)
    {
        var entity = CodeEngine.Entities
                               .Where(e => e is SqliteRepositoryEntitySetting && e.Name == entityName)
                               .OfType<SqliteRepositoryEntitySetting>()
                               .FirstOrDefault();
        return entity;
    }

    protected string GetEntityPKeyName(string entityName)
    {
        var entity = CodeEngine.Entities.FirstOrDefault(e => e.Name == entityName);
        if (entity == null)
            return string.Empty;

        var keyProperty = entity.GetInheritedSimpleProperties()
                                .FirstOrDefault(p => p.IsKey);
        return keyProperty?.Name ?? string.Empty;
    }

    protected override ModelClassEntitySetting GetModelClassEntitySetting()
    {
        return EntitySetting.ModelClassEntitySetting;
    }
    
    protected override BaseCsDbRepositoryPropertySetting GetPrimaryKey()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsKey);
    }

    protected SqliteRepositoryPropertySetting GetUpdatedAtColumn()
    {
        string updatedAtColName = EntitySetting.UpdatedAtName;
        return GetRepositoryProperties()
                    .FirstOrDefault(p => p.ColumnName.Equals(updatedAtColName, StringComparison.InvariantCultureIgnoreCase));
    }

    protected override BaseCsDbRepositoryPropertySetting GetUniqueIndexed()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsUniqueIndexed && !p.IsKey);
    }

    protected override IEnumerable<BaseCsDbRepositoryPropertySetting> GetAutoLoadProperties()
    {
        return EntitySetting.GetAutoLoadProperties();
    }

    protected override string GetEntityTypeName(bool isNullable)
    {
        return EntitySetting.GetTypeName(isNullable);
    }

    protected override IEnumerable<BaseCsDbRepositoryPropertySetting> GetForeignKeyProperties()
    {
        return GetRepositoryProperties().Where(p => p.IsForeignKey);
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetInsertProperties()
    {
        return GetRepositoryProperties().Where(p => !ShouldSkipInsert(p));
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetNonUpdateProperties()
    {
        return GetRepositoryProperties().Where(p => !p.IsKey && IsReadOnlyColumn(p));
    }

    protected override IEnumerable<ModelPropertySetting> GetSearchableNonRangeProperties()
    {
        return EntitySetting.SearchableNonRangeProperties;
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetSingleUpdateProperties()
    {
        return GetRepositoryProperties().Where(p => p.SingleColumnUpdate);
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetUpdateProperties()
    {
        string createdAtColName = EntitySetting.CreatedAtName;
        return GetRepositoryProperties()
                    .Where(p => !p.IsKey
                             && !IsReadOnlyColumn(p)
                             && !p.ColumnName.Equals(createdAtColName, StringComparison.InvariantCultureIgnoreCase));
    }

    protected string GetRepositoryInitialization(string entityName)
    {
        if (!EntitySetting.HasThisKindOfRepository(entityName))
            return "databasePath";

        return $"databasePath, {GetRepositoryName(EntitySetting.Name).ToCamelCase()}: this";
    }

    protected string GetRepositoryName(string entityName)
    {
        return EntitySetting.GetRepositoryName(entityName);
    }

    protected override bool HasIsActiveProperty()
    {
        return EntitySetting.HasIsActiveProperty();
    }

    protected virtual bool ShouldSkipInsert(SqliteRepositoryPropertySetting property)
    {
        if (!property.IsKey)
            return false;

        if (property.PropertyDefinition is not SimpleProperty simpleProperty)
            return false;

        return simpleProperty.IsStoreGenerated || simpleProperty.IsTypeIntegerNumeric();
    }

    protected virtual bool IsReadOnlyColumn(SqliteRepositoryPropertySetting property)
    {
        if (property.PropertyDefinition is not SimpleProperty simpleProperty)
            return false;

        return simpleProperty.IsImmutable || simpleProperty.IsStoreGenerated || property.PropertyDefinition.IsServerComputed;
    }

    protected string GetNonNullableTypeName(SqliteRepositoryPropertySetting property)
    {
        if (property.PropertyDefinition is StringProperty)
            return "string";

        if (property.PropertyDefinition is SimpleProperty simpleProperty && !string.IsNullOrEmpty(simpleProperty.EnumTypeName))
            return simpleProperty.EnumTypeName;

        var typeName = property.GetTypeName();
        return typeName.EndsWith("?") ? typeName[..^1] : typeName;
    }

    protected string GetColumnList(IEnumerable<SqliteRepositoryPropertySetting> properties)
    {
        return string.Join(", ", properties.Select(p => $"[{p.ColumnName}]"));
    }

    protected string GetParameterList(IEnumerable<SqliteRepositoryPropertySetting> properties)
    {
        return string.Join(", ", properties.Select(p => $"@{p.ColumnName.ToCamelCase()}"));
    }

    protected string GetMappingExpression(SqliteRepositoryPropertySetting property, string ordinalNr, bool needsComma, string readerName = "reader")
    {
        var sp = property.PropertyDefinition as SimpleProperty;
        bool isEnum = sp != null && !string.IsNullOrWhiteSpace(sp.EnumTypeName);
        bool isNullable = property.IsNullable;

        var sb = new StringBuilder();
        if (isNullable)
        {
            sb.Append(readerName);
            sb.Append(".IsDBNull(");
            sb.Append(ordinalNr);
            sb.Append(") ? null\r\n");
            AppendAlignmentSpaces(property, sb);
            sb.Append(": ");
        }

        if (isEnum)
        {
            sb.Append("(");
            sb.Append(sp.EnumTypeName);
            sb.Append(")");
        }

        bool didCatch = false;
        var storageCol = property.StorageColumnSetting;
        if (property.IsDecimal && storageCol != null)
        {
            if (storageCol.IsText)
            {
                sb.Append(readerName);
                sb.Append(".GetDecimalFromText(");
                sb.Append(ordinalNr);
                sb.Append(')');
                didCatch = true;
            }
            else if (storageCol.IsInteger && property.DecimalToIntegerScale > 0)
            {
                sb.Append(readerName);
                sb.Append(".GetDecimalFromInteger(");
                sb.Append(ordinalNr);
                sb.Append(",\r\n");
                AppendAlignmentSpaces(property, sb);
                sb.Append(' ', 16);
                sb.Append("DecimalToIntegerScale.");
                sb.Append(property.Name);
                sb.Append(')');
                didCatch = true;
            }

            if (didCatch)
            {
                if (!isNullable)
                    sb.Append(" ?? 0m");

                if (needsComma)
                    sb.Append(',');

                return sb.ToString();
            }
        }

        AppendReaderGetByType(property, readerName, isEnum, sb);
        sb.Append(ordinalNr);
        sb.Append(')');

        var typeName = GetNonNullableTypeName(property).ToLowerInvariant();
        if (typeName.Equals("bool"))
        {
            sb.Append(" == 1");
        }
        else if (typeName.Equals("date") || typeName.Equals("datetime"))
        {
            sb.Append(')');
            if (!isNullable)
            {
                sb.Append(" ?? DateTime.MinValue");
            }
        }
        if (needsComma)
            sb.Append(',');

        return sb.ToString();
    }

    private static void AppendAlignmentSpaces(SqliteRepositoryPropertySetting property, StringBuilder sb)
    {
        //append number of spaces for alignment with non-nullable mappings
        sb.Append(' ', property.Name.Length);
        sb.Append(' ', 17);
    }

    private bool AppendReaderGetByType(SqliteRepositoryPropertySetting property, string readerName, bool isEnum, StringBuilder sb)
    {
        var typeName = GetNonNullableTypeName(property).ToLowerInvariant();
        if (typeName.Equals("date") || typeName.Equals("datetime"))
        {
            sb.Append("ToLocalDateTime(");
        }

        sb.Append(readerName);
        bool didCatch = false;
        if (typeName.Equals("bool") || typeName.Equals("long") || typeName.Equals("ulong"))
        {
            sb.Append(".GetInt64(");
            didCatch = true;
        }
        else if (typeName.Equals("int") || typeName.Equals("uint") || isEnum)
        {
            sb.Append(".GetInt32(");
            didCatch = true;
        }
        else if (typeName.Equals("short") || typeName.Equals("ushort"))
        {
            sb.Append(".GetInt32(");
            didCatch = true;
        }
        else if (typeName.Equals("byte") || typeName.Equals("sbyte"))
        {
            sb.Append(".GetByte(");
            didCatch = true;
        }
        else if (typeName.Equals("float"))
        {
            sb.Append(".GetFloat(");
            didCatch = true;
        }
        else if (typeName.Equals("decimal"))
        {
            sb.Append(".GetDecimal(");
            didCatch = true;
        }
        else if (typeName.Equals("double"))
        {
            sb.Append(".GetDouble(");
            didCatch = true;
        }

        if (typeName.Equals("string") || !didCatch)
        {
            sb.Append(".GetString(");
            didCatch = true;
        }

        return didCatch;
    }

    protected static string GetConvertMethodName(string typeName)
    {
        return typeName.TrimEnd('?') switch
        {
            "int" => "ToInt32",
            "uint" => "ToUInt32",
            "long" => "ToInt64",
            "ulong" => "ToUInt64",
            "short" => "ToInt16",
            "ushort" => "ToUInt16",
            "byte" => "ToByte",
            "sbyte" => "ToSByte",
            "decimal" => "ToDecimal",
            "double" => "ToDouble",
            "float" => "ToSingle",
            _ => "ToString"
        };
    }

    public void WriteColumnsAndParameters(IEnumerable<BaseCsDbRepositoryPropertySetting> columns, BaseCsDbRepositoryPropertySetting? pkey = null, BaseCsDbRepositoryPropertySetting? createdAtCol = null, BaseCsDbRepositoryPropertySetting? updatedAtCol = null)
    {
        var model = new WriteColumnsModel
        {
            Columns = columns.ToList(),
            PKey = pkey,
            PKeyValue = pkey != null ? $"{EntitySetting.Name.ToCamelCase()}.{pkey.Name}" : string.Empty,
            CreatedAtCol = createdAtCol,
            UpdatedAtCol = updatedAtCol
        };
        WriteColumnsAndParameters(model);
    }

    public void WriteColumnsAndParameters(WriteColumnsModel model)
    {
        PushIndent("    ");
        PushIndent("    ");

        // For now, we pass DateTime.Now directly then it is converted UTC. This can be made configurable in the future if needed.
        bool useNowUTC = false;
        bool hasTimeStamp = useNowUTC
                         && model.Columns
                                 .Any(c => c.Name == model.CreatedAtCol?.Name
                                        || c.Name == model.UpdatedAtCol?.Name);
        if (hasTimeStamp)
        {
            PushIndent("    ");
            WriteLine("var nowUtc = DateTime.UtcNow;");
            PopIndent();
        }
        string val = string.Empty;
        if (model.PKey != null)
            WriteSingleColumnAndParameter(model.PKey as SqliteRepositoryPropertySetting, model.PKeyValue);

        var columns = model.Columns.ToList();
        var valueList = model.ValueList;

        for (int i = 0; i < columns.Count; i++)
        {
            var col = columns[i];
            val = valueList.Count > i ? valueList[i] : $"{EntitySetting.Name.ToCamelCase()}.{col.Name}";
            bool isTimeStamp = col.Name == model.CreatedAtCol?.Name || col.Name == model.UpdatedAtCol?.Name;
            WriteSingleColumnAndParameter(col as SqliteRepositoryPropertySetting, val, isTimeStamp);
        }
        PopIndent();
        PopIndent();
    }

    public void WriteSingleColumnAndParameter(string colName, string value, bool isTimeStamp = false, string suffix = "")
    {
        var col = EntitySetting.GetInheritedIncludedProperties().FirstOrDefault(p => p.ColumnName == colName);
        if (col != null)
            WriteSingleColumnAndParameter(col as SqliteRepositoryPropertySetting, value, isTimeStamp, suffix);
    }
}

public class WriteColumnsModel
{
    public WriteColumnsModel()
    {
        ValueList = new List<string>();
        Columns = new List<BaseCsDbRepositoryPropertySetting>();
    }

    public List<BaseCsDbRepositoryPropertySetting> Columns { get; set; }
    public BaseCsDbRepositoryPropertySetting? PKey { get; set; }
    public string PKeyValue { get; set; }
    public BaseCsDbRepositoryPropertySetting? CreatedAtCol { get; set; }
    public BaseCsDbRepositoryPropertySetting? UpdatedAtCol { get; set; }
    public List<string> ValueList { get; private set; }
}