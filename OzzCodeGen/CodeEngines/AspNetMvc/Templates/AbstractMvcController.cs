using OzzCodeGen.Templates.Cs;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Returns roleName, methodName dictionary
        /// </summary>
        protected Dictionary<string,string> IsUserInRoleMethods
        {
            get
            {
                if (_isUserInRoleMethods == null)
                {
                    _isUserInRoleMethods = new Dictionary<string, string>();
                    _isUserInRoleMethods.Add(CodeEngine.AdminRole, "IsUser" + CodeEngine.AdminRole.ToPascalCase());
                    var roles = CodeEngine.SecurityRoles.Where(r => !r.Equals(CodeEngine.AdminRole));
                    foreach (var role in roles)
                    {
                        _isUserInRoleMethods.Add(role.Trim(), "IsUser" + role.ToPascalCase());
                    }

                }
                return _isUserInRoleMethods;
            }
        }
        Dictionary<string, string> _isUserInRoleMethods;



        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = base.DefaultUsingNamespaceList();
            namespaces.AddUnique("System.Threading.Tasks");
            namespaces.AddUnique(CodeEngine.ModelsNamespace);
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
