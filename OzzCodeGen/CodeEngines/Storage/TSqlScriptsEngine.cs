using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.Storage.Templates;
using OzzCodeGen.CodeEngines.Storage.Templates.MsSql;
using OzzCodeGen.Definitions;
using System;

namespace OzzCodeGen.CodeEngines.Storage
{
    [XmlInclude(typeof(StorageEntitySetting))]
    public class TSqlScriptsEngine : StorageCodeEngine
    {
        public override string EngineId
        {
            get { return EngineTypes.TSqlScriptsId; }
        }

        public override string ProjectTypeName
        {
            get { return "T-SQL Scripts Generator"; }
        }

        [XmlIgnore]
        public static string DefaultFileName { get { return "TSqlScriptsGen.settings"; } }

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        public override string GetDefaultTargetFolder()
        {
            return "T-SQL Scripts";
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>() { "ScriptFile" };
        }

        protected override void FixColumnType(StorageColumnSetting column)
        {
            if (column.IsDateTime)
            {
                column.DataType = "DateTime";
                return;
            }


            int colLen = 255;
            column.DataType = column.DataType.Trim();
            if (column.DataType.ToLowerInvariant().StartsWith("nullable<"))
            {
                column.DataType = column.DataType.Remove(0, "nullable<".Length).Replace(">", "");
            }   
            column.DataType.Replace("?", "");

            switch (column.DataType.ToLowerInvariant())
            {
                case "string":
                    column.DataType = "nVarChar";
                    if (column.PropertyDefinition.DefinitionType == DefinitionType.String)
                    {
                        var strProperty = (StringProperty)column.PropertyDefinition;
                        colLen = strProperty.MaxLenght;// > 0 ? strProperty.MaxLenght : 2048;
                    }
                    column.Lenght = colLen;
                    break;

                case "bool":
                    column.DataType = "bit";
                    break;

                case "byte":
                    column.DataType = "tinyint";
                    break;

                case "byte[]":
                    column.DataType = "varbinary";
                    break;

                case "decimal":
                    column.DataType = "[decimal](18, 4)";
                    break;

                case "system.guid":
                    column.DataType = "UniqueIdentifier";
                    break;

                case "guid":
                    column.DataType = "UniqueIdentifier";
                    break;


                default:
                    break;
            }

        }

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static TSqlScriptsEngine OpenFile(string fileName)
        {
            TSqlScriptsEngine instance = GetInstanceFromFile(fileName, typeof(TSqlScriptsEngine)) as TSqlScriptsEngine;
            foreach (var item in instance.EntitySettings)
            {
                ((StorageEntitySetting)item).CodeEngine = instance;
            }
            return instance;
        }

        protected override AbstractStorageTemplate GetCreateTableTemplate(StorageEntitySetting tableDefinition)
        {
            return new CreateTSqlTable(tableDefinition);
        }

        public override string GetPrimaryKeyDeclaration(StorageEntitySetting table)
        {
            StringBuilder sb = new StringBuilder();

            AppendColumnNameType(table.PrimaryKeyColumn, sb);

            if (table.PrimaryKeyColumn.PropertyDefinition.IsTypeNumeric() &
                !table.UseInheritance)
            {
                sb.Append(" Identity(1,1)");
            }

            AppendColumnNullability(table.PrimaryKeyColumn, sb);

            if (table.UseInheritance)
            {
                sb.Append(' ');
                sb.Append(this.GetForeignKeyReferences(table.PrimaryKeyColumn, table, false, table.GetBaseTable()));
            }

            return sb.ToString();
        }

        public override string GetColumnType(StorageColumnSetting column)
        {
            StringBuilder sb = new StringBuilder();

            if (!column.DataType.Contains("["))
                sb.Append('[');
            sb.Append(column.DataType);
            if (!column.DataType.Contains("]"))
                sb.Append(']');

            string dataType = column.DataType.ToLowerInvariant();
            if (dataType.Equals("nvarchar") || dataType.Equals("varchar") ||
                dataType.Equals("nchar") || dataType.Equals("char") || dataType.Equals("varbinary"))
            {
                sb.Append('(');
                if (column.Lenght > 0)
                    sb.Append(column.Lenght);
                else
                    sb.Append("max");
                sb.Append(')');
            }

            return sb.ToString();
        }

