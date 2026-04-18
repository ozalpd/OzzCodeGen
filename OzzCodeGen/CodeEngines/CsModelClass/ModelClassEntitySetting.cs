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
        public bool GenerateQueryParam
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

        public IEnumerable<ModelPropertySetting> SearchableProperties
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

        IEnumerable<ModelPropertySetting> SearchableComplexProperties
        {
            get
            {
                return SearchableProperties.Where(p => p.IsComplex)
                                           .OrderBy(p => p.PropertyDefinition.DisplayOrder);
            }
        }

        public IEnumerable<ModelPropertySetting> SearchableNonRangeProperties
        {
            get { return SearchableSimpleProperties.Where(p => !p.IsDateTime && !p.IsFractionalNumeric); }
        }

        public IEnumerable<ModelPropertySetting> SearchableRangeProperties
        {
            get { return SearchableSimpleProperties.Where(p => p.IsDateTime || p.IsFractionalNumeric); }
        }

        public IEnumerable<ModelPropertySetting> SearchableSimpleProperties
        {
            get
            {
                return SearchableProperties.Where(p => p.IsSimpleOrString)
                                           .OrderBy(p => p.PropertyDefinition.DisplayOrder);
            }
        }
    }
}
