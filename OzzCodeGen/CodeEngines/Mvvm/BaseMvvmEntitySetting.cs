using OzzCodeGen.CodeEngines.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Mvvm;

public abstract class BaseMvvmEntitySetting<TPropertySetting> : BaseCSharpEntitySetting<TPropertySetting>, IMvvmEntitySetting
    where TPropertySetting : BaseMvvmPropertySetting
{
    [XmlIgnore]
    [JsonIgnore]
    public virtual BaseMvvmCodeEngine CodeEngine { get; set; }

    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }

    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<BaseMvvmPropertySetting> MvvmProperties => Properties.Cast<BaseMvvmPropertySetting>();

    public bool GenerateCreateView
    {
        get { return _generateCreateView ?? true; }
        set
        {
            if (_generateCreateView == value) return;
            _generateCreateView = value;
            RaisePropertyChanged(nameof(GenerateCreateView));
        }
    }
    private bool? _generateCreateView;

    public bool GenerateEditView
    {
        get { return _generateEditView ?? true; }
        set
        {
            if (_generateEditView == value) return;
            _generateEditView = value;
            RaisePropertyChanged(nameof(GenerateEditView));
        }
    }
    private bool? _generateEditView;

    public bool GenerateCreateViewModel
    {
        get { return _generateCreateViewModel ?? true; }
        set
        {
            if (_generateCreateViewModel == value) return;
            _generateCreateViewModel = value;
            RaisePropertyChanged(nameof(GenerateCreateViewModel));
        }
    }
    private bool? _generateCreateViewModel;

    public bool GenerateEditViewModel
    {
        get { return _generateEditViewModel ?? true; }
        set
        {
            if (_generateEditViewModel == value) return;
            _generateEditViewModel = value;
            RaisePropertyChanged(nameof(GenerateEditViewModel));
        }
    }
    private bool? _generateEditViewModel;

    public bool GenerateCommands
    {
        get { return _generateCommands ?? true; }
        set
        {
            if (_generateCommands == value) return;
            _generateCommands = value;
            RaisePropertyChanged(nameof(GenerateCommands));
        }
    }
    private bool? _generateCommands;

    public BaseMvvmPropertySetting FindProperty(string propertyName)
    {
        return Properties.FirstOrDefault(p => p.Name == propertyName);
    }

    public void AddProperty(BaseMvvmPropertySetting propertySetting)
    {
        Properties.Add((TPropertySetting)propertySetting);
    }

    public bool RemoveProperty(BaseMvvmPropertySetting propertySetting)
    {
        return Properties.Remove((TPropertySetting)propertySetting);
    }

    public void SortProperties()
    {
        Properties = Properties.OrderBy(p => p.PropertyDefinition.DisplayOrder).ToList();
    }
}
