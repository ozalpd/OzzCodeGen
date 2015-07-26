using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OzzUtils;
using System.ComponentModel;
using System.Xml.Serialization;
using OzzCodeGen.Providers;

namespace OzzCodeGen.Definitions
{
    public class EntityDefinition : BaseSavable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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

        [XmlIgnore]
        public DataModel DataModel
        {
            get { return dataModel; }
            set
            {
                if (dataModel == value) return;
                if (dataModel != null) dataModel.Remove(this);
                dataModel = value;
                dataModel.Add(this);
            }
        }
        DataModel dataModel;

        public List<EntitySource> EntitySource
        {
            get
            {
                if (_entitySource == null)
                    _entitySource = new List<EntitySource>();
                return _entitySource;
            }
            set
            {
                if (_entitySource == value) return;
                _entitySource = value;
                RaisePropertyChanged("EntitySource");
            }
        }
        List<EntitySource> _entitySource;

        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment == value) return;
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        string _comment;


        /// <summary>
        /// NamespaceName of the entity
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

        /// <summary>
        /// Display order for UI
        /// </summary>
        public int DisplayOrder
        {
            get { return _displayOrder; }
            set
            {
                if (_displayOrder == value) return;
                _displayOrder = value;
                RaisePropertyChanged("DisplayOrder");
            }
        }
        int _displayOrder;

        public bool Abstract
        {
            get { return _abstract; }
            set
            {
                if (_abstract == value) return;
                _abstract = value;
                RaisePropertyChanged("Abstract");
            }
        }
        bool _abstract;

        public bool LetDownloadAll
        {
            get { return _letDownloadAll; }
            set
            {
                if (_letDownloadAll == value) return;
                _letDownloadAll = value;
                RaisePropertyChanged("LetDownloadAll");
            }
        }
        private bool _letDownloadAll;
        

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
        /// Display property that represents entity
        /// </summary>
        public string DisplayMember
        {
            get
            {
                if (string.IsNullOrEmpty(displayMember))
                {
                    displayMember = GetDefaultDisplayMember();
                }
                return displayMember;
            }
            set
            {
                if (displayMember == value) return;
                displayMember = value;
                RaisePropertyChanged("DisplayMember");
            }
        }
        string displayMember;

        /// <summary>
        /// BaseTypeName of the WebForm
        /// </summary>
        public string BaseTypeName
        {
            get { return _baseTypeName; }
            set
            {
                if (_baseTypeName == value) return;
                _baseTypeName = value;
                RaisePropertyChanged("BaseTypeName");
            }
        }
        private string _baseTypeName;


        public EntityPropertyList Properties
        {
            set
            {
                _propertyList = value;
                _propertyList.EntityDefinition = this;
            }
            get
            {
                if (_propertyList == null)
                {
                    _propertyList = new EntityPropertyList();
                    _propertyList.EntityDefinition = this;

                }
                return _propertyList;
            }
        }
        private EntityPropertyList _propertyList;


        public override string ToString()
        {
           if(string.IsNullOrEmpty(BaseTypeName)) return Name;
           return string.Format("{0} <= {1}", Name, BaseTypeName);
        }


        public EntityDefinition GetBaseEntity()
        {
            if (string.IsNullOrEmpty(BaseTypeName))
                return null;

            return DataModel.FirstOrDefault(e => e.Name.Equals(BaseTypeName));
        }

        public string GetDefaultDisplayMember()
        {
            var baseEntity = GetBaseEntity();
            if (baseEntity != null)
            {
                return baseEntity.DisplayMember;
            }

            SimpleProperty p = Properties.SimpleProperties
                .Where(x => x.Name.ToLowerInvariant().Contains("name"))
                .FirstOrDefault();

            if (p == null)
            {
                p = Properties.SimpleProperties
                    .Where(x => x.Name.ToLowerInvariant().Contains("title"))
                    .FirstOrDefault();
            }

            if (p == null)
            {
                p = Properties.SimpleProperties
                    .Where(x => x.TypeName.ToLowerInvariant().Contains("string"))
                    .FirstOrDefault();
            }

            if (p == null) p = Properties.SimpleProperties.FirstOrDefault();
            if (p != null)
            {
                return p.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        public static EntityDefinition CreateDefaultEntityDefinition()
        {
            var entity = new EntityDefinition();

            var idProperty = BaseProperty.CreatePropertyDefinition("int");
            idProperty.Name = "Id";
            idProperty.IsStoreGenerated = true;
            ((SimpleProperty)idProperty).IsNullable = false;
            entity.Properties.Add(idProperty);

            return entity;
        }

        public EntityDefinition Clone()
        {
            var clone = new EntityDefinition()
            {
                Name = this.Name + "Clone",
                NamespaceName = this.NamespaceName,
                BaseTypeName = this.BaseTypeName,
                Comment = this.Comment,
                Abstract = this.Abstract,
                DisplayMember = this.DisplayMember,
                LetDownloadAll = this.LetDownloadAll
            };

            foreach (var p in Properties)
            {
                clone.Properties.Add(p.Clone());
            }

            return clone;
        }
    }
}
