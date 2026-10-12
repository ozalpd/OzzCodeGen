using System.ComponentModel;
using System.IO;

namespace OzzCodeGen.Wpf.Models
{
    public class FileDefinition : INotifyPropertyChanged
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        private string _name;

        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                if (_fullPath == value) return;
                _fullPath = value;
                Name = Path.GetFileNameWithoutExtension(_fullPath);
                RaisePropertyChanged(nameof(FullPath));
            }
        }
        private string _fullPath;

        public bool FileExists
        {
            get { return File.Exists(FullPath); }
        }


        public override string ToString()
        {
            return Name;
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
