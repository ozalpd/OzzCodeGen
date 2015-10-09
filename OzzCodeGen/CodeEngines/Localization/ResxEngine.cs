using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using OzzCodeGen.CodeEngines.Localization.Templates;
using OzzCodeGen.CodeEngines.Localization.UI;
using OzzCodeGen.Definitions;
using OzzUtils;
using System.Collections.ObjectModel;
using OzzLocalization;

namespace OzzCodeGen.CodeEngines.Localization
{
    [XmlInclude(typeof(LocalizationEntitySetting))]
    public class ResxEngine : BaseCodeEngine
    {
        public override string EngineId { get { return EngineTypes.LocalizationResxGenId; } }
        public override string ProjectTypeName { get { return "Localization Resource Generator"; } }

        [XmlIgnore]
        public static string DefaultFileName { get { return "ResourceGen.settings"; } }
        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        protected override string GetDefaultNamespace()
        {
            if (Project==null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(TargetDirectory))
            {
                return Project.NamespaceName;
            }
            string targetFolderName = Path.GetFileName(TargetDirectory);
            return string.Format("{0}.{1}", Project.NamespaceName, targetFolderName);
        }

        protected override void OnTargetDirectoryChanging()
        {
            usingDefaultNamespace = NamespaceName.Equals(GetDefaultNamespace());
        }

        protected override void OnTargetDirectoryChanged()
        {
            base.OnTargetDirectoryChanged();
            if (usingDefaultNamespace)
            {
                NamespaceName = GetDefaultNamespace();
            }
        }
        bool usingDefaultNamespace = false;

