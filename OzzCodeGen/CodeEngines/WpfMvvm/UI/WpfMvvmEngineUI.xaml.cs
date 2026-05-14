using OzzCodeGen.CodeEngines.Mvvm;
using OzzCodeGen.UI;
using System.Linq;
using System.Windows.Controls;

namespace OzzCodeGen.CodeEngines.WpfMvvm.UI;

public partial class WpfMvvmEngineUI : AbstractEngineUI
{
    public WpfMvvmEngineUI()
    {
        InitializeComponent();

        HiddenColumns.Add("CodeEngine");
        HiddenColumns.Add("Properties");
        HiddenColumns.Add("MvvmProperties");
        HiddenColumns.Add("BaseEntitySetting");
        HiddenColumns.Add("IncludedProperties");
        HiddenColumns.Add("ModelPropertySetting");
        HiddenColumns.Add("GetBaseEntitySetting");
        HiddenColumns.Add("IsInEditDialogService");
        HiddenColumns.Add("RepositoryInstanceName");
        HiddenColumns.Add("IsInCreateDialogService");

        HiddenColumns.Add("ClrTypeName");
        HiddenColumns.Add("IsFloat");
        HiddenColumns.Add("IsDouble");
        HiddenColumns.Add("IsSimple");
        HiddenColumns.Add("IsString");
        HiddenColumns.Add("IsDecimal");
        HiddenColumns.Add("IsComplex");
        HiddenColumns.Add("IsBoolean");
        HiddenColumns.Add("IsDateTime");
        HiddenColumns.Add("IsNullable");
        HiddenColumns.Add("IsCollection");
        HiddenColumns.Add("RepositoryName");
        HiddenColumns.Add("IsIntegerNumeric");
        HiddenColumns.Add("IsNullableString");
        HiddenColumns.Add("CommandVmTypeName");
        HiddenColumns.Add("MvvmEntitySetting");
        HiddenColumns.Add("IsLoadingFromFile");
        HiddenColumns.Add("CommandVmNamespace");
        HiddenColumns.Add("IsFractionalNumeric");
        HiddenColumns.Add("RepositoryInstanceName");

        ColumnWidths["GenerateCreateView"] = 80;
        ColumnWidths["GenerateEditView"] = 72;
        ColumnWidths["GenerateCreateViewModel"] = 72;
        ColumnWidths["GenerateEditViewModel"] = 64;
        ColumnWidths["GenerateLookupService"] = 80;
        ColumnWidths["GenerateCommands"] = 80;
        ColumnWidths["IncludeInView"] = 80;
        ColumnWidths["IncludeInViewModel"] = 64;
        ColumnWidths["IsReadOnly"] = 80;

        EntitiesDataGrid = grdEntitySettings;
        PropertiesDataGrid = grdPropertySettings;

        btnMovePropertyTop.IsEnabled = false;
        btnMovePropertyUp.IsEnabled = false;
        btnMovePropertyDown.IsEnabled = false;
        btnMovePropertyBottom.IsEnabled = false;
    }

    public BaseMvvmPropertySetting GetSelectedCreateEditPropertySetting()
    {
        return (BaseMvvmPropertySetting)grdPropertiesInCreateEdit.SelectedItem;
    }

    private void MoveProperty_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_isMovingProperty || e.Source is Button == false)
            return;

        _isMovingProperty = true;
        var propertySetting = GetSelectedCreateEditPropertySetting();
        var entitySetting = (WpfMvvmEntitySetting)grdEntitySettings.SelectedItem;
        if (propertySetting == null || entitySetting == null)
        {
            _isMovingProperty = false;
            return;
        }

        if (!entitySetting.MvvmProperties.Contains(propertySetting))
        {
            _isMovingProperty = false;
            return;
        }

        var button = (Button)e.Source;
        bool isDown = button.Name == "btnMovePropertyDown" || button.Name == "btnMovePropertyBottom";

        var extremeOne = isDown ? entitySetting.MvvmProperties
                                               .OrderByDescending(p => p.CreateEditOrder)
                                               .FirstOrDefault()
                                : entitySetting.MvvmProperties
                                               .OrderBy(p => p.CreateEditOrder)
                                               .FirstOrDefault();

        if (propertySetting.Name == extremeOne.Name)
        {
            _isMovingProperty = false;
            return;
        }

        if (button.Name == "btnMovePropertyTop" || button.Name == "btnMovePropertyBottom")
        {
            int orderFirst = propertySetting.CreateEditOrder;
            propertySetting.CreateEditOrder = extremeOne.CreateEditOrder;
            int change = isDown ? -100 : 100;
            var propertiesToChange = isDown
                                   ? entitySetting.MvvmProperties
                                                  .Where(p => p.CreateEditOrder >= orderFirst
                                                           && p.Name != propertySetting.Name)
                                                  .OrderByDescending(p => p.CreateEditOrder)
                                   : entitySetting.MvvmProperties
                                                  .Where(p => p.CreateEditOrder <= orderFirst
                                                           && p.Name != propertySetting.Name)
                                                  .OrderBy(p => p.CreateEditOrder);
            foreach (var property in propertiesToChange)
            {
                property.CreateEditOrder = property.CreateEditOrder + change;
            }
        }
        else if (button.Name == "btnMovePropertyUp")
        {
            var prevOne = entitySetting.MvvmProperties
                                       .FirstOrDefault(p => p.Name != propertySetting.Name
                                                         && p.CreateEditOrder == propertySetting.CreateEditOrder - 100);
            if (prevOne != null)
            {
                propertySetting.CreateEditOrder -= 100;
                prevOne.CreateEditOrder += 100;
            }
        }
        else if (button.Name == "btnMovePropertyDown")
        {
            var nextOne = entitySetting.MvvmProperties
                                       .FirstOrDefault(p => p.Name != propertySetting.Name
                                                         && p.CreateEditOrder == propertySetting.CreateEditOrder + 100);
            if (nextOne != null)
            {
                propertySetting.CreateEditOrder += 100;
                nextOne.CreateEditOrder -= 100;
            }
        }
        int i = grdEntitySettings.SelectedIndex;
        grdEntitySettings.SelectedIndex = -1;
        entitySetting.RefreshPropertiesInCreateEditOrder();
        grdEntitySettings.SelectedIndex = i;
        grdPropertiesInCreateEdit.SelectedItem = propertySetting;
        _isMovingProperty = false;
    }
    private bool _isMovingProperty = false;

    private void grdPropertiesInCreateEdit_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var propertySetting = GetSelectedCreateEditPropertySetting();
        var entitySetting = (WpfMvvmEntitySetting)grdEntitySettings.SelectedItem;
        bool enabled = propertySetting != null && entitySetting != null && entitySetting.MvvmProperties.Contains(propertySetting);

        btnMovePropertyTop.IsEnabled = enabled;
        btnMovePropertyUp.IsEnabled = enabled;
        btnMovePropertyDown.IsEnabled = enabled;
        btnMovePropertyBottom.IsEnabled = enabled;
    }

    private void grdEntitySettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var entitySetting = (WpfMvvmEntitySetting)grdEntitySettings.SelectedItem;
        if (entitySetting != null)
            entitySetting.RefreshPropertiesInCreateEditOrder();
    }
}
