using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.Storage.Templates.MsSql
{
    public partial class CreateMsSqlDb : AbstractStorageTemplate
    {
        public CreateMsSqlDb(StorageScriptsEngine engine) : base(engine) { }

        public override string GetDefaultFileName()
        {
            return string.Format("_Create_{0}.bat", AppEngine.DatabaseName);
        }
    }
}
