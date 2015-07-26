using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;
using OzzCodeGen.Templates;

namespace OzzCodeGen.AppEngines.DataLayer
{
    public class DalEntitySetting : AbstractEntitySetting<DalPropertySetting>
    {

        [XmlIgnore]
        public DataLayerEngine AppEngine { get; set; }

        public bool CreateCustomFile
        {
            get
            {
                if (!_createCustomFile.HasValue) _createCustomFile = true;
                return _createCustomFile.Value;
            }
            set
            {
                if (_createCustomFile == value) return;
                _createCustomFile = value;
                RaisePropertyChanged("CreateCustomFile");
            }
        }
        private bool? _createCustomFile;

        public bool CreateViewModelFile
        {
            get
            {
                return _createViewModelFile;
            }
            set
            {
                if (_createViewModelFile == value) return;
                _createViewModelFile = value;
                RaisePropertyChanged("CreateViewModelFile");
            }
        }
        private bool _createViewModelFile;
        
        /// <summary>
        /// ToStringReturn of the Property
        /// </summary>
        public string ToStringReturn
        {
            get
            {
                if (string.IsNullOrEmpty(_toString))
                {
                    _toString = GetDefaultToString(EntityDefinition);
                }
                return _toString;
            }
            set
            {
                if (_toString == value) return;
                _toString = value;
                RaisePropertyChanged("ToStringReturn");
            }
        }
        private string _toString;

        private string GetDefaultToString(EntityDefinition entity)
        {
            return entity.DisplayMember;
        }

        public override AbstractEntitySetting<DalPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;

            return AppEngine
                    .Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }
    }
}
