using OzzCodeGen.UI;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.ModelClass.UI
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
            HiddenColumns.Add("MetadataEntitySetting");
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
