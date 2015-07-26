using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public partial class ObjcClassExtension : BaseObjcClassImpl
    {
        public ObjcClassExtension(string objectiveCName, ObjcEntitySetting entity)
            : base(entity)
        {
            ObjectiveCName = objectiveCName;
            ExtensionMethod = ObjExtendMethod.SubClass;
        }

        public ObjcClassExtension(ObjcEntitySetting entity)
            : base(entity)
        {
            ExtensionMethod = ObjExtendMethod.SubClass;
        }

        public List<string> HeaderImports
        {
            get
            {
                if (_headerImports == null)
                {
                    _headerImports = new List<string>();
                }
                return _headerImports;
            }
        }
        List<string> _headerImports;


        public string ExtensionName
        {
            set { _extensionName = value; }
            get
            {
                if (string.IsNullOrEmpty(_extensionName))
                {
                    _extensionName = ObjectiveCName + "Extension";
                }
                return _extensionName;
            }
        }
        private string _extensionName;

        public ObjExtendMethod ExtensionMethod { get; set; }

        public override BaseObjcHeader GetDefaultHeader()
        {
            var header = new ObjcExtensionHeader(Entity, ObjExtendMethod.SubClass)
            {
                ObjectiveCName = ObjectiveCName
            };
            header.Imports.AddRange(HeaderImports);
            header.ExtensionName = ExtensionName;

            return header;
        }
    }
}
