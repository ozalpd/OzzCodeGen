using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.CodeEngines.Storage;
using System.IO;

namespace OzzCodeGen.CodeEngines.ObjectiveC.Templates
{
    public partial class ObjcDbContext : BaseObjcClassImpl
    {
        public ObjcDbContext(ObjcEngine codeEngine)
            : base(codeEngine)
        {
            ObjectiveCName = "BaseDbContext";
        }


        public override BaseObjcHeader GetDefaultHeader()
        {
            return new ObjcContextHeader(CodeEngine)
            {
                ObjectiveCName = ObjectiveCName
            };
        }

        protected override string GetDirectoryName(string path)
        {
            return Path.Combine(base.GetDirectoryName(path), "Generated");
        }

        public override string GetDefaultFileName()
        {
            return ObjectiveCName + "-Gen.m";
        }

        public List<StorageEntitySetting> StorageEntitySettings
        {
            get
            {
                return HeaderTemplate.StorageEntitySettings;
            }
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            bool isWritten = base.WriteToFile(FilePath, overwriteExisting);

            var customTmp = new ObjcDbContextCustom(CodeEngine);
            string custFile = Path.Combine(base.GetDirectoryName(FilePath), customTmp.GetDefaultFileName());
            return customTmp.WriteToFile(custFile, true) & isWritten;
        }
    }
}
