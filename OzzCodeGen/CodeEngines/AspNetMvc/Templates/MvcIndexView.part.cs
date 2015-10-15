using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcIndexView : AbstractMvcView
    {
        public MvcIndexView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return "Index.cshtml";
        }

        public string ModalDialogPartial
        {
            get
            {
                return Path.GetFileNameWithoutExtension(ModalDialog.DefaultFileName);
            }
        }


        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            string sharedViewsDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(FilePath)), "Shared");

            string modalFilePath = Path.Combine(sharedViewsDir, ModalDialog.DefaultFileName);
            if (!File.Exists(modalFilePath))
            {
                var modalTemplate = new ModalDialog(Entity);
                modalTemplate.WriteToFile(modalFilePath, false);
            }

            string pagerFilePath = Path.Combine(sharedViewsDir, PagerPartial.DefaultFileName);
            if (!File.Exists(pagerFilePath))
            {
                var pagerTemplate = new PagerPartial(Entity);
                pagerTemplate.WriteToFile(pagerFilePath, false);
            }

            string scriptFilePath = Path.Combine(sharedViewsDir, IndexScriptsPartial.DefaultFileName);
            if (!File.Exists(scriptFilePath))
            {
                var scriptTemplate = new IndexScriptsPartial(Entity);
                scriptTemplate.WriteToFile(scriptFilePath, false);
            }

            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
