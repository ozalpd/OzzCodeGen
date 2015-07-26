using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.AspNetMvc.Templates
{
    public partial class MvcEditView : AbstractMvcView
    {
        public MvcEditView(AspNetMvcEntitySetting entity) : base(entity) { }
        public MvcEditView(AspNetMvcEntitySetting entity, bool createForm)
            : base(entity)
        {
            CreateForm = createForm;
        }

        public bool CreateForm { get; private set; }

        public override string GetDefaultFileName()
        {
            if (CreateForm)
            {
                return "Create.cshtml";
            }
            else
            {
                return "Edit.cshtml";
            }
        }
    }
}
