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
                    return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetBy{column.Name}Async({column.GetTypeName()} {column.Name.ToCamelCase()}{(EntitySetting.HasIsActiveProperty() ? ", bool? isActive = null" : "")})";
                break;

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
        var cols = EntitySetting.GetInheritedIncludedProperties()
                            .OfType<SqliteRepositoryPropertySetting>()
                            .Where(p => p.AutoLoad)
                            .OrderBy(p => p.PropertyDefinition.DisplayOrder);

        return cols;
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

    protected string GetRepositoryName(string entityName)
    {
        string fixedName = entityName.EndsWith("Dto") ? entityName[..^3] : entityName;
        if (fixedName.StartsWith("ICollection<"))
            fixedName = fixedName.Substring(12, fixedName.Length - 13);

        return $"{fixedName}Repository";
    }

    protected virtual bool ShouldSkipInsert(SqliteRepositoryPropertySetting property)
    {
        if (!property.IsKey)
            return false;

        if (property.PropertyDefinition is not SimpleProperty simpleProperty)
            return false;

        return simpleProperty.IsStoreGenerated || simpleProperty.IsTypeIntNumeric();
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

    protected string GetMappingExpression(SqliteRepositoryPropertySetting property, string ordinalNr, string readerName = "reader")
    {
        var sp = property.PropertyDefinition as SimpleProperty;
        bool isEnum = sp != null && !string.IsNullOrWhiteSpace(sp.EnumTypeName);
        bool isNullable = sp != null && sp.IsNullable;

        var sb = new StringBuilder();
        if (isNullable)
        {
            sb.Append(readerName);
            sb.Append(".IsDBNull(");
            sb.Append(ordinalNr);
            sb.Append(") ? null : ");
        }

        if (isEnum)
        {
            sb.Append("(");
            sb.Append(sp.EnumTypeName);
            sb.Append(")");
        }

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

        sb.Append(ordinalNr);
        sb.Append(')');

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

        return sb.ToString();
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
        bool hasTimeStamp = model.Columns.Any(c => c.Name == model.CreatedAtCol?.Name || c.Name == model.UpdatedAtCol?.Name);
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
}

public enum MethodType
{
    Create,
    DeleteByPKey,
    DeleteByUniqueIndex,
    GetAll,
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