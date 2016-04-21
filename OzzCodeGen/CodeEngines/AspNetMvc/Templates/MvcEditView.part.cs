using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcEditView : AbstractMvcView
    {
        public MvcEditView(AspNetMvcEntitySetting entity, bool createForm = false, bool partialContainer = false)
            : base(entity, partialContainer)
        {
            CreateForm = createForm;
            if (createForm)
            {
                PartialView = entity.CreatePartialView;
            }
            else
            {
                PartialView = entity.EditPartialView;
            }
            PartialView = partialContainer ? false : PartialView;
        }

        public bool CreateForm { get; private set; }

        public string PartialViewName
        {
            get
            {
                return CreateForm ? "_Create" : "_Edit";
            }
        }

        public override string GetDefaultFileName()
        {
            if (PartialView)
            {
                return PartialViewName + ".cshtml";
            }
            else if (CreateForm)
            {
                return "Create.cshtml";
            }
            else
            {
                return "Edit.cshtml";
            }
        }

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            string sharedViewsDir = Path.Combine(Path.GetDirectoryName(folderPath), "Shared");
            string buttonsViewPath = Path.Combine(sharedViewsDir, MvcPartialSaveButtons.DefaultFileName);
            if (!File.Exists(buttonsViewPath))
            {
                var buttonsTemplate = new MvcPartialSaveButtons(Entity);
                buttonsTemplate.WriteToFile(buttonsViewPath, false);
            }

            if (PartialView)
            {
                var container = new MvcEditView(Entity, CreateForm, true);
                container.WriteToFile(container.GetDefaultFilePath(), overwriteExisting);
            }

            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
