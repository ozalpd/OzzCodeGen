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
using OzzCodeGen.UI;

namespace OzzCodeGen.AppEngines.ObjectiveC.UI
{
    /// <summary>
    /// Interaction logic for ObjcEngineUI.xaml
    /// </summary>
    public partial class ObjcEngineUI : AbstractEngineUI
    {
        public ObjcEngineUI()
        {
            InitializeComponent();

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;

            ColumnWidths.Add("ReadOnly", 70);
            ColumnWidths.Add("AutoSaveToLocal", 70);

            //ReadOnlyColumns.Add("PrimaryKeyColumn");

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("AppEngine");
            HiddenColumns.Add("PropertiesIncluded");
        }


    }
}
