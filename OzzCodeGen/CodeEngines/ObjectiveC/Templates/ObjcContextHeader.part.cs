using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public partial class ObjcContextHeader : BaseObjcHeader
    {
        public ObjcContextHeader(ObjcEntitySetting entity)
            : base(ObjcHeaderType.EntityContext, entity)
        {

        }

        public ObjcContextHeader( ObjcEngine codeEngine)
            : base(ObjcHeaderType.DbContext, codeEngine)
        {

        }

        protected override List<string> GetDefaultImports()
        {
            var imports = base.GetDefaultImports();
            switch (HeaderType)
            {
                case ObjcHeaderType.EntityContext:
                    if (CodeEngine.BaseEntityContext == "NSObject")
                    {
                        imports.Add(string.Format("\"{0}.h\"", CodeEngine.SqliteHelper));
                    }
                    else
                    {
                        imports.Add(string.Format("\"{0}.h\"", CodeEngine.BaseEntityContext));
                    }
                    imports.Add(string.Format("\"{0}.h\"", Entity.ObjectiveCName));
                    break;

                case ObjcHeaderType.DbContext:
                    imports.Add(string.Format("\"{0}.h\"", CodeEngine.SqliteHelper));
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
