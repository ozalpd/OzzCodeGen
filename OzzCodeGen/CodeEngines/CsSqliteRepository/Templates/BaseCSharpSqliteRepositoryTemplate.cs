using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;

public abstract class BaseCSharpSqliteRepositoryTemplate : AbstractTemplate
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
        return string.Join(", ", properties.Select(p => $"@{p.ColumnName}"));
    }

    protected string GetSqlLiteral(string value)
    {
        return value.Replace("\"", "\"\"");
    }

    protected string GetReadExpression(SqliteRepositoryPropertySetting property)
    {
        var ordinal = $"reader.GetOrdinal(\"{GetSqlLiteral(property.ColumnName)}\")";

        if (property.PropertyDefinition is StringProperty)
        {
            return $"reader.IsDBNull({ordinal}) ? null : Convert.ToString(reader.GetValue({ordinal}))";
        }

        if (property.PropertyDefinition is not SimpleProperty simpleProperty)
            return $"default({property.GetTypeName()})";

        if (!string.IsNullOrEmpty(simpleProperty.EnumTypeName))
        {
            if (simpleProperty.IsNullable)
                return $"reader.IsDBNull({ordinal}) ? null : ({simpleProperty.EnumTypeName}?)Convert.ToInt32(reader.GetValue({ordinal}))";

            return $"({simpleProperty.EnumTypeName})Convert.ToInt32(reader.GetValue({ordinal}))";
        }

        if (simpleProperty.IsTypeBoolean())
        {
            if (simpleProperty.IsNullable)
                return $"reader.IsDBNull({ordinal}) ? null : Convert.ToInt32(reader.GetValue({ordinal})) != 0";

            return $"Convert.ToInt32(reader.GetValue({ordinal})) != 0";
        }

        if (simpleProperty.IsTypeDateTime())
        {
            if (simpleProperty.IsNullable)
                return $"reader.IsDBNull({ordinal}) ? null : Convert.ToDateTime(reader.GetValue({ordinal}))";

            return $"Convert.ToDateTime(reader.GetValue({ordinal}))";
        }

        if (simpleProperty.IsTypeGuid())
        {
            if (simpleProperty.IsNullable)
                return $"reader.IsDBNull({ordinal}) ? null : Guid.Parse(Convert.ToString(reader.GetValue({ordinal}))!)";

            return $"Guid.Parse(Convert.ToString(reader.GetValue({ordinal}))!)";
        }

        if (simpleProperty.IsNullable)
            return $"reader.IsDBNull({ordinal}) ? null : ({GetNonNullableTypeName(property)}?)Convert.{GetConvertMethodName(simpleProperty.TypeName)}(reader.GetValue({ordinal}))";

        return $"Convert.{GetConvertMethodName(simpleProperty.TypeName)}(reader.GetValue({ordinal}))";
    }

    protected string GetDbValueExpression(SqliteRepositoryPropertySetting property, string expression, bool expressionCanBeNull = true)
    {
        if (property.PropertyDefinition is StringProperty)
            return expressionCanBeNull ? $"(object?){expression} ?? DBNull.Value" : expression;

        if (property.PropertyDefinition is not SimpleProperty simpleProperty)
            return expression;

        if (!string.IsNullOrEmpty(simpleProperty.EnumTypeName))
        {
            if (simpleProperty.IsNullable && expressionCanBeNull)
                return $"{expression}.HasValue ? (object)(int){expression}.Value : DBNull.Value";

            return $"(int){expression}";
        }

        if (simpleProperty.IsTypeBoolean())
        {
            if (simpleProperty.IsNullable && expressionCanBeNull)
                return $"{expression}.HasValue ? (object)({expression}.Value ? 1 : 0) : DBNull.Value";

            return $"{expression} ? 1 : 0";
        }

        if (simpleProperty.IsTypeDateTime())
        {
            if (simpleProperty.IsNullable && expressionCanBeNull)
                return $"{expression}.HasValue ? (object){expression}.Value.ToString(\"O\") : DBNull.Value";

            return $"{expression}.ToString(\"O\")";
        }

        if (simpleProperty.IsTypeGuid())
        {
            if (simpleProperty.IsNullable && expressionCanBeNull)
                return $"{expression}.HasValue ? (object){expression}.Value.ToString() : DBNull.Value";

            return $"{expression}.ToString()";
        }

        if (simpleProperty.IsNullable && expressionCanBeNull)
            return $"{expression}.HasValue ? (object){expression}.Value : DBNull.Value";

        return expression;
    }

    protected string GetLastInsertAssignment(SqliteRepositoryPropertySetting property, string targetExpression)
    {
        var typeName = GetNonNullableTypeName(property);
        return typeName switch
        {
            "int" => $"{targetExpression} = (int)connection.LastInsertRowId;",
            "uint" => $"{targetExpression} = (uint)connection.LastInsertRowId;",
            "long" => $"{targetExpression} = connection.LastInsertRowId;",
            "ulong" => $"{targetExpression} = (ulong)connection.LastInsertRowId;",
            "short" => $"{targetExpression} = (short)connection.LastInsertRowId;",
            "ushort" => $"{targetExpression} = (ushort)connection.LastInsertRowId;",
            "byte" => $"{targetExpression} = (byte)connection.LastInsertRowId;",
            "sbyte" => $"{targetExpression} = (sbyte)connection.LastInsertRowId;",
            _ => string.Empty
        };
    }

    private static string GetConvertMethodName(string typeName)
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
