using System;
using System.Xml.Serialization;

namespace OzzCodeGen
{
    /// <summary>
    /// Overwrite method when saving a file
    /// </summary>
    public enum OverwriteMethod
    {
        SilentlyOverwrite,
        NeverOverwrite,
        AskToUser
    }

    public enum DefinitionType
    {
        Collection,
        Complex,
        DateTime,
        Simple,
        String
    }
}
