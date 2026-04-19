using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates;

public class WpfMvvmViewXamlTemplate : AbstractTemplate
{
    private readonly WpfMvvmEntitySetting _entitySetting;
    private readonly WpfMvvmCodeEngine _codeEngine;
    private readonly bool _isEdit;

    public WpfMvvmViewXamlTemplate(WpfMvvmEntitySetting entitySetting, bool isEdit)
    {
        _entitySetting = entitySetting;
        _codeEngine = (WpfMvvmCodeEngine)entitySetting.CodeEngine;
        _isEdit = isEdit;
    }

    public override string GetDefaultFileName()
    {
        return $"{_entitySetting.Name}{(_isEdit ? "Edit" : "Create")}View.xaml";
    }

    public override string TransformText()
    {
        var props = _entitySetting
            .GetInheritedIncludedProperties()
            .OfType<WpfMvvmPropertySetting>()
            .Where(p => p.IncludeInView)
            .ToList();

        var rows = new StringBuilder();
        for (int i = 0; i < props.Count; i++)
        {
            rows.AppendLine("        <RowDefinition Height=\"Auto\" />");
        }

        var fields = new StringBuilder();
        for (int i = 0; i < props.Count; i++)
        {
            var prop = props[i];
            var rowIndex = i;
            fields.AppendLine($"    <TextBlock Grid.Row=\"{rowIndex}\" Grid.Column=\"0\" Margin=\"0,4,8,4\" VerticalAlignment=\"Center\" Text=\"{prop.Name}:\" />");

            var controlType = prop.GetEditorControlType();
            if (controlType == "CheckBox")
            {
                fields.AppendLine($"    <CheckBox Grid.Row=\"{rowIndex}\" Grid.Column=\"1\" Margin=\"0,4,0,4\" IsChecked=\"{{Binding {prop.Name}, Mode=TwoWay}}\" IsEnabled=\"{(!prop.IsReadOnly).ToString().ToLowerInvariant()}\" />");
            }
            else if (controlType == "DatePicker")
            {
                fields.AppendLine($"    <DatePicker Grid.Row=\"{rowIndex}\" Grid.Column=\"1\" Margin=\"0,4,0,4\" SelectedDate=\"{{Binding {prop.Name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}\" IsEnabled=\"{(!prop.IsReadOnly).ToString().ToLowerInvariant()}\" />");
            }
            else
            {
                fields.AppendLine($"    <TextBox Grid.Row=\"{rowIndex}\" Grid.Column=\"1\" Margin=\"0,4,0,4\" Text=\"{{Binding {prop.Name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}\" IsReadOnly=\"{prop.IsReadOnly.ToString().ToLowerInvariant()}\" />");
            }
        }

        var buttonRow = props.Count;

        return $"<UserControl x:Class=\"{_codeEngine.ViewNamespaceName}.{_entitySetting.Name}{(_isEdit ? "Edit" : "Create")}View\"\n"
             + "             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n"
             + "             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n"
             + "             MinWidth=\"320\" MinHeight=\"240\">\n"
             + "  <Grid Margin=\"12\">\n"
             + "    <Grid.RowDefinitions>\n"
             + rows
             + "        <RowDefinition Height=\"Auto\" />\n"
             + "    </Grid.RowDefinitions>\n"
             + "    <Grid.ColumnDefinitions>\n"
             + "      <ColumnDefinition Width=\"180\" />\n"
             + "      <ColumnDefinition Width=\"*\" />\n"
             + "    </Grid.ColumnDefinitions>\n\n"
             + fields
             + $"    <StackPanel Grid.Row=\"{buttonRow}\" Grid.ColumnSpan=\"2\" Orientation=\"Horizontal\" HorizontalAlignment=\"Right\" Margin=\"0,12,0,0\">\n"
             + "      <Button Margin=\"0,0,8,0\" MinWidth=\"96\" Content=\"Save\" Command=\"{Binding Commands.SaveCommand}\" />\n"
             + "      <Button MinWidth=\"96\" Content=\"Cancel\" Command=\"{Binding Commands.CancelCommand}\" />\n"
             + "    </StackPanel>\n"
             + "  </Grid>\n"
             + "</UserControl>\n";
    }
}
