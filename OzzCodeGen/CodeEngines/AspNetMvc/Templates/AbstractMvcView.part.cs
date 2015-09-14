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
            if (Entity.CodeEngine.ResxEngine != null)
            {
                namespaces.Add("Resources");
            }

            return namespaces;
        }

        public string GetDefaultFilePath()
        {
            return Path.Combine(Entity.CodeEngine.TargetViewsDir, Entity.ControllerName, GetDefaultFileName());
        }
    }
}