        public override string GetColumnDeclaration(StorageColumnSetting column, StorageEntitySetting table)
        {
            StringBuilder sb = new StringBuilder();
            AppendColumnNameType(column, sb);
            AppendColumnNullability(column, sb);

            var foreignTable = GetForeignTable(column);
            if (foreignTable != null)
            {
                if (foreignTable.ForeignTables.Contains(table))
                {
                    string fKeyRef = string.Format("Alter Table [{0}].[{1}] Add {2}",
                                        table.SchemaName,
                                        table.TableName,
                                        GetForeignKeyReferences(column, table, true));

                    if (!AdditionalCommands.Contains(fKeyRef))
                    {
                        AdditionalCommands.Add(fKeyRef);
                    }
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(GetForeignKeyReferences(column, table, false));
                }
            }

            if (!string.IsNullOrEmpty(column.InsertDefault))
            {
                if (!column.InsertDefault.Trim().StartsWith("@") &&
                    !column.InsertDefault.Trim().Equals("null", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    sb.Append(" Default ");
                    sb.Append(column.InsertDefault);
                }
                if (column.InsertDefault.Trim().Equals("null", System.StringComparison.InvariantCultureIgnoreCase) && !column.Nullable)
                {
                    sb.Append(" /* Warning! Column's default is null */");
                }
            }
            
            return sb.ToString();
        }

        private void AppendColumnNameType(StorageColumnSetting column, StringBuilder sb)
        {
            sb.Append('[');
            sb.Append(column.Name);
            sb.Append(']');
            sb.Append(' ');
            sb.Append(GetColumnType(column));
        }

        private void AppendColumnNullability(StorageColumnSetting column, StringBuilder sb)
        {
            if (column.DataType.StartsWith("as ", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (column.Nullable)
            {
                sb.Append(" Null");
            }
            else
            {
                sb.Append(" Not Null");
            }
        }

        private string GetForeignKeyReferences(StorageColumnSetting column, StorageEntitySetting table, bool putColumnName, 
            StorageEntitySetting foreignTable = null)
        {
            if (foreignTable == null)
            {
                foreignTable = GetForeignTable(column);
            }
            if (foreignTable == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("Constraint FK_");
            sb.Append(table.Name);
            sb.Append('_');
            sb.Append(column.Name);
            sb.Append(" Foreign Key");
            if (putColumnName)
            {
                sb.Append("([");
                sb.Append(column.Name);
                sb.Append("])");
            }
            sb.Append(" References [");
            sb.Append(foreignTable.SchemaName);
            sb.Append("].[");
            sb.Append(foreignTable.TableName);
            sb.Append("]([");
            sb.Append(foreignTable.PrimaryKeyColumn.Name);
            sb.Append("])");

            return sb.ToString();
        }

        public override StorageColumnSetting GetDefaultPrimaryKey()
        {
            return new StorageColumnSetting()
            {
                Name = "Id",
                DataType = "int",
                Nullable = false,
                PrimaryKey = true
            };
        }

        //TODO: Read this from a file
        public override List<StorageColumnSetting> GetModifyTrackColumns()
        {
            if (_modifyTracColumns != null)
                return _modifyTracColumns;

            _modifyTracColumns = new List<StorageColumnSetting>();
            _modifyTracColumns.Add(new StorageColumnSetting()
            {
                Name = "ModifyNr",
                DataType = "int",
                InsertDefault = "1"
            });

            _modifyTracColumns.Add(new StorageColumnSetting()
            {
                Name = "ModifierIp",
                DataType = "varchar",
                Lenght = 50
            });
            _modifyTracColumns.Add(new StorageColumnSetting()
            {
                Name = "CreatorIp",
                DataType = "varchar",
                Lenght = 50
            });

            _modifyTracColumns.Add(new StorageColumnSetting()
            {
                Name = "ModifyDate",
                DataType = "DateTime",
                InsertDefault = "GetDate()",
                Indexed = true,
                SortDesc = true
            });
            _modifyTracColumns.Add(new StorageColumnSetting()
            {
                Name = "CreateDate",
                DataType = "DateTime",
                InsertDefault = "GetDate()"
            });
            return _modifyTracColumns;
        }
        List<StorageColumnSetting> _modifyTracColumns;

        protected override AbstractStorageTemplate GetCreateDbTemplate()
        {
            return new CreateMsSqlDb(this);
        }

        protected override AbstractStorageTemplate GetDropDbTemplate()
        {
            return new DropMsSqlDb(this);
        }
    }
}
