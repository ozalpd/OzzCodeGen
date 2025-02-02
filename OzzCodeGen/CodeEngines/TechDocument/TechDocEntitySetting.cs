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

        [XmlIgnore]
        public StorageEntitySetting StorageEntity
        {
            get
            {
                if (_lookedEntitySetting)
                    return _storageEntity;

                var engine = CodeEngine.GetStorageEngine();
                if (engine != null)
                {
                    _storageEntity = engine.Entities.FirstOrDefault(e => e.Name.Equals(Name));
                }
                _lookedEntitySetting = true;
                return _storageEntity;
            }
        }
        bool _lookedEntitySetting;
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
