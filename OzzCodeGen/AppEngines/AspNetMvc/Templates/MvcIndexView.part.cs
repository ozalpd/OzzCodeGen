using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.AspNetMvc.Templates
{
    public partial class MvcIndexView : AbstractMvcView
    {
        public MvcIndexView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return "Index.cshtml";
        }
    }
}
