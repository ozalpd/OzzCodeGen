using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.AppEngines.Storage.Templates;
using OzzCodeGen.AppEngines.Storage.UI;
using OzzCodeGen.Definitions;
using OzzUtils;

namespace OzzCodeGen.AppEngines.Storage
{
    public abstract class StorageScriptsEngine : BaseAppEngine
    {
        public bool PluralizeTableNames
        {
            get { return _pluralize; }
            set
            {
                _pluralize = value;
                RaisePropertyChanged("PluralizeTableNames");
            }
        }
        private bool _pluralize = true;


        public string DatabaseName
        {
            get
            {
                if (string.IsNullOrEmpty(_databaseName))
                    _databaseName = GetDefaultDbName();
                return _databaseName;
            }
            set
            {
                _databaseName = value;
                RaisePropertyChanged("DatabaseName");
            }
        }
        private string _databaseName;

        public string DatabaseServer
        {
            get
            {
                if (string.IsNullOrEmpty(_databaseServer))
                    _databaseServer = ".\\SqlExpress";
                return _databaseServer;
            }
            set
            {
                if (_databaseServer == value)
                    return;
                RaisePropertyChanged("DatabaseServer");
                _databaseServer = value;
            }
        }
        private string _databaseServer;

        /// <summary>
        /// Script files will be executed before creating database
        /// File names must be seperated by a semicolon
        /// </summary>
        public string BeforeScripts
        {
            set
            {
                _beforeScripts = value;
                RaisePropertyChanged("BeforeScripts");
            }
            get { return _beforeScripts; }
        }
        private string _beforeScripts;

        /// <summary>
        /// Script files will be executed after creating database
        /// File names must be seperated by a semicolon
        /// </summary>
        public string AfterScripts
        {
            set
            {
                _lastScripts = value;
                RaisePropertyChanged("AfterScripts");
            }
            get { return _lastScripts; }
        }
        private string _lastScripts;

        [XmlIgnore]
        public List<StorageEntitySetting> Entities
        {
            get { return _entitySettings; }
            set
            {
                if (_entitySettings == value) return;
                _entitySettings = value;
                RaisePropertyChanged("Entities");
            }
        }
        private List<StorageEntitySetting> _entitySettings;


        protected virtual string GetDefaultDbName()
        {
            if (Project == null)
                return string.Empty;

            string s;
            if (Project.Name.EndsWith("DAL"))
            {
                s = Project.Name.Substring(0, Project.Name.Length - 3);
            }
            else
            {
                s = Project.Name;
            }
            return s + "DB";
        }

        [XmlIgnore]
        public List<string> AdditionalCommands
        {
            get
            {
                if (_additionalCommands == null)
                    _additionalCommands = new List<string>();
                return _additionalCommands;
            }
            set { _additionalCommands = value; }
        }
        private List<string> _additionalCommands;


        protected override void OnEntitySettingsChanged()
        {
            var entities = new List<StorageEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (StorageEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            Entities = entities;
        }

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new StorageEntitySetting()
            {
                Name = entity.Name,
                DataModel = this.Project.DataModel,
                InheritedEntity = entity.BaseTypeName,
                AppEngine = this,
                Exclude = entity.Abstract
            };

            var propList = entity.Properties;
            foreach (var property in propList)
            {
                var ps = GetDefaultPropertySetting(property, setting);
            }
            return setting;
        }

        protected StorageColumnSetting GetDefaultPropertySetting(BaseProperty property, StorageEntitySetting setting)
        {
            if (property.DefinitionType == DefinitionType.Collection ||
                property.IsClientComputed)
            {
                return null;
            }

            var ps = GetModifyTrackColumns().FirstOrDefault(c => c.Name == property.Name);

            ComplexProperty complex = null;
            SimpleProperty simple = null;
            if (ps == null)
            {
                ps = new StorageColumnSetting();
                if (property is SimpleProperty)
                {
                    ps.Name = property.Name;
                    simple = (SimpleProperty)property;
                    complex = simple.GetDependent();
                }
                else if (property is ComplexProperty)
                {
                    complex = (ComplexProperty)property;
                    simple = complex.GetDependency();
                    if (simple != null)
                    {
                        ps.Name = simple.Name;
                        ps.DataType = simple.TypeName;
                    }
                    else
                    {
                        ps.Name = complex.DependentPropertyName;
                        ps.DataType = complex.DependentPropertyType;
                    }
                }

                if (simple != null)
                {
                    bool triggerNullable = ps.Nullable;//Force to set default value
                    ps.Nullable = simple.IsNullable;
                }

                if (complex != null)
                {
                    ps.ForeignKeyTable = GetTableName(complex.TypeName);
                }
            }

            ps.EntitySetting = setting;
            SetColumnType(ps);
            if (!setting.Properties.Where(p => p.Name.Equals(ps.Name)).Any())
            {
                setting.Properties.Add(ps);
            }

            return ps;
        }

        protected string GetTableName(string typeName)
        {
            StorageEntitySetting table = Entities == null ? null :
                Entities.Where(e => typeName.Equals(e.Name)).FirstOrDefault();

            if (table != null)
            {
                return table.TableName;
            }
            else if (PluralizeTableNames)
            {
                return typeName.Pluralize();
            }
            else
            {
                return typeName;
            }
        }

