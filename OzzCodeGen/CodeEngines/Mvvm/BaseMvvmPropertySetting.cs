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



    /// <summary>
    /// Gets or sets the display order of the property in the create or edit view.
    /// </summary>
    public int CreateEditOrder
    {
        get { return _createEditOrder ?? PropertyDefinition.DisplayOrder; }
        set
        {
            if (_createEditOrder == value) return;
            _createEditOrder = value;
            RaisePropertyChanged(nameof(CreateEditOrder));
            RaisePropertyChanged(nameof(MvvmEntitySetting.PropertiesInCreateEditOrder));
        }
    }
    private int? _createEditOrder;

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

    [XmlIgnore]
    [JsonIgnore]
    public bool IsReadOnlyInCreate => CreateViewMode == ViewFieldMode.ReadOnly;

    [XmlIgnore]
    [JsonIgnore]
    public bool IsReadOnlyInEdit => EditViewMode == ViewFieldMode.ReadOnly;

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

    public ViewFieldMode CreateViewMode
    {
        get { return _createViewMode ?? ViewFieldMode.Editable; }
        set
        {
            if (_createViewMode == value) return;
            _createViewMode = value;
            RaisePropertyChanged(nameof(CreateViewMode));
        }
    }
    private ViewFieldMode? _createViewMode;

    public ViewFieldMode EditViewMode
    {
        get
        {
            if (_editViewMode == null)
            {
                _editViewMode = IsKey || IsImmutable
                                 || PropertyDefinition.IsServerComputed
                                 || PropertyDefinition.IsClientComputed
                              ? ViewFieldMode.ReadOnly
                              : ViewFieldMode.Editable;
            }

            return _editViewMode ?? ViewFieldMode.Editable;
        }
        set
        {
            if (_editViewMode == value) return;
            _editViewMode = value;
            RaisePropertyChanged(nameof(EditViewMode));
        }
    }
    private ViewFieldMode? _editViewMode;

    public bool ShowInCollection
    {
        get { return _showInCollection ?? true; }
        set
        {
            if (_showInCollection == value) return;
            _showInCollection = value;
            RaisePropertyChanged(nameof(ShowInCollection));
        }
    }
    private bool? _showInCollection;

    public bool ShowInDetail
    {
        get { return _showInDetail ?? true; }
        set
        {
            if (_showInDetail == value) return;
            _showInDetail = value;
            RaisePropertyChanged(nameof(ShowInDetail));
        }
    }
    private bool? _showInDetail;


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

    /// <summary>
    /// Gets the dependent property setting associated with this property, if one exists.
    /// </summary>
    /// <remarks>A dependent property is determined based on the underlying property definition. This method
    /// searches for a matching property by name within the entity's MVVM property collection.</remarks>
    /// <returns>A <see cref="BaseMvvmPropertySetting"/> representing the dependent property, or <see langword="null"/> if no
    /// dependent property is found.</returns>
    public BaseMvvmPropertySetting GetDependent()
    {
        if (PropertyDefinition is SimpleProperty simpleProperty && IsForeignKey)
        {
            var dependentProperty = simpleProperty.GetDependent();
            if (dependentProperty != null)
            {
                return MvvmEntitySetting.MvvmProperties
                                        .FirstOrDefault(p => dependentProperty.Name.Equals(p.Name));
            }
        }
        return null;
    }

    protected override BaseCodeEngine GetCodeEngine()
    {
        return CodeEngine;
    }
}
