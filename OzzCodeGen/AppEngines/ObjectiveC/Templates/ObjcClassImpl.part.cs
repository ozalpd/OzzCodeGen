using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcClassImpl : BaseObjcClassImpl
    {
        public ObjcClassImpl(ObjcEntitySetting entity)
            : base(entity) { }

        public override BaseObjcHeader GetDefaultHeader()
        {
            return new ObjcHeader(ObjcHeaderType.EntityClass, Entity)
            {
                ObjectiveCName = ObjectiveCName
            };
        }
    }
}
