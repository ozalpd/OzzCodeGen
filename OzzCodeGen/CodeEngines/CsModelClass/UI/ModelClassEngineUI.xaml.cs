using OzzCodeGen.UI;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.CsModelClass.UI
{
    /// <summary>
    /// Interaction logic for ModelClassEngineUI.xaml
    /// </summary>
    public partial class ModelClassEngineUI : AbstractEngineUI
    {
        public ModelClassEngineUI()
        {
            InitializeComponent();

            ColumnWidths.Add("UIHint", 120);
            ColumnWidths.Add("Range", 120);
            ColumnWidths.Add("RegularExpression", 120);
            ColumnWidths["Name"] = 120;
            ColumnWidths.Add("DTOExclusion", 60);

            HiddenColumns.Add("DataType");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("HasCustomAttributes");
            HiddenColumns.Add("HtmlEncode");
            HiddenColumns.Add("IsSimple");
            HiddenColumns.Add("IsString");
            HiddenColumns.Add("IsIntegerNumeric");
            HiddenColumns.Add("IsNullableString");
            HiddenColumns.Add("IsFractionalNumeric");
            HiddenColumns.Add("IsCollection");
            HiddenColumns.Add("IsNullable");
            HiddenColumns.Add("IsComplex");
            HiddenColumns.Add("IsBoolean");
            HiddenColumns.Add("IsDateTime");
            HiddenColumns.Add("SearchableNonRangeProperties");
            HiddenColumns.Add("SearchableSimpleProperties");
            HiddenColumns.Add("SearchableRangeProperties");
            HiddenColumns.Add("MetadataEntitySetting");
            HiddenColumns.Add("SearchableProperties");
            HiddenColumns.Add("IsLoadingFromFile");
            HiddenColumns.Add("ModelProperties");
            HiddenColumns.Add("Properties");

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        public List<string> DataTypeList
        {
            get
            {
                return DataTypes.DataTypeList;
            }
        }
    }
}
