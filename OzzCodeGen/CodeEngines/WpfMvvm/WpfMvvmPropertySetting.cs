using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

public class WpfMvvmPropertySetting : Mvvm.BaseMvvmPropertySetting
{
    public string GetEditorControlType()
    {
        if (PropertyDefinition.IsTypeBoolean())
            return "CheckBox";

        if (PropertyDefinition is SimpleProperty simple && simple.IsTypeDateTime())
            return "DatePicker";

        return "TextBox";
    }
}
