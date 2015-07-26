using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override List<string> GetUsableTypeNames()
        {
            var typeNames = new List<string>();
            foreach (var item in this.EntityDefinition.DataModel)
            {
                typeNames.Add(string.Format("ICollection<{0}>", item.Name));
            }

            return typeNames;
        }
    }
}
