using OzzCodeGen.Definitions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines
{
    public abstract class BasePropertySetting : INotifyPropertyChanged
    {
        protected abstract BaseCodeEngine GetCodeEngine();

        /// <summary>
        /// Name of the Property
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value != null ? value.Replace(" ", "") : string.Empty;
                RaisePropertyChanged(nameof(Name));
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets a value indicating whether this item is excluded from processing.
        /// </summary>
        public bool Exclude
        {
            get { return _exclude; }
            set
            {
                if (_exclude == value) return;
                _exclude = value;
                RaisePropertyChanged(nameof(Exclude));
            }
        }
        private bool _exclude;

        [XmlIgnore]
        [JsonIgnore]
        public BaseEntitySetting EntitySetting { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public BaseProperty PropertyDefinition
        {
            get
            {
                if (_propertyDefinition == null && EntitySetting != null)
                {
                    SetPropertyDefinition();
                }
                return _propertyDefinition;
            }
        }
        BaseProperty _propertyDefinition = null;

        protected virtual void SetPropertyDefinition()
        {
            _propertyDefinition = EntitySetting
                                        .EntityDefinition
                                        .Properties
                                        .FirstOrDefault(e => e.Name == Name);
        }

        public string GetForeignKeyName()
        {
            if (PropertyDefinition is ComplexProperty)
                return ((ComplexProperty)PropertyDefinition).DependentPropertyName;

            return string.Empty;
        }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsLoadingFromFile
        {
            get { return _isLoadingFromFile; }
            set { _isLoadingFromFile = value; }
        }
        bool _isLoadingFromFile = true;


        /// <summary>
        /// Gets a value indicating whether the property represents a collection type.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsCollection => PropertyDefinition is CollectionProperty;

        /// <summary>
        /// Gets a value indicating whether the property represents a complex type (i.e., a non-primitive, non-string type).
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsComplex => PropertyDefinition is ComplexProperty;


        /// <summary>
        /// Gets a value indicating whether the property is the C# <see langword="decimal"/> type.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsDecimal => PropertyDefinition.TypeName.Equals("decimal", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the property type is a double-precision floating-point number.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsDouble => PropertyDefinition.TypeName.Equals("double", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the property type is a single-precision floating-point number.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsFloat => PropertyDefinition.TypeName.Equals("float", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the property is a fractional numeric type like <see langword="float"/>, <see langword="double"/>, or <see langword="decimal"/>.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="true"/> for <see langword="float"/>, <see langword="double"/>, and <see langword="decimal"/>.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsFractionalNumeric => PropertyDefinition is SimpleProperty && ((SimpleProperty)PropertyDefinition).IsFractionalNumeric();

        /// <summary>
        /// Gets a value indicating whether the property is of an integer numeric type.
        /// </summary>
        /// <remarks>This property returns <see langword="true"/> if the underlying property definition
        /// represents an integer-based numeric type, such as <see cref="int"/> or <see cref="long"/>. Use this property
        /// to determine if integer-specific logic should be applied.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsIntegerNumeric => PropertyDefinition is SimpleProperty && ((SimpleProperty)PropertyDefinition).IsTypeIntegerNumeric();


        [XmlIgnore]
        [JsonIgnore]
        public bool IsNullable => PropertyDefinition is SimpleProperty && ((SimpleProperty)PropertyDefinition).IsNullable;

        /// <summary>
        /// Gets a value indicating whether the property definition represents a simple or string property.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsSimpleOrString
        {
            get
            {
                return PropertyDefinition is SimpleProperty || PropertyDefinition is StringProperty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property definition represents a simple property.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsSimple
        {
            get
            {
                return PropertyDefinition is SimpleProperty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property is defined as a string type.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsString
        {
            get
            {
                return PropertyDefinition is StringProperty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property is of type string and is nullable.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsNullableString => PropertyDefinition is SimpleProperty && ((SimpleProperty)PropertyDefinition).IsNullable && PropertyDefinition.IsTypeString();

        [XmlIgnore]
        [JsonIgnore]
        public bool IsBoolean
        {
            get
            {
                return PropertyDefinition is SimpleProperty &&
                    ((SimpleProperty)PropertyDefinition).IsTypeBoolean();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property is of type DateTime.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsDateTime
        {
            get
            {
                return PropertyDefinition is SimpleProperty &&
                    ((SimpleProperty)PropertyDefinition).IsTypeDateTime();
            }
        }

        public string GetNullableTypeName()
        {
            return PropertyDefinition.GetNullableTypeName();
        }

        /// <summary>
        /// Determines whether the property represents a foreign key relationship.
        /// </summary>
        /// <returns>true if the property is a foreign key; otherwise, false.</returns>
        public bool IsForeignKey()
        {
            return PropertyDefinition is SimpleProperty &&
                    ((SimpleProperty)PropertyDefinition).IsForeignKey;
        }

        /// <summary>
        /// Gets a value indicating whether the property is a key.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
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
