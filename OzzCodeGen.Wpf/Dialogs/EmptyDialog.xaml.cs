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
using System.Windows.Shapes;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for EmptyDialog.xaml
    /// </summary>
    public partial class EmptyDialog : Window
    {
        public EmptyDialog()
        {
            InitializeComponent();
        }

        public void PutUserControl(UserControl userControl)
        {
            userControl.Visibility = Visibility.Collapsed;
            MainGrid.Children.Add(userControl);
            userControl.Visibility = Visibility.Visible;
        }
    }
}
