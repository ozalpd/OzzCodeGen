using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.AppEngines.Storage;
using System.IO;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcDbContext : BaseObjcClassImpl
    {
        public ObjcDbContext(ObjcEngine appEngine)
            : base(appEngine)
        {
            ObjectiveCName = "BaseDbContext";
        }


        public override BaseObjcHeader GetDefaultHeader()
        {
            return new ObjcContextHeader(AppEngine)
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

            var customTmp = new ObjcDbContextCustom(AppEngine);
            string custFile = Path.Combine(base.GetDirectoryName(FilePath), customTmp.GetDefaultFileName());
            return customTmp.WriteToFile(custFile, true) & isWritten;
        }
    }
}
