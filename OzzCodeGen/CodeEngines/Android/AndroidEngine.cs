using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.Android.Templates;
using OzzCodeGen.CodeEngines.Java;
using OzzCodeGen.CodeEngines.Java.Templates;

namespace OzzCodeGen.CodeEngines.Android
{
    [XmlInclude(typeof(JavaEntitySetting))]
    public class AndroidEngine : AbstractJavaEngine
    {
        public AndroidEngine()
        {
            SelectedTemplate = javaDataModel;
        }

        public override string EngineId { get { return EngineTypes.AndroidEngineId; } }

        [XmlIgnore]
        public static string DefaultFileName { get { return "AndroidEngine.settings"; } }
        public override string ProjectTypeName { get { return "Android Code Generation Engine"; } }

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }



        protected override System.Windows.Controls.UserControl GetUiControl()
        {
            if (_engineUI == null)
            {
                _engineUI = new UI.AndroidEngineUI()
                {
                    CodeEngine = this
                };
            }
            return _engineUI;
        }
        UI.AndroidEngineUI _engineUI;

        public override System.Windows.Controls.UserControl GetSettingsDlgUI()
        {
            return null;
        }

        public string GeneratedSqliteHelper
        {
            get
            {
                if (string.IsNullOrEmpty(_generatedSqliteHelper))
                {
                    _generatedSqliteHelper = "GenSqliteHelper";
                }
                return _generatedSqliteHelper;
            }
            set
            {
                _generatedSqliteHelper = value;
                RaisePropertyChanged("GeneratedSqliteHelper");
            }
        }
        private string _generatedSqliteHelper;

        public string CustomSqliteHelper
        {
            get
            {
                if (string.IsNullOrEmpty(_customSqliteHelper))
                {
                    _customSqliteHelper = "BaseSqliteHelper";
                }
                return _customSqliteHelper;
            }
            set
            {
                _customSqliteHelper = value;
                RaisePropertyChanged("CustomSqliteHelper");
            }
        }
        private string _customSqliteHelper;

        public string GeneratedDataContext
        {
            get
            {
                if (string.IsNullOrEmpty(_generatedDataContext))
                {
                    _generatedDataContext = "GenDataContext";
                }
                return _generatedDataContext;
            }
            set
            {
                _generatedDataContext = value;
                RaisePropertyChanged("GeneratedDataContext");
            }
        }
        private string _generatedDataContext;

        public string CustomDataContext
        {
            get
            {
                if (string.IsNullOrEmpty(_customDataContext))
                {
                    _customDataContext = "DataContext";
                }
                return _customDataContext;
            }
            set
            {
                _customDataContext = value;
                RaisePropertyChanged("CustomDataContext");
            }
        }
        private string _customDataContext;


        public string GetModelsPackage()
        {
            return Package + "." + ModelsFolder;
        }

        public string GetCustomsPackage()
        {
            return Package + "." + CustomsFolder;
        }

        public string GetGeneratedsPackage()
        {
            return Package + "." + GeneratedsFolder;
        }

        
        public override string GetDefaultTargetFolder()
        {
            return "Android";
        }

        public string GetModelsDir()
        {
            return Path.Combine(TargetDirectory, ModelsFolder);
        }

        public string GetCustomsDir()
        {
            return Path.Combine(TargetDirectory, CustomsFolder);
        }

