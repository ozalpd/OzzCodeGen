using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.Storage;

namespace OzzCodeGen.AppEngines.Java
{
    public class JavaEntitySetting : BaseEntitySetting
    {
        public List<JavaPropertySetting> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<JavaPropertySetting>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<JavaPropertySetting> _properties;

        public bool AutoSaveToLocal
        {
            get { return _autoSaveToLocal; }
            set
            {
                _autoSaveToLocal = value;
            }
        }
        private bool _autoSaveToLocal;


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


        [XmlIgnore]
        public AbstractJavaEngine AppEngine { get; set; }

        public JavaEntitySetting GetBaseEntity()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return GetEntityByTypeName(EntityDefinition.BaseTypeName);
        }

        public JavaEntitySetting GetEntityByTypeName(string entityName)
        {
            return AppEngine
                    .Entities
                    .FirstOrDefault(e => e.Name == entityName);
        }

        public JavaPropertySetting GetPropertyByName(string propertyName)
        {
            var property = Properties
                           .FirstOrDefault(p => p.Exclude == false &
                               p.Name == propertyName.Trim());
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

        public List<JavaPropertySetting> GetStorageProperties()
        {
            if (_storageProperties == null)
            {
                _storageProperties = new List<JavaPropertySetting>();
                var table = GetStorageEntitySetting();
                var columns = table.GetColumnList();
                foreach (var item in columns.Where(p => p.Exclude == false))
                {
                    var property = GetPropertyByName(item.Name);
                    _storageProperties.Add(property);
                }
            }
            return _storageProperties;
        }
        List<JavaPropertySetting> _storageProperties;

        public JavaPropertySetting GetPrimaryKey()
        {
            var table = GetStorageEntitySetting();
            return GetPropertyByName(table.PrimaryKeyColumn.Name);
        }
    }
}
