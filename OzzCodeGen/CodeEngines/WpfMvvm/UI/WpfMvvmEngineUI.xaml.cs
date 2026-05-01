using OzzCodeGen.UI;

namespace OzzCodeGen.CodeEngines.WpfMvvm.UI;

public partial class WpfMvvmEngineUI : AbstractEngineUI
{
    public WpfMvvmEngineUI()
    {
        InitializeComponent();

        HiddenColumns.Add("CodeEngine");
        HiddenColumns.Add("Properties");
        HiddenColumns.Add("MvvmProperties");
        HiddenColumns.Add("IncludedProperties");
        HiddenColumns.Add("GetBaseEntitySetting");
        HiddenColumns.Add("BaseEntitySetting");

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
        HiddenColumns.Add("RepositoryName");
        HiddenColumns.Add("IsIntegerNumeric");
        HiddenColumns.Add("IsNullableString");
        HiddenColumns.Add("CommandVmTypeName");
        HiddenColumns.Add("MvvmEntitySetting");
        HiddenColumns.Add("IsLoadingFromFile");
        HiddenColumns.Add("CommandVmNamespace");
        HiddenColumns.Add("IsFractionalNumeric");
        HiddenColumns.Add("RepositoryInstanceName");

        ColumnWidths["GenerateCreateView"] = 80;
        ColumnWidths["GenerateEditView"] = 72;
        ColumnWidths["GenerateCreateViewModel"] = 72;
        ColumnWidths["GenerateEditViewModel"] = 64;
        ColumnWidths["GenerateLookupService"] = 80;
        ColumnWidths["GenerateCommands"] = 80;
        ColumnWidths["IncludeInView"] = 80;
        ColumnWidths["IncludeInViewModel"] = 64;
        ColumnWidths["IsReadOnly"] = 80;

        EntitiesDataGrid = grdEntitySettings;
        PropertiesDataGrid = grdPropertySettings;
    }
}
