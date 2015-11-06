using Microsoft.Win32;
using OzzCodeGen.Definitions;
using OzzCodeGen.Wpf.Dialogs;
using OzzCodeGen.Wpf.Models;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace OzzCodeGen.Wpf
{
    public partial class MainWindow
    {
        public void AddProperty(DefinitionType propertyType, EntityDefinition entity)
        {
            var dlg = new NewPropertyDialog();
            var p = BaseProperty.CreatePropertyDefinition(propertyType, "UntitledProperty");
            p.EntityDefinition = entity;
            dlg.PropertyDefinition = p;

            dlg.Closed += AddPropertyDialog_Closed;
            dlg.PropertyDefinition.TypeName = dlg.PropertyDefinition.UsableTypeNames.FirstOrDefault();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        void AddPropertyDialog_Closed(object sender, EventArgs e)
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

            dlg.Closed -= AddPropertyDialog_Closed;
        }

        public CloseWindow CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new CloseWindow(this);
                }
                return _closeCommand;
            }
        }
        private CloseWindow _closeCommand;

        public NewProjectCommand NewProjectCommand
        {
            get
            {
                if (_newProjectCommand == null)
                {
                    _newProjectCommand = new NewProjectCommand(this);
                }
                return _newProjectCommand;
            }
        }
        private NewProjectCommand _newProjectCommand;

        public void NewProject()
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

        public OpenProjectCommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new OpenProjectCommand(this);
                }
                return _openProjectCommand;
            }
        }
        private OpenProjectCommand _openProjectCommand;

        public OpenMostRecentProjectCommand OpenMostRecentProjectCommand
        {
            get
            {
                if (_openMostRecentProject == null)
                {
                    _openMostRecentProject = new OpenMostRecentProjectCommand(this);
                }
                return _openMostRecentProject;
            }
        }
        private OpenMostRecentProjectCommand _openMostRecentProject;

        public void OpenProject()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Code Generator Model Files|*" + fileExtension;
            if (openDlg.ShowDialog(this) ?? false)
            {
                OpenProject(openDlg.FileName);
            }
        }

        private void OpenProject(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("File \"{0}\" not found!", fileName));
            }
            BusyView.Visibility = System.Windows.Visibility.Visible;
            ProgressValue = 40;
            tmrProgressValue.Start();
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(4000000);
            timer.Tick += (object sender, EventArgs e) =>
            {
                CodeGenProject project = CodeGenProject.OpenFile(fileName);
                Settings.AddToRecentFiles(fileName);
                Project = project;
                Project.HasProjectChanges = false;
                SelectedEntity = Project.DataModel.FirstOrDefault();
                Project.ModelProvider = GetModelProviders().First(m => m.ProviderId == Project.ModelProviderId);
                btnRefresh.IsEnabled = Project.ModelProvider.CanRefresh;
                BusyView.Visibility = System.Windows.Visibility.Hidden;

                tmrProgressValue.Stop();
                timer.Stop();
            };
            timer.Start();
        }

        public void OpenMostRecentProject()
        {
            var recentProject = MostRecentProjectFile;
            if(recentProject!=null && recentProject.FileExists)
            {
                OpenProject(recentProject.FullPath);
            }
        }

        public void SaveModelAs()
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

        public SaveProjectCommand SaveProjectCommand
        {
            get
            {
                if (_saveProjectCommand == null)
                {
                    _saveProjectCommand = new SaveProjectCommand(this);
                }
                return _saveProjectCommand;
            }
        }
        private SaveProjectCommand _saveProjectCommand;

        public void SaveProject()
        {
            if (Project == null)
                return;

            if (string.IsNullOrEmpty(Project.SavedFileName) || !File.Exists(Project.SavedFileName))
            {
                SaveModelAs();
            }
            else
            {
                Project.SaveToFile();
            }
            SystemSounds.Asterisk.Play();
        }
    }
}
