using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.Storage;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.AppEngines.Java
{
    public abstract class AbstractJavaEngine : BaseAppEngine
    {
        [XmlIgnore]
        public List<JavaEntitySetting> Entities
        {
            get { return _entities; }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged("Entities");
            }
        }
        private List<JavaEntitySetting> _entities;


        [XmlIgnore]
        public SqliteScriptsEngine SqliteEngine
        {
            get
            {
                if (_sqliteEngine == null && Project != null)
                {
                    var engine = Project.GetAppEngine(EngineTypes.SqliteScriptsId);
                    _sqliteEngine = engine != null ? (SqliteScriptsEngine)engine : null;
                }
                return _sqliteEngine;
            }
        }
        private SqliteScriptsEngine _sqliteEngine;

        public string Package
        {
            get
            {
                if (string.IsNullOrEmpty(_package))
                {
                    _package = "com.donduren." + this.NamespaceName.ToLowerInvariant();
                }
                return _package;
            }
            set
            {
                _package = value;
                RaisePropertyChanged("Package");
            }
        }
        private string _package;


        public string ModelsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_modelsFolder))
                {
                    _modelsFolder = "Models";
                }
                return _modelsFolder;
            }
            set
            {
                _modelsFolder = value;
                RaisePropertyChanged("ModelsFolder");
            }
        }
        private string _modelsFolder;

        public string CustomsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_customsFolder))
                {
                    _customsFolder = "Custom";
                }
                return _customsFolder;
            }
            set
            {
                _customsFolder = value;
                RaisePropertyChanged("CustomsFolder");
            }
        }
        private string _customsFolder;

        public string GeneratedsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_generatedsFolder))
                {
                    _generatedsFolder = "Generated";
                }
                return _generatedsFolder;
            }
            set
            {
                _generatedsFolder = value;
                RaisePropertyChanged("GeneratedsFolder");
            }
        }
        private string _generatedsFolder;



        protected override void OnEntitySettingsChanged()
        {
            var entities = new List<JavaEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (JavaEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            Entities = entities;
        }

        public IEnumerable<JavaEntitySetting> GetIncludedEntities()
        {
            return Entities.Where(e => e.Exclude == false);
        }

        public JavaEntitySetting GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(e => e.Name == name);
        }


        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new JavaEntitySetting()
            {
                DataModel = this.Project.DataModel,
                AppEngine = this,
                //Exclude = entity.Abstract,
                Name = entity.Name
            };

            foreach (var property in entity.Properties)
            {
                if (property.DefinitionType != DefinitionType.Collection)
                {
                    var ps = GetDefaultPropertySetting(property, setting);
                }
            }

            return setting;
        }

        protected virtual JavaPropertySetting GetDefaultPropertySetting(BaseProperty property, JavaEntitySetting setting)
        {
            var ps = new JavaPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting,
                ReadOnly = property.IsClientComputed
            };
            setting.Properties.Add(ps);

            return ps;
        }

        protected override void RefreshSetting(BaseEntitySetting setting, Definitions.EntityDefinition entity, bool cleanRemovedItems)
        {
            var entitySetting = (JavaEntitySetting)setting;
            entitySetting.DataModel = Project.DataModel;
            entitySetting.AppEngine = this;

            List<JavaPropertySetting> remvProp = new List<JavaPropertySetting>();
            foreach (var propSetting in entitySetting.Properties)
            {
                if (entity.Properties.FirstOrDefault(p => p.Name == propSetting.Name) == null)
                {
                    remvProp.Add(propSetting);
                }
            }

            foreach (var propSetting in remvProp)
            {
                entitySetting.Properties.Remove(propSetting);
            }

            foreach (var propSetting in entity.Properties)
            {
                var ps = entitySetting.Properties.FirstOrDefault(p => p.Name == propSetting.Name);
                if (ps == null)// && property.DefinitionType != DefinitionType.Collection
                {
                    ps = GetDefaultPropertySetting(propSetting, entitySetting);
                }
                else //if (ps != null)
                {
                    ps.EntitySetting = setting;
                }
            }
        }

        public override string GetDefaultTargetFolder()
        {
            return "Java";
        }
    }
}
