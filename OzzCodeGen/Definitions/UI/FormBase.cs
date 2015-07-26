using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OzzCodeGen.Utilities;
using System.Xml.Serialization;
using System.ComponentModel;

namespace OzzCodeGen.Definitions.UI
{
    public abstract class FormBase : BaseSavable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Name;
        }



        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                if (string.IsNullOrEmpty(displayName))
                {
                    DisplayName = name.ToTitleCase();
                }
                RaisePropertyChanged("Name");
            }
        }
        private string name;

        /// <summary>
        /// Display text for UI
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = Name.ToTitleCase();
                }
                return displayName;
            }
            set
            {
                if (displayName == value) return;
                displayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        string displayName;

        /// <summary>
        /// BaseTypeName of the WebForm
        /// </summary>
        public string BaseTypeName
        {
            get { return baseTypeName; }
            set
            {
                if (baseTypeName == value) return;
                baseTypeName = value;
                RaisePropertyChanged("BaseTypeName");
            }
        }
        private string baseTypeName;

        /// <summary>
        /// NamespaceName of the WebForm
        /// </summary>
        public string NamespaceName
        {
            get { return namespaceName; }
            set
            {
                if (namespaceName == value) return;
                namespaceName = value;
                RaisePropertyChanged("NamespaceName");
            }
        }
        private string namespaceName;


        public Guid ProjectId
        {
            get { return _projectId; }
            set
            {
                if (_projectId == value) return;
                _projectId = value;
                RaisePropertyChanged("ProjectId");
            }
        }
        private Guid _projectId;


        public string TargetProjectDir
        {
            get { return _targetProjectDir; }
            set
            {
                if (_targetProjectDir == value) return;
                _targetProjectDir = value;
                RaisePropertyChanged("TargetProjectDir");
            }
        }
        private string _targetProjectDir;
        

        public string TargetSubDir
        {
            get { return _targetSubDir; }
            set
            {
                if (_targetSubDir == value) return;
                _targetSubDir = value;
                RaisePropertyChanged("TargetSubDir");
            }
        }
        private string _targetSubDir;
        

        [XmlIgnore]
        public CodeGenProject CodeGenProject
        {
            get { return _codeGenProject; }
            set
            {
                if (_codeGenProject == value) return;
                _codeGenProject = value;
                RaisePropertyChanged("CodeGenProject");
                DataModel = _codeGenProject.DataModel;
                ProjectId = _codeGenProject.ProjectId;
            }
        }
        private CodeGenProject _codeGenProject;
        

        [XmlIgnore]
        public DataModel DataModel
        {
            get { return dataModel; }
            set
            {
                if (dataModel == value) return;
                dataModel = value;
                RaisePropertyChanged("DataModel");
            }
        }
        DataModel dataModel;

        [XmlIgnore]
        public EntityDefinition EntityDefinition
        {
            get { return entityDefinition; }
            set
            {
                if (entityDefinition == value) return;
                entityDefinition = value;
                EntityDefinitionName = entityDefinition.Name;
                RaisePropertyChanged("EntityDefinition");
            }
        }
        EntityDefinition entityDefinition;

        /// <summary>
        /// Primary EntityDefinition's Name of the WebForm
        /// </summary>
        public string EntityDefinitionName
        {
            get { return entityDefinitionName; }
            set
            {
                if (entityDefinitionName == value) return;
                entityDefinitionName = value;
                RaisePropertyChanged("EntityDefinitionName");
            }
        }
        private string entityDefinitionName;
    }
}
