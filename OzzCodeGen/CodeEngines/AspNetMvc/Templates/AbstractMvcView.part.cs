using OzzCodeGen.CodeEngines.Localization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OzzUtils;
using System.Text;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public abstract partial class AbstractMvcView : AbstractTemplate
    {
        public AbstractMvcView(AspNetMvcEntitySetting entity, bool partialContainer = false)
        {
            Entity = entity;
            PartialContainer = partialContainer;
        }

        public AspNetMvcEntitySetting Entity { get; private set; }
        public bool PartialView { get; protected set; }
        public bool PartialContainer { get; protected set; }

        public ResxEngine Resx { get { return Entity.CodeEngine.ResxEngine; } }
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

        public string DisplayMember
        {
            get
            {
                return Entity.EntityDefinition.DisplayMember;
            }
        }

        public List<string> UsingNamespaces
        {
            get
            {
                if (_usingNamespaces == null)
                {
                    _usingNamespaces = DefaultUsingNamespaceList();
                }
                return _usingNamespaces;
            }
        }
        List<string> _usingNamespaces;

        protected virtual List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            namespaces.AddUnique(Entity.CodeEngine.ViewModelsNamespace);
            namespaces.AddUnique(GetControllersNamespace());
            namespaces.AddUnique(Entity.CodeEngine.ModelsNamespace);
            namespaces.AddUnique(Entity.CodeEngine.DataModelsNamespace);
            if (!string.IsNullOrEmpty(Entity.CodeEngine.AddtNamespacesForView))
            {
                var addt = Entity.CodeEngine.AddtNamespacesForView.Split(';');
                foreach (var item in addt)
                {
                    namespaces.AddUnique(item.Trim());
                }
            }

            if (Resx != null)
            {
                namespaces.AddUnique(Resx.NamespaceName);
            }

            return namespaces;
        }

        protected virtual string GetControllersNamespace()
        {
            if (string.IsNullOrEmpty(Entity?.Area))
            {
                return Entity.CodeEngine.ControllersNamespace;
            }
            else
            {
                string[] parts = Entity.CodeEngine.ControllersNamespace.Split('.');
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

        public virtual string GetDefaultFilePath()
        {
            if (string.IsNullOrEmpty(Entity?.Area))
            {
                return Path.Combine(Entity.CodeEngine.TargetViewsDir, Entity.ControllerName, GetDefaultFileName());
            }
            else
            {
                return Path.Combine(Entity.CodeEngine.TargetDirectory,
                                    "Areas",
                                    Entity.Area,
                                    Entity.CodeEngine.TargetViewsFolder,
                                    Entity.ControllerName,
                                    GetDefaultFileName());
            }
        }
    }
}
