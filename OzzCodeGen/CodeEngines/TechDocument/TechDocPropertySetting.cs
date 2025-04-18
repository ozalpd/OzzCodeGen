using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.TechDocument
{
    public class TechDocPropertySetting : BasePropertySetting
    {
        [XmlIgnore]
        public TechDocEntitySetting TechDocEntitySetting
        {
            get { return (TechDocEntitySetting)EntitySetting; }
        }

        [XmlIgnore]
        public TechDocumentEngine CodeEngine
        {
            get { return TechDocEntitySetting.CodeEngine; }
        }


        public string AddtDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_addtDescription))
                    return string.IsNullOrWhiteSpace(PropertyDefinition.Comment)
                         ? string.Empty
                         : PropertyDefinition.Comment;
                return _addtDescription;
            }
            set
            {
                _addtDescription = value;
                RaisePropertyChanged("AddtDescription");
            }
        }
        private string _addtDescription;

        string langCode = "tr";
        public string GetTypeDescription()
        {
            if (PropertyDefinition is CollectionProperty)
            {
                var collProp = (CollectionProperty)PropertyDefinition;
                return $"{collProp.GetObjectTypeName()} tipinde koleksiyondur.";
            }

            var sb = new StringBuilder();
            if (PropertyDefinition is ComplexProperty)
            {
                var complex = (ComplexProperty)PropertyDefinition;
                var dependentProp = complex.Dependency;
                if (dependentProp != null)
                {
                    sb.Append(dependentProp.Name);
                    sb.Append(" ile işaret edilen ");
                }
                sb.Append(complex.TypeName);
                sb.Append(" tipinde");

                string localTypeName = CodeEngine.GetTranslation(Name, langCode);
                if (!string.IsNullOrEmpty(localTypeName))
                {
                    sb.Append(" bağlı ");
                    sb.Append(localTypeName);
                    sb.Append(" verisini tutan");
                }

                sb.Append(" saha.");

                return sb.ToString();
            }

            ComplexProperty dependedComplex = null;
            if (PropertyDefinition is StringProperty)
            {
                var strProperty = (StringProperty)PropertyDefinition;
                sb.Append("string (karakter dizisi) tipinde, ");
                if (strProperty.MaxLenght > 0)
                {
                    sb.Append(strProperty.MaxLenght);
                }
                else
                {
                    sb.Append("azami");
                }
                sb.Append(" karakter genişliğinde");
            }

            if (PropertyDefinition is SimpleProperty)
            {
                var simple = (SimpleProperty)PropertyDefinition;
                bool isNull = simple.IsNullable;

                switch (PropertyDefinition.TypeName.ToLowerInvariant())
                {
                    case "bool":
                        return isNull ? "Nullable<bool> (boş kalabilir boolean) tipinde."
                                      : "bool boolean tipinde.";

                    case "datetime":
                        return isNull ? "Nullable<DateTime> (boş kalabilir zaman) tipinde."
                                      : "DateTime (zaman) tipinde.";

                    case "int":
                        if (!string.IsNullOrWhiteSpace(simple.EnumTypeName))
                        {
                            sb.Append("enum ");
                            sb.Append(simple.EnumTypeName);
                            sb.Append(", veri tabanı tablosunda ");
                        }
                        if (simple.IsNullable)
                        {
                            sb.Append("Nullable<int> (boş kalabilir 64 bit tamsayı) tipinde");
                        }
                        else
                        {
                            sb.Append("int (64 bit tamsayı) tipinde");
                        }

                        dependedComplex = GetDependedComplex(simple.Name);
                        if (dependedComplex != null)
                        {
                            sb.Append(" bağlı ");
                            string dependTranslation = CodeEngine.GetTranslation(dependedComplex.Name, langCode);
                            sb.Append(dependTranslation);

                            if (dependedComplex.Name.Equals(dependTranslation, StringComparison.InvariantCultureIgnoreCase) == false)
                            {
                                sb.Append(" (");
                                sb.Append(dependedComplex.Name);
                                sb.Append(")");
                            }
                            sb.Append(" verisine ait kayıt numarasını tutan");
                        }
                        break;

                    case "byte":
                        if (isNull)
                        {
                            sb.Append("Nullable<byte> (boş kalabilir 8 bit tamsayı) tipinde");
                        }
                        else
                        {
                            sb.Append("byte (8 bit tamsayı) tipinde");
                        }
                        break;

                    case "decimal":
                        return isNull ? "Nullable<decimal> (boş kalabilir ondalık sayı) tipinde."
                                      : "decimal (ondalık sayı) tipinde.";

                    //case "system.guid":
                    //    column.DataType = "UniqueIdentifier";
                    //    break;

                    case "guid":
                        return isNull ? "Nullable<Guid> (boş kalabilir 128 Bit Global Unique Identifier) tipinde."
                                      : "Guid (128 Bit Global Unique Identifier) tipinde.";


                    default:
                        break;
                }
            }

            if (sb.Length > 0)
            {
                sb.Append(" sahadır.");
                return sb.ToString();
            }

            return PropertyDefinition.TypeName;
        }

        private ComplexProperty GetDependedComplex(string propertyName)
        {
            ComplexProperty dependedComplex;
            var propDefs = TechDocEntitySetting.Properties.Select(p => p.PropertyDefinition);
            var complexes = propDefs.OfType<ComplexProperty>().ToList();

            dependedComplex = complexes.FirstOrDefault(p => p.DependentPropertyName == propertyName);
            return dependedComplex;
        }
    }
}
