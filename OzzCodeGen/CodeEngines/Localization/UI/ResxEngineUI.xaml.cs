using System.Linq;
using System.Windows;
using OzzCodeGen.UI;
using OzzLocalization;
using System.Windows.Controls;

namespace OzzCodeGen.CodeEngines.Localization.UI
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
            ColumnWidths["BaseTypeName"] = 120;
            ColumnWidths["Name"] = 120;
            ColumnWidths.Add("BaseType", 120);
            ColumnWidths.Add("LocalizeEntityName", 100);
            ColumnWidths.Add("LocalizeRequiredMsg", 120);
            ColumnWidths.Add("LocalizeValidationMsg", 120);

            EntitiesDataGrid = grdEntitySettings;
            PropertiesDataGrid = grdPropertySettings;

            btnAddSupplmProperty.IsEnabled = false;
        }

        string[] hiddenSupplementaryColumns = {
                "BaseTypeName",
                "BaseType"
        };

        protected ResxEngine ResxEngine { get { return (ResxEngine)CodeEngine; } }

        private void btnRefreshVocabulary_Click(object sender, RoutedEventArgs e)
        {
            var vocabularies = Vocabularies.OpenVocabularies(ResxEngine.VocabularyDir);
            var combinedVocabulary = vocabularies.GetCombinedVocabulary();

            var combinedEntity = ResxEngine.CombineEntities();
            foreach (var item in combinedEntity.Properties)
            {
                combinedVocabulary.AddUnique(new Vocab() { Name = item.Name });
            }

            foreach (var entity in ResxEngine.SupplementaryEntities.Where(s => s.Properties.Count > 0))
            {
                foreach (var item in entity.Properties)
                {
                    combinedVocabulary.AddUnique(new Vocab() { Name = item.Name });
                }
            }

            foreach (var dict in vocabularies)
            {
                var vocabulary = dict.Value;
                foreach (var item in combinedVocabulary)
                {
                    vocabulary.AddUnique(new Vocab() { Name = item.Name });
                }

                vocabulary.OrderByName();
                vocabulary.SaveToFile(vocabulary.FilePath);
            }
        }

        protected override void grdEntitySettings_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            base.grdEntitySettings_AutoGeneratingColumn(sender, e);
            if (sender == grdSupplementaryEntities)
            {
                e.Cancel = e.Cancel || hiddenSupplementaryColumns.Contains(e.Column.Header.ToString());
            }
        }

        private void btnAddDefaultSupplmEntities_Click(object sender, RoutedEventArgs e)
        {
            ResxEngine.SetSupplementaryEntities();
        }

        private void txtNewSupplmProperty_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                AddSupplmProperty();
            }
        }

        private void btnAddSupplmProperty_Click(object sender, RoutedEventArgs e)
        {
            AddSupplmProperty();
        }

        private void AddSupplmProperty()
        {
            if (ResxEngine.AddSupplmProperty(txtNewSupplmProperty.Text))
            {
                txtNewSupplmProperty.Text = string.Empty;
            }
            txtNewSupplmProperty.Focus();
        }

        private void txtNewSupplmProperty_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckForAddSupplmProperty();
        }

        private void grdSupplementaryEntities_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ResxEngine.SupplementaryEntities.Count < ResxEngine.DefaultSupplementaryEntities.Length)
            {
                ResxEngine.SetSupplementaryEntities();
            }
            CheckForAddSupplmProperty();
        }

        private void CheckForAddSupplmProperty()
        {
            string text = txtNewSupplmProperty.Text;
            btnAddSupplmProperty.IsEnabled = ResxEngine.SelectedSuplmEntity != null &&
                        text.Length > 1 &&
                        !ResxEngine.ExistSupplmProperty(text);
        }

        private void btnRemoveSupplmProperty_Click(object sender, RoutedEventArgs e)
        {
            if (ResxEngine.SelectedSuplmProperty == null)
                return;
            ResxEngine.SelectedSuplmEntity.Properties.Remove(ResxEngine.SelectedSuplmProperty);
        }
    }
}
