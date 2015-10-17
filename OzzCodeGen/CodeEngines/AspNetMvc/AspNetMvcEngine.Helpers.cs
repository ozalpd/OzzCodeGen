using OzzUtils;
using System.Collections.ObjectModel;
using System.Linq;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public partial class AspNetMvcEngine
    {
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
        string[] rolesMeanDifferent = { "users", "everyone" };
    }
}