        protected virtual void SetColumnType(StorageColumnSetting column)
        {
            column.DataType = column.PropertyDefinition.TypeName;

            //TODO: Find dependent property
            if (column.PropertyDefinition.DefinitionType == DefinitionType.Complex &&
                column.DataType != "ICollection")
            {
                column.Name += "Id";
                column.ForeignKeyTable = GetTableName(column.PropertyDefinition.TypeName);
                column.DataType = "int"; // We assume Foreign table's PrimaryKey is int
            }
            if (column.DataType == "ICollection") column.Exclude = true;
            FixColumnType(column);
        }

        protected abstract void FixColumnType(StorageColumnSetting column);

        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var entitySetting = (StorageEntitySetting)setting;
            entitySetting.DataModel = Project.DataModel;

            List<StorageColumnSetting> remvProp = new List<StorageColumnSetting>();
            foreach (var thisProp in entitySetting.Properties)
            {
                if (entity.Properties.FirstOrDefault(p => p.Name == thisProp.Name) == null)
                {
                    remvProp.Add(thisProp);
                }
            }

            foreach (var dalProp in remvProp)
            {
                entitySetting.Properties.Remove(dalProp);
            }

            foreach (var property in entity.Properties)
            {
                StorageColumnSetting ps = entitySetting.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (ps == null)
                {
                    ps = GetDefaultPropertySetting(property, entitySetting);
                }
                else
                {
                    ps.EntitySetting = setting;
                }
            }

            entitySetting.Properties = entitySetting
                .Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder)
                .ToList();
        }

        protected override UserControl GetUiControl()
        {
            if (_uiControl == null)
            {
                _uiControl = new StorageEngineUI();
                _uiControl.AppEngine = this;
            }
            return _uiControl;
        }
        StorageEngineUI _uiControl = null;

        protected abstract AbstractStorageTemplate GetCreateTableTemplate(StorageEntitySetting tableDefinition);
        protected abstract AbstractStorageTemplate GetCreateDbTemplate();
        protected abstract AbstractStorageTemplate GetDropDbTemplate();

        public abstract string GetColumnType(StorageColumnSetting column);
        public abstract string GetColumnDeclaration(StorageColumnSetting column, StorageEntitySetting table);
        public abstract string GetPrimaryKeyDeclaration(StorageEntitySetting table);
        public abstract StorageColumnSetting GetDefaultPrimaryKey();
        public abstract List<StorageColumnSetting> GetModifyTrackColumns();

        public StorageEntitySetting GetForeignTable(StorageColumnSetting column)
        {
            if (string.IsNullOrEmpty(column.ForeignKeyTable))
                return null;

            return Entities.FirstOrDefault(e => e.TableName.Equals(column.ForeignKeyTable));
        }

        public List<StorageEntitySetting> GetCreateTableList()
        {
            if (_createTableList != null)
                return _createTableList;

            _createTableList = new List<StorageEntitySetting>();
            foreach (var item in Entities.Where(e => !e.Exclude))
            {
                var tables = item.ForeignTables;
            }
            foreach (var item in Entities.Where(e => !e.Exclude))
            {
                AddToCreateTableList(item);
            }
            return _createTableList;
        }
        List<StorageEntitySetting> _createTableList;

        private void AddToCreateTableList(StorageEntitySetting table)
        {
            if (_createTableList.Contains(table))
                return;

            foreach (var item in table.ForeignTables)
            {
                if (!item.ForeignTables.Contains(table))
                    AddToCreateTableList(item);
            }
            _createTableList.Add(table);
        }

        public void RenderCreateDbTemplate()
        {
            var createTemplate = GetCreateDbTemplate();
            if (createTemplate != null)
            {
                string file = Path.Combine(TargetDirectory, createTemplate.GetDefaultFileName());
                createTemplate.WriteToFile(file, true);
            }

            var dropTemplate = GetDropDbTemplate();
            if (dropTemplate != null)
            {
                string file = Path.Combine(TargetDirectory, dropTemplate.GetDefaultFileName());
                dropTemplate.WriteToFile(file, true);
            }

            var additnTemplate = new Additionals(this);
            if (additnTemplate != null)
            {
                string file = Path.Combine(TargetDirectory, additnTemplate.GetDefaultFileName());
                additnTemplate.WriteToFile(file, true);
            }
        }

        protected bool RenderCreateTableTemplate(StorageEntitySetting entity)
        {
            var template = GetCreateTableTemplate(entity);

            string file = Path.Combine(TargetDirectory, template.GetDefaultFileName());
            return template.WriteToFile(file, OverwriteExisting || entity.OverwriteExisting);
        }

        public override bool RenderSelectedTemplate()
        {
            if (string.IsNullOrEmpty(TargetDirectory))
                return false;

            if (RenderAllEntities)
            {
                bool allWritten = true;
                foreach (StorageEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
                {
                    allWritten = RenderCreateTableTemplate(setting) & allWritten;
                }
                RenderCreateDbTemplate();
                return allWritten;
            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                return RenderCreateTableTemplate((StorageEntitySetting)CurrentEntitySetting);
            }
        }

        //Not used in this engine
        public override bool RenderAllTemplates()
        {
            throw new NotImplementedException();
        }

        public override UserControl GetSettingsDlgUI()
        {
            return new StorageEngineSettingsUI();
        }
    }
}
