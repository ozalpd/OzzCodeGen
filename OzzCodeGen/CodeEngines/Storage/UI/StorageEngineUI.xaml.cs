using System.Windows;
using OzzCodeGen.UI;

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

            HiddenColumns.Add("IsSimple");
            HiddenColumns.Add("IsString");
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

            var pKey = ((StorageCodeEngine)CodeEngine).GetDefaultPrimaryKey();
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
