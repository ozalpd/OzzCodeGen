using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;
using OzzCodeGen.AppEngines.Java;

namespace OzzCodeGen.AppEngines.Android.Templates
{
    public partial class EntityContext : AbstractDataHelper
    {
        public EntityContext(JavaEntitySetting entity, bool isCustomFile)
            : base(entity, isCustomFile)
        {
            genParserName = "Gen" + Entity.Name + "Parser";
            ClassName = IsCustomFile ? Entity.Name + "Parser" : genParserName;
            BaseClassName = IsCustomFile ? genParserName :
                AndroidAppEngine.BaseEntityContext.StripNamespace() +
                "<" + Entity.Name + ">";
        }

        private string genParserName;

        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();

            if (IsCustomFile)
            {
                imports.Add(AndroidAppEngine.GetGeneratedsPackage() + "." + BaseClassName);
            }
            else
            {
                imports.Add("android.database.Cursor");
                imports.Add("android.database.sqlite.SQLiteDatabase");
                imports.Add("android.util.Log");
                imports.Add("");
                imports.Add(AndroidAppEngine.BaseEntityContext);
            }
            imports.Add("");
            imports.Add(AndroidAppEngine.GetGeneratedsPackage() + "." + AndroidAppEngine.GeneratedDataContext);
            imports.Add(AndroidAppEngine.GetModelsPackage() + "." + Entity.Name);

            return imports;
        }

        public string GetStorageTableName()
        {
            return Entity.GetStorageEntitySetting().TableName;
        }
    }
}
