using OzzCodeGen.CodeEngines.CsDbRepository;
using System;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository;

/// <summary>
/// Provides configuration settings for a property within a generated SQLite repository entity, including column
/// mapping, autoload behavior, update method generation, and storage-specific options.
/// </summary>
/// <remarks>This class is used by the SQLite repository code generation engine to control how individual
/// properties are mapped to database columns and how they participate in repository operations. It supports advanced
/// features such as automatic loading of navigation properties, per-column update methods, decimal-to-integer scaling
/// for precise storage, and unique index detection. Most settings are relevant only for simple, string, or complex
/// properties, and some options may have no effect depending on the property type or storage mapping.</remarks>
public class SqliteRepositoryPropertySetting : BaseCsDbRepositoryPropertySetting
{
    /// <summary>
    /// Gets or sets the base-10 scale used when converting <see langword="decimal"/> values to SQLite integer storage
    /// and back. If the value is zero, no scaling is applied and the decimal value is stored as-is,
    /// if it is greater than 0, the property value will be multiplied by 10 raised to this power before being stored in the database.
    /// When value is less than 0, it is treated as 0, meaning no scaling will be applied.
    /// </summary>
    /// <remarks>
    /// The value represents the power of 10 applied during conversion. For example, a scale of 4 stores 123.66 as
    /// 1236600, and a scale of 6 stores 0.06654 as 66540.
    /// </remarks>
    public int DecimalToIntegerScale
    {
        set
        {
            int newValue = 0;
            if (IsLoadingFromFile || IsDecimal)
            {
                newValue = value < 0 ? 0 : value;
            }
            if (newValue != _decimalScale)
            {
                _decimalScale = newValue;
                RaisePropertyChanged(nameof(DecimalToIntegerScale));
            }
        }
        get
        {
            if (!_decimalScale.HasValue)
            {
                bool isStorageIntType = IsDecimal &&
                                        StorageColumnSetting?.DataType
                                       .StartsWith("INT", StringComparison.OrdinalIgnoreCase) == true;

                _decimalScale = isStorageIntType ? 4 : 0;
            }
            return _decimalScale.Value;
        }
    }
    private int? _decimalScale;


    protected override BaseCodeEngine GetCodeEngine()
    {
        SqliteRepositoryEntitySetting setting = (SqliteRepositoryEntitySetting)EntitySetting;
        return setting?.CodeEngine;
    }
}
