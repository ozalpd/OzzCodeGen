using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.Definitions
{
    public class CollectionProperty : BaseClassProperty
    {
        public string DependentPropertyDeclaringType { get; set; }

        public override DefinitionType DefinitionType
        {
            get { return DefinitionType.Collection; }
        }

        public override BaseProperty Clone()
        {
            var clone = (CollectionProperty)base.Clone();
            clone.DependentPropertyDeclaringType = this.DependentPropertyDeclaringType;

            return clone;
        }

        public string GetObjectTypeName()
        {
            if (TypeName.StartsWith("ICollection<"))
            {
                return TypeName.Remove(TypeName.Length - 1, 1).Remove(0, "ICollection<".Length);
            }
            return TypeName;
        }

        public override List<string> GetUsableTypeNames()
        {
            var typeNames = new List<string>();
            foreach (var entity in this.EntityDefinition.DataModel.OrderBy(p => p.Name))
            {
                typeNames.Add(string.Format("ICollection<{0}>", entity.Name));
            }

            return typeNames;
        }
    }
}
