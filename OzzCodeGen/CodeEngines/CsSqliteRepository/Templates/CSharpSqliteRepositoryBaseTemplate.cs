using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates;

public class CSharpSqliteRepositoryBaseTemplate : BaseCSharpSqliteRepositoryTemplate
{
    public CSharpSqliteRepositoryBaseTemplate(CSharpSqliteRepositoryEngine codeEngine)
        : base(codeEngine)
    {
    }

    public override string GetDefaultFileName()
    {
        return $"{CodeEngine.BaseRepositoryClassName}.cs";
    }

    public override string TransformText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using Microsoft.Data.Sqlite;");
        sb.AppendLine();
        sb.AppendLine($"namespace {CodeEngine.NamespaceName};");
        sb.AppendLine();
        sb.AppendLine($"public abstract class {CodeEngine.BaseRepositoryClassName}");
        sb.AppendLine("{");
        sb.AppendLine($"    protected {CodeEngine.BaseRepositoryClassName}(string connectionString)");
        sb.AppendLine("    {");
        sb.AppendLine("        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    protected string ConnectionString { get; }");
        sb.AppendLine();
        sb.AppendLine("    protected SqliteConnection CreateConnection()");
        sb.AppendLine("    {");
        sb.AppendLine("        return new SqliteConnection(ConnectionString);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
