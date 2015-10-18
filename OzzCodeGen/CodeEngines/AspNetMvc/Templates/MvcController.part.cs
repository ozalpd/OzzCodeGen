using OzzUtils;
using System.Collections.Generic;
using System.IO;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcController : AbstractMvcController
    {
        public MvcController(AspNetMvcEntitySetting entity, bool customFile = false)
            : base(entity, customFile) { }

        public override string GetDefaultFileName()
        {
            if (CustomFile)
            {
                return Entity.ControllerName + "Controller.part.cs";
            }
            else
            {
                return Entity.ControllerName + "Controller.g.cs";
            }
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = base.DefaultUsingNamespaceList();
            namespaces.AddUnique(
                        "System.Net",
                        "System.Web.Mvc");
            return namespaces;
        }

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            if (!CustomFile)
            {
                var customFile = new MvcController(Entity, true);
                string customPath = Path.Combine(Path.GetDirectoryName(filePath), customFile.GetDefaultFileName());
                customFile.WriteToFile(customPath, false);
            }
            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
