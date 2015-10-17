using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcViewSnippets
    {
        public MvcViewSnippets(AspNetMvcEntitySetting entity) : base(entity)
        {
            CodeEngine = entity.CodeEngine;
        }

        public AspNetMvcEngine CodeEngine { get; private set; }


        public override string GetDefaultFileName()
        {
            return Entity.Name + "Snippets.cshtml";
        }

        public override string GetDefaultFilePath()
        {
            return Path.Combine(CodeEngine.SnippetsDir, GetDefaultFileName());
        }
    }
}
