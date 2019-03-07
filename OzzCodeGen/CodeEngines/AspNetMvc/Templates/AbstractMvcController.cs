using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.Templates.Cs;
using OzzUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public abstract partial class AbstractMvcController : CsClassBase
    {
        public AbstractMvcController(AspNetMvcEngine codeEngine, bool customFile = false)
        {
            CodeEngine = codeEngine;
            CustomFile = customFile;
        }

        public AbstractMvcController(AspNetMvcEntitySetting entity, bool customFile = false)
        {
            Entity = entity;
            CodeEngine = entity.CodeEngine;
            CustomFile = customFile;
        }

        public AspNetMvcEntitySetting Entity { get; private set; }
        public AspNetMvcEngine CodeEngine { get; private set; }
        public bool CustomFile { get; private set; }
        public ResxEngine Resx { get { return CodeEngine.ResxEngine; } }

        /// <summary>
        /// Returns roleName, methodName dictionary
        /// </summary>
        protected Dictionary<string, string> IsUserInRoleMethods
        {
            get
            {
                if (_isUserInRoleMethods == null)
                {
                    _isUserInRoleMethods = CodeEngine.IsUserInRoleMethods;
                }
                return _isUserInRoleMethods;
            }
        }
        private Dictionary<string, string> _isUserInRoleMethods;



        public string DisplayMember
        {
            get
            {
                return Entity?.EntityDefinition == null ?
                            "Entity is null!" : Entity.EntityDefinition.DisplayMember;
            }
        }

        public AspNetMvcPropertySetting DisplayProperty
        {
            get
            {
                if (_displayProperty == null)
                    SetDisplayProperty();
                return _displayProperty;
            }
        }

        private void SetDisplayProperty()
        {
            _displayProperty = Entity.Properties
                                .FirstOrDefault(p => p.Name.Equals(DisplayMember));
            if (_displayProperty == null)
            {
                var parts = DisplayMember.Split('.');
                if (parts.Length == 2)
                {
                    var ent = Entity
                                .CodeEngine
                                .Entities
                                .FirstOrDefault(e => e.Name.Equals(parts[0]));
                    if (ent != null)
                        _displayProperty = ent.Properties
                                            .FirstOrDefault(p => p.Name.Equals(parts[1]));
                }
                //TODO: maybe => else if(parts.Length == 3)
            }
        }
        private AspNetMvcPropertySetting _displayProperty;

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = base.DefaultUsingNamespaceList();
            namespaces.AddUnique("System.Text",
                                "System.Net",
                                "System.Web.Mvc",
                                "System.Data",
                                "System.Data.Entity",
                                "System.Threading.Tasks");
            namespaces.AddUnique(CodeEngine.DataModelsNamespace);
            namespaces.AddUnique(CodeEngine.ViewModelsNamespace);
            namespaces.AddUnique(CodeEngine.ModelsNamespace);
            if (Resx != null)
            {
                namespaces.AddUnique(Resx.NamespaceName);
            }
            return namespaces;
        }

        public string CanCreateAttrib
        {
            get
            {
                return CodeEngine.GetAuthorizeAttrib(Entity.RolesCanCreate);
            }
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

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            var rolesPath = Path.Combine(CodeEngine.TargetModelsDir, MvcSecurityRoles.DefaultFileName);
            if (!File.Exists(rolesPath))
            {
                CodeEngine.RenderSecurityRoles();
            }

            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
