using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.CsModelClass.UI;
using OzzCodeGen.Definitions;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;

public abstract partial class BaseCSharpSqliteRepositoryTemplate : AbstractTemplate
{
    protected BaseCSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting = null)
    {
        CodeEngine = codeEngine;
        EntitySetting = entitySetting;
    }

    protected CSharpSqliteRepositoryEngine CodeEngine { get; }
    protected SqliteRepositoryEntitySetting EntitySetting { get; }

    protected List<string> SignatureList = new List<string>();

    protected void AddSignature(string signature)
    {
        if (!SignatureList.Contains(signature))
            SignatureList.Add(signature);
    }

    protected string GetSignature(MethodType methodType, SqliteRepositoryPropertySetting column = null)
    {
        var pkey = GetPrimaryKey();
        var unique = GetUniqueIndexed();
        ModelClassEntitySetting modelEntity = EntitySetting.ModelClassEntitySetting;
        CSharpModelClassCodeEngine modelClassEngine = modelEntity?.CodeEngine as CSharpModelClassCodeEngine;
        string queryParamClassName = string.Empty;
        if (modelEntity != null && modelClassEngine != null)
            queryParamClassName = modelEntity.GenerateQueryParam
                                ? $"{EntitySetting.Name}{modelClassEngine.QueryParamClassName}"
                                : modelClassEngine.QueryParamClassName;
        bool hasIsActive = EntitySetting.HasIsActiveProperty();
        bool needIsActive = hasIsActive
                         && EntitySetting.SearchableNonRangeProperties
                                         .Count(p => p.Name.Equals("IsActive", StringComparison.InvariantCultureIgnoreCase)) == 0;
        switch (methodType)
        {
            case MethodType.Create:
                return $"Task<{pkey.GetTypeName()}> CreateAsync({EntitySetting.Name} {EntitySetting.Name.ToCamelCase()})";

            case MethodType.DeleteByPKey:
                return $"Task<bool> DeleteAsync({pkey.GetTypeName()} {pkey.Name.ToCamelCase()})";

            case MethodType.DeleteByUniqueIndex:
                if (unique != null)
                    return $"Task<bool> DeleteAsync({unique.GetTypeName()} {unique.Name.ToCamelCase()})";
                break;

            case MethodType.GetAll:
                return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetAllAsync({(EntitySetting.HasIsActiveProperty() ? "bool? isActive = null" : "")})";

            case MethodType.GetByPKey:
                return $"Task<{EntitySetting.GetTypeName(isNullable: true)}> GetBy{pkey.Name}Async({pkey.GetTypeName()}? {pkey.Name.ToCamelCase()})";

            case MethodType.GetByUnique:
                if (unique != null)
                    return $"Task<{EntitySetting.GetTypeName(isNullable: true)}> GetBy{unique.Name}Async({unique.GetTypeName()}? {unique.Name.ToCamelCase()})";
                break;

            case MethodType.GetByForeignKey:
                if (column != null)
                    return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetBy{column.Name}Async({column.GetTypeName()} {column.Name.ToCamelCase()}{(hasIsActive ? ", bool? isActive = null" : "")})";
                break;

            case MethodType.GetPaged:
                if (string.IsNullOrWhiteSpace(queryParamClassName))
                    return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetPagedAsync(int pageNumber, int pageSize, string sortBy = null, bool ascending = true{(hasIsActive ? ", bool? isActive = null" : "")})";
                else
                    return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetPagedAsync({queryParamClassName} queryParameters{(needIsActive ? ", bool? isActive = null" : "")})";

            case MethodType.UpdateEntity:
                return $"Task<bool> UpdateAsync({EntitySetting.Name} {EntitySetting.Name.ToCamelCase()})";

            case MethodType.UpdateSingleColumnById:
                if (column != null)
                    return $"Task<bool> Update{column.Name}Async({pkey.GetTypeName()} {pkey.Name.ToCamelCase()}, {column.GetTypeName()} {column.Name.ToCamelCase()})";
                break;

            default:
                break;
        }

        return string.Empty;
    }

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
        var entity = CodeEngine.Entities.FirstOrDefault(e => e.Name == entityName);
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

    protected SqliteRepositoryPropertySetting GetPrimaryKey()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsKey);
    }

    protected SqliteRepositoryPropertySetting GetUpdatedAtColumn()
    {
        string updatedAtColName = EntitySetting.UpdatedAtName;
        return GetRepositoryProperties()
                    .FirstOrDefault(p => p.ColumnName.Equals(updatedAtColName, StringComparison.InvariantCultureIgnoreCase));
    }

    protected SqliteRepositoryPropertySetting GetUniqueIndexed()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsUniqueIndexed && !p.IsKey);
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetAutoLoadProperties()
    {
        return EntitySetting.GetAutoLoadProperties();
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetForeignKeyProperties()
    {
        return GetRepositoryProperties().Where(p => p.IsForeignKey());
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetInsertProperties()
    {
        return GetRepositoryProperties().Where(p => !ShouldSkipInsert(p));
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetNonUpdateProperties()
    {
        return GetRepositoryProperties().Where(p => !p.IsKey && IsReadOnlyColumn(p));
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

    public void WriteColumnsAndParameters(IEnumerable<SqliteRepositoryPropertySetting> columns, SqliteRepositoryPropertySetting? pkey = null, SqliteRepositoryPropertySetting? createdAtCol = null, SqliteRepositoryPropertySetting? updatedAtCol = null)
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
            WriteSingleColumnAndParameter(model.PKey, model.PKeyValue);

        var columns = model.Columns.ToList();
        var valueList = model.ValueList;

        for (int i = 0; i < columns.Count; i++)
        {
            var col = columns[i];
            val = valueList.Count > i ? valueList[i] : $"{EntitySetting.Name.ToCamelCase()}.{col.Name}";
            bool isTimeStamp = col.Name == model.CreatedAtCol?.Name || col.Name == model.UpdatedAtCol?.Name;
            WriteSingleColumnAndParameter(col, val, isTimeStamp);
        }
        PopIndent();
        PopIndent();
    }

    public void WriteSingleColumnAndParameter(string colName, string value, bool isTimeStamp = false, string suffix = "")
    {
        var col = EntitySetting.GetInheritedIncludedProperties().FirstOrDefault(p => p.ColumnName == colName);
        if (col != null)
            WriteSingleColumnAndParameter(col, value, isTimeStamp, suffix);
    }
}

public enum MethodType
{
    Create,
    DeleteByPKey,
    DeleteByUniqueIndex,
    GetAll,
    GetPaged,
    GetByPKey,
    GetByUnique,
    GetByForeignKey,
    UpdateEntity,
    UpdateSingleColumnById,
}

public class WriteColumnsModel
{
    public WriteColumnsModel()
    {
        ValueList = new List<string>();
        Columns = new List<SqliteRepositoryPropertySetting>();
    }

    public List<SqliteRepositoryPropertySetting> Columns { get; set; }
    public SqliteRepositoryPropertySetting? PKey { get; set; }
    public string PKeyValue { get; set; }
    public SqliteRepositoryPropertySetting? CreatedAtCol { get; set; }
    public SqliteRepositoryPropertySetting? UpdatedAtCol { get; set; }
    public List<string> ValueList { get; private set; }
}