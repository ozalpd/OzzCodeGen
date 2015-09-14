using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Storage.Templates
{
    public partial class Additionals : AbstractStorageTemplate
    {
        public Additionals(StorageScriptsEngine engine) : base(engine) { }

        public override string GetDefaultFileName()
        {
            return "_FinishingSetup.sql";
        }
    }
}
