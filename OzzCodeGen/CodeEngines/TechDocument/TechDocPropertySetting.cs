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
                var dependentProp = complex.GetDependency();
                if (dependentProp != null)
                {
                    sb.Append(dependentProp.Name);
                    sb.Append(" ile işaret edilen ");
                }
                sb.Append(complex.TypeName);
                sb.Append(" tipinde saha.");

                return sb.ToString();
            }


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
                sb.Append(" karakter genişliğinde.");

                return sb.ToString();
            }

            if (PropertyDefinition is SimpleProperty)
            {
                var simple = (SimpleProperty)PropertyDefinition;
                bool isNull = simple.IsNullable;

                switch (PropertyDefinition.TypeName.ToLowerInvariant())
                {
                    case "bool":
                        return simple.IsNullable
                            ? "Nullable<bool> (boş kalabilir boolean) tipinde."
                            : "bool boolean tipinde.";

                    case "datetime":
                        return simple.IsNullable
                            ? "Nullable<DateTime> (boş kalabilir zaman) tipinde."
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
                            sb.Append("Nullable<int> (boş kalabilir 64 bit tamsayı) tipinde.");
                        }
                        else
                        {
                            sb.Append("int (64 bit tamsayı) tipinde.");
                        }
                        return sb.ToString();

                    case "decimal":
                        return simple.IsNullable
                            ? "Nullable<decimal> (boş kalabilir ondalık sayı) tipinde."
                            : "decimal (ondalık sayı) tipinde.";

                    //case "system.guid":
                    //    column.DataType = "UniqueIdentifier";
                    //    break;

                    //case "guid":
                    //    column.DataType = "UniqueIdentifier";
                    //    break;


                    default:
                        break;
                }
            }

            return PropertyDefinition.TypeName;
        }
    }
}
