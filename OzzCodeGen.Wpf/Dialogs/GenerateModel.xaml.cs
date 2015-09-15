using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OzzCodeGen.Providers;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for GenerateModel.xaml
    /// </summary>
    public partial class GenerateModel : Window, INotifyPropertyChanged
    {
        public GenerateModel()
        {
            InitializeComponent();
        }


        public CodeGenProject Project
        {
            get { return _project; }
            set
            {
                if (_project == value) return;
                _project = value;
                DataModel = _project.DataModel;
                RaisePropertyChanged("Project");
            }
        }
        private CodeGenProject _project;
        
        public DataModel DataModel
        {
            get { return _dataModel; }
            set
            {
                if (_dataModel == value) return;
                _dataModel = value;
                btnAccept.IsEnabled = _dataModel != null;
                RaisePropertyChanged("DataModel");
            }
        }
        private DataModel _dataModel;
        
        public IModelProvider SelectedModelProvider
        {
            get { return _selectedModelProvider; }
            set
            {
                if (_selectedModelProvider == value) return;
                _selectedModelProvider = value;
                btnOpenSource.IsEnabled = _selectedModelProvider != null;
                RaisePropertyChanged("SelectedModelProvider");
            }
        }
        private IModelProvider _selectedModelProvider;
        
        public List<IModelProvider> ModelProviders
        {
            get { return _modelProviders; }
            set
            {
                if (_modelProviders == value) return;
                _modelProviders = value;
                RaisePropertyChanged("ModelProviders");
            }
        }
        private List<IModelProvider> _modelProviders;

        public string DefaultsFolder
        {
            set
            {
                _defaultsFolder = value;
                RaisePropertyChanged("DefaultsFolder");
            }
            get { return _defaultsFolder; }
        }
        private string _defaultsFolder;


        private void btnOpenSource_Click(object sender, RoutedEventArgs e)
        {
            IModelProvider provider = SelectedModelProvider;
            if (provider is EmptyModel)
            {
                ((EmptyModel)provider).DefaultsFolder = DefaultsFolder;
            }

            string source = provider.SelectSource();
            if (string.IsNullOrEmpty(source))
                return;

            Project = provider.GenerateProject(source);
            cboProviders.IsEnabled = false;

            FillCboFilterNamespace();
        }

        private void FillCboFilterNamespace()
        {
            cboFilterNamspace.Items.Clear();
            cboFilterNamspace.Items.Add(cboFilterNamspaceDefault);

            foreach (var entity in Project.DataModel.OrderBy(e => e.NamespaceName))
            {
                if (!cboFilterNamspace.Items.Contains(entity.NamespaceName))
                {
                    cboFilterNamspace.Items.Add(entity.NamespaceName);
                }
            }

            cboFilterNamspace.SelectedItem = cboFilterNamspaceDefault;
            cboFilterNamspace.IsEnabled = cboFilterNamspace.Items.Count > 2;
        }
        const string cboFilterNamspaceDefault = "All Namespaces";

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (!cboFilterNamspaceDefault.Equals(cboFilterNamspace.SelectedItem))
            {
                string s = cboFilterNamspace.SelectedItem.ToString();

                var entities = Project.DataModel
                                .Where(entity => entity.NamespaceName.Equals(s) |
                                    entity.NamespaceName.StartsWith(s + "."))
                                .ToList();

                DataModel filtered = new DataModel();
                foreach (var item in entities)
                {
                    filtered.Add(item);
                }

                Project.DataModel = filtered;
            }
            DialogResult = true;
        }

        private void cboFilterNamspace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

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
