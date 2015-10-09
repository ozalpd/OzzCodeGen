using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines
{
    public abstract class BasePropertySetting : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of the Property
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
            get { return _exclude; }
            set
            {
                if (_exclude == value) return;
                _exclude = value;
                RaisePropertyChanged("Exclude");
            }
        }
        private bool _exclude;

        [XmlIgnore]
        public BaseEntitySetting EntitySetting { get; set; }

        [XmlIgnore]
        public BaseProperty PropertyDefinition
        {
            get
            {
                if (_propertyDefinition == null && EntitySetting != null)
                {
                    _propertyDefinition = EntitySetting
                                        .EntityDefinition
                                        .Properties
                                        .FirstOrDefault(e => e.Name == Name);
                }
                return _propertyDefinition;
            }
        }
        BaseProperty _propertyDefinition = null;

        [XmlIgnore]
        public bool IsSimpleOrString
        {
            get
            {
                return PropertyDefinition is SimpleProperty || PropertyDefinition is StringProperty;
            }
        }

        public bool IsForeignKey()
        {
            return PropertyDefinition is SimpleProperty && 
                    ((SimpleProperty)PropertyDefinition).IsForeignKey;
        }

        public bool IsKey
        {
            get
            {
                return PropertyDefinition is SimpleProperty &&
                    ((SimpleProperty)PropertyDefinition).IsKey;
            }
        }

        public string FormatComment(string lineBegins, int maxLineWidth = 76)
        {
            if (string.IsNullOrEmpty(PropertyDefinition.Comment))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var sbOut = new StringBuilder();
            string[] words = PropertyDefinition.Comment.Split(' ');
            int i = 0;
            foreach (var item in words)
            {
                if (sb.Length == 0)
                {
                    sb.Append(lineBegins);
                    sb.Append(item);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(item);
                }

                i++;
                string nextWord = words.Length > i ? words[i] : string.Empty;
                if (string.IsNullOrEmpty(nextWord))
                {
                    sbOut.Append(sb.ToString());
                    sb.Clear();
                }
                else if ((sb.Length + nextWord.Length) > maxLineWidth)
                {
                    sbOut.AppendLine(sb.ToString());
                    sb.Clear();
                }
            }
            return sbOut.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
