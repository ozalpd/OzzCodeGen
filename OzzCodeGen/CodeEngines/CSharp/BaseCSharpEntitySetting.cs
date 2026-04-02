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
    }
}
