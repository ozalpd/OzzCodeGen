using OzzCodeGen.Definitions;
using System.Text;

namespace OzzCodeGen.CodeEngines.CSharp
{
    public abstract class BaseCSharpPropertySetting : BasePropertySetting
    {
        public virtual string GetTypeName()
        {
            StringBuilder sb = new StringBuilder();
            if (PropertyDefinition.IsTypeNumeric() && PropertyDefinition is SimpleProperty)
            {
                sb.Append(((SimpleProperty)PropertyDefinition).EnumTypeName);
            }
            if (sb.Length == 0)
            {
                sb.Append(PropertyDefinition.TypeName);
            }
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
    }
}
