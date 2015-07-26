using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OzzCodeGen.AppEngines;
using OzzCodeGen.UI;
using OzzCodeGen.Utilities;

namespace OzzCodeGen.AppEngines.DataLayer.UI
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
            HiddenColumns.Add("AppEngine");

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }
    }
}
