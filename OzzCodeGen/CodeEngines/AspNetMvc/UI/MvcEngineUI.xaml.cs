using System.Windows;
using OzzCodeGen.UI;
using System.ComponentModel;

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
            //HiddenColumns.Add("GenerateController");
            //HiddenColumns.Add("IndexView");
            //HiddenColumns.Add("DetailsView");
            //HiddenColumns.Add("CreateView");
            //HiddenColumns.Add("EditView");
            HiddenColumns.Add("OverwriteExisting");

            ColumnWidths["Name"] = 120;
            ColumnWidths.Add("BaseType", 120);
            ColumnWidths.Add("GenerateController", 90);
            ColumnWidths.Add("IndexView", 40);
            ColumnWidths.Add("DetailsView", 40);
            ColumnWidths.Add("CreateView", 40);
            ColumnWidths.Add("EditView", 40);

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        public AspNetMvcEngine Engine { get { return (AspNetMvcEngine)CodeEngine; } }

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

        private void btnSetToControllers_Click(object sender, RoutedEventArgs e)
        {
            Engine.SetSaveParameterToControllers();
        }

        private void btnSetRolesToControllers_Click(object sender, RoutedEventArgs e)
        {
            Engine.SetRolesToControllers();
        }
    }
}
