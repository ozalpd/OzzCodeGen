using OzzCodeGen.CodeEngines.Mvvm;
using OzzCodeGen.Definitions;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

public class WpfMvvmPropertySetting : BaseMvvmPropertySetting
{
    public WpfMvvmEntitySetting GetLookupEntity()
    {
        var dependent = IsForeignKey
                      ? GetDependent()
                      : IsComplex ? this : null;
        if (dependent == null)
            return null;

        string dependentType = dependent.GetTypeName(getReturnType: true);
        var codeEngine = (WpfMvvmCodeEngine)CodeEngine;

        var entitySetting = codeEngine.EntitySettings
                                      .OfType<WpfMvvmEntitySetting>()
                                      .FirstOrDefault(e => dependentType == e.Name
                                                        && e.GenModeLookupService > FileGenerationMode.DoNotGenerate);
        return entitySetting;
    }

    public string GetEditorControlType()
    {
        if (PropertyDefinition.IsTypeBoolean())
            return "CheckBox";

        if (PropertyDefinition is SimpleProperty simple && simple.IsTypeDateTime())
            return "DatePicker";

        return "TextBox";
    }
}
