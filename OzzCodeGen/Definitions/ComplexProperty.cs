using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

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
            var dataModel = EntityDefinition?.DataModel;
            if (dataModel != null)
            {
                foreach (var item in dataModel.OrderBy(p => p.Name))
                {
                    typeNames.Add(item.Name);
                }
            }
            return typeNames;
        }

        [XmlIgnore]
        public SimpleProperty Dependency
        {
            get
            {
                if (_chkedDependency)
                    return _dependency;

                _dependency = GetDependency();
                _chkedDependency = true;
                return _dependency;
            }
        }
        SimpleProperty _dependency;
        bool _chkedDependency = false;

        public SimpleProperty GetDependency()
        {
            var dependency = from p in EntityDefinition.Properties.OfType<SimpleProperty>()
                             where p.Name == DependentPropertyName
                             select p;

            return dependency.FirstOrDefault();
        }
    }
}
