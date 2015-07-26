using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Definitions
{
    public class ComplexProperty : BaseClassProperty
    {
        public override DefinitionType DefinitionType
        {
            get { return DefinitionType.Complex; }
        }

        public override List<string> GetUsableTypeNames()
        {
            var typeNames = new List<string>();
            foreach (var item in this.EntityDefinition.DataModel)
            {
                typeNames.Add(item.Name);
            }

            return typeNames;
        }

        public SimpleProperty GetDependency()
        {
            var dependency = from p in EntityDefinition.Properties.OfType<SimpleProperty>()
                             where p.Name == DependentPropertyName
                             select p;

            return dependency.FirstOrDefault();
        }
    }
}
