namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialModalDialog : AbstractMvcView
    {
        public MvcPartialModalDialog(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_MessageBoxPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
