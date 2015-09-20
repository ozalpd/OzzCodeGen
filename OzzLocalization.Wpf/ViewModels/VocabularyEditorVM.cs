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

        public Vocab SelectedVocab
        {
            set
            {
                _selectedVocab = value;
                RaisePropertyChanged("SelectedVocab");
            }
            get
            {
                return _selectedVocab;
            }
        }
        private Vocab _selectedVocab;


        public void SetSelectedVocabulary()
        {
            if (Vocabularies == null)
                return;
            IEnumerable<Vocab> vocabs = Vocabularies.GetVocabulary(SelectedCultureCode);

            if (vocabs == null)
            {
                SelectedVocabulary = null;
                return;
            }

            if (FilterUntranslatedOnly)
            {
                vocabs = vocabs.Where(v => v.IsTranslated == false);
            }
            if (!string.IsNullOrEmpty(FilterByName))
            {
                vocabs = vocabs.Where(v => v.Name.ToLower().Contains(FilterByName.ToLower()));
            }
            if (!string.IsNullOrEmpty(FilterByTranslation))
            {
                vocabs = vocabs.Where(v => v.Translation.ToLower().Contains(FilterByTranslation.ToLower()));
            }
            if (!string.IsNullOrEmpty(FilterByAnyString))
            {
                vocabs = vocabs.Where(v => v.Name.ToLower().Contains(FilterByAnyString.ToLower()) ||
                                           v.Translation.ToLower().Contains(FilterByAnyString.ToLower()) ||
                                           (v.ToolTip!=null && v.ToolTip.ToLower().Contains(FilterByAnyString.ToLower())) ||
                                           (v.RequiredMsg != null && v.RequiredMsg.ToLower().Contains(FilterByAnyString.ToLower())) ||
                                           (v.ValidationMsg != null && v.ValidationMsg.ToLower().Contains(FilterByAnyString.ToLower())));
            }

            SelectedVocabulary = new ObservableCollection<Vocab>(vocabs);
        }

        //public bool UntranslatedOnly

        public bool FilterUntranslatedOnly
        {
            set
            {
                _filterUntranslatedOnly = value;
                SetSelectedVocabulary();
                RaisePropertyChanged("FilterUntranslatedOnly");
            }
            get { return _filterUntranslatedOnly; }
        }
        private bool _filterUntranslatedOnly;


        public string FilterByAnyString
        {
            set
            {
                _filterByAnyString = value;
                SetSelectedVocabulary();
                RaisePropertyChanged("FilterByAnyString");
            }
            get { return _filterByAnyString; }
        }
        private string _filterByAnyString;

        public string FilterByName
        {
            set
            {
                _filterName = value;
                SetSelectedVocabulary();
                RaisePropertyChanged("FilterByName");
            }
            get { return _filterName; }
        }
        private string _filterName;

        public string FilterByTranslation
        {
            set
            {
                _filterByTranslation = value;
                SetSelectedVocabulary();
                RaisePropertyChanged("FilterByTranslation");
            }
            get { return _filterByTranslation; }
        }
        private string _filterByTranslation;


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

        public void CombineVocabularies()
        {
            var combinedVocabulary = Vocabularies.GetCombinedVocabulary();
            foreach (var dict in Vocabularies)
            {
                var v = dict.Value;
                foreach (var item in combinedVocabulary)
                {
                    v.AddUnique(item);
                }
                v.OrderByName();
            }
        }

        public void OpenSelectedProject()
        {
            if (SelectedProject == null)
                return;

            if (Directory.Exists(SelectedProject.FullPath))
            {
                Vocabularies = Vocabularies.OpenVocabularies(SelectedProject.FullPath);
                CultureCodes = Vocabularies.GetCultureCodes();
                var firstCulture = CultureCodes
                        .Where(c => c.Equals(Vocabularies.NotrCode) == false)
                        .FirstOrDefault();
                SelectedCultureCode = string.IsNullOrEmpty(firstCulture) ? 
                            CultureCodes.FirstOrDefault() :
                            firstCulture;
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
            SetSelectedVocabulary();
        }

    }
}
