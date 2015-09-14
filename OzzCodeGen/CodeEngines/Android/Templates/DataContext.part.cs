using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;
using OzzCodeGen.CodeEngines.Java;

namespace OzzCodeGen.CodeEngines.Android.Templates
{
    public partial class DataContext : AbstractDataHelper
    {
        public DataContext(AndroidEngine engine, bool isCustomFile)
            : base(engine, isCustomFile)
        {
            ClassName = IsCustomFile ? AndroidCodeEngine.CustomDataContext : AndroidCodeEngine.GeneratedDataContext;
            BaseClassName = IsCustomFile ? AndroidCodeEngine.GeneratedDataContext : AndroidCodeEngine.BaseDataContext.StripNamespace();
        }


        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();

            imports.Add("android.content.Context");
            imports.Add("android.database.sqlite.SQLiteDatabase.CursorFactory");
            imports.Add("");
            imports.Add(AndroidCodeEngine.GetModelsPackage() + ".*");

            if (IsCustomFile)
            {
                imports.Add(AndroidCodeEngine.GetGeneratedsPackage() + "." + BaseClassName);
            }
            else
            {
                imports.Add(AndroidCodeEngine.BaseDataContext);
                imports.Add(AndroidCodeEngine.GetCustomsPackage() + ".*");
                //imports.Add("android.database.sqlite.SQLiteDatabase");
                //imports.Add("android.database.Cursor");
            }

            return imports;
        }
    }
}
