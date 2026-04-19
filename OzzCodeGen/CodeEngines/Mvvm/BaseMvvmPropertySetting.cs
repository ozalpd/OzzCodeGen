using OzzCodeGen.CodeEngines.CSharp;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Mvvm;

public abstract class BaseMvvmPropertySetting : BaseCSharpPropertySetting
{
    [XmlIgnore]
    [JsonIgnore]
    public IMvvmEntitySetting MvvmEntitySetting => (IMvvmEntitySetting)EntitySetting;

    [XmlIgnore]
    [JsonIgnore]
    public BaseMvvmCodeEngine CodeEngine => MvvmEntitySetting.CodeEngine;

    public bool IncludeInView
    {
        get { return _includeInView ?? true; }
        set
        {
            if (_includeInView == value) return;
            _includeInView = value;
            RaisePropertyChanged(nameof(IncludeInView));
        }
    }
    private bool? _includeInView;

    public bool IncludeInViewModel
    {
        get { return _includeInViewModel ?? true; }
        set
        {
            if (_includeInViewModel == value) return;
            _includeInViewModel = value;
            RaisePropertyChanged(nameof(IncludeInViewModel));
        }
    }
    private bool? _includeInViewModel;

    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }
}
