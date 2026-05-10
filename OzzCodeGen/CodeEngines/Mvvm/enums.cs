namespace OzzCodeGen.CodeEngines.Mvvm;

/// <summary>
/// Specifies the types of base view models used for MVVM code generation.
/// </summary>
/// <remarks>This enumeration is used to identify and select the foundational view model types when generating
/// MVVM infrastructure or entity-specific view models. The values correspond to common MVVM patterns, such as base view
/// models, data error information support, collection view models, and create/edit view models.</remarks>
public enum BaseVM
{
    BaseViewModel,
    DataErrorInfoVM,
    CollectionVM,
    CreateEditVM
}

/// <summary>
/// Specifies the types of lookup service templates that can be generated.
/// </summary>
/// <remarks>This enumeration is used to distinguish between interface, design-time, and run-time implementations
/// of lookup services in code generation scenarios. The values correspond to different template outputs for lookup
/// service generation.</remarks>
public enum LookupTemplate
{
    Interface,
    DesignTimeClass,
    RunTimeClass
}

/// <summary>
/// Specifies the type of command, view model, or view to be generated, such as creating, editing, deleting, or
/// managing a collection of items.
/// </summary>
public enum MvvmTemplate
{
    Create,
    Edit,
    Delete,
    Collection
}
