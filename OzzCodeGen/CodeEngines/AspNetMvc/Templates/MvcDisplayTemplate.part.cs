using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcDisplayTemplate
    {
        public MvcDisplayTemplate(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return Entity.Name + ".cshtml";
        }

        public override string GetDefaultFilePath()
        {
            return Path.Combine(Entity.CodeEngine.TargetViewsDir, "Shared", "DisplayTemplates", GetDefaultFileName());
        }
    }
}
