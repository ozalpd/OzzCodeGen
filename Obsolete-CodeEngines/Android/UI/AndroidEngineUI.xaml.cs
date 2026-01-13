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

namespace OzzCodeGen.CodeEngines.Android.UI
{
    /// <summary>
    /// Interaction logic for AndroidEngineUI.xaml
    /// </summary>
    public partial class AndroidEngineUI : AbstractEngineUI
    {
        public AndroidEngineUI()
        {
            InitializeComponent();

            ColumnWidths.Add("ReadOnly", 70);
            ColumnWidths.Add("AutoSaveToLocal", 70);

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("PropertiesIncluded");

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        private void mnuEntityOptions_Click(object sender, RoutedEventArgs e)
        {
            var engine = (AndroidEngine)CodeEngine;
            foreach (var entity in engine.Entities)
            {
                foreach (var item in entity.Properties)
                {

                }
            }
        }
    }
}
