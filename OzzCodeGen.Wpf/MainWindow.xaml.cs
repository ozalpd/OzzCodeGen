using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OzzCodeGen.Definitions;
using Microsoft.Win32;
using OzzCodeGen.Wpf.Dialogs;
using System.ComponentModel;
using OzzCodeGen.Wpf.Models;
using System.IO;
using OzzCodeGen.CodeEngines;
using OzzCodeGen.Providers;
using System.Windows.Threading;

namespace OzzCodeGen.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = string.Format("Ozz Code Generator - {0}", BuildInfo.Date);
            MainContainer.IsEnabled = false;
            settingsFile = Path.Combine(
                            Path.GetDirectoryName(
                            System.Reflection.Assembly.GetExecutingAssembly().Location),
                            "OzzCodeGen.settings");

            if (File.Exists(settingsFile))
            {
                Settings = AppSettings.OpenFile(settingsFile);
            }
            else
            {
                Settings = new AppSettings();
            }

            Settings.MainWindowPosition.SetWindowPositions(this);

            SimplePropertyGrid.Visibility = Visibility.Collapsed;
            StringPropertyGrid.Visibility = Visibility.Collapsed;
            ClassPropertyGrid.Visibility = Visibility.Collapsed;
            CollectionPropertyGrid.Visibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            grdProperties.Items.CurrentChanged += OnPropertiesGridItemChanged;
        }

        private void OnPropertiesGridItemChanged(object sender, EventArgs e)
        {
            SimplePropertyGrid.Visibility = BoolToVisibility(grdProperties.SelectedItem is SimpleProperty);
            StringPropertyGrid.Visibility = BoolToVisibility(grdProperties.SelectedItem is StringProperty);
            ClassPropertyGrid.Visibility = BoolToVisibility(grdProperties.SelectedItem is BaseClassProperty);
            CollectionPropertyGrid.Visibility = BoolToVisibility(grdProperties.SelectedItem is CollectionProperty);
            if (grdProperties.SelectedItem is BaseClassProperty)
            {
                lblPropertyHeader.Text = "Property's Dependency Definitions";
            }
            else
            {
                lblPropertyHeader.Text = "Property's Definitions";
            }
        }

        private void cboCodeEngines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cboCodeEngines.SelectedItem;
            if (selectedItem == null)
            {
                return; //TODO: Niye null oluyor arastir...
            }
            string strEngine = selectedItem.ToString();
            var engine = Project.GetCodeEngine(strEngine);
            if (engine != null)
            {
                Project.CurrentCodeEngine = engine;
            }
            Project.CodeEngineList.Remove(strEngine);
            Project.CodeEngineList.Insert(0, strEngine);
        }

        private void AddCodeEngineName(List<string> engines, string engineId)
        {
            if (!Project.CodeEngineList.Contains(engineId))
            {
                engines.Add(engineId);
            }
        }

        private List<string> GetCodeEngines()
        {
            var engines = new List<string>();

            AddCodeEngineName(engines, EngineTypes.MetadataCodeEngineId);
            AddCodeEngineName(engines, EngineTypes.LocalizationResxGenId);
            AddCodeEngineName(engines, EngineTypes.AspNetMvcEngineId);
            AddCodeEngineName(engines, EngineTypes.TSqlScriptsId);
            AddCodeEngineName(engines, EngineTypes.SqliteScriptsId);
            AddCodeEngineName(engines, EngineTypes.AndroidEngineId);
            AddCodeEngineName(engines, EngineTypes.ObjcEngineId);
            AddCodeEngineName(engines, EngineTypes.EfDbFirstDataLayerId);

            return engines;
        }

        private void btnAddTarget_Click(object sender, RoutedEventArgs e)
        {
            SelectEngine dlgEngine = new SelectEngine();
            dlgEngine.Engines = GetCodeEngines();
            dlgEngine.Owner = this;

            if (dlgEngine.ShowDialog() ?? false)
            {
                var engine = EngineTypes.GetInstance(dlgEngine.SelectedEngine);
                //engine.TargetFolder = engine.GetDefaultTargetFolder();
                var settingsUI = engine.GetSettingsDlgUI();
                Project.AddEngine(engine);
                if (settingsUI != null)
                {
                    var dlg = new EmptyDialog();
                    dlg.Owner = this;
                    dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    dlg.PutUserControl(settingsUI);
                    settingsUI.DataContext = engine;
                    dlg.ShowDialog();
                }
            }
        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {
            GenerateModel modelGenDlg = new GenerateModel();
            modelGenDlg.ModelProviders = GetModelProviders();
            modelGenDlg.Owner = this;
            modelGenDlg.DefaultsFolder = GetDefaultsFolder();
            if (modelGenDlg.ShowDialog() ?? false)
            {
                Project = modelGenDlg.Project;
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Code Generator Model Files|*" + fileExtension;
            if (openDlg.ShowDialog(this) ?? false)
            {
                OpenProject(openDlg.FileName);
            }
        }



        private string GetDefaultsFolder()
        {
            if (string.IsNullOrEmpty(Settings.DefaultsFolder)
                || !Directory.Exists(Settings.DefaultsFolder))
            {
                var openDlg = new OpenFileDialog();
                openDlg.Filter = "DefaultEmptyProvider project file|" + EmptyModel.ProjectTemplateFile;
                if (openDlg.ShowDialog(this) ?? false)
                {
                    Settings.DefaultsFolder = Path.GetDirectoryName(openDlg.FileName);
                }
            }

            return Settings.DefaultsFolder;
        }

        protected List<IModelProvider> GetModelProviders()
        {
            var modelProviders = new List<IModelProvider>();
            modelProviders.Add(new EmptyModel());
            modelProviders.Add(new OzzCodeGen.Providers.Ef.Ef5());
            modelProviders.Add(new OzzCodeGen.Providers.SourceCode.ObjectiveC());

            return modelProviders;
        }

        private void OpenProject(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("File \"{0}\" not found!", fileName));
            }
            CodeGenProject project = CodeGenProject.OpenFile(fileName);
            Settings.AddToRecentFiles(fileName);
            Project = project;
            Project.HasProjectChanges = false;

            Project.ModelProvider = GetModelProviders().First(m => m.ProviderId == Project.ModelProviderId);
            btnRefresh.IsEnabled = Project.ModelProvider.CanRefresh;
        }

        const string fileExtension = ".OzzGen";

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        private void SaveProject()
        {
            if (string.IsNullOrEmpty(Project.SavedFileName) ||
                !System.IO.File.Exists(Project.SavedFileName))
            {
                SaveModelAs();
            }
            else
            {
                Project.SaveToFile();
            }
        }

        private void SaveModelAs()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();

            saveDlg.Filter = "Code Generator Model Files|*" + fileExtension;
            if ((saveDlg.ShowDialog(this) ?? false) &&
                !string.IsNullOrEmpty(saveDlg.FileName))
            {
                string fileName;
                if (System.IO.Path.GetExtension(saveDlg.FileName).Equals(fileExtension))
                {
                    fileName = saveDlg.FileName;
                }
                else
                {
                    fileName = saveDlg.FileName + fileExtension;
                }
                Project.SaveToFile(fileName);
                Settings.AddToRecentFiles(fileName);
                Settings.SaveToFile(settingsFile);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!Project.ModelProvider.CanRefresh) return;

            Project.RefreshDataModel(true);
            RefreshEntitiesGrid();
        }

        private Visibility BoolToVisibility(bool b)
        {
            if (b) return Visibility.Visible;
            else return Visibility.Collapsed;
        }


        public FileDefinition SelectedProjectFile
        {
            get { return _selectedProjectFile; }
            set
            {
                if (_selectedProjectFile == value)
                {
                    return;
                }

                if (value != null && File.Exists(value.FullPath))
                {
                    OpenProject(value.FullPath);
                }
                else if (value != null) //means not File.Exists(value.FullPath)
                {
                    AskToRemoveFromRecentFiles(value);
                }
                _selectedProjectFile = value;
                RaisePropertyChanged("SelectedProjectFile");
            }
        }
        private FileDefinition _selectedProjectFile;

        private void AskToRemoveFromRecentFiles(FileDefinition file)
        {
            var dlgResult = MessageBox.Show(
                    string.Format("Project file \"{0}\" does not exist! Do you want to remove the reference to the file from Recent File list?", file.Name),
                    "File Not Foud!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation);
            if (dlgResult == MessageBoxResult.Yes)
            {
                var recentFiles = Settings.RecentProjectFiles.Where(f => f != file);
                var timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(100000);
                timer.Tick += (object sender, EventArgs e) =>
                {
                    cboRecentProjects.SelectedItem = null;
                    Settings.RecentProjectFiles = recentFiles.ToList();
                    timer.Stop();
                };
                timer.Start();
            }
        }
        
        public AppSettings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == value) return;
                _settings = value;
                RaisePropertyChanged("Settings");
            }
        }
        AppSettings _settings;
        string settingsFile;
         

        public CodeGenProject Project
        {
            get { return _project; }
            set
            {
                if (_project == value) return;
                _project = value;
                DataModel = _project.DataModel;
                grdEntities.ItemsSource = DataModel;
                cboCodeEngines.ItemsSource = null;
                cboCodeEngines.ItemsSource = Project.CodeEngineList;

                grdEnums.ItemsSource = null;
                grdEnums.ItemsSource = Project.EnumDefinitions;

                _project.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "CurrentCodeEngine")
                    {
                        SetTargetProjectUI();
                    }
                    else if (e.PropertyName == "CodeEngineList")
                    {
                        cboCodeEngines.ItemsSource = null;
                        cboCodeEngines.ItemsSource = Project.CodeEngineList;
                    }
                };
                RaisePropertyChanged("Project");
                MainContainer.IsEnabled = true;
                SetTargetProjectUI();
            }
        }

        public List<string> CodeEngineList { get { return Project.CodeEngineList; } }

        public EntityDefinition SelectedEntity
        {
            get
            {
                if (grdEntities.SelectedItem == null)
                    return null;
                return (EntityDefinition)grdEntities.SelectedItem;
            }
        }

        public BaseProperty SelectedProperty
        {
            get
            {
                if (grdProperties.SelectedItem == null)
                    return null;
                return (BaseProperty)grdProperties.SelectedItem;
            }
        }

        private void SetTargetProjectUI()
        {
            if (Project.CurrentCodeEngine == null) return;

            if (engineUI != null) engineUI.Visibility = Visibility.Collapsed;
            engineUI = Project.CurrentCodeEngine.UiControl;
            if (this.TargetGrid.Children.Contains(engineUI))
            {
                engineUI.Visibility = Visibility.Visible;
            }
            else
            {
                this.TargetGrid.Children.Add(engineUI);
                Grid.SetColumn(engineUI, 0);
                Grid.SetRow(engineUI, 1);
            }
        }
        private CodeGenProject _project;
        private UserControl engineUI;

        public DataModel DataModel
        {
            get { return _dataModel; }
            set
            {
                if (_dataModel == value) return;
                _dataModel = value;
                RaisePropertyChanged("DataModel");
            }
        }
        DataModel _dataModel;

        public DataModel EnumDefinitions
        {
            get { return _enumDefinitions; }
            set
            {
                if (_enumDefinitions == value) return;
                _enumDefinitions = value;
                RaisePropertyChanged("EnumDefinitions");
            }
        }
        DataModel _enumDefinitions;

        private void window_Closing(object sender, CancelEventArgs e)
        {
            Settings.MainWindowPosition.GetWindowPositions(this);
            Settings.SaveToFile(settingsFile);
        }

        private BaseProperty GetSelectedProperty()
        {
            if (grdProperties.SelectedItem is BaseProperty)
            {
                return (BaseProperty)grdProperties.SelectedItem;
            }
            else
            {
                MessageBox.Show("No selected property found!", "No Settings!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private NewEntityDialog GetNewEntityDialog()
        {
            var dlg = new NewEntityDialog();
            dlg.EntityDefinition = new EntityDefinition()
            {
                Name = "Untitled",
                NamespaceName = Project.NamespaceName,
                DisplayOrder = Project.DataModel.Count
            };
            dlg.Closed += NewEntityDialog_Closed;
            dlg.Owner = this;
            return dlg;
        }

        void NewEntityDialog_Closed(object sender, EventArgs e)
        {
            var dlg = (NewEntityDialog)sender;
            if (dlg.DialogResult ?? false)
            {
                DataModel.Add(dlg.EntityDefinition);
                dlg.EntityDefinition.DisplayName = string.Empty;
                RefreshEntitiesGrid();
                grdEntities.SelectedIndex = grdEntities.Items.Count - 1;
            }
            dlg.Closed-=NewEntityDialog_Closed;
        }

        private void mnuAddEntity_Click(object sender, RoutedEventArgs e)
        {
            var dlg = GetNewEntityDialog();
            var tag = ((MenuItem)sender).Tag;
            if (tag != null && tag.ToString() == "Inherited")
                dlg.EntityDefinition.BaseTypeName = ((EntityDefinition)grdEntities.SelectedItem).Name;
            dlg.ShowDialog();
        }

        private void mnuDuplicateEntity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity == null) return;
            var dlg = GetNewEntityDialog();
            dlg.EntityDefinition = SelectedEntity.Clone();

            dlg.ShowDialog();
        }

        private void btnAddEntity_Click(object sender, RoutedEventArgs e)
        {
            mnuAddEntityMenu.IsOpen = true;
        }

        private void btnRemoveEntity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity == null ||
                MessageBox.Show(string.Format("Are you sure to delete {0}?", SelectedEntity.Name),
                        string.Format("Deleting {0}!", SelectedEntity.Name),
                        MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            Project.RemoveEntity(SelectedEntity);
            RefreshEntitiesGrid();
        }


        private void btnMoveEntityTop_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity != null)
            {
                DataModel.MoveTop(SelectedEntity);
                RefreshEntitiesGrid();
                SetEnumMemberGridIndex(0, grdEntities);
            }
        }

        private void btnMoveEntityUp_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity != null)
            {
                int i = grdEntities.SelectedIndex - 1;
                DataModel.MoveUp(SelectedEntity);
                RefreshEntitiesGrid();
                SetEnumMemberGridIndex(i, grdEntities);
            }
        }

        private void btnMoveEntityDown_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity != null)
            {
                int i = grdEntities.SelectedIndex + 1;
                DataModel.MoveDown(SelectedEntity);
                RefreshEntitiesGrid();
                SetEnumMemberGridIndex(i, grdEntities);
            }
        }

        private void btnMoveEntityBottom_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity != null)
            {
                DataModel.MoveBottom(SelectedEntity);
                RefreshEntitiesGrid();
                SetEnumMemberGridIndex(grdEntities.Items.Count - 1, grdEntities);
            }
        }

        public void AddProperty(DefinitionType propertyType, EntityDefinition entity)
        {
            var dlg = new NewPropertyDialog();
            var p = BaseProperty.CreatePropertyDefinition(propertyType, "UntitledProperty");
            p.EntityDefinition = entity;
            dlg.PropertyDefinition = p;

            dlg.Closed += NewPropertyDialog_Closed;
            dlg.PropertyDefinition.TypeName = dlg.PropertyDefinition.UsableTypeNames.FirstOrDefault();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        void NewPropertyDialog_Closed(object sender, EventArgs e)
        {
            var dlg = (NewPropertyDialog)sender;
            if (dlg.DialogResult ?? false)
            {
                int i = grdEntities.SelectedIndex;
                var property = dlg.PropertyDefinition;
                var entity = property.EntityDefinition;

                if (property.DefinitionType == DefinitionType.Complex && dlg.CreateDependecy)
                {
                    entity.Properties.Add(dlg.DependentProperty, true);
                    dlg.DependentProperty.DisplayName = string.Empty;
                    ((BaseClassProperty)property).DependentPropertyName = dlg.DependentProperty.Name;
                    ((BaseClassProperty)property).DependentPropertyType = dlg.DependentProperty.TypeName;
                }
                entity.Properties.Add(property, true);
                property.DisplayName = string.Empty;

                grdEntities.SelectedIndex = -1;
                grdEntities.SelectedIndex = i;
                int j = grdProperties.Items.Count - 1;
                grdProperties.SelectedIndex = j;
            }

            dlg.Closed -= NewPropertyDialog_Closed;
        }

        private void btnAddProperty_Click(object sender, RoutedEventArgs e)
        {
            mnuAddProperty.IsOpen = true;
        }

        private void btnRemoveProperty_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty == null ||
                MessageBox.Show(string.Format("Are you sure to delete {0}?", SelectedProperty.Name),
                        string.Format("Deleting {0}!", SelectedProperty.Name),
                        MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            
            var properties = SelectedProperty.EntityDefinition.Properties;
            properties.Remove(SelectedProperty);
            RefreshPropertiesGrid();
        }

        private void mnuAddProperty_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntity == null) return;
            var menuItem = (MenuItem)sender;
            DefinitionType propertyType;
            switch (menuItem.Tag.ToString())
            {
                case "string":
                    propertyType = DefinitionType.String;
                    break;

                case "complex":
                    propertyType = DefinitionType.Complex;
                    break;

                case "collection":
                    propertyType = DefinitionType.Collection;
                    break;

                default:
                    propertyType = DefinitionType.Simple;
                    break;
            }

            AddProperty(propertyType, SelectedEntity);
        }

        private void btnOrderPropertiesByName_Click(object sender, RoutedEventArgs e)
        {
            var properties = SelectedProperty.EntityDefinition.Properties;
            var ordered = properties.OrderByDescending(p => p.Name).ToList();
            foreach (var item in ordered)
            {
                properties.MoveTop(item);
            }
            RefreshPropertiesGrid();
        }

        private void btnMovePropertyTop_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty != null)
            {
                var properties = SelectedProperty.EntityDefinition.Properties;
                properties.MoveTop(SelectedProperty);
                RefreshPropertiesGrid();
                SetEnumMemberGridIndex(0, grdProperties);
            }
        }

        private void btnMovePropertyUp_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty != null)
            {
                var properties = SelectedProperty.EntityDefinition.Properties;
                int i = grdProperties.SelectedIndex - 1;
                properties.MoveUp(SelectedProperty);
                RefreshPropertiesGrid();
                SetEnumMemberGridIndex(i, grdProperties);
            }
        }

        private void btnMovePropertyDown_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty != null)
            {
                var properties = SelectedProperty.EntityDefinition.Properties;
                int i = grdProperties.SelectedIndex + 1;
                properties.MoveDown(SelectedProperty);
                RefreshPropertiesGrid();
                SetEnumMemberGridIndex(i, grdProperties);
            }
        }

        private void btnMovePropertyBottom_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty != null)
            {
                var properties = SelectedProperty.EntityDefinition.Properties;
                properties.MoveBottom(SelectedProperty);
                RefreshPropertiesGrid();
                SetEnumMemberGridIndex(grdProperties.Items.Count - 1, grdProperties);
            }
        }

        private void RefreshEntitiesGrid()
        {
            grdEntities.ItemsSource = null;
            grdEntities.ItemsSource = DataModel;
        }

        private void RefreshPropertiesGrid()
        {
            int i = grdEntities.SelectedIndex;
            grdEntities.SelectedIndex = -1;
            grdEntities.SelectedIndex = i;
        }

        private void RefreshEnumMemberGrid()
        {
            int i = grdEnums.SelectedIndex;
            grdEnums.SelectedIndex = -1;
            grdEnums.SelectedIndex = i;
        }

        private void SetEnumMemberGridIndex(int index, DataGrid grid)
        {
            if (grid.Items.Count > 0)
            {
                if (index < 0) index = 0;
                if (index > grid.Items.Count - 1)
                {
                    index = grid.Items.Count - 1;
                }
                grid.SelectedIndex = index;
            }
        }



        private EnumMember GetSelectedEnumMember()
        {
            if (grdEnumMembers.SelectedItem == null)
            {
                return null;
            }
            else
            {
                return (EnumMember)grdEnumMembers.SelectedItem;
            }
        }

        private EnumDefinition GetSelectedEnumDefinition()
        {
            if (grdEnums.SelectedItem == null)
            {
                return null;
            }
            else
            {
                return (EnumDefinition)grdEnums.SelectedItem;
            }
        }

        private void btnAddEnum_Click(object sender, RoutedEventArgs e)
        {
            var newEnum = new EnumDefinition()
            {
                Name = "UntitledEnum",
                NamespaceName = Project.NamespaceName
            };
            newEnum.Members.Add(new EnumMember()
            {
                Name = "UntitledMember",
                Value = 1
            });
            grdEnums.ItemsSource = null;
            Project.EnumDefinitions.Add(newEnum);
            grdEnums.ItemsSource = Project.EnumDefinitions;
            grdEnums.SelectedIndex = grdEnums.Items.Count - 1;
        }

        private void btnRemoveEnum_Click(object sender, RoutedEventArgs e)
        {

            var enumDef = GetSelectedEnumDefinition();
            if (enumDef == null ||
                MessageBox.Show(string.Format("Are you sure to delete {0}?", enumDef.Name),
                        string.Format("Deleting {0}!", enumDef.Name),
                        MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            grdEnums.ItemsSource = null;
            Project.EnumDefinitions.Remove(enumDef);
            grdEnums.ItemsSource = Project.EnumDefinitions;
            grdEnums.SelectedIndex = grdEnums.Items.Count - 1;
        }

        private void btnAddEnumMember_Click(object sender, RoutedEventArgs e)
        {
            var enumDef = GetSelectedEnumDefinition();
            if (enumDef != null)
            {
                int i = enumDef.Members.Count + 1;
                enumDef.Members.Add(new EnumMember()
                {
                    Name = "UntitledMember",
                    Value = i
                });

                RefreshEnumMemberGrid();
            }
        }

        private void btnRemoveEnumMember_Click(object sender, RoutedEventArgs e)
        {
            var enumDef = GetSelectedEnumDefinition();
            var enumMember = GetSelectedEnumMember();

            if (enumMember == null || enumDef == null ||
                MessageBox.Show(string.Format("Are you sure to delete {0}.{1}?", enumDef.Name, enumMember.Name),
                        string.Format("Deleting {0}.{1}!", enumDef.Name, enumMember.Name),
                        MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            enumDef.Members.Remove(enumMember);
            RefreshEnumMemberGrid();
        }

        private void btnMoveEnumMemberTop_Click(object sender, RoutedEventArgs e)
        {
            var enumMember = GetSelectedEnumMember();
            if (enumMember != null)
            {
                var members = enumMember.EnumDefinition.Members;
                members.MoveTop(enumMember);
                RefreshEnumMemberGrid();
                SetEnumMemberGridIndex(0, grdEnumMembers);
            }
        }

        private void btnMoveEnumMemberUp_Click(object sender, RoutedEventArgs e)
        {
            var enumMember = GetSelectedEnumMember();
            if (enumMember != null)
            {
                var members = enumMember.EnumDefinition.Members;
                int i = grdEnumMembers.SelectedIndex - 1;
                members.MoveUp(enumMember);
                RefreshEnumMemberGrid();
                SetEnumMemberGridIndex(i, grdEnumMembers);
            }
        }

        private void btnMoveEnumMemberDown_Click(object sender, RoutedEventArgs e)
        {
            var enumMember = GetSelectedEnumMember();
            if (enumMember != null)
            {
                var members = enumMember.EnumDefinition.Members;
                int i = grdEnumMembers.SelectedIndex + 1;
                members.MoveDown(enumMember);
                RefreshEnumMemberGrid();
                SetEnumMemberGridIndex(i, grdEnumMembers);
            }
        }

        private void btnMoveEnumMemberBottom_Click(object sender, RoutedEventArgs e)
        {
            var enumMember = GetSelectedEnumMember();
            if (enumMember != null)
            {
                var members = enumMember.EnumDefinition.Members;
                members.MoveBottom(enumMember);
                RefreshEnumMemberGrid();
                SetEnumMemberGridIndex(grdEnumMembers.Items.Count - 1, grdEnumMembers);
            }
        }
    }
}
