using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcAgGridIndexView : AbstracMvcIndexView
    {
        public MvcAgGridIndexView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            string sharedViewsDir = GetSharedViewsDir(filePath);
            string scriptFilePath = Path.Combine(sharedViewsDir, MvcPartialAgGridScripts.DefaultFileName);

            if (!File.Exists(scriptFilePath))
            {
                var scriptTemplate = new MvcPartialAgGridScripts(Entity);
                scriptTemplate.WriteToFile(scriptFilePath, false);
            }

            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
