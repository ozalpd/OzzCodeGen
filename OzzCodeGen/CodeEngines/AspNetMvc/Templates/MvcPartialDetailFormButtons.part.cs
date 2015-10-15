namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialDetailFormButtons
    {
        public MvcPartialDetailFormButtons(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_DetailFormButtonsPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
