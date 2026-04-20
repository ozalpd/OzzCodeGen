using OzzCodeGen.CodeEngines.Mvvm.Templates;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public abstract class BaseCSharpWpfMvvmTemplate : BaseCSharpMvvmTemplate
    {
        protected BaseCSharpWpfMvvmTemplate(WpfMvvmCodeEngine codeEngine, WpfMvvmEntitySetting entitySetting = null)
        {
            CodeEngine = codeEngine;
            EntitySetting = entitySetting;
        }

        public WpfMvvmCodeEngine CodeEngine { get; }

        public WpfMvvmEntitySetting EntitySetting { get; }
    }
}
