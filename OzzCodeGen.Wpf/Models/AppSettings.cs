using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.Wpf.Models
{
    [Serializable()]
    public class AppSettings : INotifyPropertyChanged
    {

        public WindowPosition MainWindowPosition
        {
            set { _mainWindowPosition = value; }
            get
            {
                if (_mainWindowPosition == null)
                {
                    _mainWindowPosition = new WindowPosition();
                }
                return _mainWindowPosition;
            }
        }
        WindowPosition _mainWindowPosition;

        public List<FileDefinition> RecentProjectFiles
        {
            get
            {
                if (_recentProjectFiles == null)
                {
                    _recentProjectFiles = new List<FileDefinition>();
                }
                return _recentProjectFiles;
            }
            set
            {
                if (_recentProjectFiles == value) return;
                _recentProjectFiles = value;
                RaisePropertyChanged("RecentProjectFiles");
            }
        }
        private List<FileDefinition> _recentProjectFiles;


        public void AddToRecentFiles(string fileName)
        {
            RemoveFromRecentFiles(fileName);
            List<FileDefinition> tempList = new List<FileDefinition>();

            tempList.Add(new FileDefinition() { FullPath = fileName });
            foreach (FileDefinition f in RecentProjectFiles)
            {
                tempList.Add(f);
            }
            RecentProjectFiles = tempList;
        }

        public bool RemoveFromRecentFiles(string fileName)
        {
            var fileDef = RecentProjectFiles
                            .Where(f => f.FullPath.Equals(fileName))
                            .FirstOrDefault();
            if (fileDef != null)
            {
                RecentProjectFiles.Remove(fileDef);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static AppSettings OpenFile(string FileName)
        {
            StreamReader reader = new StreamReader(FileName);
            XmlSerializer x = new XmlSerializer(typeof(AppSettings));
            AppSettings settings = (AppSettings)x.Deserialize(reader);
            reader.Close();

            return settings;
        }

        public virtual void SaveToFile(string FileName)
        {
            StreamWriter writer = new StreamWriter(FileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
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
