using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
   public partial class MvcPartialPager : AbstractMvcView
    {
        public MvcPartialPager(AspNetMvcEntitySetting entity) : base(entity) { }

        public static string DefaultFileName = "_PagerPartial.cshtml";

        public override string GetDefaultFileName()
        {
            return DefaultFileName;
        }
    }
}
