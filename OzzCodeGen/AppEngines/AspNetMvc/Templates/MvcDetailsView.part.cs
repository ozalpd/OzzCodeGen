using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.AspNetMvc.Templates
{
    public partial class MvcDetailsView : AbstractMvcView
    {
        public MvcDetailsView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return "Details.cshtml";
        }
    }
}
