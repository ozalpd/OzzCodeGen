using System;
using System.Collections.Generic;
using System.Linq;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines
{
    public abstract class AbstractEntitySetting<T> : BaseEntitySetting where T : BasePropertySetting
    {
        /// <summary>
        /// Time stamp property name for creation time of the entity
        /// </summary>
        public string CreatedAtName
        {
            get
            {
                if (string.IsNullOrEmpty(_createdAtProptyName)
                    && GetInheritedIncludedProperties().Any(p => p.PropertyDefinition.Name == _defaultCreatedAtProptyName))
                    _createdAtProptyName = _defaultCreatedAtProptyName;

                return _createdAtProptyName;
            }
            set
            {
                if (_createdAtProptyName == value) return;
                _createdAtProptyName = value != null ? value.Replace(" ", "") : string.Empty;
                RaisePropertyChanged("CreatedAtName");
            }
        }
        private string _createdAtProptyName;
        // Default time stamp property name for creation time of the entity
        // CreateDate name is used in TSqlScriptsEngine for same puprpose to generate table and set value in insert stored procedure,
        // for preventing complications we use CreatedAt as default value for CreatedAtProperty.
        private static readonly string _defaultCreatedAtProptyName = "CreatedAt";

        /// <summary>
        /// Property name for update time of the entity
        /// </summary>
        public string UpdatedAtName
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedAtProptyName)
                    && GetInheritedIncludedProperties().Any(p => p.PropertyDefinition.Name == _defaultModifiedAtProptyName))
                    _updatedAtProptyName = _defaultModifiedAtProptyName;

                return _updatedAtProptyName;
            }
            set
            {
                if (_updatedAtProptyName == value) return;
                _updatedAtProptyName = value != null ? value.Replace(" ", "") : string.Empty;
                RaisePropertyChanged("UpdatedAtName");
            }
        }
        private string _updatedAtProptyName;
        // Default time stamp property name for update time of the entity
        // ModifyDate name is used in TSqlScriptsEngine for same puprpose to generate table and set value in update stored procedure,
        // for preventing complications we use UpdatedAt as default value for UpdatedAtName.
        private static readonly string _defaultModifiedAtProptyName = "UpdatedAt";


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

        /// <summary>
        /// Returns InheritedProperties those Exclude is false
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetInheritedIncludedProperties()
        {
            return GetInheritedProperties()
                    .Where(c => c.Exclude == false)
                    .OrderBy(c => c.PropertyDefinition.DisplayOrder);
        }

        public T GetPrimaryKey()
        {
            return GetInheritedSimpleProperties()
                    .FirstOrDefault(p => ((SimpleProperty)p.PropertyDefinition).IsKey);
        }

        /// <summary>
        /// Returns instance of property's DependentProperty instance from this or ancestors of this entity
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public T GetForeignDependentProperty(T property)
        {
            if (property == null || !property.IsForeignKey())
                return null;
            
            return GetInheritedComplexProperties()
                                .FirstOrDefault(p => ((ComplexProperty)p.PropertyDefinition).DependentPropertyName == property.Name);
        }

        public bool HasDisplayOrderProperty()
        {
            return GetInheritedIncludedProperties().Any(p => p.Name.Equals("DisplayOrder", StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasIsActiveProperty()
        {
            return GetInheritedIncludedProperties().Any(p => p.Name.Equals("IsActive", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
