using System;
using System.ComponentModel;
using System.Windows;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for NewEntity.xaml
    /// </summary>
    public partial class NewEntityDialog : Window, INotifyPropertyChanged
    {
        public NewEntityDialog()
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
            //txtEntityName.Focus();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEntityName.Focus();
            txtEntityName.SelectAll();
        }
    }
}
