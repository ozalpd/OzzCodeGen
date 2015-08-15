using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.Storage;

namespace OzzCodeGen.AppEngines.ObjectiveC
{
    public class ObjcEntitySetting : BaseEntitySetting
    {
        public List<ObjcPropertySetting> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<ObjcPropertySetting>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<ObjcPropertySetting> _properties;


        [XmlIgnore]
        public List<ObjcPropertySetting> PropertiesIncluded
        {
            get
            {
                if (_propertiesIncluded == null)
                {
                    _propertiesIncluded = Properties
                                            .Where(p => p.Exclude == false)
                                            .ToList();
                }
                return _propertiesIncluded;
            }
            set
            {
                if (_propertiesIncluded == value) return;
                _propertiesIncluded = value;
                RaisePropertyChanged("PropertiesIncluded");
            }
        }
        protected List<ObjcPropertySetting> _propertiesIncluded;


        [XmlIgnore]
        public ObjcEngine AppEngine { get; set; }

        public override bool Exclude
        {
            get { return _exclude; }
            set
            {
                _exclude = value;
                RaisePropertyChanged("Exclude");
            }
        }
        bool _exclude;

        public string ObjectiveCName
        {
            get
            {
                if (string.IsNullOrEmpty(_objcName))
                    _objcName = AppEngine.ClassPrefix + Name;
                return _objcName;
            }
            set
            {
                _objcName = value;
                RaisePropertyChanged("ObjectiveCName");
            }
        }
        private string _objcName;

        public bool AutoSaveToLocal
        {
            get { return _autoSaveToLocal; }
            set
            {
                _autoSaveToLocal = value;
            }
        }
        private bool _autoSaveToLocal;

        public ObjcEntitySetting GetBaseEntity()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return GetEntityByTypeName(EntityDefinition.BaseTypeName);
        }

        public ObjcEntitySetting GetEntityByTypeName(string entityName)
        {
            return AppEngine
                    .Entities
                    .FirstOrDefault(e => e.Name == entityName);
        }

        public ObjcPropertySetting GetPropertyByName(string propertyName)
        {
            var property = PropertiesIncluded.FirstOrDefault(p => p.Name == propertyName.Trim());
            if (property == null)
            {
                var entity = GetBaseEntity();
                if (entity == null)
                {
                    return null;
                }
                else
                {
                    return entity.GetPropertyByName(propertyName);
                }
            }
            else
            {
                return property;
            }
        }

        public StorageEntitySetting GetStorageEntitySetting()
        {
            if (AppEngine.SqliteEngine == null)
            {
                return null;
            }
            else if (_storageEntitySetting == null)
            {
                _storageEntitySetting = AppEngine
                                        .SqliteEngine
                                        .Entities
                                        .FirstOrDefault(e => e.Name == Name);
            }
            return _storageEntitySetting;
        }
        StorageEntitySetting _storageEntitySetting;


        public List<string> GetHeaderImports()
        {
            var imports = new List<string>();
            ObjcEntitySetting baseEntity = GetBaseEntity();
            List<string> baseImports;
            if (baseEntity != null)
            {
                var s = EntityDefinition.BaseTypeName;
                imports.Add(string.Format("\"{0}.h\"", baseEntity.ObjectiveCName));
                baseImports = baseEntity.GetHeaderImports();
            }
            else if (AppEngine.RenderConst)
            {
                imports.Add(string.Format("\"{0}\"", AppEngine.ConstFile));
                baseImports = new List<string>();
            }
            else
            {
                imports.Add(string.Format("\"{0}\"", AppEngine.EnumsFile));
                baseImports = new List<string>();
            }

            foreach (var item in PropertiesIncluded)
            {
                var propEntity = item.GetEntity();
                if (propEntity != null && propEntity.Name != this.EntityDefinition.Name)
                {
                    string s = string.Format("\"{0}.h\"", propEntity.ObjectiveCName);
                    if (!imports.Contains(s) && !baseImports.Contains(s))
                    {
                        imports.Add(s);
                    }
                }
            }
            return imports;
        }

        public string GetObjcBaseClassName()
        {
            if (string.IsNullOrEmpty(_objcBaseClassName))
            {
                var baseEntity = GetBaseEntity();
                if (baseEntity == null)
                {
                    _objcBaseClassName = "NSObject";
                }
                else
                {
                    _objcBaseClassName = baseEntity.ObjectiveCName;
                }
            }
            return _objcBaseClassName;
        }
        string _objcBaseClassName;

        public bool GenerateCpp
        {
            get { return _cpp; }
            set
            {
                _cpp = value;
                RaisePropertyChanged("GenerateCpp");
            }
        }
        private bool _cpp;
        
    }
}
