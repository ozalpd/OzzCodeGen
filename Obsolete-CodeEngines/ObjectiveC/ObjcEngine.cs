using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.ObjectiveC.Templates;
using OzzCodeGen.CodeEngines.ObjectiveC.UI;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.ObjectiveC
{
    public enum ObjExtendMethod
    {
        Category,
        SubClass
    }

    [XmlInclude(typeof(ObjcEntitySetting))]
    public class ObjcEngine : BaseCodeEngine
    {
        public override string EngineId
        {
            get { return EngineTypes.ObjcEngineId; }
        }

        public override string ProjectTypeName
        {
            get { return "Objective-C Code Generation Engine"; }
        }


        public string ClassPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_classPrefix))
                    _classPrefix = "Ozz";
                return _classPrefix;
            }
            set
            {
                _classPrefix = value;
                RaisePropertyChanged("ClassPrefix");
            }
        }
        private string _classPrefix;


        public string EnumsFile
        {
            get
            {
                if (string.IsNullOrEmpty(_enumsFile))
                    _enumsFile = string.Format("Enums{0}.h", GetRootNamespace());
                return _enumsFile;
            }
            set
            {
                if (value.EndsWith(".h"))
                {
                    _enumsFile = value;
                }
                else
                {
                    _enumsFile = value + ".h";
                }
                RaisePropertyChanged("EnumsFile");
            }
        }
        private string _enumsFile;


        public string ConstFile
        {
            get
            {
                if (string.IsNullOrEmpty(_constFile))
                    _constFile = string.Format("Const{0}.h", GetRootNamespace());
                return _constFile;
            }
            set
            {
                if (value.EndsWith(".h"))
                {
                    _constFile = value;
                }
                else
                {
                    _constFile = value + ".h";
                }
                RaisePropertyChanged("ConstFile");
            }
        }
        private string _constFile;


        public string BaseEntityContext
        {
            get
            {
                if (string.IsNullOrEmpty(_baseEntityContext))
                {
                    _baseEntityContext = "AbstractEntityContext";
                }
                return _baseEntityContext;
            }
            set
            {
                _baseEntityContext = value;
                RaisePropertyChanged("BaseEntityContext");
            }
        }
        private string _baseEntityContext;

        public bool RenderConst
        {
            get { return _renderConst; }
            set
            {
                _renderConst = value;
                RaisePropertyChanged("RenderConst");
            }
        }
        private bool _renderConst;


        [XmlIgnore]
        public string ServerUrl
        {
            get
            {
                return Project.ServiceUrl;
            }
        }

        public string SqliteHelper
        {
            get
            {
                if (string.IsNullOrEmpty(_sqliteHelper))
                {
                    _sqliteHelper = "OzzSqliteHelper";
                }
                return _sqliteHelper;
            }
            set
            {
                _sqliteHelper = value;
                RaisePropertyChanged("SqliteHelper");
            }
        }
        private string _sqliteHelper;
        

        [XmlIgnore]
        public static string DefaultFileName { get { return "ObjectiveC_Engine.settings"; } }
        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        protected override void OnSearchStringChanged()
        {
            RaisePropertyChanged("Entities");
        }

        [XmlIgnore]
        public List<ObjcEntitySetting> Entities
        {
            get
            {
                if (Project == null || string.IsNullOrEmpty(Project.SearchString))
                {
                    return _entities;
                }
                else
                {
                    var result = _entities
                        .Where(e => e.Name.StartsWith(Project.SearchString, System.StringComparison.InvariantCultureIgnoreCase) ||
                            e.Properties.Where(p => p.Name.StartsWith(Project.SearchString, System.StringComparison.InvariantCultureIgnoreCase)).Any());
                    return result.ToList();
                }
            }
            set
            {
                if (_entities == value) return;
                _entities = value;
                RaisePropertyChanged("Entities");
            }
        }
        private List<ObjcEntitySetting> _entities;

        public IEnumerable<ObjcEntitySetting> GetIncludedEntities()
        {
            return Entities.Where(e => e.Exclude == false);
        }

        public ObjcEntitySetting GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(e => e.Name == name);
        }


        [XmlIgnore]
        public SqliteScriptsEngine SqliteEngine
        {
            get
            {
                if (_sqliteEngine == null && Project != null)
                {
                    var engine = Project.GetCodeEngine(EngineTypes.SqliteScriptsId);
                    _sqliteEngine = engine != null ? (SqliteScriptsEngine)engine : null;
                }
                return _sqliteEngine;
            }
        }
        private SqliteScriptsEngine _sqliteEngine;

        public string GetModelsDir()
        {
            return Path.Combine(TargetDirectory, "Models");
        }

        public string GetContextDir()
        {
            return Path.Combine(TargetDirectory, "Context");
        }


        [XmlIgnore]
        public bool RenderDataContext
        {
            get
            {
                if (!_renderSqliteParsers.HasValue)
                {
                    _renderSqliteParsers = SqliteEngine != null;
                }
                return _renderSqliteParsers.Value;
            }
            set
            {
                if (value && SqliteEngine == null)
                {
                    MessageBox.Show(string.Format(
                        "Parser generator needs additional engine to render parsers!\r\nThis could be {0}",
                        EngineTypes.SqliteScriptsId),
                        "Project does not have necessary engines!");
                }
                _renderSqliteParsers = value && SqliteEngine != null;
                RaisePropertyChanged("RenderDataContext");
            }
        }
        private bool? _renderSqliteParsers;
        

        protected override void OnEntitySettingsChanged()
        {
            var entities = new List<ObjcEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (ObjcEntitySetting item in EntitySettings)
                {
                    item.CodeEngine = this;
                    entities.Add(item);
                }
            }
            Entities = entities;
        }


        protected string GetRootNamespace()
        {
            if (string.IsNullOrEmpty(NamespaceName))
            {
                return string.Empty;
            }
            else
            {
                string[] s = NamespaceName.Split('.');
                return s[0];
            }
        }

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new ObjcEntitySetting()
            {
                Name = entity.Name,
                DataModel = this.Project.DataModel,
                CodeEngine = this
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

        protected ObjcPropertySetting GetDefaultPropertySetting(BaseProperty property, ObjcEntitySetting setting)
        {
            var ps = new ObjcPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting,
                ReadOnly = property.IsClientComputed
            };
            setting.Properties.Add(ps);

            return ps;
        }

        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var entitySetting = (ObjcEntitySetting)setting;
            entitySetting.DataModel = Project.DataModel;
            entitySetting.CodeEngine = this;

            List<ObjcPropertySetting> remvProp = new List<ObjcPropertySetting>();
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
                ObjcPropertySetting ps = entitySetting.Properties.FirstOrDefault(p => p.Name == propSetting.Name);
                if (ps == null)// && property.DefinitionType != DefinitionType.Collection
                {
                    ps = GetDefaultPropertySetting(propSetting, entitySetting);
                }
                else //if (ps != null)
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
                _uiControl = new ObjcEngineUI();
                _uiControl.CodeEngine = this;
            }
            return _uiControl;
        }
        ObjcEngineUI _uiControl = null;

        public ObjcHeader GetConstHeader()
        {
            var constHeader = new ObjcHeader(ObjcHeaderType.Constants, this);
            constHeader.Imports.Add(string.Format("\"{0}\"", EnumsFile));
            return constHeader;
        }

        public ObjcHeader GetEnumHeader()
        {
            return new ObjcHeader(ObjcHeaderType.Enums, this);
        }



        [XmlIgnore]
        public ObjcDbContext DbContextTemplate
        {
            get
            {
                if (_dbContextTemplate == null)
                {
                    _dbContextTemplate = new ObjcDbContext(this);
                }
                return _dbContextTemplate;
            }
        }
        private ObjcDbContext _dbContextTemplate;
        

        [XmlIgnore]
        public List<string> PropertyNames
        {
            get
            {
                if (_propertyNames == null)
                {
                    _propertyNames = new List<string>();
                    foreach (var entity in GetIncludedEntities())
                    {
                        _propertyNames.Add(string.Format("// {0}", entity.Name));
                        foreach (var item in entity.Properties)
                        {
                            if (!_propertyNames.Contains(item.Name))
                            {
                                _propertyNames.Add(item.Name);
                            }
                        }
                    }
                }
                return _propertyNames;
            }
        }
        private List<string> _propertyNames;
        

        public void RenderConstHeader()
        {
            var constHeader = GetConstHeader();
            string fileName = Path.Combine(TargetDirectory, constHeader.GetDefaultFileName());
            constHeader.WriteToFile(fileName, OverwriteExisting);
        }

        public void RenderEnumHeader()
        {
            var enumHeader = GetEnumHeader();
            string fileName = Path.Combine(TargetDirectory, enumHeader.GetDefaultFileName());
            enumHeader.WriteToFile(fileName, OverwriteExisting);
        }

        public void RenderDbContext()
        {
            string fileName = Path.Combine(GetContextDir(), DbContextTemplate.GetDefaultFileName());
            DbContextTemplate.WriteToFile(fileName, OverwriteExisting);
        }

        protected bool RenderEntity(ObjcEntitySetting entity)
        {
            var classTmp = GetTemplateFile(entity, SelectedTemplate);
            string fileName = Path.Combine(GetModelsDir(), classTmp.GetDefaultFileName());

            bool parserRendered = true;
            if (!entity.EntityDefinition.Abstract && RenderDataContext)
            {
                var sqliteParser = new ObjcEntityContext(entity);
                string sqliteParserFile = Path.Combine(GetContextDir(), sqliteParser.GetDefaultFileName());
                parserRendered = sqliteParser.WriteToFile(sqliteParserFile, OverwriteExisting);
            }
            return classTmp.WriteToFile(fileName, OverwriteExisting) & parserRendered;
        }

        public override bool RenderSelectedTemplate()
        {
            if (!string.IsNullOrEmpty(Project.SearchString))
            {
                Project.SearchString = string.Empty;
            }

            bool result = true;
            if (RenderAllEntities)
            {
                foreach (var item in GetIncludedEntities())
                {
                    result = result & RenderEntity(item);
                }
            }
            else if (CurrentEntitySetting == null)
            {
                result = false;
            }
            else
            {
                result = RenderEntity((ObjcEntitySetting)CurrentEntitySetting);
            }

            if (RenderConst)
            {
                RenderConstHeader();
            }
            RenderEnumHeader();
            if (SqliteEngine != null && RenderDataContext)
            {
                RenderDbContext();
            }
            return result;
        }

        public override bool RenderAllTemplates()
        {
            if (!string.IsNullOrEmpty(Project.SearchString))
            {
                Project.SearchString = string.Empty;
            }

            if (RenderConst)
            {
                RenderConstHeader();
            }
            RenderEnumHeader();
            return true;
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>() { dataModelClass };
        }
        private const string dataModelClass = "Objective-C Model Class";

        protected AbstractObjcTemplate GetTemplateFile(ObjcEntitySetting entitySettings, string templateName)
        {
            AbstractObjcTemplate template = null;
            switch (templateName)
            {
                case dataModelClass:
                    template = new ObjcClassImpl(entitySettings);
                    break;

                default:
                    template = new ObjcClassImpl(entitySettings);
                    break;
            }

            return template;
        }

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static ObjcEngine OpenFile(string fileName)
        {
            ObjcEngine instance = GetInstanceFromFile(fileName, typeof(ObjcEngine)) as ObjcEngine;

            return instance;
        }

        public override UserControl GetSettingsDlgUI()
        {
            return new ObjcEngineSettingsUI();
        }

        public override string GetDefaultTargetFolder()
        {
            return "Objective-C Files";
        }
    }
}
