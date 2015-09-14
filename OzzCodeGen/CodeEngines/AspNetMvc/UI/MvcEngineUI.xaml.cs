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

namespace OzzCodeGen.CodeEngines.AspNetMvc.UI
{
    /// <summary>
    /// Interaction logic for MvcEngineUI.xaml
    /// </summary>
    public partial class MvcEngineUI : AbstractEngineUI
    {
        public MvcEngineUI()
        {
            InitializeComponent();

            HiddenColumns.Add("Properties");
            HiddenColumns.Add("CodeEngine");
            HiddenColumns.Add("FilterViewType");
            HiddenColumns.Add("FilteredDisplayColumns");
            HiddenColumns.Add("DisplayColumns");
            HiddenColumns.Add("GenerateController");
            HiddenColumns.Add("IndexView");
            HiddenColumns.Add("DetailsView");
            HiddenColumns.Add("CreateView");
            HiddenColumns.Add("EditView");
            HiddenColumns.Add("OverwriteExisting");

            ColumnWidths.Add("BaseType", 120);
            //ColumnWidths.Add("GenerateController", 90);
            //ColumnWidths.Add("IndexView", 40);
            //ColumnWidths.Add("DetailsView", 40);
            //ColumnWidths.Add("CreateView", 40);
            //ColumnWidths.Add("EditView", 40);

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        private void mnuEntityOptions_Click(object sender, RoutedEventArgs e)
        {
            //var engine = (AspNetMvcEngine)CodeEngine;
            //foreach (var entity in engine.Entities)
            //{
            //    foreach (var item in entity.Properties)
            //    {
                    
            //    }
            //}
        }
    }
}
