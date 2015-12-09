using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcJQueryAutoComplete
    {
        public MvcJQueryAutoComplete(AspNetMvcEntitySetting entity, AspNetMvcPropertySetting fKeyProperty)
            : base(entity, fKeyProperty)
        { }

        public override string GetDefaultFileName()
        {
            return "_AutoComplete" + Entity.Name + ".cshtml";
        }

        public override string GetDefaultFilePath()
        {
            var folder = RelatedEntity == null ? "IHas" + Entity.Name : RelatedEntity.ControllerName;
            return Path.Combine(CodeEngine.SnippetsDir, folder, GetDefaultFileName());
        }
    }
}
