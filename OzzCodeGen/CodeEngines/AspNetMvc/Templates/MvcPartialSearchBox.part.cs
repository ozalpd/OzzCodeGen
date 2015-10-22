using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcPartialSearchBox
    {
        public MvcPartialSearchBox(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string ViewName = "_SearchBoxPartial";
        public static string DefaultFileName = ViewName + ".cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
