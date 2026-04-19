using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

public class WpfMvvmPropertySetting : Mvvm.BaseMvvmPropertySetting
{
    public bool IsReadOnly
    {
        get
        {
            if (_isReadOnly == null)
            {
                _isReadOnly = IsKey;
            }
            return _isReadOnly.Value;
        }
        set
        {
            if (_isReadOnly == value) return;
            _isReadOnly = value;
            RaisePropertyChanged(nameof(IsReadOnly));
        }
    }
    private bool? _isReadOnly;

    public string GetEditorControlType()
    {
        if (PropertyDefinition.IsTypeBoolean())
            return "CheckBox";

        if (PropertyDefinition is SimpleProperty simple && simple.IsTypeDateTime())
            return "DatePicker";

        return "TextBox";
    }
}
