using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.Definitions
{
    public class EnumMember : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string name;

        public int Value
        {
            get { return _value; }
            set
            {
                RaisePropertyChanged("Value");
                _value = value;
            }
        }
        private int _value;

        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment == value) return;
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        string _comment;


        [XmlIgnore]
        public EnumDefinition EnumDefinition { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
