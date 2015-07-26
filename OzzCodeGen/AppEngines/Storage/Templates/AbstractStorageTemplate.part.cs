using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.Storage.Templates
{
    public abstract partial class AbstractStorageTemplate : AbstractTemplate
    {
        public AbstractStorageTemplate(StorageScriptsEngine appEngine)
        {
            AppEngine = appEngine;
        }

        public AbstractStorageTemplate(StorageEntitySetting tableDefinition)
        {
            TableDefinition = tableDefinition;
            AppEngine = tableDefinition.AppEngine;
        }

        public StorageScriptsEngine AppEngine { get; private set; }
        public StorageEntitySetting TableDefinition { get; private set; }

        public string GetColumnType(StorageColumnSetting column)
        {
            return AppEngine.GetColumnType(column);
        }

        public override string GetDefaultFileName()
        {
            if (TableDefinition == null)
            {
                return AppEngine.DatabaseName + ".sql";
            }
            else
            {
                return TableDefinition.Name + ".sql";
            }
        }
    }
}
