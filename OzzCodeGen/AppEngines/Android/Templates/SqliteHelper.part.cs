using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;


namespace OzzCodeGen.AppEngines.Android.Templates
{
    public partial class SqliteHelper : AbstractDataHelper
    {
        public SqliteHelper(AndroidEngine appEngine, bool isCustomFile)
            : base(appEngine, isCustomFile)
        {
            ClassName = IsCustomFile ? AndroidAppEngine.CustomSqliteHelper : AndroidAppEngine.GeneratedSqliteHelper;
            BaseClassName = IsCustomFile ? AndroidAppEngine.GeneratedSqliteHelper : AndroidAppEngine.SqliteHelper.StripNamespace();
        }


        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();
            imports.Add("android.content.Context");
            imports.Add("android.database.sqlite.SQLiteDatabase.CursorFactory");
            imports.Add("");
            imports.Add("java.util.List");

            if (IsCustomFile)
            {
                imports.Add("");
                imports.Add(AndroidAppEngine.GetGeneratedsPackage() + "." + AndroidAppEngine.GeneratedSqliteHelper);
            }
            else
            {
                imports.Add("java.util.ArrayList");
                imports.Add("java.io.IOException");
                imports.Add("");
                imports.Add(AndroidAppEngine.SqliteHelper);
            }

            return imports;
        }
    }
}
