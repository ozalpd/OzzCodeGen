using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;
using OzzCodeGen.CodeEngines.Java;

namespace OzzCodeGen.CodeEngines.Android.Templates
{
    public partial class EntityContext : AbstractDataHelper
    {
        public EntityContext(JavaEntitySetting entity, bool isCustomFile)
            : base(entity, isCustomFile)
        {
            genParserName = "Gen" + Entity.Name + "Parser";
            ClassName = IsCustomFile ? Entity.Name + "Parser" : genParserName;
            BaseClassName = IsCustomFile ? genParserName :
                AndroidCodeEngine.BaseEntityContext.StripNamespace() +
                "<" + Entity.Name + ">";
        }

        private string genParserName;

        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();

            if (IsCustomFile)
            {
                imports.Add(AndroidCodeEngine.GetGeneratedsPackage() + "." + BaseClassName);
            }
            else
            {
                imports.Add("android.database.Cursor");
                imports.Add("android.database.sqlite.SQLiteDatabase");
                imports.Add("android.util.Log");
                imports.Add("");
                imports.Add(AndroidCodeEngine.BaseEntityContext);
            }
            imports.Add("");
            imports.Add(AndroidCodeEngine.GetGeneratedsPackage() + "." + AndroidCodeEngine.GeneratedDataContext);
            imports.Add(AndroidCodeEngine.GetModelsPackage() + "." + Entity.Name);

            return imports;
        }

        public string GetStorageTableName()
        {
            return Entity.GetStorageEntitySetting().TableName;
        }
    }
}
