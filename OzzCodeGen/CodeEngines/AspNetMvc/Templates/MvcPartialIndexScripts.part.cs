namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialIndexScripts
    {
        public MvcPartialIndexScripts(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_IndexScriptsPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
