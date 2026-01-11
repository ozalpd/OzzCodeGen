using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.Storage;
using OzzCodeGen.CodeEngines.TechDocument.Templates;
using OzzCodeGen.CodeEngines.TechDocument.UI;
using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.TechDocument
{
    [XmlInclude(typeof(TechDocEntitySetting))]
    public class TechDocumentEngine : BaseCodeEngine
    {
        public override string EngineId => EngineTypes.EfTechnicalDocId;

        public override string ProjectTypeName => "Technical Documents for EF";

        public static string DefaultFileName { get { return "TechDocs.settings"; } }
        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        public override string GetDefaultTargetFolder()
        {
            return "TechnicalDocs";
        }

        public override UserControl GetSettingsDlgUI()
        {
            return null;
        }

        public ResxEngine GetResxEngine()
        {
            var engine = Project.GetCodeEngine(EngineTypes.LocalizationResxGenId);
            if (engine != null && engine is ResxEngine)
            {
                var resx = (ResxEngine)engine;
                resx.OpenVocabularies();

                return resx;
            }
            return null;
        }

        public StorageCodeEngine GetStorageEngine()
        {
            var engine = Project.GetCodeEngine(EngineTypes.TSqlScriptsId);
            if (engine != null && engine is StorageCodeEngine)
                return (StorageCodeEngine)engine;

            return null;
        }

        public string GetTranslation(string name, string langCode)
        {
            var engine = GetResxEngine();
            var vocabulary = engine.Vocabularies[langCode];
            if (vocabulary == null)
                vocabulary = engine.Vocabularies["notr"];

            var vocab = vocabulary.FirstOrDefault(var => var.Name == name);

            return vocab != null ? vocab.Translation : name;
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>() { _docTemp };
        }
        private const string _docTemp = "Technical Document";

        public override bool RenderAllTemplates()
        {
            throw new NotImplementedException("There is only one template in MetadataCodeEngine!");
        }


        private TechDocPropertySetting GetSelectedProperty()
        {
            if (_engineUI.grdPropertySettings.SelectedItem == null) return null;
            return (TechDocPropertySetting)_engineUI.grdPropertySettings.SelectedItem;
        }

        private TechDocEntitySetting GetSelectedEntity()
        {
            if (_engineUI.grdEntitySettings.SelectedItem == null) return null;
            return (TechDocEntitySetting)_engineUI.grdEntitySettings.SelectedItem;
        }

        protected bool RenderTemplate(TechDocEntitySetting entitySettings)
        {
            if (entitySettings == null)
                return false;
            
            var template = new TechDocTemplate(entitySettings);
            var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
            bool allWritten = true;
            return template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting) & allWritten;
        }

        public override bool RenderSelectedTemplate()
        {
            if (!string.IsNullOrEmpty(Project.SearchString))
            {
                Project.SearchString = string.Empty;
            }

            if (RenderAllEntities)
            {
                bool allWritten = true;
                foreach (TechDocEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
                {
                    allWritten = RenderTemplate(setting) & allWritten;
                }
                return allWritten;
            }
            else
            {
                return RenderTemplate(GetSelectedEntity());
            }
        }

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new TechDocEntitySetting()
            {
                DataModel = this.Project.DataModel,
                CodeEngine = this,
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

        private TechDocPropertySetting GetDefaultPropertySetting(BaseProperty property, TechDocEntitySetting setting)
        {
            var ps = new TechDocPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting
            };
            setting.Properties.Add(ps);

            return ps;
        }
        protected override UserControl GetUiControl()
        {
            if (_engineUI == null)
            {
                _engineUI = SetNewUI();
            }
            return _engineUI;
        }
        TechDocEngineUI _engineUI = null;

        private TechDocEngineUI SetNewUI()
        {
            var control = new TechDocEngineUI()
            {
                CodeEngine = this
            };


            return control;
        }


        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var dataLayerSetting = (TechDocEntitySetting)setting;
            dataLayerSetting.DataModel = Project.DataModel;
            ((TechDocEntitySetting)setting).CodeEngine = this;

            List<TechDocPropertySetting> remvProp = new List<TechDocPropertySetting>();
            foreach (var dalProp in dataLayerSetting.Properties)
            {
                if (entity.Properties.FirstOrDefault(p => p.Name == dalProp.Name) == null)
                {
                    remvProp.Add(dalProp);
                }
            }
            foreach (var dalProp in remvProp)
            {
                dataLayerSetting.Properties.Remove(dalProp);
            }

            foreach (var property in entity.Properties)
            {
                var ps = dataLayerSetting.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (ps == null)// && property.DefinitionType != DefinitionType.Collection
                {
                    ps = GetDefaultPropertySetting(property, dataLayerSetting);
                }
                else //if (ps != null)
                {
                    ps.EntitySetting = setting;
                }
            }
            dataLayerSetting.Properties = dataLayerSetting
                .Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder)
                .ToList();
        }



        protected override void OnSearchStringChanged()
        {
            RaisePropertyChanged("Entities");
        }

        [XmlIgnore]
        public List<TechDocEntitySetting> Entities
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
        private List<TechDocEntitySetting> _entities;

        protected override void OnEntitySettingsChanged()
        {
            var entities = new List<TechDocEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (TechDocEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            Entities = entities;
        }

        [XmlIgnore]
        public ResxEngine ResxEngine
        {
            get
            {
                if (_resxEngine == null && Project != null)
                {
                    var engine = Project.GetCodeEngine(EngineTypes.LocalizationResxGenId);
                    _resxEngine = engine != null ? (ResxEngine)engine : null;
                }
                return _resxEngine;
            }
        }
        private ResxEngine _resxEngine;

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static TechDocumentEngine OpenFile(string fileName)
        {
            TechDocumentEngine instance = GetInstanceFromFile(
               fileName,
               typeof(TechDocumentEngine)) as TechDocumentEngine;

            return instance;
        }
    }
}
