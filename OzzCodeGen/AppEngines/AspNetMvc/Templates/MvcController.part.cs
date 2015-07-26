using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Templates.Cs;

namespace OzzCodeGen.AppEngines.AspNetMvc.Templates
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
            namespaces.Add("System.Web");
            namespaces.Add("System.Web.Mvc");
            namespaces.Add(Entity.AppEngine.ViewModelsNamespace);
            namespaces.Add(Entity.AppEngine.ModelsNamespace);

            return namespaces;
        }

        public string CanEditAttrib
        {
            get
            {
                return Entity.GetAuthorizeAttrib(Entity.WhoCanEdit);
            }
        }

        public string CanDeleteAttrib
        {
            get
            {
                return Entity.GetAuthorizeAttrib(Entity.WhoCanDelete);
            }
        }

        public string CanViewAttrib
        {
            get
            {
                return Entity.GetAuthorizeAttrib(Entity.WhoCanView);
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
