using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.TechDocument
{
    public class TechDocEntitySetting : AbstractEntitySetting<TechDocPropertySetting>
    {
        [XmlIgnore]
        public TechDocumentEngine CodeEngine { get; set; }

        public override AbstractEntitySetting<TechDocPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return CodeEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }

        public string GetLocalizedName(string langCode)
        {
            return CodeEngine.GetTranslation(Name, langCode);
        }

        [XmlIgnore]
        public LocalizationEntitySetting LocalizationEntity
        {
            get
            {
                if (_checkedLocalEntity)
                    return _localEntity;

                var engine = CodeEngine.GetResxEngine();
                if (engine != null)
                {
                    _localEntity = engine.Entities.FirstOrDefault(e => e.Name.Equals(Name));
                }
                _checkedLocalEntity = true;
                return _localEntity;
            }
        }
        bool _checkedLocalEntity = false;
        LocalizationEntitySetting _localEntity = null;

        [XmlIgnore]
        public StorageEntitySetting StorageEntity
        {
            get
            {
                if (_chkedStorageEntity)
                    return _storageEntity;

                var engine = CodeEngine.GetStorageEngine();
                if (engine != null)
                {
                    _storageEntity = engine.Entities.FirstOrDefault(e => e.Name.Equals(Name));
                }
                _chkedStorageEntity = true;
                return _storageEntity;
            }
        }
        bool _chkedStorageEntity = false;
        StorageEntitySetting _storageEntity = null;


        public bool UseInheritedProperties
        {
            get
            {
                if (_useInheritedroperties == null)
                    _useInheritedroperties = true;

                return _useInheritedroperties.Value;
            }
            set
            {
                _useInheritedroperties = value;
                RaisePropertyChanged("UseInheritedProperties");
            }
        }
        private bool? _useInheritedroperties;

    }
}
