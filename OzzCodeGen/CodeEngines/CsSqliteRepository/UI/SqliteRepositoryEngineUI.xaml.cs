using OzzCodeGen.UI;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.UI;

public partial class SqliteRepositoryEngineUI : AbstractEngineUI
{
    public SqliteRepositoryEngineUI()
    {
        InitializeComponent();

        ColumnWidths["Name"] = 130;
        ColumnWidths.Add("TableName", 150);
        ColumnWidths.Add("ColumnName", 150);
        ColumnWidths.Add("ClrTypeName", 100);
        ColumnWidths.Add("AutoLoad", 48);
        ColumnWidths.Add("CheckIfAltered", 48);
        ColumnWidths.Add("IsUniqueIndexed", 48);
        ColumnWidths.Add("SingleColumnUpdate", 48);
        ColumnWidths.Add("IsRepositoryColumn", 48);
        ColumnWidths.Add("DecimalToIntegerScale", 64);

        HiddenColumns.Add("CodeEngine");
        HiddenColumns.Add("HasCustomAttributes");
        HiddenColumns.Add("ModelProperties");
        HiddenColumns.Add("Properties");
        HiddenColumns.Add("OrderByClause");
        HiddenColumns.Add("StorageEntitySetting");
        HiddenColumns.Add("StorageColumnSetting");

        HiddenColumns.Add("ClrTypeName");
        HiddenColumns.Add("IsFloat");
        HiddenColumns.Add("IsDouble");
        HiddenColumns.Add("IsSimple");
        HiddenColumns.Add("IsString");
        HiddenColumns.Add("IsDecimal");
        HiddenColumns.Add("IsComplex");
        HiddenColumns.Add("IsBoolean");
        HiddenColumns.Add("IsDateTime");
        HiddenColumns.Add("IsNullable");
        HiddenColumns.Add("IsCollection");
        HiddenColumns.Add("IsIntegerNumeric");
        HiddenColumns.Add("IsNullableString");
        HiddenColumns.Add("IsLoadingFromFile");
        //HiddenColumns.Add("IsRepositoryColumn");
        HiddenColumns.Add("IsFractionalNumeric");
        HiddenColumns.Add("IsRepositoryTextColumn");
        HiddenColumns.Add("SearchableRangeProperties");
        HiddenColumns.Add("IsRepositoryIntegerColumn");
        HiddenColumns.Add("SearchableNonRangeProperties");

        EntitiesDataGrid = grdEntitySettings;
        PropertiesDataGrid = grdPropertySettings;
        DataContext = CodeEngine;
    }

    protected override void OnCodeEngineChanged()
    {
        DataContext = CodeEngine;
    }
}
