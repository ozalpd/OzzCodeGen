using OzzCodeGen.Templates.Cs;
using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcSecurityRoles : CsClassBase
    {
        public MvcSecurityRoles(AspNetMvcEngine codeEngine)
        {
            CodeEngine = codeEngine;
        }

        public AspNetMvcEngine CodeEngine { get; private set; }

        public static string DefaultFileName = "SecurityRoles.gen.cs";
        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }

        public string GetDefaultFilePath()
        {
            return Path.Combine(CodeEngine.TargetModelsDir, GetDefaultFileName());
        }
    }
}
