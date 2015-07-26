using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.AppEngines.Storage;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcHeader : BaseObjcHeader
    {
        public ObjcHeader(ObjcHeaderType headerType, ObjcEngine appEngine)
            : base(headerType, appEngine)
        {
            
        }

        public ObjcHeader(ObjcHeaderType headerType, ObjcEntitySetting entity)
            : base(headerType, entity) 
        {

        }
    }
}
