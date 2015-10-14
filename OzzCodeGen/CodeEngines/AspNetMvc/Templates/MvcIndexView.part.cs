using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
