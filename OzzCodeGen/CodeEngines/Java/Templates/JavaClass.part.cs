using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Java.Templates
{
    public partial class JavaClass : AbstractJavaTemplate
    {
        public JavaClass(JavaEntitySetting entity)
            : base(entity)
        {
            ClassName = Entity.Name;
        }
    }
}
