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
using OzzUtils;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for NewProperty.xaml
    /// </summary>
    public partial class NewProperty : Window, INotifyPropertyChanged
    {
        public NewProperty()
        {
            InitializeComponent();
            DependentProperty = BaseProperty.CreatePropertyDefinition("int");
            DependentProperty.Editable = true;
            DependentProperty.UiVisible = true;
            ((SimpleProperty)DependentProperty).IsForeignKey = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BaseProperty PropertyDefinition
        {
            get { return _propertyDefinition; }
            set
            {
                _propertyDefinition = value;
                RaisePropertyChanged("PropertyDefinition");
                OnPropertyDefinitionChanged();
            }
        }
        private BaseProperty _propertyDefinition;

        public BaseProperty DependentProperty
        {
            get { return _dependentProperty; }
            set
            {
                _dependentProperty = value;
                RaisePropertyChanged("DependentProperty");
            }
        }
        private BaseProperty _dependentProperty;

        private void OnPropertyDefinitionChanged()
        {
            PropertyDefinition.PropertyChanged += PropertyDefinition_PropertyChanged;

            if (PropertyDefinition.DefinitionType == DefinitionType.Complex)
            {
                ComplexPropertySettingsContainer.Visibility = System.Windows.Visibility.Visible;
                CollectionPropertySettingsContainer.Visibility = System.Windows.Visibility.Collapsed;
                CreateDependecy = true;
            }
            else if (PropertyDefinition.DefinitionType == DefinitionType.Collection)
            {
                ComplexPropertySettingsContainer.Visibility = System.Windows.Visibility.Visible;
                CollectionPropertySettingsContainer.Visibility = System.Windows.Visibility.Visible;
                CreateDependecy = false;
            }
            else
            {
                ComplexPropertySettingsContainer.Visibility = System.Windows.Visibility.Collapsed;
                CollectionPropertySettingsContainer.Visibility = System.Windows.Visibility.Collapsed;
                btnPropertyName.IsEnabled = false;
                CreateDependecy = false;
            }
        }

        void PropertyDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    if (string.IsNullOrEmpty(prevPropertyName) || DependentProperty.Name.StartsWith(prevPropertyName))
                    {
                        DependentProperty.Name = PropertyDefinition.Name + "Id";
                    }
                    prevPropertyName = PropertyDefinition.Name;
                    break;

                case "TypeName":
                    if (PropertyDefinition.DefinitionType == DefinitionType.Collection)
                    {
                        DependentEntityName = PropertyDefinition
                                                .TypeName
                                                .Replace("ICollection<", "")
                                                .Replace(">", "");
                    }
                    break;

                default:
                    break;
            }
        }
        string prevPropertyName = string.Empty;



        public string DependentEntityName
        {
            get { return _dependentEntityName; }
            set
            {
                _dependentEntityName = value;
                RaisePropertyChanged("DependentEntityName");
            }
        }
        string _dependentEntityName;


        public bool CreateDependecy
        {
            get { return _createDependecy; }
            set
            {
                _createDependecy = value;
                RaisePropertyChanged("CreateDependecy");
            }
        }
        private bool _createDependecy;

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnPropertyName_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyDefinition.DefinitionType == DefinitionType.Collection)
            {
                PropertyDefinition.Name = DependentEntityName.Pluralize();
            }
            else
            {
                PropertyDefinition.Name = PropertyDefinition.TypeName;
            }
        }
    }
}
