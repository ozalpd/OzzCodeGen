using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcDetailsView : AbstractMvcView
    {
        public MvcDetailsView(AspNetMvcEntitySetting entity, bool partialContainer = false)
            : base(entity, partialContainer)
        {
            PartialView = !partialContainer && entity.DetailsPartialView;
        }

        public override string GetDefaultFileName()
        {
            if (PartialView)
            {
                return "_Details.cshtml";
            }
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
            if (PartialView)
            {
                var container = new MvcDetailsView(Entity, true);
                container.WriteToFile(container.GetDefaultFilePath(), overwriteExisting);
            }
            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
