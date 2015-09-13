using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzUtils.Wpf;
using OzzUtils.Wpf.Savables;
using System.IO;
using System.Xml.Serialization;

namespace OzzLocalization.Wpf.Models
{
    public class AppSettings : SavableViewModel
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




        public List<ProjectDir> RecentProjects
        {
            get
            {
                if (_recentProjects == null)
                {
                    _recentProjects = new List<ProjectDir>();
                }
                return _recentProjects;
            }
            set
            {
                if (_recentProjects == value) return;
                _recentProjects = value;
                RaisePropertyChanged("RecentProjects");
            }
        }
        private List<ProjectDir> _recentProjects;





        public static AppSettings GetAppSettings()
        {
            if (_settings == null)
            {
                _settings = AppSettings.OpenSettings();
            }
            return _settings;
        }
        static AppSettings _settings;

        protected static string SettingsFile
        {
            get
            {
                if (string.IsNullOrEmpty(_settingsFile))
                {
                    _settingsFile = System.IO.Path.Combine(
                                System.IO.Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location),
                                "OzzVocabularyEditor.settings");
                }
                return _settingsFile;
            }
        }
        static string _settingsFile;

        protected static AppSettings OpenSettings()
        {
            if (!File.Exists(SettingsFile))
            {
                return new AppSettings();
            }

            StreamReader reader = new StreamReader(SettingsFile);
            XmlSerializer x = new XmlSerializer(typeof(AppSettings));
            AppSettings settings = (AppSettings)x.Deserialize(reader);
            reader.Close();

            return settings;
        }

        public virtual void SaveSettings()
        {
            StreamWriter writer = new StreamWriter(SettingsFile);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }
    }
}
