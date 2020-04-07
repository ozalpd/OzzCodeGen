using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Storage
{
    public class StorageColumnSetting : BasePropertySetting
    {
        /// <summary>
        /// Storage DataType of the Property
        /// </summary>
        public string DataType
        {
            get { return _DataType; }
            set
            {
                if (_DataType == value) return;
                _DataType = value;
                RaisePropertyChanged("DataType");
            }
        }
        private string _DataType;


        public int Lenght
        {
            get { return _lenght; }
            set
            {
                if (_lenght == value) return;
                _lenght = value;
                RaisePropertyChanged("Lenght");
            }
        }
        private int _lenght;

        /// <summary>
        /// PrimaryKey column
        /// </summary>
        public bool PrimaryKey
        {
            get
            {
                if (!_primaryKey.HasValue)
                    GetIsPKey();
                return _primaryKey.Value;
            }
            set
            {
                if (_primaryKey.HasValue && _primaryKey.Value == value) return;
                bool wasPrimaryKey = _primaryKey ?? false;
                _primaryKey = value;
                if (PrimaryKey)
                {
                    Nullable = false;
                    if (EntitySetting != null)
                        ((StorageEntitySetting)EntitySetting).PrimaryKeyColumn = this;
                }
                else if (wasPrimaryKey)
                {
                    ((StorageEntitySetting)EntitySetting).PrimaryKeyColumn = null;
                }
                RaisePropertyChanged("PrimaryKey");
            }
        }
        private bool? _primaryKey;


        public bool Indexed
        {
            get { return _indexed; }
            set
            {
                _indexed = value;
                RaisePropertyChanged("Indexed");
            }
        }
        private bool _indexed;


        public bool SortDesc
        {
            get { return _sortDesc; }
            set
            {
                _sortDesc = value;
                RaisePropertyChanged("SortDesc");
            }
        }
        private bool _sortDesc;
        
        /// <summary>
        /// Is column of the Property nullable
        /// </summary>
        public bool Nullable
        {
            get
            {
                if (!_nullable.HasValue)
                    GetIsPKey();
                return _nullable.Value;
            }
            set
            {
                if (_nullable.HasValue && _nullable.Value == value) return;
                _nullable = value;
                RaisePropertyChanged("Nullable");
                OnNullableChanged(value);
            }
        }
        private bool? _nullable;

        protected virtual void OnNullableChanged(bool newValue)
        {
            if (PrimaryKey && newValue)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Primary keys can't be nullable!\r\nFirst uncheck Primary Key.",
                    "Primary keys can not be null!");
                Nullable = false;
            }
        }

        public virtual string FormatComment()
        {
            var engine = ((StorageEntitySetting)EntitySetting).CodeEngine;
            
            if (engine.PutCommentsIntoScripts && !string.IsNullOrEmpty(PropertyDefinition.Comment))
            {
                return string.Format(" /* {0} */", PropertyDefinition.Comment);
            }
            return string.Empty;
        }

        private void GetIsPKey()
        {
            if (!string.IsNullOrEmpty(Name) &&
                Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
            {
                _primaryKey = true;
                _nullable = false;
            }
            else
            {
                _primaryKey = false;
                _nullable = true;
            }
        }

        public bool DoNotLog
        {
            get
            {
                return _doNotLog;
            }
            set
            {
                _doNotLog = value;
                RaisePropertyChanged("DoNotLog");
            }
        }
        private bool _doNotLog;

        public string GetLogParam()
        {
            if (string.IsNullOrEmpty(UpdateDefault) == false
                && UpdateDefault.Equals("GetDate()", StringComparison.InvariantCultureIgnoreCase))
                return "GetDate()";

            return string.Concat("[", Name, "]");
        }

        public string ForeignKeyTable
        {
            get { return _fKeyTable; }
            set
            {
                if (_fKeyTable == value) return;
                _fKeyTable = value;
                RaisePropertyChanged("ForeignKeyTable");
            }
        }
        private string _fKeyTable;

        public string InsertDefault
        {
            get { return _insertDefault; }
            set
            {
                _insertDefault = value;
                RaisePropertyChanged("InsertDefault");
            }
        }
        private string _insertDefault;

        public string UpdateDefault
        {
            get { return _updateDefault; }
            set
            {
                _updateDefault = value;
                RaisePropertyChanged("UpdateDefault");
            }
        }
        private string _updateDefault;
    }
    
}
