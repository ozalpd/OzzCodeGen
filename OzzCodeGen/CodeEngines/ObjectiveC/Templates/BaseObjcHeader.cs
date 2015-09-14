using OzzCodeGen.CodeEngines.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public abstract class BaseObjcHeader : AbstractObjcTemplate
    {
        public BaseObjcHeader(ObjcHeaderType headerType, ObjcEngine codeEngine)
            : base(codeEngine)
        {
            if (headerType == ObjcHeaderType.EntityClass)
            {
                throw new Exception("Class headers need entity setting");
            }
            HeaderType = headerType;
        }

        public BaseObjcHeader(ObjcHeaderType headerType, ObjcEntitySetting entity)
            : base(entity)
        {
            HeaderType = headerType;
        }

        public ObjcHeaderType HeaderType { get; private set; }

        public string BaseClassName
        {
            set { _baseClassName = value; }
            get
            {
                if (string.IsNullOrEmpty(_baseClassName))
                {
                    _baseClassName = GetBaseClassName();
                }
                return _baseClassName;
            }
        }
        string _baseClassName;

        private string GetBaseClassName()
        {
            if (Entity != null)
            {
                return Entity.GetObjcBaseClassName();
            }
            else
            {
                return "NSObject";
            }
        }


        public List<string> SqliteProperties { get; private set; }
        public List<ObjcPropertySetting> OrderedProperties { get; private set; }
        public List<ObjcPropertySetting> ForeignKeyProperties { get; private set; }

        public void SetSqliteProperties(string[] columnNames)
        {
            SqliteProperties = new List<string>();
            OrderedProperties = new List<ObjcPropertySetting>();
            ForeignKeyProperties = new List<ObjcPropertySetting>();

            foreach (var col in columnNames)
            {
                var property = Entity.GetPropertyByName(col);
                if (property != null && !property.Exclude)
                {
                    string s = property != null ?
                        property.ObjectiveCName :
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


        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();
            switch (HeaderType)
            {
                case ObjcHeaderType.Enums:
                    imports.Add("<Foundation/Foundation.h>");
                    break;

                case ObjcHeaderType.Constants:
                    //imports.Add(string.Format("\"{0}\"", CodeEngine.EnumsFile));
                    break;

                case ObjcHeaderType.EntityClass:
                    imports.AddRange(Entity.GetHeaderImports());
                    break;

                default:
                    break;
            }

            return imports;
        }

        public override string GetDefaultFileName()
        {
            switch (HeaderType)
            {
                case ObjcHeaderType.Enums:
                    return CodeEngine.EnumsFile;

                case ObjcHeaderType.Constants:
                    return CodeEngine.ConstFile;

                default:
                    return ObjectiveCName + ".h";
            }
        }
    }
}
