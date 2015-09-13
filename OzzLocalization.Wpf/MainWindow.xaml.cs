using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace OzzLocalization.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            settingsFile = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(
                            System.Reflection.Assembly.GetExecutingAssembly().Location),
                            "OzzVocabularyEditor.settings");
        }
        string settingsFile;

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }



        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Settings.MainWindowPosition.GetWindowPositions(this);
            //Settings.SaveToFile(settingsFile);
        }
    }
}
