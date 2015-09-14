using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public abstract class BaseObjcClassImpl : AbstractObjcTemplate
    {
        public BaseObjcClassImpl(ObjcEntitySetting entity)
            : base(entity) { }

        public BaseObjcClassImpl(ObjcEngine codeEngine)
            : base(codeEngine) { }

        public BaseObjcHeader HeaderTemplate
        {
            get
            {
                if (_headerTemplate == null)
                {
                    _headerTemplate = GetDefaultHeader();
                }
                return _headerTemplate;
            }
            set { _headerTemplate = value; }
        }
        private BaseObjcHeader _headerTemplate;

        public abstract BaseObjcHeader GetDefaultHeader();


        public List<ObjcPropertySetting> OrderedProperties
        {
            get
            {
                return HeaderTemplate.OrderedProperties;
            }
        }

        public string GetValidationTest(ObjcPropertySetting p)
        {
            if (p.PropertyDefinition.IsTypeNumeric())
            {
                return string.Format("{0} > 0", p.ObjectiveCName);
            }
            else
            {
                return string.Format("{0} != nil", p.ObjectiveCName);
            }
        }

        public List<ObjcPropertySetting> NonNullableProperties
        {
            get
            {
                if (_nonNullableProperties == null)
                {
                    _nonNullableProperties = new List<ObjcPropertySetting>();
                    foreach (var p in OrderedProperties)
                    {
                        if (p.PropertyDefinition is SimpleProperty)
                        {
                            SimpleProperty simple = (SimpleProperty)p.PropertyDefinition;

                            bool nonNullable = simple.IsKey || (simple.IsForeignKey && !simple.IsNullable) ||
                                (!simple.IsTypeBoolean() && !simple.IsClientComputed &&
                                !simple.IsNullable && !simple.IsTypeNumeric() && !p.ReadOnly);

                            if (nonNullable)
                            {
                                _nonNullableProperties.Add(p);
                            }
                        }
                    }
                }
                return _nonNullableProperties;
            }
        }
        List<ObjcPropertySetting> _nonNullableProperties;

        public List<ObjcPropertySetting> ForeignKeyProperties
        {
            get
            {
                return HeaderTemplate.ForeignKeyProperties;
            }
        }
        public List<string> SqliteProperties { get; private set; }


        public override string GetDefaultFileName()
        {
            if (Entity.GenerateCpp)
            {
                return ObjectiveCName + ".mm";
            }
            else
            {
                return ObjectiveCName + ".m";
            }
        }

        protected virtual string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            var headerTmp = GetDefaultHeader();
            string header = headerTmp.GetDefaultFileName();
            string headerPath = Path.Combine(GetDirectoryName(FilePath), header);
            string import = string.Format("\"{0}\"", header);

            if (!Imports.Contains(import))
            {
                Imports.Add(import);
            }

            string implFile = Path.GetFileName(FilePath);

            return base.WriteToFile(Path.Combine(GetDirectoryName(FilePath), implFile), overwriteExisting) &
                headerTmp.WriteToFile(headerPath, overwriteExisting);
        }
    }
}
