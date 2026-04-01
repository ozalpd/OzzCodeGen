using OzzCodeGen.UI;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.UI;

public partial class SqliteRepositoryEngineUI : AbstractEngineUI
{
    public SqliteRepositoryEngineUI()
    {
        InitializeComponent();

        ColumnWidths["Name"] = 130;
        ColumnWidths.Add("TableName", 150);
        ColumnWidths.Add("ColumnName", 150);
        ColumnWidths.Add("ClrTypeName", 110);

        HiddenColumns.Add("CodeEngine");
        HiddenColumns.Add("HasCustomAttributes");
        HiddenColumns.Add("ModelProperties");
        HiddenColumns.Add("Properties");
        HiddenColumns.Add("StorageEntitySetting");

        HiddenColumns.Add("ClrTypeName");
        //HiddenColumns.Add("IsRepositoryColumn");
        HiddenColumns.Add("IsSimple");
        HiddenColumns.Add("IsString");
        HiddenColumns.Add("IsBoolean");
        HiddenColumns.Add("IsDateTime");

        EntitiesDataGrid = grdEntitySettings;
        PropertiesDataGrid = grdPropertySettings;
        DataContext = CodeEngine;
    }

    protected override void OnCodeEngineChanged()
    {
        DataContext = CodeEngine;
    }
}
