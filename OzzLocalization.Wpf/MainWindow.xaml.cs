using Microsoft.Win32;
using OzzLocalization.Wpf.Dialogs;
using OzzLocalization.Wpf.Models;
using OzzLocalization.Wpf.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

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
            Settings.MainWindowPosition.SetWindowPositions(this);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ViewModel.AppSettings = Settings;

            btnNewProject.IsEnabled = false;//Is this button really neccessary?
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.MainWindowPosition.GetWindowPositions(this);
            Settings.SaveSettings();
        }

        public VocabularyEditorVM ViewModel
        {
            get
            {
                return (VocabularyEditorVM)DataContext;
            }
        }

        protected AppSettings Settings { get { return AppSettings.GetAppSettings(); } }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = string.Format("Localized file |{0}|Notr file|{1}",
                Vocabularies.FilePattern,
                Vocabularies.GetNotrFileName());
            if (openDlg.ShowDialog(this) ?? false)
            {
                var project = new ProjectDir();
                project.FullPath = System.IO.Path.GetDirectoryName(openDlg.FileName);
                var dlgProject = new SetProjectTitle();
                dlgProject.Project = project;
                dlgProject.ShowDialog();
                ViewModel.SelectedProject = project;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Vocabularies.SaveVocabularies();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CombineVocabularies();
            ViewModel.SetSelectedVocabulary();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetSelectedVocabulary();
        }
    }
}
