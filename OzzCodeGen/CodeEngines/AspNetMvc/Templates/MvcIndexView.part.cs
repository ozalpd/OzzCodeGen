using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcIndexView : AbstracMvcIndexView
    {
        public MvcIndexView(AspNetMvcEntitySetting entity) : base(entity) { }
    }
}
