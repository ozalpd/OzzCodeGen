using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialAgGridScripts
    {
        public MvcPartialAgGridScripts(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_agGridScriptsPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
