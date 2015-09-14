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
        }
    }
}
