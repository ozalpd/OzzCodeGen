using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.CodeEngines.Storage;
using System.IO;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public partial class ObjcEntityContext : BaseObjcClassImpl
    {
        public ObjcEntityContext(ObjcEntitySetting entity)
            : base(entity)
        {
            ObjectiveCName = string.Format("{0}Context", entity.Name);
        }

        public override BaseObjcHeader GetDefaultHeader()
        {
            var header = new ObjcContextHeader(Entity)
            {
                ObjectiveCName = ObjectiveCName
            };

            header.SetSqliteProperties(ColumnNames.Split(','));
            return header;
        }

        protected override string GetDirectoryName(string path)
        {
            return Path.Combine(base.GetDirectoryName(path), "Generated");
        }

        public override string GetDefaultFileName()
        {
            if (Entity.GenerateCpp)
            {
                return ObjectiveCName + "-Gen.mm";
            }
            else
            {
                return ObjectiveCName + "-Gen.m";
            }
        }

        protected string GetExtensionFilePath(string path)
        {
            string dir = Path.Combine(Path.GetDirectoryName(path), "Custom");
            if (Entity.GenerateCpp)
            {
                return Path.Combine(dir, ObjectiveCName + ".mm");
            }
            else
            {
                return Path.Combine(dir, ObjectiveCName + ".m");
            }
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            var extensionTmp = new ObjcClassExtension(ObjectiveCName, Entity);
            extensionTmp.HeaderImports.Add(string.Format("\"{0}\"", HeaderTemplate.GetDefaultFileName()));

            return base.WriteToFile(FilePath, overwriteExisting) &
                extensionTmp.WriteToFile(GetExtensionFilePath(FilePath), overwriteExisting);
        }

        public StorageEntitySetting StorageEntitySetting
        {
            get
            {
                return Entity.GetStorageEntitySetting();
            }
        }

        public string DictionaryExtensionMethod(ObjcPropertySetting property)
        {
            if (property.PropertyDefinition.IsTypeString())
            {
                return "stringForKey";
            }
            else if (property.PropertyDefinition.IsTypeDateTime())
            {
                return "dateForKey";
            }
            else if (property.PropertyDefinition.IsTypeNumeric() || property.PropertyDefinition.IsTypeBoolean())
            {
                return "integerForKey";
            }
            else
            {
                return string.Format("/* {0} */", property.PropertyDefinition.TypeName);
            }
        }

        public string ReadFromSqliteStatement(ObjcPropertySetting property)
        {
            if (property.PropertyDefinition.IsTypeString())
            {
                return "getStringFromStatement";
            }
            else if (property.PropertyDefinition.IsTypeDateTime())
            {
                return "getDateFromStatement";
            }
            else if (property.PropertyDefinition.IsTypeNumeric() || property.PropertyDefinition.IsTypeBoolean())
            {
                return "getIntegerFromStatement";
            }
            else
            {
                return string.Format("/* {0} */", property.PropertyDefinition.TypeName);
            }
        }

        public string GetInsertStatement(bool insertOrReplace = false)
        {
            StringBuilder sbPrefix = new StringBuilder();
            sbPrefix.Append("[NSString stringWithFormat:@");
            sbPrefix.Append('"');
            if (insertOrReplace)
            {
                sbPrefix.Append("Insert or Replace into %@ (%@) Values (");
            }
            else
            {
                sbPrefix.Append("Insert into %@ (%@) Values (");
            }

            int inset = 22;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbSuffix = new StringBuilder();
            foreach (var item in OrderedProperties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                bool nullValue = !insertOrReplace &&
                                    item.PropertyDefinition is SimpleProperty && 
                                    ((SimpleProperty)item.PropertyDefinition).IsKey;
                if (nullValue)
                {
                    sb.Append("Null");
                }
                else if (item.PropertyDefinition.IsTypeNumeric())
                {
                    sb.Append("%ld");
                }
                else if (item.PropertyDefinition.IsTypeBoolean())
                {
                    sb.Append("%d");
                }
                else
                {
                    sb.Append("%@");
                }

                if (!nullValue)
                {
                    if (sbSuffix.Length > 0)
                    {
                        sbSuffix.Append(", ");
                    }
                    sbSuffix.AppendLine();
                    sbSuffix.Append(new string(' ', inset));
                    if (item.PropertyDefinition.IsTypeDateTime())
                    {
                        sbSuffix.Append("[self.dbContext getUnixStringFromDate:");
                    }
                    if (item.PropertyDefinition.IsTypeString())
                    {
                        sbSuffix.Append("[self.dbContext getStringFromText:");
                    }
                    if (item.PropertyDefinition.IsTypeNumeric())
                    {
                        sbSuffix.Append("(long)");
                    }
                    sbSuffix.Append("entity.");
                    sbSuffix.Append(item.ObjectiveCName);
                    if (item.PropertyDefinition.IsTypeDateTime() || item.PropertyDefinition.IsTypeString())
                    {
                        sbSuffix.Append(']');
                    }
                }
            }
            sbPrefix.Append(sb);
            sbPrefix.Append(");");
            sbPrefix.Append('"');
            sbPrefix.Append(',');

            sbPrefix.AppendLine();
            sbPrefix.Append(new string(' ', inset));
            sbPrefix.Append("self.tableName, self.columnNames,");
            sbPrefix.Append(sbSuffix);
            sbPrefix.Append("];");
            return sbPrefix.ToString();
        }

        public string ColumnNames
        {
            get
            {
                if (string.IsNullOrEmpty(_columnNames))
                {
                    _columnNames = GetColumnNames();
                }
                return _columnNames;
            }
        }
        string _columnNames;


        protected string GetColumnNames()
        {
            StringBuilder sb = new StringBuilder();
            if (StorageEntitySetting.PrimaryKeyColumn != null)
            {
                sb.Append(StorageEntitySetting.PrimaryKeyColumn.Name);
            }
            var columns = StorageEntitySetting
                            .GetColumnList()
                            .Where(c => c.Exclude == false & c.PrimaryKey == false);

            foreach (var item in columns)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(item.Name);
            }

            return sb.ToString();
        }
    }
}
