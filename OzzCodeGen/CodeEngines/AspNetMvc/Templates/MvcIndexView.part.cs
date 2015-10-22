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
                return Path.GetFileNameWithoutExtension(MvcPartialModalDialog.DefaultFileName);
            }
        }


        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            string sharedViewsDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(FilePath)), "Shared");

            string modalFilePath = Path.Combine(sharedViewsDir, MvcPartialModalDialog.DefaultFileName);
            if (!File.Exists(modalFilePath))
            {
                var modalTemplate = new MvcPartialModalDialog(Entity);
                modalTemplate.WriteToFile(modalFilePath, false);
            }

            string pagerFilePath = Path.Combine(sharedViewsDir, MvcPartialPager.DefaultFileName);
            if (!File.Exists(pagerFilePath))
            {
                var pagerTemplate = new MvcPartialPager(Entity);
                pagerTemplate.WriteToFile(pagerFilePath, false);
            }

            string scriptFilePath = Path.Combine(sharedViewsDir, MvcPartialIndexScripts.DefaultFileName);
            if (!File.Exists(scriptFilePath))
            {
                var scriptTemplate = new MvcPartialIndexScripts(Entity);
                scriptTemplate.WriteToFile(scriptFilePath, false);
            }

            string searchboxPath = Path.Combine(sharedViewsDir, MvcPartialSearchBox.DefaultFileName);
            if (!File.Exists(searchboxPath))
            {
                var searchboxTempl = new MvcPartialSearchBox(Entity);
                searchboxTempl.WriteToFile(searchboxPath, false);
            }

            var snippetTempl = new MvcViewSnippets(Entity);
            snippetTempl.WriteToFile(snippetTempl.GetDefaultFilePath(), true);

            var displayTempl = new MvcDisplayTemplate(Entity);
            displayTempl.WriteToFile(displayTempl.GetDefaultFilePath(), false);

            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
