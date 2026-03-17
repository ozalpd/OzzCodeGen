using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.Definitions
{
    /// <summary>
    /// Represents a navigation-like property that points to another entity type.
    /// The name follows classic EF terminology.
    /// </summary>
    public class ComplexProperty : BaseClassProperty
    {
        /// <summary>
        /// Gets the definition type for this property.
        /// </summary>
        public override DefinitionType DefinitionType
        {
            get { return DefinitionType.Complex; }
        }

        /// <summary>
        /// Returns entity names that can be selected as the complex property's target type.
        /// </summary>
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

        /// <summary>
        /// Gets the simple property referenced by <see cref="DependentPropertyName"/>.
        /// The value is resolved once and then cached.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
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

        /// <summary>
        /// Finds the matching simple property on the current entity for <see cref="DependentPropertyName"/>.
        /// </summary>
        /// <returns>The matching dependent property, or <see langword="null"/> when not found.</returns>
        public SimpleProperty GetDependency()
        {
            var dependency = from p in EntityDefinition.Properties.OfType<SimpleProperty>()
                             where p.Name == DependentPropertyName
                             select p;

            return dependency.FirstOrDefault();
        }
    }
}
