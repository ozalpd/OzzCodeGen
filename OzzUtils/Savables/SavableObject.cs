using System;
using System.IO;
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
        public string SavedFileName { get; set; }

        public virtual void SaveToFile()
        {
            var folder = Path.GetDirectoryName(SavedFileName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            StreamWriter writer = new StreamWriter(SavedFileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }

        public virtual void SaveToFile(string fileName)
        {
            SavedFileName = fileName;
            SaveToFile();
        }
    }
}
