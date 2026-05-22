using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsModelClass
{
    public class ModelClassEntitySetting : BaseModelClassEntitySetting<ModelPropertySetting>
    {
        public override AbstractEntitySetting<ModelPropertySetting> GetBaseEntitySetting()
        {
            if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
                return null;
            var codeEngine = (CSharpModelClassCodeEngine)CodeEngine;

            return codeEngine.Entities
                    .OfType<ModelClassEntitySetting>()
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
        }

        /// <summary>
        /// Generates a QueryParameters helper class for this entity when enabled.
        /// </summary>
        public new bool GenerateQueryParam
        {
            set
            {
                _generateQueryParam = value;
                RaisePropertyChanged(nameof(GenerateQueryParam));
            }
            get
            {
                return _generateQueryParam;
            }
        }
        private bool _generateQueryParam;

        public string GetQueryParamClassName()
        {
            string queryParamClassName = CSharpModelClassCodeEngine.QueryParamClassName;
            return GenerateQueryParam ? $"{EntityDefinition.Name}{queryParamClassName}" : queryParamClassName;
        }

        /// <summary>
        /// Returns a collection of properties from other entities that reference this entity as a complex type.
        /// </summary>
        /// <remarks>This method identifies all properties in the model that are complex types and whose
        /// type name matches the current entity's name. Use this to discover navigation or foreign key relationships
        /// pointing to this entity.</remarks>
        /// <returns>An enumerable collection of <see cref="ModelPropertySetting"/> objects representing properties in other
        /// entities that reference this entity. The collection is ordered by property name.</returns>
        public IEnumerable<ModelPropertySetting> GetReferencingProperties()
        {
            var foreignProperties = CodeEngine.EntitySettings
                                              .OfType<ModelClassEntitySetting>()
                                              .SelectMany(e => e.Properties.OfType<ModelPropertySetting>())
                                              .Where(p => p.IsComplex
                                                       && p.PropertyDefinition.TypeName == EntityDefinition.Name)
                                              .OrderBy(p => p.Name)
                                              .ToList();

            return foreignProperties;
        }

        public new IEnumerable<ModelPropertySetting> SearchableProperties
        {
            get
            {
                if (_searchableProperties == null)
                    _searchableProperties = GetInheritedProperties().OfType<ModelPropertySetting>()
                                                           .Where(p => p.IsSearchParameter)
                                                           .OrderBy(p => p.PropertyDefinition.DisplayOrder);
                return _searchableProperties;
            }
        }
        IEnumerable<ModelPropertySetting> _searchableProperties;
    }
}
