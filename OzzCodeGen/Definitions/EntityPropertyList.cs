using OzzUtils.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OzzCodeGen.Definitions
{
    [XmlInclude(typeof(SimpleProperty))]
    [XmlInclude(typeof(StringProperty))]
    [XmlInclude(typeof(ComplexProperty))]
    [XmlInclude(typeof(CollectionProperty))]
    public class EntityPropertyList : SavableList<BaseProperty>
    {
        public override string ToString()
        {
            return string.Format("[{0}]", Count);
        }

        [XmlIgnore]
        public EntityDefinition EntityDefinition { get; set; }

        [XmlIgnore]
        public List<SimpleProperty> SimpleProperties
        {
            get
            {
                List<SimpleProperty> properties = new List<SimpleProperty>();
                foreach (var p in this)
                {
                    if(p is SimpleProperty)
                        properties.Add(p as SimpleProperty);
                }
                return properties;
            }
        }

        [XmlIgnore]
        public List<ComplexProperty> ComplexProperties
        {
            get
            {
                List<ComplexProperty> properties = new List<ComplexProperty>();
                foreach (var p in this)
                {
                    if(p is ComplexProperty)
                        properties.Add(p as ComplexProperty);
                }
                return properties;
            }
        }

        [XmlIgnore]
        public List<CollectionProperty> CollectionProperties
        {
            get
            {
                List<CollectionProperty> properties = new List<CollectionProperty>();
                foreach (var p in this)
                {
                    if (p is CollectionProperty)
                        properties.Add(p as CollectionProperty);
                }
                return properties;
            }
        }

        public new void Add(BaseProperty property)
        {
            property.EntityDefinition = this.EntityDefinition;
            base.Add(property);
        }

        public void Add(BaseProperty property, bool distinctly)
        {
            if (distinctly && this.Any(p => p.Name == property.Name)) return;
            this.Add(property);
        }
    }
}
