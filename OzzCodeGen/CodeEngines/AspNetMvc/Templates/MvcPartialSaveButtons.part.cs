namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialSaveButtons
    {
        public MvcPartialSaveButtons(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_SaveButtonsPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
