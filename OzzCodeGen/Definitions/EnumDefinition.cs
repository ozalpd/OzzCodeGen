using System.ComponentModel;
using System.Xml.Serialization;
using OzzUtils.Savables;

namespace OzzCodeGen.Definitions
{
    public class EnumDefinition : SavableObject, INotifyPropertyChanged
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
                name = value != null ? value.Replace(" ", "") : string.Empty;
                RaisePropertyChanged("Name");
            }
        }
        private string name;

        /// <summary>
        /// NamespaceName of the entity
        /// </summary>
        public string NamespaceName
        {
            get { return namespaceName; }
            set
            {
                if (namespaceName == value) return;
                namespaceName = value;
                RaisePropertyChanged("NamespaceName");
            }
        }
        private string namespaceName;


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


        public EnumMemberList Members
        {
            get
            {
                if (_members == null)
                {
                    _members = new EnumMemberList();
                    _members.EnumDefinition = this;
                }
                return _members;
            }
            set
            {
                _members = value;
                _members.EnumDefinition = this;
                RaisePropertyChanged("Members");
            }
        }
        private EnumMemberList _members;

        [XmlIgnore]
        public EnumDefinitionList EnumList { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
