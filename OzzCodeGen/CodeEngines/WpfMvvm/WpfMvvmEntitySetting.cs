using OzzCodeGen.CodeEngines.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

public class WpfMvvmEntitySetting : BaseMvvmEntitySetting<WpfMvvmPropertySetting>
{
    public override AbstractEntitySetting<WpfMvvmPropertySetting> GetBaseEntitySetting()
    {
        if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
            return null;

        return (CodeEngine as WpfMvvmCodeEngine)?
            .Entities
            .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
    }

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
}
