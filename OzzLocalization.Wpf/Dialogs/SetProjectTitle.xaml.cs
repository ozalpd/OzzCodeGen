using OzzLocalization.Wpf.Models;
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

namespace OzzLocalization.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for SetProjectTitle.xaml
    /// </summary>
    public partial class SetProjectTitle : Window, INotifyPropertyChanged
    {
        public SetProjectTitle()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtTitle.Focus();
            txtTitle.SelectAll();
        }


        public ProjectDir Project
        {
            set
            {
                _project = value;
                RaisePropertyChanged("Project");
            }
            get
            {
                return _project;
            }
        }
        private ProjectDir _project;



        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
