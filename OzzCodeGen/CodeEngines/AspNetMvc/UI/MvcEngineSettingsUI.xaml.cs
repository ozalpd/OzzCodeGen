using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OzzCodeGen.CodeEngines.AspNetMvc.UI
{
    /// <summary>
    /// Interaction logic for MvcEngineSettingsUI.xaml
    /// </summary>
    public partial class MvcEngineSettingsUI : UserControl
    {
        public MvcEngineSettingsUI()
        {
            InitializeComponent();
        }

        private void txtTargetDirectory_LostFocus(object sender, RoutedEventArgs e)
        {
            string targetDir = txtTargetDirectory.Text.Trim();
            if (string.IsNullOrEmpty(targetDir))
            {
                return;
            }

            if (string.IsNullOrEmpty(txtControllersDir.Text.Trim()))
            {
                txtControllersDir.Text = System.IO.Path.Combine(targetDir, "Controllers");
            }
            if (string.IsNullOrEmpty(txtViewsDir.Text.Trim()))
            {
                txtViewsDir.Text = System.IO.Path.Combine(targetDir, "Views");
            }

            //TODO: put this logic in to code engine
            //This view knows nothing about the model
            //so, I'm using a funny way to set default properties
            txtRolesCanDelete.Text = "admin";
            txtRolesCanEdit.Text = "editor, admin";
            txtRolesCanCreate.Text = "editor, admin";
            txtRolesCanView.Text = "everyone";
        }

        //part 2 a funny way to set default properties
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtTargetDirectory.Focus();
            var interval = new TimeSpan(0, 0, 0, 0, 200);

            int i = 0;
            var tmrFocus = new DispatcherTimer();
            tmrFocus.Interval = interval;
            tmrFocus.Start();

            tmrFocus.Tick += (o, a) =>
            {
                switch (i)
                {
                    case 0:
                        txtRolesCanDelete.Focus();
                        break;

                    case 1:
                        txtRolesCanEdit.Focus();
                        break;

                    case 2:
                        txtRolesCanView.Focus();
                        break;

                    case 3:
                        txtRolesCanCreate.Focus();
                        break;

                    case 4:
                        txtTargetDirectory.Focus();
                        txtTargetDirectory.SelectAll();
                        break;

                    default:
                        tmrFocus.Stop();
                        break;
                }
                i++;
            };
        }
    }
}
