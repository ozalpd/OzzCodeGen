using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class IndexScriptsPartial
    {
        public IndexScriptsPartial(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string DefaultFileName = "_IndexScriptsPartial.cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
