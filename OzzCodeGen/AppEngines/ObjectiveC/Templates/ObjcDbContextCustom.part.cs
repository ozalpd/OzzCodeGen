using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcDbContextCustom : BaseObjcClassImpl
    {
        public ObjcDbContextCustom(ObjcEngine appEngine)
            : base(appEngine)
        {
            ObjectiveCName = appEngine.ClassPrefix + "DataContext";
        }

        public override BaseObjcHeader GetDefaultHeader()
        {
            var header = new ObjcHeader(ObjcHeaderType.EmptyClass, AppEngine)
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
