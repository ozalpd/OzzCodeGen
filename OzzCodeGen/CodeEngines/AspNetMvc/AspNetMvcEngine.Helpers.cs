using OzzCodeGen.Definitions;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public partial class AspNetMvcEngine
    {
        public void SetSaveParameterToControllers()
        {
            foreach (var item in Entities)
            {
                item.SaveParameter = SaveParameter;
            }
        }

        /// <summary>
        /// Gets AspNetMvcEntitySetting instance of property's object type
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public AspNetMvcEntitySetting GetForeignKeyEntity(AspNetMvcPropertySetting property)
        {
            if (property == null)
                return null;

            var entity = (AspNetMvcEntitySetting)property.EntitySetting;
            var complexProp = entity.GetForeignDependentProperty(property);
            if (complexProp == null)
                return null;

            return Entities
                    .FirstOrDefault(e => e.EntityDefinition.Name.Equals(complexProp.PropertyDefinition.TypeName));
        }

        #region Helpers for Authorization
        public void SetRolesToControllers()
        {
            foreach (var item in Entities)
            {
                item.RolesCanDelete = RolesCanDelete;
                item.RolesCanEdit = RolesCanEdit;
                item.RolesCanView = RolesCanView;
                item.RolesCanCreate = RolesCanCreate;
            }

            RefreshSecurityRoles();
        }

        /// <summary>
        /// Returns [Authorize(Roles = \"roleName\")] attribute or empty string depending on roleName parameter.
        /// </summary>
        /// <param name="roleName">If value is 'users' returns [Authorize]. If value is empty string or 'everyone' returns empty string.</param>
        /// <returns></returns>
        public string GetAuthorizeAttrib(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) || roleName.Equals(NoAuthorizationRole, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            else if (roleName.Equals(AuthorizeForAllUsersRole, StringComparison.InvariantCultureIgnoreCase))
            {
                return "[Authorize]";
            }
            else
            {
                return string.Format("[Authorize(Roles = \"{0}\")]", roleName);
            }
        }
        public static string NoAuthorizationRole = "everyone";
        public static string AuthorizeForAllUsersRole = "users";

        /// <summary>
        /// RoleName, MethodName dictionary
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> IsUserInRoleMethods
        {
            get
            {
                RefreshSecurityRoles();
                var _isUserInRoleMethods = new Dictionary<string, string>();
                _isUserInRoleMethods.Add(AdminRole, "IsUser" + AdminRole.ToPascalCase());

                var roles = SecurityRoles.Where(r => !r.Equals(AdminRole));
                foreach (var role in roles)
                {
                    _isUserInRoleMethods.Add(role, "IsUser" + role.ToPascalCase());
                }

                return _isUserInRoleMethods;
            }
        }
        
        public string MergeIsUserMethods(string[] roles, bool isAsync = true)
        {
            if (roles == null || roles.Length == 0 || roles.Contains(NoAuthorizationRole))
                return "true";

            var sb = new StringBuilder();
            sb.Append("Request.IsAuthenticated");
            if (roles.Contains(AuthorizeForAllUsersRole))
                return sb.ToString();

            int i = 0; int j = (4 * (sb.Length / 4)) + 16;
            sb.Append(" && (");
            foreach (var role in roles)
            {
                if (i > 0)
                {
                    sb.Append(" ||");
                }
                sb.Append("\r\n");
                sb.Append(' ', j);
                if (isAsync) sb.Append("await ");
                sb.Append(IsUserInRoleMethods[role]);
                if (isAsync) sb.Append("Async");
                sb.Append("()");

                i++;
            }
            sb.Append(")");

            return sb.ToString();
        }

        public ObservableCollection<string> SecurityRoles
        {
            get
            {
                if (_securityRoles == null)
                    _securityRoles = new ObservableCollection<string>();
                return _securityRoles;
            }
            set
            {
                _securityRoles = value;
                RaisePropertyChanged("SecurityRoles");
            }
        }
        private ObservableCollection<string> _securityRoles;


        public void RefreshSecurityRoles()
        {
            if (string.IsNullOrEmpty(AdminRole))
                AdminRole = "admin";

            AppendSecurityRoles(AdminRole);

            if (!string.IsNullOrEmpty(RolesCanCreate))
                AppendSecurityRoles(RolesCanCreate.Split(','));
            if (!string.IsNullOrEmpty(RolesCanEdit))
                AppendSecurityRoles(RolesCanEdit.Split(','));
            if (!string.IsNullOrEmpty(RolesCanDelete))
                AppendSecurityRoles(RolesCanDelete.Split(','));
            if (!string.IsNullOrEmpty(RolesCanView))
                AppendSecurityRoles(RolesCanView.Split(','));

            if (Entities != null)
            {
                foreach (var entity in Entities)
                {
                    AppendSecurityRoles(entity.RolesCanCreateToArray());
                    AppendSecurityRoles(entity.RolesCanEditToArray());
                    AppendSecurityRoles(entity.RolesCanDeleteToArray());
                    AppendSecurityRoles(entity.RolesCanViewToArray());
                }
            }

            SecurityRoles = new ObservableCollection<string>(SecurityRoles.OrderBy(x => x));
        }

        private void AppendSecurityRoles(params string[] roles)
        {
            if (roles == null)
                return;

            foreach (var role in roles)
            {
                AddSecurityRole(role);
            }
        }

        public void AddSecurityRole(string role)
        {
            string s = FormatRole(role);
            if (!SecurityRoles.Contains(s) && !rolesMeanDifferent.Contains(s))
            {
                SecurityRoles.Add(s);
            }
        }

        private string FormatRole(string role)
        {
            return role.ToLowerInvariant().Trim();
        }
        string[] rolesMeanDifferent = { AuthorizeForAllUsersRole, NoAuthorizationRole };


        public string AdminRole
        {
            get { return _adminRole; }
            set
            {
                string oldValue = _adminRole;
                onAdminRoleChanging(value);
                onAdminRoleChanged(oldValue);
            }
        }
        private string _adminRole;
        protected virtual void onAdminRoleChanging(string newValue)
        {
            _adminRole = newValue.ToPascalCase().ToLowerInvariant();
        }
        protected virtual void onAdminRoleChanged(string oldValue)
        {
            RaisePropertyChanged("AdminRole");
            if (string.IsNullOrEmpty(oldValue))
                return;

            if (SecurityRoles.Contains(oldValue))
            {
                SecurityRoles.Remove(oldValue);
                SecurityRoles.Insert(0, AdminRole);
            }

            if (!string.IsNullOrEmpty(RolesCanCreate))
                RolesCanCreate = RolesCanCreate.Replace(oldValue, AdminRole);
            if (!string.IsNullOrEmpty(RolesCanEdit))
                RolesCanEdit = RolesCanEdit.Replace(oldValue, AdminRole);
            if (!string.IsNullOrEmpty(RolesCanDelete))
                RolesCanDelete = RolesCanDelete.Replace(oldValue, AdminRole);
            if (!string.IsNullOrEmpty(RolesCanView))
                RolesCanView = RolesCanView.Replace(oldValue, AdminRole);
            foreach (var entity in Entities)
            {
                if (!string.IsNullOrEmpty(entity.RolesCanCreate))
                    entity.RolesCanCreate = entity.RolesCanCreate.Replace(oldValue, AdminRole);
                if (!string.IsNullOrEmpty(entity.RolesCanEdit))
                    entity.RolesCanEdit = entity.RolesCanEdit.Replace(oldValue, AdminRole);
                if (!string.IsNullOrEmpty(entity.RolesCanDelete))
                    entity.RolesCanDelete = entity.RolesCanDelete.Replace(oldValue, AdminRole);
                if (!string.IsNullOrEmpty(entity.RolesCanView))
                    entity.RolesCanView = entity.RolesCanView.Replace(oldValue, AdminRole);
            }
        }
        #endregion


        #region Event methods called by this.Project
        protected override void OnTargetDirectoryChanged()
        {
            base.OnTargetDirectoryChanged();
            RaisePropertyChanged("TargetControllersDir");
            RaisePropertyChanged("TargetViewsDir");
            RaisePropertyChanged("TargetModelsDir");
        }

        public override void OnProjectNameChanged(string oldValue)
        {
            if (DataContextClass.Equals(GetDefaultDataContextClass(oldValue)))
                DataContextClass = GetDefaultDataContextClass(Project.Name);

            TargetFolder = TargetFolder.Replace(oldValue, Project.Name);
        }

        public override void OnProjectNamespaceChanged(string oldValue)
        {
            base.OnProjectNamespaceChanged(oldValue);

            if (ControllersNamespace.Equals(GetDefaultControllersNamespace(oldValue)))
                ControllersNamespace = GetDefaultControllersNamespace(Project.NamespaceName);

            if (ModelsNamespace.Equals(GetDefaultModelsNamespace(oldValue)))
                ModelsNamespace = GetDefaultModelsNamespace(Project.NamespaceName);

            if (DataModelsNamespace.Equals(GetDefaultDataModelsNamespace(oldValue)))
                DataModelsNamespace = GetDefaultDataModelsNamespace(Project.NamespaceName);

            if (ViewModelsNamespace.Equals(GetDefaultViewModelsNamespace(oldValue)))
                ViewModelsNamespace = GetDefaultViewModelsNamespace(Project.NamespaceName);

            if (ViewsNamespace.Equals(GetDefaultViewsNamespace(oldValue)))
                ViewsNamespace = GetDefaultViewsNamespace(Project.NamespaceName);
        }
        #endregion

    }
}
