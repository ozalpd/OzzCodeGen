using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzUtils;

namespace OzzCodeGen.AppEngines.Storage
{
    public class StorageEntitySetting : BaseEntitySetting
    {

        public string InheritedEntity
        {
            get { return _inheritEntityName; }
            set
            {
                if (_inheritEntityName == value) return;
                _inheritEntityName = value;
                RaisePropertyChanged("InheritedEntity");
            }
        }
        private string _inheritEntityName;


        public bool UseInheritance
        {
            get { return _tableInheritance; }
            set
            {
                _tableInheritance = value & !string.IsNullOrEmpty(InheritedEntity);
                RaisePropertyChanged("UseInheritance");
            }
        }
        private bool _tableInheritance;
        


        [XmlIgnore]
        public StorageColumnSetting PrimaryKeyColumn
        {
            get
            {
                if (_primaryKey == null)
                {
                    _primaryKey = Properties.FirstOrDefault(p => p.PrimaryKey);
                    if (_primaryKey == null)
                    {
                        var baseTable = GetBaseTable();
                        if (baseTable != null)
                            _primaryKey = baseTable.PrimaryKeyColumn;
                    }
                    if (_primaryKey != null)
                        RaisePropertyChanged("PrimaryKeyColumn");
                }
                return _primaryKey;
            }
            set
            {
                if (_primaryKey == value) return;
                RaisePropertyChanged("PrimaryKeyColumn");
                _primaryKey = value;
            }
        }
        private StorageColumnSetting _primaryKey;


        public string SchemaName
        {
            get
            {
                if (string.IsNullOrEmpty(_schemaName))
                    _schemaName = "dbo";
                return _schemaName;
            }
            set
            {
                if (_schemaName == value) return;
                _schemaName = value;
                RaisePropertyChanged("SchemaName");
            }
        }
        private string _schemaName;


        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName))
                {
                    if(AppEngine.PluralizeTableNames)
                    {
                        _tableName = Name.Pluralize();
                    }
                    else
                    {
                        _tableName = Name;
                    }
                }
                return _tableName;
            }
            set
            {
                OnTableNameChanging(value);
                _tableName = value;
                RaisePropertyChanged("TableName");
            }
        }
        private string _tableName;
        private void OnTableNameChanging(string newName)
        {
            if (AppEngine == null)
                return;
            foreach (var entity in this.AppEngine.Entities)
            {
                if (entity.Properties != null)
                {
                    foreach (var p in entity.Properties)
                    {
                        if (!string.IsNullOrEmpty(p.ForeignKeyTable) &&
                            p.ForeignKeyTable.Equals(TableName))
                        {
                            p.ForeignKeyTable = newName;
                        }
                    }
                }
            }
        }


        public string FinishingScript
        {
            get { return _finishingScript; }
            set
            {
                _finishingScript = value;
                RaisePropertyChanged("FinishingScript");
            }
        }
        private string _finishingScript;
        
        public bool ModifyTrack
        {
            get
            {
                if (!_modifyTrack.HasValue)
                {
                    _modifyTrack = Name == "EntityUpdate";
                }
                return _modifyTrack.Value;
            }
            set
            {
                _modifyTrack = value;
                RaisePropertyChanged("ModifyTrack");
            }
        }
        private bool? _modifyTrack;


        public bool StoredProcs
        {
            get
            {
                return _storedProcs;
            }
            set
            {
                _storedProcs = value;
                RaisePropertyChanged("StoredProcs");
            }
        }
        private bool _storedProcs;
        public bool CustomStoredProcs
        {
            get
            {
                return _customStoredProcs;
            }
            set
            {
                _customStoredProcs = value;
                RaisePropertyChanged("CustomStoredProcs");
            }
        }
        private bool _customStoredProcs;

        public List<StorageColumnSetting> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<StorageColumnSetting>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<StorageColumnSetting> _properties;


        [XmlIgnore]
        public StorageScriptsEngine AppEngine { get; set; }

        [XmlIgnore]
        public List<StorageEntitySetting> ForeignTables
        {
            get
            {
                if (_foreignTables == null)
                {
                    _foreignTables = new List<StorageEntitySetting>();

                    var baseTable = GetBaseTable();
                    if (baseTable != null && !_foreignTables.Contains(baseTable))
                    {
                        _foreignTables.Add(baseTable);
                    }

                    foreach (var item in GetColumnList()
                                        .Where(c => c.Exclude == false & !string.IsNullOrEmpty(c.ForeignKeyTable)))
                    {
                        var foreignTable = AppEngine
                                            .Entities
                                            .FirstOrDefault(e => e.TableName.Equals(item.ForeignKeyTable));
                        if (foreignTable != null && !_foreignTables.Contains(foreignTable) && foreignTable != this)
                        {
                            _foreignTables.Add(foreignTable);
                        }
                    }
                }
                return _foreignTables;
            }
        }
        List<StorageEntitySetting> _foreignTables;

        public StorageEntitySetting GetFirstBase()
        {
            if (!UseInheritance)
            {
                return this;
            }
            var baseTable = GetBaseTable();
            if (baseTable == null)
            {
                return this;
            }
            else
            {
                return baseTable.GetFirstBase();
            }
        }

        public StorageEntitySetting GetBaseTable()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return AppEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }

        public string GetPrimaryKeyDeclaration()
        {
            return AppEngine.GetPrimaryKeyDeclaration(this);
        }

        public string GetColumnDeclaration(StorageColumnSetting column)
        {
            return AppEngine.GetColumnDeclaration(column, this);
        }

        public List<StorageColumnSetting> GetColumnList()
        {
            var columns = new List<StorageColumnSetting>();
            var baseTable = GetBaseTable();
            if (baseTable != null)
            {
                var baseColumns = baseTable.GetColumnList();
                foreach (var item in baseColumns)
                {
                    columns.Add(item);
                }
            }

            foreach (var item in Properties)
            {
                columns.Add(item);
            }

            try
            {
                return columns.OrderBy(c => c.PropertyDefinition.DisplayOrder).ToList();
            }
            catch (Exception)
            {
                return columns;
            }
        }

        public StorageColumnSetting GetDeleteMarkColumn()
        {
            return GetColumnList()
                    .FirstOrDefault(c =>
                        c.DataType.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &
                        (c.Name == "IsDeleted" | c.Name == "Deleted"));
        }
    }
}
