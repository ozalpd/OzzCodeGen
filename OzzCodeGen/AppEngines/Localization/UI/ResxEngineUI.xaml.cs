using System.Linq;
using System.Windows;
using OzzCodeGen.UI;
using OzzLocalization;

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

        private void btnRefreshVocabulary_Click(object sender, RoutedEventArgs e)
        {
            var engine = (ResxEngine)AppEngine;
            var combinedEntity = engine.CombineEntities();
            var vocabularies = Vocabularies.OpenVocabularies(engine.VocabularyDir);
            foreach (var dict in vocabularies)
            {
                var vocabulary = dict.Value;
                foreach (var item in combinedEntity.Properties)
                {
                    vocabulary.AddUnique(new Vocab() { Name = item.Name });
                }

                vocabulary.OrderByName();
                vocabulary.SaveToFile(vocabulary.FilePath);
            }
        }
    }
}
