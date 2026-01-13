using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public partial class ObjcDbContextCustom : BaseObjcClassImpl
    {
        public ObjcDbContextCustom(ObjcEngine codeEngine)
            : base(codeEngine)
        {
            ObjectiveCName = codeEngine.ClassPrefix + "DataContext";
        }

        public override BaseObjcHeader GetDefaultHeader()
        {
            var header = new ObjcHeader(ObjcHeaderType.EmptyClass, CodeEngine)
            {
                ObjectiveCName = ObjectiveCName
            };

            header.Imports.Add("\"BaseDbContext-Gen.h\"");
            header.BaseClassName = "BaseDbContext";
            return header;
        }

        protected override string GetDirectoryName(string path)
        {
            return Path.Combine(base.GetDirectoryName(path), "Custom");
        }

        public override string GetDefaultFileName()
        {
            return ObjectiveCName + ".m";
        }
    }
}
