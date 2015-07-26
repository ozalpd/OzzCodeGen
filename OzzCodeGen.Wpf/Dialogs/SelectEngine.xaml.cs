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
using System.Windows.Shapes;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectEngine.xaml
    /// </summary>
    public partial class SelectEngine : Window, INotifyPropertyChanged
    {
        public SelectEngine()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cboEngines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedEngine = cboEngines.SelectedItem.ToString();
        }


        public List<string> Engines
        {
            set
            {
                if (_engines == value) return;
                _engines = value;
                cboEngines.ItemsSource = null;
                RaisePropertyChanged("Engines");
                cboEngines.ItemsSource = Engines;
            }
            get { return _engines; }
        }
        private List<string> _engines;

        public string SelectedEngine
        {
            set
            {
                if (_selectedEngine == value) return;
                _selectedEngine = value;
                btnAccept.IsEnabled = !string.IsNullOrEmpty(_selectedEngine);
                RaisePropertyChanged("SelectedEngine");
            }
            get { return _selectedEngine; }
        }
        private string _selectedEngine;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
