using OzzCodeGen.CodeEngines.Localization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OzzUtils;

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
            namespaces.AddUnique(Entity.CodeEngine.ModelsNamespace);
            namespaces.AddUnique(Entity.CodeEngine.DataModelsNamespace);
            namespaces.AddUnique(Entity.CodeEngine.ViewModelsNamespace);
            if (Resx != null)
            {
                namespaces.AddUnique(Resx.NamespaceName);
            }

            return namespaces;
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
