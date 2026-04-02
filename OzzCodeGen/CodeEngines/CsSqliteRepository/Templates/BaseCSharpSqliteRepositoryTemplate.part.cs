using OzzCodeGen.Definitions;
using OzzUtils;
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

    protected SqliteRepositoryPropertySetting GetPrimaryKey()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsKey);
    }

    protected SqliteRepositoryPropertySetting GetUniqueIndexed()
    {
        return GetRepositoryProperties().FirstOrDefault(p => p.IsUniqueIndexed && !p.IsKey);
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetInsertProperties()
    {
        return GetRepositoryProperties().Where(p => !ShouldSkipInsert(p));
    }

    protected IEnumerable<SqliteRepositoryPropertySetting> GetUpdateProperties()
    {
        return GetRepositoryProperties().Where(p => !p.IsKey && !IsReadOnlyColumn(p));
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

    protected string GetEntityTypeName()
    {
        return EntitySetting?.Name ?? string.Empty;
    }

    protected string GetRepositoryClassName()
    {
        return $"{GetEntityTypeName()}Repository";
    }

    protected string GetEntityReturnType()
    {
        var entityTypeName = GetEntityTypeName();
        if (CodeEngine?.Project?.TargetPlatform == TargetDotNetPlatform.ModernDotNet)
            return entityTypeName + "?";

        return entityTypeName;
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

    protected string GetSqlLiteral(string value)
    {
        return value.Replace("\"", "\"\"");
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
}
