using System.ComponentModel;

namespace OzzCodeGen;

/// <summary>
/// Specifies the file generation behavior to use when creating or updating output files.
/// </summary>
/// <remarks>Use this enumeration to control whether files are generated, overwritten, or only created if they do
/// not already exist. This is commonly used by code generation engines to determine how to handle existing files during
/// output operations.</remarks>
public enum FileGenerationMode
{
    [Description("Do Not Generate")]
    DoNotGenerate = 0,
    [Description("Generate and Overwrite")]
    GenerateAndOverwrite = 1,
    [Description("Generate if Not Exists")]
    GenerateIfNotExists = 2
}

public enum DefinitionType
{
    Collection,
    Complex,
    DateTime,
    Simple,
    String
}

public enum TargetDotNetPlatform
{
    DotNetFramework,
    ModernDotNet
}

