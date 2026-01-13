using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.CodeEngines.Android;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.Java.Templates
{
    public abstract partial class AbstractJavaTemplate : AbstractTemplate
    {
        public AbstractJavaTemplate(AbstractJavaEngine codeEngine)
        {
            _codeEngine = codeEngine;
            Encoding = new UTF8Encoding(true);
        }

        public AbstractJavaTemplate(JavaEntitySetting entity)
        {
            Entity = entity;
            Encoding = new UTF8Encoding(true);
        }

        public AbstractJavaEngine CodeEngine
        {
            get
            {
                if (_codeEngine == null & Entity != null)
                {
                    _codeEngine = Entity.CodeEngine;
                }
                return _codeEngine;
            }
        }
        AbstractJavaEngine _codeEngine;

        public JavaEntitySetting Entity { get; private set; }
        public string ClassName { set; get; }

        public override string GetDefaultFileName()
        {
            return ClassName + ".java";
        }


        public List<string> SqliteProperties { get; private set; }
        public List<JavaPropertySetting> OrderedProperties { get; private set; }
        public List<JavaPropertySetting> ForeignKeyProperties { get; private set; }

        public void SetSqliteProperties(string[] columnNames)
        {
            SqliteProperties = new List<string>();
            OrderedProperties = new List<JavaPropertySetting>();
            ForeignKeyProperties = new List<JavaPropertySetting>();

            foreach (var col in columnNames)
            {
                var property = Entity.GetPropertyByName(col);
                if (property != null && !property.Exclude)
                {
                    string s = property != null ?
                        property.JavaName :
                        string.Format("/* ERROR! {0} not found! */", col);
                    SqliteProperties.Add(s);
                    OrderedProperties.Add(property);
                    if (property.PropertyDefinition is SimpleProperty &&
                        ((SimpleProperty)property.PropertyDefinition).IsForeignKey)
                    {
                        ForeignKeyProperties.Add(property);
                    }
                }
            }
        }

        public List<StorageEntitySetting> StorageEntitySettings
        {
            get
            {
                if (CodeEngine.SqliteEngine == null)
                {
                    return null;
                }
                if (_storageEntitySettings == null)
                {
                    _storageEntitySettings = CodeEngine
                                            .SqliteEngine
                                            .Entities
                                            .Where(e => e.Exclude == false)
                                            .ToList();
                }
                return _storageEntitySettings;
            }
        }
        List<StorageEntitySetting> _storageEntitySettings;

        public string SubFolder { get; set; }

        public virtual string GetPackage()
        {
            if (string.IsNullOrEmpty(SubFolder))
            {
                return CodeEngine.Package;
            }
            else
            {
                return CodeEngine.Package + "." + SubFolder;
            }
        }

        public List<string> Imports
        {
            get
            {
                if (_imports == null)
                    _imports = GetDefaultImports();
                return _imports;
            }
            set { _imports = value; }
        }
        private List<string> _imports;

        protected virtual List<string> GetDefaultImports()
        {
            var imports = new List<string>();
            if (Entity != null)
            {
                if (Entity.Properties.Where(p => p.JavaType == "Date" & p.Exclude == false).Any())
                {
                    imports.Add("java.util.Date");
                }

                if (Entity.Properties.Where(p => p.JavaType.StartsWith("ArrayList") & p.Exclude == false).Any())
                {
                    imports.Add("java.util.ArrayList");
                }
            }
            return imports;
        }
    }
}
