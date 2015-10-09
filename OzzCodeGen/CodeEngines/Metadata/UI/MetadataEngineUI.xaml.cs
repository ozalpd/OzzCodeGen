using OzzCodeGen.UI;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.Metadata.UI
{
    /// <summary>
    /// Interaction logic for MetadataEngineUI.xaml
    /// </summary>
    public partial class MetadataEngineUI : AbstractEngineUI
    {
        public MetadataEngineUI()
        {
            InitializeComponent();

            ColumnWidths.Add("UIHint", 120);
            ColumnWidths.Add("Range", 120);
            ColumnWidths.Add("RegularExpression", 120);
            ColumnWidths["Name"] = 120;

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("DataType");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("MetadataEntitySetting");

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
