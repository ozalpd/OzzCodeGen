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
        HiddenColumns.Add("IsSimple");
        HiddenColumns.Add("IsString");
        HiddenColumns.Add("IsComplex");
        HiddenColumns.Add("IsBoolean");
        HiddenColumns.Add("IsDateTime");
        HiddenColumns.Add("IsNullable");
        HiddenColumns.Add("IsCollection");
        HiddenColumns.Add("IsIntegerNumeric");
        HiddenColumns.Add("IsNullableString");
        HiddenColumns.Add("IsFractionalNumeric");

        ColumnWidths["GenerateCreateView"] = 72;
        ColumnWidths["GenerateEditView"] = 72;
        ColumnWidths["GenerateCreateViewModel"] = 92;
        ColumnWidths["GenerateEditViewModel"] = 92;
        ColumnWidths["GenerateCommands"] = 80;
        ColumnWidths["IncludeInView"] = 80;
        ColumnWidths["IncludeInViewModel"] = 110;
        ColumnWidths["IsReadOnly"] = 80;

        EntitiesDataGrid = grdEntitySettings;
        PropertiesDataGrid = grdPropertySettings;
    }
}
