using OzzUtils.Wpf;
using System.IO;
using System.Linq;

namespace OzzLocalization.Wpf.Models
{
    public class ProjectDir : AbstractViewModel
    {
        public string FullPath
        {
            set
            {
                _fullPath = value;
                RaisePropertyChanged("FullPath");
            }
            get
            {
                return _fullPath;
            }
        }
        private string _fullPath;


        public string Title
        {
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
            get
            {
                if (string.IsNullOrEmpty(_title)) SetTitle();
                return _title;
            }
        }
        private string _title;

        private void SetTitle()
        {
            if (string.IsNullOrEmpty(FullPath))
                return;

            _title = GetTitleFromPath(FullPath);
        }

        private string GetTitleFromPath(string path)
        {
            string lastFolder = Path.GetFileName(path);
            if (excludedNames.Contains(lastFolder.ToLowerInvariant()))
            {
                return GetTitleFromPath(Path.GetDirectoryName(path));
            }
            else
            {
                return lastFolder;
            }
        }
        static string[] excludedNames = { "vocabulary", "ozzcodegen" };


        public override string ToString()
        {
            return Title;
        }
    }
}
