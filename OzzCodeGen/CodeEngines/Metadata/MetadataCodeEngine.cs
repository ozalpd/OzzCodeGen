using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.ModelClass;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Metadata;

[XmlInclude(typeof(MetadataEntitySetting))]
public class MetadataCodeEngine : ModelClassCodeEngineBase
{
    public override string EngineId { get { return EngineTypes.MetadataCodeEngineId; } }

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName { get { return "MetadataCodeEngine.settings"; } }

    public override string ProjectTypeName { get { return "Metadata class generator"; } }

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public bool SeperateMetaDataClass
    {
        get { return _seperateMetaDataClass ?? true; }
        set
        {
            _seperateMetaDataClass = value;
            RaisePropertyChanged("SeperateMetaDataClass");
        }
    }
    private bool? _seperateMetaDataClass;


    /// <summary>
    /// Reads a project settings file and creates a ProjectSettings instance
    /// </summary>
    /// <param name="fileName">An XML file's path that contains project settings</param>
    /// <returns></returns>
    public static MetadataCodeEngine OpenFile(string fileName)
    {
        MetadataCodeEngine instance = GetInstanceFromFile(
           fileName,
           typeof(MetadataCodeEngine)) as MetadataCodeEngine;

        return instance;
    }
}
