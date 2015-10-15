using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialModalDialog : AbstractMvcView
    {
        public MvcPartialModalDialog(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string DefaultFileName = "_MessageBoxPartial.cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
