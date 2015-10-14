using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Templates.Cs;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcController : CsClassBase
    {
        public MvcController(AspNetMvcEntitySetting entity)
        {
            Entity = entity;
        }

        public MvcController(AspNetMvcEntitySetting entity, bool customFile)
        {
            Entity = entity;
            CustomFile = customFile;
        }

        public AspNetMvcEntitySetting Entity { get; private set; }
        public bool CustomFile { get; private set; }
        public AspNetMvcEngine CodeEngine { get { return Entity.CodeEngine; } }

        public override string GetDefaultFileName()
        {
            if (CustomFile)
            {
                return Entity.ControllerName + "Controller.Cust.cs";
            }
            else
            {
                return Entity.ControllerName + "Controller.Gen.cs";
            }
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = base.DefaultUsingNamespaceList();
            namespaces.Add("System.Data");
            namespaces.Add("System.Data.Entity");
            namespaces.Add("System.Net");
            namespaces.Add("System.Web.Mvc");
            namespaces.Add("System.Threading.Tasks");
            namespaces.Add(CodeEngine.ViewModelsNamespace);
            namespaces.Add(CodeEngine.ModelsNamespace);

            return namespaces;
        }

        public string CanEditAttrib
        {
            get
            {
                return CodeEngine.GetAuthorizeAttrib(Entity.RolesCanEdit);
            }
        }

        public string CanDeleteAttrib
        {
            get
            {
                return CodeEngine.GetAuthorizeAttrib(Entity.RolesCanDelete);
            }
        }

        public string CanViewAttrib
        {
            get
            {
                return CodeEngine.GetAuthorizeAttrib(Entity.RolesCanView);
            }
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            if (!CustomFile)
            {
                var customFile = new MvcController(Entity, true);
                string customPath = Path.Combine(Path.GetDirectoryName(FilePath), customFile.GetDefaultFileName());
                customFile.WriteToFile(customPath, false);
            }
            return base.WriteToFile(FilePath, overwriteExisting);
        }
    }
}
