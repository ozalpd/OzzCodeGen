using OzzCodeGen.CodeEngines.CsModelClass;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.CSharp
{
    public abstract class BaseCSharpEntitySetting<T> : AbstractEntitySetting<T> where T : BaseCSharpPropertySetting
    {
        protected abstract BaseCodeEngine GetCodeEngine();

        public virtual string GetTypeName(bool isNullable)
        {
            var engine = GetCodeEngine();
            bool isClassic = engine?.Project?.TargetPlatform == TargetDotNetPlatform.DotNetFramework;

            return isClassic && !isNullable ? Name : $"{Name}?";
        }


        /// <summary>
        /// Gets the model class entity settings associated with this entity, if available.
        /// </summary>
        /// <remarks>This property returns the corresponding model class entity settings from the model class code engine,
        /// based on the entity's name. If the settings are not available or the code engine is not initialized, the property
        /// returns null.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public ModelClassEntitySetting ModelClassEntitySetting
        {
            get
            {
                if (_modelClassEntity == null && this is ModelClassEntitySetting modelClassEntity)
                {
                    _modelClassEntity = modelClassEntity;
                }
                else if (_modelClassEntity == null)
                {
                    var codeEngine = GetCodeEngine();
                    if (codeEngine != null && codeEngine is BaseAppInfraCodeEngine appInfraEngine)
                        _modelClassEntity = appInfraEngine.ModelClassCodeEngine?
                                                          .Entities.FirstOrDefault(e => e.Name == Name);
                }

                return _modelClassEntity;
            }
        }
        private ModelClassEntitySetting _modelClassEntity;

        [XmlIgnore]
        [JsonIgnore]
        public bool GenerateQueryParam => ModelClassEntitySetting?.GenerateQueryParam == true;

        IEnumerable<ModelPropertySetting> SearchableComplexProperties
        {
            get
            {
                return SearchableProperties.Where(p => p.IsComplex)
                                           .OrderBy(p => p.PropertyDefinition.DisplayOrder);
            }
        }

        /// <summary>
        /// Gets a value indicating whether query parameter classes should be generated for the current entity based on its
        /// settings and searchable properties.
        /// </summary>
        /// <remarks>Query parameter generation is enabled when the entity's model class settings specify that
        /// query parameters should be generated and there is at least one searchable property, either as a simple search
        /// field or as a range field. This property is typically used by code generation engines to determine whether to
        /// emit query parameter classes for filtering or searching scenarios.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public bool HasSearchableProperties => GenerateQueryParam
                                            && (SearchableNonRangeProperties.Any() || SearchableRangeProperties.Any());

        public IEnumerable<ModelPropertySetting> SearchableProperties
        {
            get
            {
                if (ModelClassEntitySetting != null)
                    return ModelClassEntitySetting.SearchableProperties;
                else
                    return Enumerable.Empty<ModelPropertySetting>();
            }
        }
        /// <summary>
        /// Gets an enumerable collection of model properties that are searchable and not used for range-based filtering.
        /// </summary>
        /// <remarks>This property returns only those properties that are considered searchable but do not
        /// represent minimum or maximum values for range queries. If the associated model class entity setting is not
        /// available, the collection will be empty.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<ModelPropertySetting> SearchableNonRangeProperties
        {
            get { return SearchableSimpleProperties.Where(p => !p.IsDateTime && !p.IsFractionalNumeric); }
        }

        /// <summary>
        /// Gets the collection of model properties that support range-based search operations.
        /// </summary>
        /// <remarks>This property returns only those properties that are configured to allow searching by a
        /// range, such as minimum and maximum values. If no such properties are defined, the collection is empty.</remarks>
        [XmlIgnore]
        [JsonIgnore]
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