        public string GetGeneratedsDir()
        {
            return Path.Combine(TargetDirectory, GeneratedsFolder);
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
                    System.Windows.Forms.MessageBox.Show(string.Format(
                        "Parser generator needs additional engine to render parsers!\r\nThis could be {0}",
                        EngineTypes.SqliteScriptsId),
                        "Project does not have necessary engines!");
                }
                _renderSqliteParsers = value && SqliteEngine != null;
                RaisePropertyChanged("RenderDataContext");
            }
        }
        private bool? _renderSqliteParsers;

        public string SqliteHelper
        {
            get
            {
                if (string.IsNullOrEmpty(_sqliteHelper))
                {
                    _sqliteHelper = "com.donduren.OzzDataContext.AbstractSqliteHelper";
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

        public string BaseEntityContext
        {
            get
            {
                if (string.IsNullOrEmpty(_baseEntityContext))
                {
                    _baseEntityContext = "com.donduren.OzzDataContext.AbstractEntityContext";
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

        public string BaseDataContext
        {
            get
            {
                if (string.IsNullOrEmpty(_baseDataContext))
                {
                    _baseDataContext = "com.donduren.OzzDataContext.AbstractDataContext";
                }
                return _baseDataContext;
            }
            set
            {
                _baseDataContext = value;
                RaisePropertyChanged("BaseDataContext");
            }
        }
        private string _baseDataContext;


        public override List<string> GetTemplateList()
        {
            return new List<string>() { javaDataModel };
        }
        private const string javaDataModel = "Data Model File";

        protected AbstractTemplate GetTemplateFile(JavaEntitySetting entity, string templateName)
        {
            AbstractTemplate tmp = null;
            switch (templateName)
            {
                case javaDataModel:
                    tmp = new JavaClass(entity);
                    ((JavaClass)tmp).SubFolder = ModelsFolder;
                    break;

                default:
                    tmp = new JavaClass(entity);
                    break;
            }

            return tmp;
        }

        public void RenderDbContext()
        {
            string customsDir = GetCustomsDir();
            string generatedsDir = GetGeneratedsDir();

            var sqliteHelper = new SqliteHelper(this, false);
            var sqliteCustHelper = new SqliteHelper(this, true);
            sqliteHelper.WriteToFile(Path.Combine(generatedsDir, sqliteHelper.GetDefaultFileName()), OverwriteExisting);
            sqliteCustHelper.WriteToFile(Path.Combine(customsDir, sqliteCustHelper.GetDefaultFileName()), false);

            var baseDataContext = new DataContext(this, false);
            baseDataContext.WriteToFile(Path.Combine(generatedsDir, baseDataContext.GetDefaultFileName()), OverwriteExisting);
            var dataContext = new DataContext(this, true);
            dataContext.WriteToFile(Path.Combine(TargetDirectory, dataContext.GetDefaultFileName()), false);
        }

        protected bool RenderEntity(JavaEntitySetting entity)
        {
            var classTmp = GetTemplateFile(entity, SelectedTemplate);
            string fileName = Path.Combine(GetModelsDir(), classTmp.GetDefaultFileName());

            bool parserRendered = true;
            if (!entity.EntityDefinition.Abstract && RenderDataContext)
            {
                string customsDir = GetCustomsDir();
                string generatedsDir = GetGeneratedsDir();

                var parserTemplate = new EntityContext(entity, false);
                var parserCustom = new EntityContext(entity, true);

                parserTemplate.WriteToFile(Path.Combine(generatedsDir, parserTemplate.GetDefaultFileName()), OverwriteExisting);
                parserCustom.WriteToFile(Path.Combine(customsDir, parserCustom.GetDefaultFileName()), false);
            }
            return classTmp.WriteToFile(fileName, OverwriteExisting) & parserRendered;
        }

        public override bool RenderSelectedTemplate()
        {
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
                result = RenderEntity((JavaEntitySetting)CurrentEntitySetting);
            }

            //if (RenderConst)
            //{
            //    RenderConstHeader();
            //}
            //RenderEnumHeader();
            if (SqliteEngine != null && RenderDataContext)
            {
                RenderDbContext();
            }
            return result;
        }

        public override bool RenderAllTemplates()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="FileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static AndroidEngine OpenFile(string fileName)
        {
            AndroidEngine instance = GetInstanceFromFile(
                fileName,
                typeof(AndroidEngine)) as AndroidEngine;

            return instance;
        }
    }
}
