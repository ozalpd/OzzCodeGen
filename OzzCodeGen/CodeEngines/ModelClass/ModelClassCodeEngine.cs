using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.ModelClass;

public class ModelClassCodeEngine : ModelClassCodeEngineBase
{
    public override string EngineId { get { return EngineTypes.ModelClassCodeEngineId; } }

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName { get { return "ModelClassCodeEngine.settings"; } }

    public override string ProjectTypeName { get { return "Model class generator"; } }

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    /// <summary>
    /// Reads a project settings file and creates a ProjectSettings instance
    /// </summary>
    /// <param name="fileName">An XML file's path that contains project settings</param>
    /// <returns></returns>
    public static ModelClassCodeEngine OpenFile(string fileName)
    {
        ModelClassCodeEngine instance = GetInstanceFromFile(
           fileName,
           typeof(ModelClassCodeEngine)) as ModelClassCodeEngine;

        return instance;
    }
}
