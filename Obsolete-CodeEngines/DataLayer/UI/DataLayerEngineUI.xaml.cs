using OzzCodeGen.UI;

namespace OzzCodeGen.CodeEngines.DataLayer.UI
{
    /// <summary>
    /// Interaction logic for DataLayerEngineUI.xaml
    /// </summary>
    public partial class DataLayerEngineUI : AbstractEngineUI
    {
        public DataLayerEngineUI()
        {
            InitializeComponent();

            ColumnWidths.Add("UIHint", 120);
            ColumnWidths.Add("Range", 120);
            ColumnWidths.Add("RegularExpression", 120);
            ColumnWidths.Add("CreateViewModelFile", 120);
            ColumnWidths.Add("InMetadata", 70);
            ColumnWidths.Add("InViewModel", 70);

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("CodeEngine");

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }
    }
}
