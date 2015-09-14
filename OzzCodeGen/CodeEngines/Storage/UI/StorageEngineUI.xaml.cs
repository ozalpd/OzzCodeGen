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
using OzzCodeGen.Utilities;

namespace OzzCodeGen.CodeEngines.Storage.UI
{
    /// <summary>
    /// Interaction logic for StorageEngineUI.xaml
    /// </summary>
    public partial class StorageEngineUI : AbstractEngineUI
    {
        public StorageEngineUI()
        {
            InitializeComponent();

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;

            ColumnWidths.Add("UseInheritance", 70);
            ColumnWidths.Add("PrimaryKeyColumn", 70);
            ColumnWidths.Add("SchemaName", 70);

            //ReadOnlyColumns.Add("PrimaryKeyColumn");

            HiddenColumns.Add("PrimaryKeyColumn");
            //HiddenColumns.Add("SchemaName");
            HiddenColumns.Add("ModifyTrack");
            HiddenColumns.Add("OverwriteExisting");
            HiddenColumns.Add("ForeignTables");
            HiddenColumns.Add("Properties");
            HiddenColumns.Add("CodeEngine");
            //HiddenColumns.Add("StoredProcs"); //Abandoned
        }

        private void btnAddPrimaryKey_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = (StorageEntitySetting)grdEntitySettings.SelectedItem;
            if (selectedEntity == null || selectedEntity.PrimaryKeyColumn != null)
                return;

            var pKey = ((StorageScriptsEngine)CodeEngine).GetDefaultPrimaryKey();
            pKey.EntitySetting = selectedEntity;
            selectedEntity.Properties.Insert(0, pKey);
            selectedEntity.PrimaryKeyColumn = pKey;

            //Refresh grid
            int i = grdEntitySettings.SelectedIndex;
            grdEntitySettings.SelectedIndex = -1;
            grdEntitySettings.SelectedIndex = i;
        }

        private void btnCreateDB_Click(object sender, RoutedEventArgs e)
        {
            CodeEngine.RenderAllEntities = true;
            CodeEngine.OverwriteExisting = true;
            CodeEngine.RenderSelectedTemplate();
            //((StorageScriptsEngine)CodeEngine).RenderCreateDbTemplate();
        }
    }
}
