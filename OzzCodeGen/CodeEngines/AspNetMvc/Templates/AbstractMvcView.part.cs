using OzzCodeGen.CodeEngines.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public abstract partial class AbstractMvcView : AbstractTemplate
    {
        public AbstractMvcView(AspNetMvcEntitySetting entity)
        {
            Entity = entity;
        }

        public AspNetMvcEntitySetting Entity { get; private set; }

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
            namespaces.Add(Entity.CodeEngine.ViewModelsNamespace);
            namespaces.Add(Entity.CodeEngine.ModelsNamespace);
            if (Resx != null)
            {
                namespaces.Add(Resx.NamespaceName);
            }

            return namespaces;
        }

        public string GetDefaultFilePath()
        {
            return Path.Combine(Entity.CodeEngine.TargetViewsDir, Entity.ControllerName, GetDefaultFileName());
        }
    }
}
