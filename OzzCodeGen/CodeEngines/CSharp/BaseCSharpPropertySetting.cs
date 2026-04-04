using OzzCodeGen.Definitions;
using System.Text;

namespace OzzCodeGen.CodeEngines.CSharp
{
    public abstract class BaseCSharpPropertySetting : BasePropertySetting
    {
        public virtual string GetEnumTypeName()
        {
            if (PropertyDefinition.IsTypeNumeric() && PropertyDefinition is SimpleProperty)
            {
                return ((SimpleProperty)PropertyDefinition).EnumTypeName ?? string.Empty;
            }
            return string.Empty; ;
        }

        public virtual string GetTypeName(bool getReturnType = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEnumTypeName());

            if (sb.Length == 0)
            {
                sb.Append(PropertyDefinition.TypeName);
            }

            if (getReturnType)
                return sb.ToString();

            if (PropertyDefinition is SimpleProperty && !PropertyDefinition.IsTypeString() && ((SimpleProperty)PropertyDefinition).IsNullable)
            {
                sb.Append("?");
                return sb.ToString();
            }

            var engine = GetCodeEngine();
            bool isClassic = engine?.Project?.TargetPlatform == TargetDotNetPlatform.DotNetFramework;
            if (isClassic || PropertyDefinition is CollectionProperty)
                return sb.ToString();

            if (PropertyDefinition is ComplexProperty && ((ComplexProperty)PropertyDefinition).Dependency?.IsNullable == true)
            {
                sb.Append("?");
                return sb.ToString();
            }

            if (PropertyDefinition is StringProperty && ((SimpleProperty)PropertyDefinition).IsNullable)
            {
                sb.Append("?");
            }

            return sb.ToString();
        }


        public virtual string GetSafeValueExpr()
        {
            if (IsNullableString)
                return $"{Name} ?? string.Empty";

            if (IsNullable && IsDecimalNumeric)
                return $"{Name} ?? 0m";

            if (IsNullable && IsIntNumeric)
                return $"{Name} ?? 0";

            if (IsNullable && IsBoolean)
                return $"{Name} ?? false";

            if (IsNullable && IsDateTime)
                return $"{Name} ?? DateTime.MinValue";


            return Name;
        }
    }
}
