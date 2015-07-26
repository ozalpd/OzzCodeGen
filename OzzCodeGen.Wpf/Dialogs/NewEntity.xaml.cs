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
using OzzCodeGen.Definitions;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for NewEntity.xaml
    /// </summary>
    public partial class NewEntity : Window, INotifyPropertyChanged
    {
        public NewEntity()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public EntityDefinition EntityDefinition
        {
            get { return _entityDefinition; }
            set
            {
                _entityDefinition = value;
                RaisePropertyChanged("EntityDefinition");
            }
        }
        private EntityDefinition _entityDefinition;

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void window_Activated(object sender, EventArgs e)
        {
            txtName.Focus();
        }
        
    }
}
