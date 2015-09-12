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
            var vocabularies = Vocabulary.OpenVocabularies(engine.VocabularyDir);
            foreach (var dict in vocabularies)
            {
                foreach (var item in combinedEntity.Properties)
                {
                    dict.Value.AddUnique(new Vocab() { Name = item.Name });
                }

                var sortedVocabulary = new Vocabulary(dict.Value.CultureCode);
                foreach (var item in dict.Value.OrderBy(v => v.Name))
                {
                    sortedVocabulary.Add(item);
                }
                sortedVocabulary.SaveToFile(dict.Value.FilePath);
            }
        }
    }
}
