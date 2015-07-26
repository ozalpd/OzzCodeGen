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

namespace OzzCodeGen.AppEngines.Localization.UI
{
    /// <summary>
    /// Interaction logic for ResxEngineUI.xaml
    /// </summary>
    public partial class ResxEngineUI : AbstractEngineUI
    {
        public ResxEngineUI()
        {
            InitializeComponent();

            HiddenColumns.Add("Properties");
            ColumnWidths.Add("BaseType", 120);

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;
        }

        private void mnuEntityOptions_Click(object sender, RoutedEventArgs e)
        {
            var engine = (ResxEngine)AppEngine;
            foreach (var entity in engine.Entities)
            {
                foreach (var item in entity.Properties)
                {
                    item.ResetLocalizeRequiredMsg();
                }
            }
        }

    }
}
