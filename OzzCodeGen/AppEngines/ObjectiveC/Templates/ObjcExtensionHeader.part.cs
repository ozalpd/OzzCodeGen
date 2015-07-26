using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcExtensionHeader : BaseObjcHeader
    {
        public ObjcExtensionHeader(ObjcEntitySetting entity, ObjExtendMethod extensionMethod)
            : base(ObjcHeaderType.ClassExtension, entity) 
        {
            ExtensionMethod = extensionMethod;
        }

        public string ExtensionName { get; set; }
        public ObjExtendMethod ExtensionMethod { get; private set; }
    }
}
