using OzzLocalization.Wpf.Models;
using OzzUtils.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace OzzLocalization.Wpf.ViewModels
{
    public class VocabularyEditorVM : AbstractViewModel
    {

        public AppSettings AppSettings
        {
            set
            {
                _appSettings = value;
                RaisePropertyChanged("AppSettings");
            }
            get
            {
                return _appSettings;
            }
        }
        private AppSettings _appSettings;

        public ProjectDir SelectedProject
        {
            set
            {
                if (_selectedProject == value)
                    return;

                _selectedProject = value;
                if (_selectedProject != null)
                {
                    var recentProjects = AppSettings.RecentProjects
                        .Where(p => p.FullPath
                            .Equals(_selectedProject.FullPath, StringComparison.InvariantCultureIgnoreCase) == false)
                        .ToList();
                    recentProjects.Insert(0, _selectedProject);
                    AppSettings.RecentProjects = recentProjects;
                    OpenSelectedProject();
                }
                RaisePropertyChanged("SelectedProject");
            }
            get
            {
                return _selectedProject;
            }
        }
        private ProjectDir _selectedProject;

        public ObservableCollection<Vocab> SelectedVocabulary
        {
            set
            {
                _selectedVocabulary = value;
                RaisePropertyChanged("SelectedVocabulary");
            }
            get
            {
                return _selectedVocabulary;
            }
        }
        private ObservableCollection<Vocab> _selectedVocabulary;

        public void SetSelectedVocabulary()
        {
            var vocabs = Vocabularies.GetVocabulary(SelectedCultureCode);
            if (vocabs == null)
            {
                SelectedVocabulary = null;
                return;
            }
            //TODO: Filtre vs.
            //.Where(v=>v.IsTranslated()==false)
            SelectedVocabulary = vocabs != null ? new ObservableCollection<Vocab>(vocabs) : null;
        }

        public Vocabularies Vocabularies
        {
            set
            {
                _vocabularies = value;
                RaisePropertyChanged("Vocabularies");
            }
            get
            {
                return _vocabularies;
            }
        }
        private Vocabularies _vocabularies;

        public bool CanSelectCultureCode
        {
            get
            {
                return CultureCodes != null && CultureCodes.Count > 1;
            }
        }

        public List<string> CultureCodes
        {
            set
            {
                _cultures = value;
                RaisePropertyChanged("CultureCodes");
                RaisePropertyChanged("CanSelectCultureCode");
            }
            get
            {
                return _cultures;
            }
        }
        private List<string> _cultures;

        public string SelectedCultureCode
        {
            set
            {
                if (_selectedCultureCode == value) return;
                _selectedCultureCode = value;
                SetSelectedVocabulary();
                RaisePropertyChanged("SelectedCultureCode");
            }
            get
            {
                return _selectedCultureCode;
            }
        }
        private string _selectedCultureCode;


        public void OpenSelectedProject()
        {
            if (SelectedProject == null)
                return;

            if (Directory.Exists(SelectedProject.FullPath))
            {
                Vocabularies = Vocabularies.OpenVocabularies(SelectedProject.FullPath);
                CultureCodes = Vocabularies.GetCultureCodes();
                SelectedCultureCode = CultureCodes.FirstOrDefault();
            }
            else
            {
                var dlgResult = MessageBox.Show("Project directory could not be found! Do you want to remove the reference to the file from Recent File list?",
                                    "File Not Foud!",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation);
                if (dlgResult == MessageBoxResult.Yes)
                {
                    var recentProjects = AppSettings.RecentProjects
                            .Where(p => p.FullPath
                                .Equals(SelectedProject.FullPath, StringComparison.InvariantCultureIgnoreCase) == false)
                            .ToList();
                    AppSettings.RecentProjects = recentProjects;
                }
            }
        }

    }
}
