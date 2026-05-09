using OzzCodeGen.CodeEngines.CSharp;
using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.Definitions;
using System;
using System.Linq;
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


    public string DefaultValue
    {
        get
        {
            if (_defaultValue == null)
            {
                if (Name.Equals("DisplayOrder", StringComparison.InvariantCultureIgnoreCase))
                    _defaultValue = "1000";

                else if (Name.Equals("IsActive", StringComparison.InvariantCultureIgnoreCase))
                    _defaultValue = "true";

                else
                    _defaultValue = string.Empty;
            }
            return _defaultValue;
        }
        set
        {
            if (_defaultValue == value) return;
            _defaultValue = value;
            RaisePropertyChanged(nameof(DefaultValue));
        }
    }
    private string _defaultValue;

    public string FormatDefaultValue()
    {
        if (string.IsNullOrWhiteSpace(DefaultValue))
            return DefaultValue;

        if (IsString)
            return $"\"{DefaultValue}\"";

        if (IsDecimal)
            return $"{DefaultValue}m";

        if (IsDateTime)
            return $"DateTime.Parse(\"{DefaultValue}\")";

        if (IsFloat)
            return $"{DefaultValue}f";

        if (IsDouble)
            return $"{DefaultValue}d";


        return _defaultValue;
    }

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

    public bool IsPreselectedInCreate
    {
        get { return _isPreselectInCreate; }
        set
        {
            if (_isPreselectInCreate == value || !(IsLoadingFromFile || IsComplex))
                return;

            _isPreselectInCreate = value;
            RaisePropertyChanged(nameof(IsPreselectedInCreate));
        }
    }
    private bool _isPreselectInCreate;

    public bool IsReadOnlyInCreate
    {
        get
        {
            if (_isReadOnlyInCreate == null)
            {
                _isReadOnlyInCreate = IsKey;
            }
            return _isReadOnlyInCreate.Value;
        }
        set
        {
            if (_isReadOnlyInCreate == value) return;
            _isReadOnlyInCreate = value;
            RaisePropertyChanged(nameof(IsReadOnlyInCreate));
        }
    }
    private bool? _isReadOnlyInCreate;

    public bool IsReadOnlyInEdit
    {
        get
        {
            if (_isReadOnlyInEdit == null)
            {
                _isReadOnlyInEdit = IsKey || IsImmutable
                                 || PropertyDefinition.IsServerComputed
                                 || PropertyDefinition.IsClientComputed;
            }
            return _isReadOnlyInEdit.Value;
        }
        set
        {
            if (_isReadOnlyInEdit == value) return;
            _isReadOnlyInEdit = value;
            RaisePropertyChanged(nameof(IsReadOnlyInEdit));
        }
    }
    private bool? _isReadOnlyInEdit;

    public bool IsMultiLine
    {
        get
        {
            if (_isMultiLine == null && PropertyDefinition is StringProperty)
            {
                var sProperty = (StringProperty)PropertyDefinition;
                if (ModelPropertySetting != null)
                {
                    _isMultiLine = DataTypes.MultilineText.Equals(ModelPropertySetting.DataType);
                }
                else
                {
                    _isMultiLine = sProperty.MaxLength > 100;
                }
            }
            else if (_isMultiLine == null)
            {
                _isMultiLine = false;
            }
            return _isMultiLine ?? false;
        }
        set
        {
            if (_isMultiLine == value) return;
            _isMultiLine = value;
            RaisePropertyChanged(nameof(IsMultiLine));
        }
    }
    bool? _isMultiLine;

    [XmlIgnore]
    [JsonIgnore]
    public ModelPropertySetting ModelPropertySetting
    {
        get
        {
            if (_modelPropertySetting == null)
            {
                var modelEntity = CodeEngine.ModelClassCodeEngine
                                            .GetEntitySettingByName(EntitySetting.Name);
                _modelPropertySetting = modelEntity.Properties
                                                   .FirstOrDefault(p => p.Name == Name);
            }
            return _modelPropertySetting;
        }
    }
    private ModelPropertySetting _modelPropertySetting;

    /// <summary>
    /// Gets or sets the constraint applied to the property's value like .ToUpperInvariant() or < 0 ? 0 : value.
    /// </summary>
    public string ValueConstraint
    {
        get { return _valueSuffix ?? string.Empty; }
        set
        {
            if (_valueSuffix == value) return;
            _valueSuffix = value;
            RaisePropertyChanged(nameof(ValueConstraint));
        }
    }
    private string _valueSuffix;


    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }
}
