using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.AppEngines
{
    public abstract class AbstractEntitySetting<T> : BaseEntitySetting where T : BasePropertySetting
    {
        public List<T> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<T>();
                return _properties;
            }
            set
            {
                if (_properties == value) return;
                _properties = value;
                RaisePropertyChanged("Properties");
            }
        }
        protected List<T> _properties;

        public abstract AbstractEntitySetting<T> GetBaseEntitySetting();


        public List<T> GetInheritedProperties()
        {
            var allProperties = new List<T>();
            var baseEntity = GetBaseEntitySetting();
            if (baseEntity != null)
            {
                var baseProperties = baseEntity.GetInheritedProperties();
                foreach (var item in baseProperties)
                {
                    allProperties.Add(item);
                }
            }

            foreach (var item in Properties)
            {
                allProperties.Add(item);
            }
            return allProperties;
        }

        public IEnumerable<T> GetInheritedSimpleProperties()
        {
            return GetInheritedProperties()
                    .Where(c => c.PropertyDefinition is SimpleProperty)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
        }

        public IEnumerable<T> GetInheritedComplexProperties()
        {
            return GetInheritedProperties()
                    .Where(c => c.PropertyDefinition is ComplexProperty)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
        }

        public IEnumerable<T> GetInheritedCollectionProperties()
        {
            return GetInheritedProperties()
                    .Where(c => c.PropertyDefinition is CollectionProperty)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
        }

        public IEnumerable<T> GetInheritedIncludedProperties()
        {
            return GetInheritedProperties()
                    .Where(c => c.Exclude == false)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
        }

        public T GetPrimeryKey()
        {
            return GetInheritedSimpleProperties()
                    .FirstOrDefault(p => ((SimpleProperty)p.PropertyDefinition).IsKey);
        }
    }
}
