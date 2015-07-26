using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;

namespace OzzCodeGen.AppEngines.Java
{
    public class JavaPropertySetting : BasePropertySetting
    {
        public string JavaName
        {
            get
            {
                if (string.IsNullOrEmpty(_javaName))
                {
                    _javaName = GetDefaultJavaName();
                }
                return _javaName;
            }
            set
            {
                _javaName = value;
                RaisePropertyChanged("JavaName");
            }
        }
        private string _javaName;

        private string GetDefaultJavaName()
        {
            if (PropertyDefinition.IsTypeBoolean() &&
                Name.ToSentenceCase().StartsWith("Is "))
            {
                return Name.Substring(2).ToCamelCase();
            }

            switch (Name.ToUpperInvariant())
            {
                case "ID":
                    return "uniqueId";

                default:
                    return Name.ToCamelCase();
            }
        }

        public string JavaType
        {
            get
            {
                if (string.IsNullOrEmpty(_javaType))
                    _javaType = GetJavaType();
                return _javaType;
            }
            set
            {
                _javaType = value;
                RaisePropertyChanged("JavaType");
            }
        }
        private string _javaType;

        protected string GetJavaType()
        {
            string typeName = PropertyDefinition.TypeName;
            if (IsArrayType())
            {
                return PropertyDefinition
                    .TypeName
                    .Replace("ICollection", "ArrayList");
            }
            var entityType = GetEntity();
            if (entityType != null)
            {
                return entityType.Name;
            }
            switch (typeName.ToLowerInvariant())
            {
                case "string":
                    return "String";

                case "datetime":
                    return "Date";

                case "bool":
                    return "boolean";

                //case "int":
                //    return "NSInteger";

                //case "uint":
                //    return "NSUInteger";

                default:
                    return typeName;
            }
        }

        public string Getter
        {
            get
            {
                if (string.IsNullOrEmpty(_getter))
                    _getter = GetGetter();
                return _getter;
            }
            set
            {
                _getter = value;
                RaisePropertyChanged("Getter");
            }
        }
        private string _getter;

        private string GetGetter()
        {
            if (PropertyDefinition.IsTypeBoolean() &&
                Name.ToSentenceCase().StartsWith("Is "))
            {
                return Name.ToCamelCase();
            }
            else if (PropertyDefinition.IsTypeBoolean() &&
                !Name.ToSentenceCase().StartsWith("Can "))
            {
                return "is" + Name;
            }
            else
            {
                return JavaName;
                //return "get" + JavaName.ToPascalCase();
            }
        }

        public string Setter
        {
            get
            {
                if (string.IsNullOrEmpty(_Setter))
                    _Setter = GetSetter();
                return _Setter;
            }
            set
            {
                _Setter = value;
                RaisePropertyChanged("Setter");
            }
        }
        private string _Setter;

        private string GetSetter()
        {
            //return JavaName;
            return "set" + JavaName.ToPascalCase();
        }

        public bool IsArrayType()
        {
            return PropertyDefinition.TypeName.StartsWith("ICollection");
        }

        public JavaEntitySetting GetEntity()
        {
            string typeName;
            if (IsArrayType())
            {
                typeName = PropertyDefinition
                    .TypeName
                    .Replace("ICollection", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Trim();
            }
            else
            {
                typeName = PropertyDefinition.TypeName;
            }
            return ((JavaEntitySetting)EntitySetting).GetEntityByTypeName(typeName);
        }


        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                RaisePropertyChanged("ReadOnly");
            }
        }
        private bool _readOnly;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="dateTimeCast">Cast method for java.util.Date</param>
        /// <returns></returns>
        public string GetStorageGetter(string variableName, string dateTimeCast)
        {
            StringBuilder sb = new StringBuilder();

            if (JavaType == "Date")
            {
                sb.Append(dateTimeCast);
                sb.Append('(');
            }
            if (!string.IsNullOrEmpty(variableName))
            {
                sb.Append(variableName);
                sb.Append('.');
            }
            sb.Append(Getter);
            sb.Append("()");

            if (JavaType == "Date")
            {
                sb.Append(')');
            }

            return sb.ToString();
        }

        public string GetCursorSetter(string cursorVariable, string colIdxVariable, string dateTimeCast)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Setter);
            sb.Append('(');

            if (JavaType == "Date")
            {
                sb.Append(dateTimeCast);
                sb.Append('(');
            }
            if (!string.IsNullOrEmpty(cursorVariable))
            {
                sb.Append(cursorVariable);
                sb.Append('.');
            }

            switch (JavaType.ToLowerInvariant())
            {
                case "boolean":
                    sb.Append("getInt");
                    break;

                case "int":
                    sb.Append("getInt");
                    break;

                case "long":
                    sb.Append( "getLong");
                    break;

                case "date":
                sb.Append("getLong");
                    break;

                case "string":
                    sb.Append( "getString");
                    break;

                default:
                    return "!!! Can't get CursorGetter for " + JavaType;
            }

            sb.Append('(');
            sb.Append(colIdxVariable);
            if (JavaType == "boolean")
            {
                sb.Append(") != 0)");
            }
            else if (JavaType == "Date")
            {
                sb.Append(")))");
            }
            else
            {
                sb.Append("))");
            }

            return sb.ToString();
        }

    }
}
