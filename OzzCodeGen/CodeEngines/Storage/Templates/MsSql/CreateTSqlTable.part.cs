using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Storage.Templates.MsSql
{
    public partial class CreateTSqlTable : AbstractStorageTemplate
    {
        public CreateTSqlTable(StorageEntitySetting tableDefinition)
            : base(tableDefinition)
        {

        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            if (TableDefinition.StoredProcs && !TableDefinition.CustomStoredProcs)
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
