using OzzCodeGen.Definitions;
using OzzCodeGen.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OzzCodeGen.Wpf
{
    public partial class MainWindow
    {
        public AppSettings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == value) return;
                _settings = value;
                RaisePropertyChanged("Settings");
                RaisePropertyChanged("MostRecentProjectFile");
            }
        }
        private AppSettings _settings;
        private string settingsFile;

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
        private DataModel _dataModel;

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
        private DataModel _enumDefinitions;

        public FileDefinition MostRecentProjectFile
        {
            get
            {
                return Settings?.RecentProjectFiles.FirstOrDefault();
            }
        }

        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                if (_progressValue == value)
                    return;
                _progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }
        private int _progressValue;

        public EntityDefinition SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (_selectedEntity == value)
                    return;
                _selectedEntity = value;
                RaisePropertyChanged("SelectedEntity");
            }
        }
        private EntityDefinition _selectedEntity;

        public BaseProperty SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                if (_selectedProperty == value)
                    return;
                _selectedProperty = value;
                RaisePropertyChanged("SelectedProperty");
            }
        }
        private BaseProperty _selectedProperty;

        public TabItem SelectedTabItem
        {
            get
            {
                return _selectedTabItem;
            }
            set
            {
                if (_selectedTabItem == value)
                    return;
                _selectedTabItem = value;
                RaisePropertyChanged("SelectedTabItem");
            }
        }
        private TabItem _selectedTabItem;

    }
}
