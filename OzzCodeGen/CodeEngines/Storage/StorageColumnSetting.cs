using System;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.Storage
{
    /// <summary>
    /// Represents storage-specific settings for a column in a database table.
    /// Extends BasePropertySetting with database-specific metadata such as data type, 
    /// constraints, indexing, and default values.
    /// </summary>
    public class StorageColumnSetting : BasePropertySetting
    {
        /// <summary>
        /// Gets or sets the storage data type of the column (e.g., 'VARCHAR', 'INT', 'DATETIME').
        /// </summary>
        public string DataType
        {
            get { return _DataType; }
            set
            {
                if (_DataType == value) return;
                _DataType = value;
                RaisePropertyChanged(nameof(DataType));
            }
        }
        private string _DataType;


        /// <summary>
        /// Gets or sets the maximum length of the column (applicable for string data types).
        /// </summary>
        public int Length
        {
            get { return _length; }
            set
            {
                if (_length == value) return;
                _length = value;
                RaisePropertyChanged(nameof(Length));
            }
        }
        private int _length;

        /// <summary>
        /// Gets or sets a value indicating whether this column is a primary key.
        /// Setting to true automatically sets Nullable to false and updates the EntitySetting's PrimaryKeyColumn.
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
                RaisePropertyChanged(nameof(PrimaryKey));
            }
        }
        private bool? _primaryKey;


        /// <summary>
        /// Gets or sets a value indicating whether this column should be indexed.
        /// </summary>
        public bool Indexed
        {
            get { return _indexed; }
            set
            {
                _indexed = value;
                RaisePropertyChanged(nameof(Indexed));
            }
        }
        private bool _indexed;


        /// <summary>
        /// Gets or sets a value indicating whether this column has a unique constraint.
        /// </summary>
        public bool Unique
        {
            get { return _unique; }
            set
            {
                _unique = value;
                RaisePropertyChanged(nameof(Unique));
            }
        }
        private bool _unique;


        /// <summary>
        /// Gets or sets a value indicating whether the index on this column should be sorted in descending order.
        /// </summary>
        public bool SortDesc
        {
            get { return _sortDesc; }
            set
            {
                _sortDesc = value;
                RaisePropertyChanged(nameof(SortDesc));
            }
        }
        private bool _sortDesc;


        /// <summary>
        /// Gets or sets additional columns to include in a composite index.
        /// Multiple columns are delimited by semicolons (e.g., 'UpdatedAt DESC;CreatedAt').
        /// </summary>
        public string CompositeIndexColumns
        {
            get { return _compositeIndexColumns; }
            set
            {
                _compositeIndexColumns = GetColumnNamesWithExactCase(value);
                RaisePropertyChanged(nameof(CompositeIndexColumns));
            }
        }
        private string _compositeIndexColumns;


        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable in the database.
        /// Setting to true when PrimaryKey is true will trigger a warning and reset to false.
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
                RaisePropertyChanged(nameof(Nullable));
                OnNullableChanged(value);
            }
        }
        private bool? _nullable;

        /// <summary>
        /// Handles changes to the Nullable property, preventing primary keys from being set to nullable.
        /// </summary>
        /// <param name="newValue">The new nullable value.</param>
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

        /// <summary>
        /// Gets or sets a value indicating whether columns in stored procedures should include change detection logic.
        /// Used to determine if a column value has been altered during update operations.
        /// </summary>
        public bool CheckIfAltered
        {
            get
            {
                return _checkIfAltered;
            }
            set
            {
                _checkIfAltered = value;
                RaisePropertyChanged(nameof(CheckIfAltered));
            }
        }
        private bool _checkIfAltered;

        /// <summary>
        /// Gets or sets a value indicating whether this column should not be logged.
        /// </summary>
        public bool DoNotLog
        {
            get
            {
                return _doNotLog;
            }
            set
            {
                _doNotLog = value;
                RaisePropertyChanged(nameof(DoNotLog));
            }
        }
        private bool _doNotLog;

        /// <summary>
        /// Gets or sets the name of the foreign key table that this column references.
        /// </summary>
        public string ForeignKeyTable
        {
            get { return _fKeyTable; }
            set
            {
                if (_fKeyTable == value) return;
                _fKeyTable = value;
                RaisePropertyChanged(nameof(ForeignKeyTable));
            }
        }
        private string _fKeyTable;


        /// <summary>
        /// Gets or sets the default value for insert operations.
        /// </summary>
        public string InsertDefault
        {
            get { return _insertDefault; }
            set
            {
                _insertDefault = value;
                RaisePropertyChanged(nameof(InsertDefault));
            }
        }
        private string _insertDefault;


        /// <summary>
        /// Gets or sets default values for both insert and update operations.
        /// Format: 'Insert:value1;Update:value2' to specify different defaults for each operation.
        /// </summary>
        public string InsertOrUpdateDefault
        {
            get { return _insertOrUpdateDefault; }
            set
            {
                _insertOrUpdateDefault = value;
                RaisePropertyChanged(nameof(InsertOrUpdateDefault));
            }
        }
        private string _insertOrUpdateDefault;

        /// <summary>
        /// Gets or sets the default value for update operations.
        /// </summary>
        public string UpdateDefault
        {
            get { return _updateDefault; }
            set
            {
                _updateDefault = value;
                RaisePropertyChanged(nameof(UpdateDefault));
            }
        }
        private string _updateDefault;

        /// <summary>
        /// Formats a SQL comment for this column based on its property definition and engine settings.
        /// </summary>
        /// <returns>A formatted SQL comment string, or empty string if comments are disabled or no comment exists.</returns>
        public virtual string FormatComment()
        {
            var engine = ((StorageEntitySetting)EntitySetting).CodeEngine;

            if (engine.PutCommentsIntoScripts && !string.IsNullOrEmpty(PropertyDefinition.Comment))
            {
                return string.Format(" /* {0} */", PropertyDefinition.Comment);
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a SQL expression to check if a column value has been altered, accounting for nullability.
        /// </summary>
        /// <returns>A SQL WHERE clause fragment comparing the stored column value with the parameter value.</returns>
        public string GetAlterCheckLine()
        {
            if (Nullable == false)
                return $"[{Name}] != @{Name}";

            if (IsString)
                return $"IsNull([{Name}], '') != IsNull(@{Name}, '')";

            return $"(IsNull([{Name}], @{Name}) Is Not Null And @{Name} Is Null)\r\n" +
                   $"Or (IsNull(@{Name}, [{Name}]) Is Not Null And [{Name}] Is Null)\r\n" +
                   $"Or [{Name}] != @{Name}";
        }

        /// <summary>
        /// Gets the code engine associated with this column setting through its parent entity setting.
        /// </summary>
        /// <returns>The BaseCodeEngine instance, or null if no entity setting is assigned.</returns>
        protected override BaseCodeEngine GetCodeEngine()
        {
            var setting = (StorageEntitySetting)EntitySetting;
            return setting?.CodeEngine;
        }

        /// <summary>
        /// Gets the column names from a semicolon-delimited string, ensuring they match the exact case of the properties defined in the entity setting.
        /// </summary>
        /// <param name="columnNames">A semicolon-delimited string of column names.</param>
        /// <returns>A semicolon-delimited string of column names with exact casing as defined in the entity setting.</returns>
        protected string GetColumnNamesWithExactCase(string columnNames)
        {
            if (IsLoadingFromFile)
                return columnNames; // Return as-is during loading to avoid issues with case sensitivity in file data

            if (string.IsNullOrWhiteSpace(columnNames))
                return string.Empty;

            var names = columnNames.Split(';')
                                   .Select(name => name.Trim())
                                   .Where(name => !string.IsNullOrEmpty(name))
                                   .ToArray();
            var entitySetting = EntitySetting as StorageEntitySetting;
            var inheritedColumns = entitySetting.GetInheritedIncludedProperties();
            if (inheritedColumns == null || !inheritedColumns.Any())
                return string.Empty;

            var exactCaseNames = new List<string>();
            bool hasAsc = false;
            bool hasDesc = false;
            foreach (var n in names)
            {
                string name = n;
                hasDesc = name.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase);
                if (hasDesc)
                    name = name.Substring(0, name.Length - 5).TrimEnd();

                hasAsc = name.EndsWith(" asc", StringComparison.InvariantCultureIgnoreCase);
                if (hasAsc)
                    name = name.Substring(0, name.Length - 4).TrimEnd();

                var prop = inheritedColumns.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (prop != null)
                {// We don't need to put asc here also we don't want to put both asc and desc
                    exactCaseNames.Add(prop.Name + (hasDesc ? " DESC" : ""));
                }
            }
            return string.Join("; ", exactCaseNames);
        }

        /// <summary>
        /// Gets the default value to use for an insert operation.
        /// Prioritizes InsertOrUpdateDefault if forInsOrUpdate is true and the property is set.
        /// </summary>
        /// <param name="forInsOrUpdate">If true, check InsertOrUpdateDefault before InsertDefault.</param>
        /// <returns>The default value string, or empty string if no default is defined.</returns>
        public string GetInsertDefault(bool forInsOrUpdate)
        {
            if (forInsOrUpdate && string.IsNullOrWhiteSpace(InsertOrUpdateDefault) == false)
            {
                return GetInsertOrUpdateDefault(insParam);
            }
            else if (string.IsNullOrEmpty(InsertDefault))
            {
                return string.Empty;
            }
            return InsertDefault;
        }

        /// <summary>
        /// Extracts the default value for a specific operation from the combined InsertOrUpdateDefault property.
        /// </summary>
        /// <param name="paramName">The operation prefix to search for (e.g., 'Insert:' or 'Update:').</param>
        /// <returns>The extracted default value for the specified operation, or empty string if not found.</returns>
        private string GetInsertOrUpdateDefault(string paramName)
        {
            if (string.IsNullOrWhiteSpace(InsertOrUpdateDefault))
                return string.Empty;

            var param = InsertOrUpdateDefault.Split(';')
                                             .Select(p => p.Trim())
                                             .FirstOrDefault(p => p.StartsWith(paramName, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(param))
                return string.Empty;

            return param.Substring(paramName.Length, param.Length - paramName.Length);
        }
        static string insParam = "Insert:";
        static string updParam = "Update:";
        //TODO: IfNullDefault:N'xxx' if stored proc param is null insert default value (eg N'xxx') or
        static string ifNullParam = "IfNullDefault:"; //update with previous value (first declare and fill $"@prev{Name}" var



        /// <summary>
        /// Gets the value to insert for this column, using either a default value or a parameter placeholder.
        /// </summary>
        /// <param name="forInsOrUpdate">If true, consider InsertOrUpdateDefault when determining the value.</param>
        /// <returns>Either the default value or a parameter placeholder in the format '@{ColumnName}'.</returns>
        public string GetInsertValue(bool forInsOrUpdate)
        {
            var val = GetInsertDefault(forInsOrUpdate);
            return string.IsNullOrEmpty(val) ? $"@{Name}" : val;
        }

        /// <summary>
        /// Gets the default value to use for an update operation.
        /// Prioritizes InsertOrUpdateDefault if forInsOrUpdate is true and the property is set.
        /// </summary>
        /// <param name="forInsOrUpdate">If true, check InsertOrUpdateDefault before UpdateDefault.</param>
        /// <returns>The default value string, or empty string if no default is defined.</returns>
        public string GetUpdateDefault(bool forInsOrUpdate)
        {
            if (forInsOrUpdate && string.IsNullOrWhiteSpace(InsertOrUpdateDefault) == false)
            {
                return GetInsertOrUpdateDefault(updParam);
            }
            else if (string.IsNullOrEmpty(UpdateDefault))
            {
                return string.Empty;
            }
            return UpdateDefault;
        }

        /// <summary>
        /// Gets the value to update for this column, using either a default value or a parameter placeholder.
        /// </summary>
        /// <param name="forInsOrUpdate">If true, consider InsertOrUpdateDefault when determining the value.</param>
        /// <returns>Either the default value or a parameter placeholder in the format '@{ColumnName}'.</returns>
        public string GetUpdateValue(bool forInsOrUpdate)
        {
            var val = GetUpdateDefault(forInsOrUpdate);
            return string.IsNullOrEmpty(val) ? $"@{Name}" : val;
        }

        /// <summary>
        /// Initializes the PrimaryKey and Nullable properties based on the column name.
        /// Sets PrimaryKey to true and Nullable to false if the name is 'Id' (case-insensitive).
        /// </summary>
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

        /// <summary>
        /// Gets the value to log for this column in stored procedures.
        /// Returns GetDate() for timestamp columns, otherwise returns the bracketed column name.
        /// </summary>
        /// <returns>A SQL expression representing the column value to be logged.</returns>
        public string GetLogParam()
        {
            if (string.IsNullOrEmpty(UpdateDefault) == false
                && UpdateDefault.Equals("GetDate()", StringComparison.InvariantCultureIgnoreCase))
                return "GetDate()";

            return string.Concat("[", Name, "]");
        }

    }

}
