using System.Windows;
using OzzCodeGen.UI;
using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.AspNetMvc.UI
{
    /// <summary>
    /// Interaction logic for MvcEngineUI.xaml
    /// </summary>
    public partial class MvcEngineUI : AbstractEngineUI
    {
        public MvcEngineUI()
        {
            InitializeComponent();

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("FilterViewType");
            HiddenColumns.Add("FilteredDisplayColumns");
            HiddenColumns.Add("DisplayColumns");
            //HiddenColumns.Add("GenerateController");
            //HiddenColumns.Add("IndexView");
            //HiddenColumns.Add("DetailsView");
            //HiddenColumns.Add("CreateView");
            //HiddenColumns.Add("EditView");
            HiddenColumns.Add("OverwriteExisting");
            HiddenColumns.Add("StrongForeignKeyProperty");
            HiddenColumns.Add("StrongForeignEntity");

            ColumnWidths["Name"] = 120;
            ColumnWidths.Add("BaseType", 120);
            ColumnWidths.Add("GenerateController", 90);
            ColumnWidths.Add("AfterSaveRedirectToDetails", 90);
            ColumnWidths.Add("IndexView", 40);
            ColumnWidths.Add("SaveParameter", 60);
            ColumnWidths.Add("DetailsView", 40);
            ColumnWidths.Add("CreateView", 40);
            ColumnWidths.Add("EditView", 40);

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        public AspNetMvcEngine Engine { get { return (AspNetMvcEngine)CodeEngine; } }

        private void mnuEntityOptions_Click(object sender, RoutedEventArgs e)
        {
            //var engine = (AspNetMvcEngine)CodeEngine;
            //foreach (var entity in engine.Entities)
            //{
            //    foreach (var item in entity.Properties)
            //    {
                    
            //    }
            //}
        }

        private void btnSetToControllers_Click(object sender, RoutedEventArgs e)
        {
            Engine.SetSaveParameterToControllers();
        }

        private void btnSetRolesToControllers_Click(object sender, RoutedEventArgs e)
        {
            Engine.SetRolesToControllers();
        }

        private void btnRefreshRoles_Click(object sender, RoutedEventArgs e)
        {
            Engine.RefreshSecurityRoles();
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            AddSecurityRole();
        }

        private void AddSecurityRole()
        {
            if (string.IsNullOrEmpty(txtAddRole.Text))
                return;
            Engine.AddSecurityRole(txtAddRole.Text.Replace(" ", ""));
            txtAddRole.Text = string.Empty;
            Engine.RefreshSecurityRoles();
        }

        private void btnRemoveRole_Click(object sender, RoutedEventArgs e)
        {
            if (lstSecurityRoles.SelectedItem == null)
                return;
            Engine.SecurityRoles.Remove((string)lstSecurityRoles.SelectedItem);
            Engine.RefreshSecurityRoles();
        }

        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                AddSecurityRole();
            }
        }

        private void btnRenderRolesClass_Click(object sender, RoutedEventArgs e)
        {
            Engine.RenderSecurityRoles();
        }

        private void lstSecurityRoles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstSecurityRoles.SelectedItem == null)
                return;
            txtAddRole.Text = lstSecurityRoles.SelectedItem.ToString();
        }
    }
}
