using OzzUtils.Savables;
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
            SetNewDisplayOrder(property);
        }

        public void Add(BaseProperty property, bool distinctly)
        {
            if (distinctly && this.Any(p => p.Name == property.Name)) return;
            this.Add(property);
        }

        public override void MoveUp(BaseProperty item)
        {
            int oldIndex = IndexOf(item);
            if (oldIndex == 0)
                return;

            int displayOrder = item.DisplayOrder;
            base.MoveUp(item);

            int newIndex = IndexOf(item);
            int oldItemOrder = this[oldIndex].DisplayOrder;

            if (oldItemOrder == 0)
                SetNewDisplayOrder(item);
            else
                item.DisplayOrder = oldItemOrder;

            if (displayOrder == 0)
                SetNewDisplayOrder(this[oldIndex]);
            else
                this[oldIndex].DisplayOrder = displayOrder;
        }

        public override void MoveDown(BaseProperty item)
        {
            int oldIndex = IndexOf(item);
            if (oldIndex == Count - 1)
                return;

            int displayOrder = item.DisplayOrder;
            base.MoveDown(item);

            int newIndex = IndexOf(item);
            int oldItemOrder = this[oldIndex].DisplayOrder;

            if (oldItemOrder == 0)
                SetNewDisplayOrder(item);
            else
                item.DisplayOrder = oldItemOrder;

            if (displayOrder == 0)
                SetNewDisplayOrder(this[oldIndex]);
            else
                this[oldIndex].DisplayOrder = displayOrder;
        }

        public override void MoveBottom(BaseProperty item)
        {
            base.MoveBottom(item);
            SetNewDisplayOrder(item);
        }

        public override void MoveTop(BaseProperty item)
        {
            base.MoveTop(item);
            SetNewDisplayOrder(item);
        }

        protected void SetNewDisplayOrder(BaseProperty item)
        {
            int index = IndexOf(item);
            if (index == Count - 1)
            {
                item.DisplayOrder = this.Select(p => p.DisplayOrder).Max() + 100;
            }
            else if (index == 0 && Count > 1)
            {
                item.DisplayOrder = this[1].DisplayOrder - 100;
            }
        }
    }
}
