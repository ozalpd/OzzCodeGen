using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.CodeEngines.Storage;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public partial class ObjcHeader : BaseObjcHeader
    {
        public ObjcHeader(ObjcHeaderType headerType, ObjcEngine codeEngine)
            : base(headerType, codeEngine)
        {
            
        }

        public ObjcHeader(ObjcHeaderType headerType, ObjcEntitySetting entity)
            : base(headerType, entity) 
        {

        }
    }
}
