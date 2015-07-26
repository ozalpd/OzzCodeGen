using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.AppEngines
{
    [Serializable()]
    public abstract class BaseEntitySetting : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string _name;

        public bool Exclude
        {
            get
            {
                if (!_exclude.HasValue)
                {
                    _exclude = EntityDefinition.Abstract;
                }
                return _exclude.Value;
            }
            set
            {
                if (_exclude == value) return;
                _exclude = value;
                RaisePropertyChanged("Exclude");
            }
        }
        private bool? _exclude;

        public bool OverwriteExisting
        {
            get { return _overwriteExisting; }
            set
            {
                if (_overwriteExisting == value) return;
                _overwriteExisting = value;
                RaisePropertyChanged("OverwriteExisting");
            }
        }
        private bool _overwriteExisting;

        [XmlIgnore]
        public DataModel DataModel
        {
            get { return _dataModel; }
            set
            {
                if (_dataModel == value) return;
                _dataModel = value;
                RaisePropertyChanged("DataModel");
            }
        }
        DataModel _dataModel;

        [XmlIgnore]
        public EntityDefinition EntityDefinition
        {
            get
            {
                if (_entityDefinition == null)
                {
                    _entityDefinition = DataModel.FirstOrDefault(e => e.Name == Name);
                }
                return _entityDefinition;
            }
        }
        EntityDefinition _entityDefinition = null;


        public override string ToString()
        {
            return Name;
        }

        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
