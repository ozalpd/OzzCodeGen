using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;
using OzzCodeGen.AppEngines.Java;

namespace OzzCodeGen.AppEngines.Android.Templates
{
    public partial class DataContext : AbstractDataHelper
    {
        public DataContext(AndroidEngine appEngine, bool isCustomFile)
            : base(appEngine, isCustomFile)
        {
            ClassName = IsCustomFile ? AndroidAppEngine.CustomDataContext : AndroidAppEngine.GeneratedDataContext;
            BaseClassName = IsCustomFile ? AndroidAppEngine.GeneratedDataContext : AndroidAppEngine.BaseDataContext.StripNamespace();
        }


        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();

            imports.Add("android.content.Context");
            imports.Add("android.database.sqlite.SQLiteDatabase.CursorFactory");
            imports.Add("");
            imports.Add(AndroidAppEngine.GetModelsPackage() + ".*");

            if (IsCustomFile)
            {
                imports.Add(AndroidAppEngine.GetGeneratedsPackage() + "." + BaseClassName);
            }
            else
            {
                imports.Add(AndroidAppEngine.BaseDataContext);
                imports.Add(AndroidAppEngine.GetCustomsPackage() + ".*");
                //imports.Add("android.database.sqlite.SQLiteDatabase");
                //imports.Add("android.database.Cursor");
            }

            return imports;
        }
    }
}
