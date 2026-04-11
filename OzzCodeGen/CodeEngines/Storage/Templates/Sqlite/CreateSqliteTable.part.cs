using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.Storage.Templates.Sqlite
{
    public partial class CreateSqliteTable : AbstractStorageTemplate
    {
        public CreateSqliteTable(StorageEntitySetting tableDefinition)
            : base(tableDefinition)
        {

        }

        public string GetIndexDeclaration(StorageColumnSetting column)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Create");
            if (column.Unique)
            {
                sb.Append(" Unique");
            }
            sb.Append(" Index If Not Exists idx_");
            sb.Append(TableDefinition.TableName);
            sb.Append('_');
            sb.Append(column.Name);
            sb.Append(" on ");
            sb.Append(TableDefinition.TableName);
            sb.Append('(');
            sb.Append(column.Name);
            if (column.PropertyDefinition.IsTypeString())
            {
                sb.Append(" COLLATE NOCASE");
            }
            if (column.SortDesc)
            {
                sb.Append(" DESC");
            }

            AppendColumnNames(column, sb);
            sb.Append(')');

            return sb.ToString();
        }

        private static void AppendColumnNames(StorageColumnSetting column, StringBuilder sb)
        {
            if (string.IsNullOrWhiteSpace(column.CompositeIndexColumns))
                return;

            var names = column.CompositeIndexColumns.Split(';')
                              .Select(name => name.Trim())
                              .Where(name => !string.IsNullOrEmpty(name))
                              .ToArray();
            foreach (var item in names)
            {
                sb.Append(", ");
                sb.Append(item);
            }
        }
    }
}
