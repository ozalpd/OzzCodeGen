using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Storage.Templates
{
    public abstract partial class AbstractStorageTemplate : AbstractTemplate
    {
        public AbstractStorageTemplate(StorageCodeEngine engine)
        {
            CodeEngine = engine;
        }

        public AbstractStorageTemplate(StorageEntitySetting tableDefinition)
        {
            TableDefinition = tableDefinition;
            CodeEngine = tableDefinition.CodeEngine;
        }

        public StorageCodeEngine CodeEngine { get; private set; }
        public StorageEntitySetting TableDefinition { get; private set; }

        public string GetColumnType(StorageColumnSetting column)
        {
            return CodeEngine.GetColumnType(column);
        }

        public override string GetDefaultFileName()
        {
            if (TableDefinition == null)
            {
                return CodeEngine.DatabaseName + ".sql";
            }
            else
            {
                return TableDefinition.Name + ".sql";
            }
        }
    }
}
