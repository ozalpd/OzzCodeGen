using System.IO;
using OzzCodeGen.CodeEngines.TechDocument;
using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines.Storage;
using System;

namespace OzzCodeGen.CodeEngines
{
    public static class EngineTypes
    {
        public const string AspNetMvcEngineId = "AspNetMvc_Controller_View_Generator";
        public const string EfDbFirstDataLayerId = "EF_DatabaseFirst_DataLayer";
        public const string EfTechnicalDocId = "EF_Technical_Document";
        public const string LocalizationResxGenId = "Localization_Resource_Generator";
        public const string MetadataCodeEngineId = "Metadata_Class_Generator";

        public const string TSqlScriptsId = "T-Sql_Scripts_Generator";
        public const string SqliteScriptsId = "Sqlite_Scripts_Generator";

        public const string JavaEngineId = "Java_Code_Generator";
        public const string AndroidEngineId = "Android_Code_Generator";

        public const string ObjcEngineId = "ObjectiveC_Code_Generator";


        public static BaseCodeEngine GetInstance(string targetProjectId)
        {
            switch (targetProjectId)
            {
                case MetadataCodeEngineId:
                    return new Metadata.MetadataCodeEngine();

                case AspNetMvcEngineId:
                    return new AspNetMvc.AspNetMvcEngine();

                case EfDbFirstDataLayerId:
                    // Entity Framework Database First Data Layer Engine removed in this version 2026-01-13 Özalp
                    return null;//new DataLayerEngine();

                case EfTechnicalDocId:
                    return new TechDocumentEngine();

                case LocalizationResxGenId:
                    return new ResxEngine();

                case TSqlScriptsId:
                    return new TSqlScriptsEngine();

                case ObjcEngineId:
                    //Objective-C code generation engine removed in this version 2026-01-13 Özalp
                    return null; // new ObjectiveC.ObjcEngine();

                case SqliteScriptsId:
                    return new SqliteScriptsEngine();

                case AndroidEngineId:
                    // Android and Java code generation engine removed in this version 2026-01-13 Özalp
                    return null; // new Android.AndroidEngine();

                default:
                    return null;
            }
        }

        static string removeFromXML = "Remove the line containing '{0}' text from the CodeEngineList section of the project file.";

        public static BaseCodeEngine OpenFile(string Directory, string targetProjectId)
        {
            string fileName;

            switch (targetProjectId)
            {
                case MetadataCodeEngineId:
                    fileName = Path.Combine(Directory, Metadata.MetadataCodeEngine.DefaultFileName);
                    return Metadata.MetadataCodeEngine.OpenFile(fileName);

                case AspNetMvcEngineId:
                    fileName = Path.Combine(Directory, AspNetMvc.AspNetMvcEngine.DefaultFileName);
                    return AspNetMvc.AspNetMvcEngine.OpenFile(fileName);

                case EfDbFirstDataLayerId:
                    // Entity Framework Database First Data Layer Engine removed in this version 2026-01-13 Özalp
                    throw new NotImplementedException("Entity Framework Database First Data Layer Engine removed!\r\n"
                        + string.Format(removeFromXML, EfDbFirstDataLayerId));
                    //fileName = Path.Combine(Directory, DataLayerEngine.DefaultFileName);
                    //return DataLayerEngine.OpenFile(fileName);

                case EfTechnicalDocId:
                    fileName = Path.Combine(Directory, TechDocumentEngine.DefaultFileName);
                    return TechDocumentEngine.OpenFile(fileName);

                case LocalizationResxGenId:
                    fileName = Path.Combine(Directory, ResxEngine.DefaultFileName);
                    return ResxEngine.OpenFile(fileName);

                case TSqlScriptsId:
                    fileName = Path.Combine(Directory, TSqlScriptsEngine.DefaultFileName);
                    return TSqlScriptsEngine.OpenFile(fileName);

                case ObjcEngineId:
                    //Objective-C code generation engine removed in this version 2026-01-13 Özalp
                    throw new NotImplementedException("Objective-C code generation engine removed!\r\n"
                        + string.Format(removeFromXML, ObjcEngineId));
                //fileName = Path.Combine(Directory, ObjectiveC.ObjcEngine.DefaultFileName);
                //return ObjectiveC.ObjcEngine.OpenFile(fileName);

                case SqliteScriptsId:
                    fileName = Path.Combine(Directory, SqliteScriptsEngine.DefaultFileName);
                    return SqliteScriptsEngine.OpenFile(fileName);

                case AndroidEngineId:
                    // Android code generation engine removed in this version 2026-01-13 Özalp
                    throw new NotImplementedException("Android code generation engine removed!\r\n"
                        + string.Format(removeFromXML, AndroidEngineId));
                    //fileName = Path.Combine(Directory, Android.AndroidEngine.DefaultFileName);
                    //return Android.AndroidEngine.OpenFile(fileName);

                default:
                    return null;
            }
        }
    }
}
