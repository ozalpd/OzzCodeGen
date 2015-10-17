using OzzCodeGen.Templates.Cs;
using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcBaseController : AbstractMvcController
    {
        public MvcBaseController(AspNetMvcEngine codeEngine)
            : base(codeEngine) { }

        public override string GetDefaultFileName()
        {
            return CodeEngine.BaseControllerName + ".g.cs";
        }

        public string GetDefaultFilePath()
        {
            return Path.Combine(CodeEngine.TargetControllersDir, GetDefaultFileName());
        }
    }
}
