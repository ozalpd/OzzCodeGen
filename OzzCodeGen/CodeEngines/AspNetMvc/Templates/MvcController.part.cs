using OzzCodeGen.CodeEngines.Localization;
using OzzUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public virtual string GetDefaultFilePath()
        {
            if (string.IsNullOrEmpty(Entity?.Area))
            {
                return Path.Combine(Entity.CodeEngine.TargetControllersDir, GetDefaultFileName());
            }
            else
            {
                return Path.Combine(Entity.CodeEngine.TargetDirectory,
                                    "Areas",
                                    Entity.Area,
                                    Entity.CodeEngine.TargetControllersFolder,
                                    GetDefaultFileName());
            }
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var list = base.DefaultUsingNamespaceList();
            if (!string.IsNullOrEmpty(Entity?.Area))
            {
                list.AddUnique(Entity.CodeEngine.ControllersNamespace);
            }
            return list;
        }

        public new string NamespaceName
        {
            get
            {
                if (string.IsNullOrEmpty(base.NamespaceName))
                {
                    base.NamespaceName = GetNamespaceName();
                }
                return base.NamespaceName;
            }
        }

        private string GetNamespaceName()
        {
            if (string.IsNullOrEmpty(Entity?.Area))
            {
                return CodeEngine.ControllersNamespace;
            }
            else
            {
                string[] parts = CodeEngine.ControllersNamespace.Split('.');
                var sb = new StringBuilder();
                if (parts.Length > 1)
                {
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        sb.Append(parts[i]);
                        sb.Append('.');
                    }
                }
                sb.Append("Areas");
                sb.Append('.');
                sb.Append(Entity.Area);
                sb.Append('.');
                sb.Append(parts[parts.Length - 1]);
                return sb.ToString();
            }
        }

        public LocalizationEntitySetting ResxEntity
        {
            get
            {
                if (Resx != null && _resxEntity == null)
                {
                    _resxEntity = Resx
                                    .Entities
                                    .FirstOrDefault(e => e.Name.Equals(Entity.Name));
                }
                return _resxEntity;
            }
        }
        LocalizationEntitySetting _resxEntity;

        public string EntityResource
        {
            get
            {
                if (Resx != null)
                {
                    return Resx.SingleResx ? Resx.SingleResxFilename :
                        ResxEntity != null ? Resx.GetDefaultTargetFile(ResxEntity) : string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
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
