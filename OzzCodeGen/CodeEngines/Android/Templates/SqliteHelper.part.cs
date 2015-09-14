using System.Collections.Generic;
using OzzUtils;


namespace OzzCodeGen.CodeEngines.Android.Templates
{
    public partial class SqliteHelper : AbstractDataHelper
    {
        public SqliteHelper(AndroidEngine engine, bool isCustomFile)
            : base(engine, isCustomFile)
        {
            ClassName = IsCustomFile ? AndroidCodeEngine.CustomSqliteHelper : AndroidCodeEngine.GeneratedSqliteHelper;
            BaseClassName = IsCustomFile ? AndroidCodeEngine.GeneratedSqliteHelper : AndroidCodeEngine.SqliteHelper.StripNamespace();
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
                imports.Add(AndroidCodeEngine.GetGeneratedsPackage() + "." + AndroidCodeEngine.GeneratedSqliteHelper);
            }
            else
            {
                imports.Add("java.util.ArrayList");
                imports.Add("java.io.IOException");
                imports.Add("");
                imports.Add(AndroidCodeEngine.SqliteHelper);
            }

            return imports;
        }
    }
}
