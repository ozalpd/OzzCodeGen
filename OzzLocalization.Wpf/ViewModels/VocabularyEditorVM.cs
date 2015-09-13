using OzzLocalization.Wpf.Models;
using OzzUtils.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (_selectedProject == value) return;

                _selectedProject = value;
                var recentProjects = AppSettings.RecentProjects
                    .Where(p => p.FullPath
                        .Equals(_selectedProject.FullPath, StringComparison.InvariantCultureIgnoreCase) == false)
                    .ToList();
                recentProjects.Insert(0, _selectedProject);
                AppSettings.RecentProjects = recentProjects;
                RaisePropertyChanged("SelectedProject");
            }
            get
            {
                return _selectedProject;
            }
        }
        private ProjectDir _selectedProject;

    }
}
