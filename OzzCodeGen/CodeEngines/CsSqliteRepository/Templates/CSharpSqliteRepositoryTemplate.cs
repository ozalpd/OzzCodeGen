using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;

public class CSharpSqliteRepositoryTemplate : BaseCSharpSqliteRepositoryTemplate
{
    public CSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting)
        : base(codeEngine, entitySetting)
    {
    }

    public override string GetDefaultFileName()
    {
        return $"{GetRepositoryClassName()}.cs";
    }

    public override string TransformText()
    {
        var repositoryProperties = GetRepositoryProperties().ToList();
        var insertProperties = GetInsertProperties().ToList();
        var updateProperties = GetUpdateProperties().ToList();
        var primaryKey = GetPrimaryKey();
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Microsoft.Data.Sqlite;");
        sb.AppendLine($"using {CodeEngine.ModelNamespaceName};");
        sb.AppendLine();
        sb.AppendLine($"namespace {CodeEngine.NamespaceName};");
        sb.AppendLine();
        sb.AppendLine($"public class {GetRepositoryClassName()} : {CodeEngine.BaseRepositoryClassName}");
        sb.AppendLine("{");
        sb.AppendLine($"    public {GetRepositoryClassName()}(string connectionString) : base(connectionString)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine();

        WriteGetAll(sb, repositoryProperties);

        if (primaryKey != null)
        {
            sb.AppendLine();
            WriteGetById(sb, repositoryProperties, primaryKey);
            sb.AppendLine();
            WriteDelete(sb, primaryKey);
        }

        sb.AppendLine();
        WriteInsert(sb, insertProperties, primaryKey);

        if (primaryKey != null && updateProperties.Count > 0)
        {
            sb.AppendLine();
            WriteUpdate(sb, updateProperties, primaryKey);
        }

        sb.AppendLine();
        WriteMap(sb, repositoryProperties);
        sb.AppendLine("}");

        return sb.ToString();
    }

    private void WriteGetAll(StringBuilder sb, List<SqliteRepositoryPropertySetting> repositoryProperties)
    {
        sb.AppendLine($"    public List<{GetEntityTypeName()}> GetAll()");
        sb.AppendLine("    {");
        sb.AppendLine("        using var connection = CreateConnection();");
        sb.AppendLine("        connection.Open();");
        sb.AppendLine("        using var command = connection.CreateCommand();");
        sb.AppendLine($"        command.CommandText = \"SELECT {GetColumnList(repositoryProperties)} FROM [{GetSqlLiteral(EntitySetting.TableName)}]\";");
        sb.AppendLine("        using var reader = command.ExecuteReader();");
        sb.AppendLine($"        var items = new List<{GetEntityTypeName()}>();");
        sb.AppendLine("        while (reader.Read())");
        sb.AppendLine("        {");
        sb.AppendLine("            items.Add(Map(reader));");
        sb.AppendLine("        }");
        sb.AppendLine("        return items;");
        sb.AppendLine("    }");
    }

    private void WriteGetById(StringBuilder sb, List<SqliteRepositoryPropertySetting> repositoryProperties, SqliteRepositoryPropertySetting primaryKey)
    {
        sb.AppendLine($"    public {GetEntityReturnType()} GetById({GetNonNullableTypeName(primaryKey)} id)");
        sb.AppendLine("    {");
        sb.AppendLine("        using var connection = CreateConnection();");
        sb.AppendLine("        connection.Open();");
        sb.AppendLine("        using var command = connection.CreateCommand();");
        sb.AppendLine($"        command.CommandText = \"SELECT {GetColumnList(repositoryProperties)} FROM [{GetSqlLiteral(EntitySetting.TableName)}] WHERE [{GetSqlLiteral(primaryKey.ColumnName)}] = @{GetSqlLiteral(primaryKey.ColumnName)} LIMIT 1\";");
        sb.AppendLine($"        command.Parameters.AddWithValue(\"@{GetSqlLiteral(primaryKey.ColumnName)}\", {GetDbValueExpression(primaryKey, "id", false)});");
        sb.AppendLine("        using var reader = command.ExecuteReader();");
        sb.AppendLine("        if (!reader.Read())");
        sb.AppendLine("        {");
        sb.AppendLine("            return null;");
        sb.AppendLine("        }");
        sb.AppendLine("        return Map(reader);");
        sb.AppendLine("    }");
    }

    private void WriteInsert(StringBuilder sb, List<SqliteRepositoryPropertySetting> insertProperties, SqliteRepositoryPropertySetting primaryKey)
    {
        sb.AppendLine($"    public void Insert({GetEntityTypeName()} entity)");
        sb.AppendLine("    {");
        sb.AppendLine("        using var connection = CreateConnection();");
        sb.AppendLine("        connection.Open();");
        sb.AppendLine("        using var command = connection.CreateCommand();");
        if (insertProperties.Count == 0)
        {
            sb.AppendLine($"        command.CommandText = \"INSERT INTO [{GetSqlLiteral(EntitySetting.TableName)}] DEFAULT VALUES\";");
        }
        else
        {
            sb.AppendLine($"        command.CommandText = \"INSERT INTO [{GetSqlLiteral(EntitySetting.TableName)}] ({GetColumnList(insertProperties)}) VALUES ({GetParameterList(insertProperties)})\";");
            foreach (var property in insertProperties)
            {
                sb.AppendLine($"        command.Parameters.AddWithValue(\"@{GetSqlLiteral(property.ColumnName)}\", {GetDbValueExpression(property, $"entity.{property.Name}")});");
            }
        }
        sb.AppendLine("        command.ExecuteNonQuery();");
        if (primaryKey != null && ShouldSkipInsert(primaryKey))
        {
            var assignment = GetLastInsertAssignment(primaryKey, $"entity.{primaryKey.Name}");
            if (!string.IsNullOrEmpty(assignment))
            {
                sb.AppendLine($"        {assignment}");
            }
        }
        sb.AppendLine("    }");
    }

    private void WriteUpdate(StringBuilder sb, List<SqliteRepositoryPropertySetting> updateProperties, SqliteRepositoryPropertySetting primaryKey)
    {
        var setClause = string.Join(", ", updateProperties.Select(p => $"[{p.ColumnName}] = @{p.ColumnName}"));
        sb.AppendLine($"    public void Update({GetEntityTypeName()} entity)");
        sb.AppendLine("    {");
        sb.AppendLine("        using var connection = CreateConnection();");
        sb.AppendLine("        connection.Open();");
        sb.AppendLine("        using var command = connection.CreateCommand();");
        sb.AppendLine($"        command.CommandText = \"UPDATE [{GetSqlLiteral(EntitySetting.TableName)}] SET {setClause} WHERE [{GetSqlLiteral(primaryKey.ColumnName)}] = @{GetSqlLiteral(primaryKey.ColumnName)}\";");
        foreach (var property in updateProperties)
        {
            sb.AppendLine($"        command.Parameters.AddWithValue(\"@{GetSqlLiteral(property.ColumnName)}\", {GetDbValueExpression(property, $"entity.{property.Name}")});");
        }
        sb.AppendLine($"        command.Parameters.AddWithValue(\"@{GetSqlLiteral(primaryKey.ColumnName)}\", {GetDbValueExpression(primaryKey, $"entity.{primaryKey.Name}")});");
        sb.AppendLine("        command.ExecuteNonQuery();");
        sb.AppendLine("    }");
    }

    private void WriteDelete(StringBuilder sb, SqliteRepositoryPropertySetting primaryKey)
    {
        sb.AppendLine($"    public void Delete({GetNonNullableTypeName(primaryKey)} id)");
        sb.AppendLine("    {");
        sb.AppendLine("        using var connection = CreateConnection();");
        sb.AppendLine("        connection.Open();");
        sb.AppendLine("        using var command = connection.CreateCommand();");
        sb.AppendLine($"        command.CommandText = \"DELETE FROM [{GetSqlLiteral(EntitySetting.TableName)}] WHERE [{GetSqlLiteral(primaryKey.ColumnName)}] = @{GetSqlLiteral(primaryKey.ColumnName)}\";");
        sb.AppendLine($"        command.Parameters.AddWithValue(\"@{GetSqlLiteral(primaryKey.ColumnName)}\", {GetDbValueExpression(primaryKey, "id", false)});");
        sb.AppendLine("        command.ExecuteNonQuery();");
        sb.AppendLine("    }");
    }

    private void WriteMap(StringBuilder sb, List<SqliteRepositoryPropertySetting> repositoryProperties)
    {
        sb.AppendLine($"    private static {GetEntityTypeName()} Map(SqliteDataReader reader)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = new {GetEntityTypeName()}();");
        foreach (var property in repositoryProperties)
        {
            sb.AppendLine($"        entity.{property.Name} = {GetReadExpression(property)};");
        }
        sb.AppendLine("        return entity;");
        sb.AppendLine("    }");
    }
}
