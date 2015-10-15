using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcEditView : AbstractMvcView
    {
        public MvcEditView(AspNetMvcEntitySetting entity) : base(entity) { }
        public MvcEditView(AspNetMvcEntitySetting entity, bool createForm)
            : base(entity)
        {
            CreateForm = createForm;
        }

        public bool CreateForm { get; private set; }

        public override string GetDefaultFileName()
        {
            if (CreateForm)
            {
                return "Create.cshtml";
            }
            else
            {
                return "Edit.cshtml";
            }
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            string sharedViewsDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(FilePath)), "Shared");

            string buttonsViewPath = Path.Combine(sharedViewsDir, MvcPartialSaveButtons.DefaultFileName);
            if (!File.Exists(buttonsViewPath))
            {
                var buttonsTemplate = new MvcPartialSaveButtons(Entity);
                buttonsTemplate.WriteToFile(buttonsViewPath, false);
            }

            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
