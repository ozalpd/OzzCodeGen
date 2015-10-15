using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcDetailsView : AbstractMvcView
    {
        public MvcDetailsView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return "Details.cshtml";
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            string sharedViewsDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(FilePath)), "Shared");

            string buttonsViewPath = Path.Combine(sharedViewsDir, MvcPartialDetailFormButtons.DefaultFileName);
            if (!File.Exists(buttonsViewPath))
            {
                var buttonsTemplate = new MvcPartialDetailFormButtons(Entity);
                buttonsTemplate.WriteToFile(buttonsViewPath, false);
            }

            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
