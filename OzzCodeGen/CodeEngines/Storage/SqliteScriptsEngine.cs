using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.Storage.Templates;
using OzzCodeGen.CodeEngines.Storage.Templates.Sqlite;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.Storage
{
    [XmlInclude(typeof(StorageEntitySetting))]
    public class SqliteScriptsEngine : StorageScriptsEngine
    {
        public override string EngineId
        {
            get { return EngineTypes.SqliteScriptsId; }
        }

        public override string ProjectTypeName
        {
            get { return "Sqlite Scripts Generator"; }
        }


        [XmlIgnore]
        public static string DefaultFileName { get { return "SqliteScriptsGen.settings"; } }

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        public override string GetDefaultTargetFolder()
        {
            return "SqliteScripts";
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>() { "ScriptFile" };
        }

        protected override string GetDefaultDbName()
        {
            return "LocalDB.Sqlite";
        }

        protected override void FixColumnType(StorageColumnSetting column)
        {
            string s = column.DataType.ToLowerInvariant();
            if (s.Equals("bool") || s.Equals("int") ||
                s.Equals("uint") || s.Equals("datetime"))
            {
                column.DataType = "INTEGER";
            }
            else if (s.Equals("string"))
            {
                column.DataType = "TEXT";
            }
            else if (s.Equals("float") || s.Equals("decimal") || s.Equals("double"))
            {
                column.DataType = "REAL";
            }
        }

        protected override AbstractStorageTemplate GetCreateTableTemplate(StorageEntitySetting tableDefinition)
        {
            return new CreateSqliteTable(tableDefinition);
        }

        protected override AbstractStorageTemplate GetCreateDbTemplate()
        {
            return null;
        }

        protected override AbstractStorageTemplate GetDropDbTemplate()
        {
            return null;
        }

        public override string GetPrimaryKeyDeclaration(StorageEntitySetting table)
        {
            throw new NotImplementedException();
        }

        public override string GetColumnDeclaration(StorageColumnSetting column, StorageEntitySetting table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(column.Name);
            sb.Append(' ');
            sb.Append(column.DataType);

            if (!column.Nullable)
            {
                sb.Append(" Not Null");
            }

            return sb.ToString();
        }

        public override string GetColumnType(StorageColumnSetting column)
        {
            return column.DataType;
        }

        public override StorageColumnSetting GetDefaultPrimaryKey()
        {
            return new StorageColumnSetting()
            {
                Name = "Id",
                DataType = "INTEGER",
                Nullable = false,
                PrimaryKey = true
            };
        }

        public override List<StorageColumnSetting> GetModifyTrackColumns()
        {
            return new List<StorageColumnSetting>();
        }

        public static SqliteScriptsEngine OpenFile(string fileName)
        {
            SqliteScriptsEngine instance = GetInstanceFromFile(fileName, typeof(SqliteScriptsEngine)) as SqliteScriptsEngine;
            foreach (var item in instance.EntitySettings)
            {
                ((StorageEntitySetting)item).CodeEngine = instance;
            }
            return instance;
        }
    }
}
