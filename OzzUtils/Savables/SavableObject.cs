using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;


namespace OzzUtils.Savables
{
    [Serializable()]
    public abstract class SavableObject
    {
        protected static SavableObject GetInstanceFromFile(string fileName, Type type)
        {
            if (!typeof(SavableObject).IsAssignableFrom(type))
            {
                throw new Exception("The type must be inherited from BaseSavable");
            }
            StreamReader reader = new StreamReader(fileName);
            XmlSerializer x = new XmlSerializer(type);
            SavableObject instance = x.Deserialize(reader) as SavableObject;
            reader.Close();
            instance.SavedFileName = fileName;

            return instance;
        }

        [XmlIgnore]
        [JsonIgnore]
        public string SavedFileName { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool SaveAsJson { get; set; }

        public virtual void SaveToFile()
        {
            var folder = Path.GetDirectoryName(SavedFileName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (SaveAsJson)
            {
                SaveToJson();
                return;
            }
            SaveToXML();
        }

        public virtual void SaveToFile(string fileName)
        {
            SavedFileName = fileName;
            SaveToFile();
        }

        public virtual void SaveToJson()
        {
            string jsonFileName = $"{SavedFileName}.json";
            using (StreamWriter writer = new StreamWriter(jsonFileName))
            {
                string json = JsonSerializer.Serialize(this, this.GetType());
                writer.Write(json);
                writer.Close();
            }
        }

        public virtual void SaveToXML()
        {
            StreamWriter writer = new StreamWriter(SavedFileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }
    }
}
