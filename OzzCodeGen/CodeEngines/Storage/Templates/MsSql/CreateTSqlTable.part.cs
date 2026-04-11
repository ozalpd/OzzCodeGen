using System.IO;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.Storage.Templates.MsSql
{
    public partial class CreateTSqlTable : AbstractStorageTemplate
    {
        public CreateTSqlTable(StorageEntitySetting tableDefinition)
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
            else
            {
                sb.Append(" Nonclustered");
            }
            //sb.Append(" Index If Not Exists");
            sb.Append(" Index [idx_");
            if (column.Unique)
                sb.Append("uniq_");
            sb.Append(TableDefinition.TableName);
            sb.Append('_');
            sb.Append(column.Name);
            sb.Append("] On [");
            sb.Append(TableDefinition.SchemaName);
            sb.Append("].[");
            sb.Append(TableDefinition.TableName);
            sb.Append("]([");
            sb.Append(column.Name);
            sb.Append("]");
            if (column.SortDesc)
            {
                sb.Append(" Desc");
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
                sb.Append(", [");
                sb.Append(item);
                sb.Append(']');
            }
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            if (TableDefinition.StoredProcGeneration > StoredProcGeneration.NoStoredProcedure && !TableDefinition.CustomStoredProcs)
            {
                var spTemplate = new TsqlStoredProcs(TableDefinition);
                var spPath = Path.Combine(Path.GetDirectoryName(FilePath), spTemplate.GetDefaultFileName());

                return base.WriteToFile(FilePath, overwriteExisting) &
                    spTemplate.WriteToFile(spPath, overwriteExisting);
            }
            else
            {
                return base.WriteToFile(FilePath, overwriteExisting);
            }
        }
    }
}
