using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcContextHeader : BaseObjcHeader
    {
        public ObjcContextHeader(ObjcEntitySetting entity)
            : base(ObjcHeaderType.EntityContext, entity)
        {

        }

        public ObjcContextHeader( ObjcEngine appEngine)
            : base(ObjcHeaderType.DbContext, appEngine)
        {

        }

        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();
            switch (HeaderType)
            {
                case ObjcHeaderType.EntityContext:
                    if (AppEngine.BaseEntityContext == "NSObject")
                    {
                        imports.Add(string.Format("\"{0}.h\"", AppEngine.SqliteHelper));
                    }
                    else
                    {
                        imports.Add(string.Format("\"{0}.h\"", AppEngine.BaseEntityContext));
                    }
                    imports.Add(string.Format("\"{0}.h\"", Entity.ObjectiveCName));
                    break;

                case ObjcHeaderType.DbContext:
                    imports.Add(string.Format("\"{0}.h\"", AppEngine.SqliteHelper));
                    foreach (var entity in StorageEntitySettings)
                    {
                        imports.Add(string.Format("\"{0}Context.h\"", entity.Name));
                    }
                    break;

                default:
                    break;
            }

            return imports;
        }

        public override string GetDefaultFileName()
        {
            return ObjectiveCName + "-Gen.h";
        }
    }
}
