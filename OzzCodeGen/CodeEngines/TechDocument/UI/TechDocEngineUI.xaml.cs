using OzzCodeGen.UI;
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

namespace OzzCodeGen.CodeEngines.TechDocument.UI
{
    /// <summary>
    /// Interaction logic for TechDocEngineUI.xaml
    /// </summary>
    public partial class TechDocEngineUI : AbstractEngineUI
    {
        public TechDocEngineUI()
        {
            InitializeComponent();

            HiddenColumns.Add("DataType");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("HtmlEncode");
            HiddenColumns.Add("IsSimple");
            HiddenColumns.Add("IsString");
            HiddenColumns.Add("TechDocEntitySetting");
            HiddenColumns.Add("Properties");

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }
    }
}
