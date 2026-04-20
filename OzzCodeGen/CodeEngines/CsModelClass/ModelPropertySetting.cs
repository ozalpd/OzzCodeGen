
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.CsModelClass
{
    public class ModelPropertySetting : BaseModelClassPropertySetting
    {
        protected override BaseCodeEngine GetCodeEngine()
        {
            var entitySetting = (ModelClassEntitySetting)EntitySetting;
            return entitySetting?.CodeEngine;
        }

        public bool IsSearchParameter
        {
            get
            {
                if (!_useInSearch.HasValue)
                {
                    _useInSearch = IsForeignKey ||
                        (PropertyDefinition.IsTypeString() &&
                        ((StringProperty)PropertyDefinition).MaxLength < 257);
                }
                return _useInSearch.Value;
            }
            set
            {
                _useInSearch = value;
                RaisePropertyChanged(nameof(IsSearchParameter));
            }
        }
        private bool? _useInSearch;

    }
}
