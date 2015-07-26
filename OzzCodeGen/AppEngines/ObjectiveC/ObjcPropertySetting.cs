using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils;

namespace OzzCodeGen.AppEngines.ObjectiveC
{
    public class ObjcPropertySetting : BasePropertySetting
    {
        public string ObjectiveCName
        {
            get
            {
                if (string.IsNullOrEmpty(_objcName))
                    _objcName = GetDefaultObjcName();
                return _objcName;
            }
            set
            {
                _objcName = value;
                RaisePropertyChanged("ObjectiveCName");
            }
        }
        private string _objcName;

        private string GetDefaultObjcName()
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

        public string ObjcType
        {
            get
            {
                if (string.IsNullOrEmpty(_objcType))
                    _objcType = GetObjcType();
                return _objcType;
            }
            set
            {
                _objcType = value;
                RaisePropertyChanged("ObjcType");
            }
        }
        private string _objcType;

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
                return "none";
            }
        }


        public string GetPropertyDeclaration()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(nonatomic");
            if (!(PropertyDefinition.IsTypeBoolean()
                | PropertyDefinition.IsTypeNumeric()))
            {
                sb.Append(", strong");
            }
            if (this.ReadOnly)
            {
                sb.Append(", readonly");
            }
            sb.Append(") ");

            string objcType = GetObjcType();
            sb.Append(objcType);
            sb.Append(' ');

            if (!(PropertyDefinition.IsTypeBoolean()
                | PropertyDefinition.IsTypeNumeric()))
            {
                sb.Append('*');
            }
            sb.Append(ObjectiveCName);
            return sb.ToString();
        }

        public string DeclareAddMethod()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("- (void)add");
            sb.Append(Name.Singularize());
            sb.Append(':');
            sb.Append('(');
            var entityType = GetEntity();
            sb.Append(entityType.ObjectiveCName);
            sb.Append(" *)");
            sb.Append(Name.Singularize().ToCamelCase());

            return sb.ToString();
        }

        public string DeclareAtIndexMethod()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("- (");
            var entityType = GetEntity();
            sb.Append(entityType.ObjectiveCName);
            sb.Append(" *)");
            sb.Append(Name.Singularize().ToCamelCase());
            sb.Append("AtIndex:(NSUInteger)index");

            return sb.ToString();
        }

        public bool IsArrayType()
        {
            return PropertyDefinition.TypeName.StartsWith("ICollection");
        }

        public ObjcEntitySetting GetEntity()
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
            return ((ObjcEntitySetting)EntitySetting).GetEntityByTypeName(typeName);
        }

        protected string GetObjcType()
        {
            string typeName = PropertyDefinition.TypeName;
            if (IsArrayType())
            {
                return "NSArray";
            }
            var entityType = GetEntity();
            if (entityType != null)
            {
                return entityType.ObjectiveCName;
            }
            switch (typeName.ToLowerInvariant())
            {
                case "string":
                    return "NSString";

                case "datetime":
                    return "NSDate";

                case "bool":
                    return "BOOL";

                case "int":
                    return "NSInteger";

                case "uint":
                    return "NSUInteger";

                default:
                    return typeName;
            }
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
        
    }
}
