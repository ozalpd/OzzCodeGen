using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.Mvvm;

public interface IMvvmEntitySetting
{
    string Name { get; }
    BaseMvvmCodeEngine CodeEngine { get; set; }
    IEnumerable<BaseMvvmPropertySetting> MvvmProperties { get; }
    BaseMvvmPropertySetting FindProperty(string propertyName);
    void AddProperty(BaseMvvmPropertySetting propertySetting);
    bool RemoveProperty(BaseMvvmPropertySetting propertySetting);
    void SortProperties();
}
