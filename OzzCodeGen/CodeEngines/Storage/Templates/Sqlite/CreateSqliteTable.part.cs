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

            sb.Append("Create  Index idx_");
            sb.Append(TableDefinition.TableName);
            sb.Append('_');
            sb.Append(column.Name);
            sb.Append(" on ");
            sb.Append(TableDefinition.TableName);
            sb.Append('(');
            sb.Append(column.Name);
            sb.Append(' ');
            if (column.PropertyDefinition.IsTypeString())
            {
                sb.Append("COLLATE NOCASE ");
            }
            if (column.SortDesc)
            {
                sb.Append("DESC ");
            }
            else
            {
                sb.Append("ASC ");
            }
            sb.Append(')');

            return sb.ToString();
        }
    }
}
