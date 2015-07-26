using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OzzCodeGen.Definitions.UI
{
    public class PropertyBase : BaseSavable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string name;


        /// <summary>
        /// Display order for UI
        /// </summary>
        public int DisplayOrder
        {
            get
            {
                if (displayOrder.HasValue) return displayOrder ?? 0;
                if (Property != null) displayOrder = Property.DisplayOrder;
                return displayOrder ?? 0;
            }
            set
            {
                if (displayOrder.Equals(value)) return;
                displayOrder = value;
                RaisePropertyChanged("DisplayOrder");
            }
        }
        private int? displayOrder;

        [XmlIgnore]
        public string TypeName
        {
            get
            {
                if (!string.IsNullOrEmpty(typeName)) return typeName;
                if (Property == null) return string.Empty;
                typeName = Property.TypeName;
                return typeName;
            }
        }
        string typeName;

        [XmlIgnore]
        public EntityDefinition EntityDefinition
        {
            get { return Property.EntityDefinition; }
        }

        [XmlIgnore]
        public BaseProperty Property
        {
            get
            {
                if (property != null) return property;
                if (string.IsNullOrEmpty(Name)) return null;
                property = EntityDefinition.Properties
                            .Where(p => p.Name.Equals(this.Name))
                            .FirstOrDefault();
                return property;
            }
        }
        private BaseProperty property;
        
        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Property.TypeName);
        }

    }
}