        protected override BaseEntitySetting GetDefaultSetting(EntityDefinition entity)
        {
            var setting = new LocalizationEntitySetting()
            {
                DataModel = this.Project.DataModel,
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


        public string ErrorResxFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_errorResxFilename))
                {
                    _errorResxFilename = "ErrorStrings";
                }
                return _errorResxFilename;
            }
            set
            {
                _errorResxFilename = value;
                RaisePropertyChanged("ErrorResxFilename");
            }
        }
        private string _errorResxFilename;


        public bool SingleResx
        {
            get { return _singleResx ?? true; }
            set
            {
                _singleResx = value;
                RaisePropertyChanged("SingleResx");
            }
        }

        public string GetDefaultTargetFile(LocalizationEntitySetting entity)
        {
            if (SingleResx)
            {
                return entity.Name;
            }
            else
            {
                return string.Format("{0}{1}", entity.Name, "String");
            }
        }

        private bool? _singleResx;

        public string SingleResxFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_singleResxFilename))
                {
                    _singleResxFilename = "EntityStrings";
                }
                return _singleResxFilename;
            }
            set
            {
                _singleResxFilename = value;
                RaisePropertyChanged("SingleResxFilename");
            }
        }
        private string _singleResxFilename;


        public string VocabularyFolder
        {
            get { return _vocabularyFolder; }
            set
            {
                _vocabularyFolder = value;
                RaisePropertyChanged("VocabularyFolder");
            }
        }
        private string _vocabularyFolder;

        [XmlIgnore]
        public string VocabularyDir
        {
            get
            {
                if (Project != null &&
                    !string.IsNullOrEmpty(Project.SavedFileName) &&
                    !string.IsNullOrEmpty(VocabularyFolder))
                {
                    return Path.GetFullPath(
                            Path.Combine(
                                Path.GetDirectoryName(Project.SavedFileName), VocabularyFolder));
                }
                else if (Project != null && !string.IsNullOrEmpty(Project.SavedFileName))
                {
                    return Path.GetDirectoryName(Project.SavedFileName);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [XmlIgnore]
        public ObservableCollection<LocalizationEntitySetting> Entities
        {
            get { return _entities; }
            set
            {           //This should only be changed inside of the engine
                return; //WPF binding cause an error when set is private in .NET 4.6
            }           //TODO: Find more proper way than this!
        }
        private ObservableCollection<LocalizationEntitySetting> _entities;

        protected override void OnEntitySettingsChanged()
        {
            var entities = new ObservableCollection<LocalizationEntitySetting>();
            if (EntitySettings != null)
            {
                foreach (LocalizationEntitySetting item in EntitySettings)
                {
                    entities.Add(item);
                }
            }
            _entities = entities;
            RaisePropertyChanged("Entities");
        }


        protected LocalizationPropertySetting GetDefaultPropertySetting(BaseProperty property, LocalizationEntitySetting setting)
        {
            var ps = new LocalizationPropertySetting()
            {
                Name = property.Name,
                EntitySetting = setting
            };
            setting.Properties.Add(ps);

            return ps;
        }

        protected override void RefreshSetting(BaseEntitySetting setting, EntityDefinition entity, bool cleanRemovedItems)
        {
            var entitySetting = (LocalizationEntitySetting)setting;
            entitySetting.DataModel = Project.DataModel;

            List<LocalizationPropertySetting> remvProp = new List<LocalizationPropertySetting>();
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
            entitySetting.Properties = entitySetting
                .Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder)
                .ToList();
        }

        protected override System.Windows.Controls.UserControl GetUiControl()
        {
            if (_uiControl == null)
            {
                _uiControl = new ResxEngineUI();
                _uiControl.CodeEngine = this;
            }
            return _uiControl;
        }
        ResxEngineUI _uiControl = null;

        public override List<string> GetTemplateList()
        {
            return new List<string>() { "ResourceFile" };
        }

        protected bool RenderTemplate(LocalizationResx template)
        {
            string file = Path.Combine(TargetDirectory, template.GetDefaultFileName());
            return template.WriteToFile(file, OverwriteExisting || template.EntitySetting.OverwriteExisting);
        }

        protected bool RenderTemplate(LocalizationEntitySetting entitySettings)
        {
            bool allWritten = true; // RenderTemplate(new LocalizationResx(entitySettings));

            foreach (var item in Vocabularies)
            {
                allWritten = RenderTemplate(
                        new LocalizationResx(entitySettings, item.Value, this)) & allWritten;
            }
            return allWritten;
        }

        /// <summary>
        /// Combines all entities in to a single one
        /// </summary>
        /// <returns></returns>
        public LocalizationEntitySetting CombineEntities()
        {
            var combinedEntity = new LocalizationEntitySetting()
            {
                Name = SingleResxFilename,
                Properties = new List<LocalizationPropertySetting>()
            };

            foreach (LocalizationEntitySetting entity in EntitySettings.Where(e => e.Exclude == false))
            {
                foreach (LocalizationPropertySetting p in entity.Properties)
                {
                    if (!combinedEntity.Properties.Where(lp => lp.Name == p.Name).Any())
                    {
                        combinedEntity.Properties.Add(p);
                    }
                }
            }
            var entityNames = new List<string>();
            foreach (LocalizationEntitySetting entity in
                EntitySettings.Where(e => e.Exclude == false &&
                ((LocalizationEntitySetting)e).LocalizeEntityName))
            {
                entityNames.Add(entity.Name);
                entityNames.Add(string.Format("Create{0}", entity.Name));
                entityNames.Add(string.Format("Edit{0}", entity.Name));
                entityNames.Add(string.Format("Delete{0}", entity.Name));
                entityNames.Add(string.Format("{0}List", entity.Name.Pluralize()));
                entityNames.Add(entity.Name.Pluralize());
            }
            foreach (string entityName in entityNames)
            {
                if (!combinedEntity.Properties.Where(lp => lp.Name == entityName).Any())
                {
                    combinedEntity.Properties.Add(new LocalizationPropertySetting()
                    {
                        Name = entityName,
                        LocalizeRequiredMsg = false,
                        LocalizeValidationMsg = false
                    });
                }
            }

            combinedEntity.Properties = combinedEntity.Properties.OrderBy(p => p.Name).ToList();
            return combinedEntity;
        }

        public override bool RenderSelectedTemplate()
        {
            OpenVocabularies();
            if (SingleResx)
            {
                return RenderTemplate(CombineEntities());
            }
            else if (RenderAllEntities)
            {
                bool allWritten = true;
                foreach (LocalizationEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
                {
                    allWritten = RenderTemplate(setting) & allWritten;
                }
                return allWritten;
            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                return RenderTemplate((LocalizationEntitySetting)CurrentEntitySetting);
            }
        }

        public override bool RenderAllTemplates()
        {
            throw new NotImplementedException();
        }

        public override void SaveToFile()
        {
            base.SaveToFile();
            if (SaveWithVocabularies)
            {
                Vocabularies.DuplicateVocabularies(VocabularyDir);
            }
        }

        public override void SaveToFile(string fileName)
        {
            base.SaveToFile(fileName);
        }

        [XmlIgnore]
        public bool SaveWithVocabularies { get; set; }

        [XmlIgnore]
        public Vocabularies Vocabularies
        {
            set { _vocabularies = value; }
            get
            {
                if (_vocabularies == null)
                    _vocabularies = new Vocabularies();
                return _vocabularies;
            }
        }
        Vocabularies _vocabularies;

        public void OpenVocabularies()
        {
            if (string.IsNullOrEmpty(VocabularyDir))
                return;

            Vocabularies = Vocabularies.OpenVocabularies(VocabularyDir);
        }

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static ResxEngine OpenFile(string fileName)
        {
            ResxEngine instance = GetInstanceFromFile(fileName, typeof(ResxEngine)) as ResxEngine;
            //instance.OpenVocabularies();

            return instance;
        }

        public override UserControl GetSettingsDlgUI()
        {
            return null;
        }

        public override string GetDefaultTargetFolder()
        {
            return "App_GlobalResources";
        }
    }
}
