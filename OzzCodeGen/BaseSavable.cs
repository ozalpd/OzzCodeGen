using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen
{
    [Serializable()]
    public abstract class BaseSavable
    {
        protected static BaseSavable GetInstanceFromFile(string fileName, Type type)
        {
            if (!typeof(BaseSavable).IsAssignableFrom(type))
            {
                throw new Exception("The type must be inherited from BaseSavable");
            }
            StreamReader reader = new StreamReader(fileName);
            XmlSerializer x = new XmlSerializer(type);
            BaseSavable instance = x.Deserialize(reader) as BaseSavable;
            reader.Close();
            instance.SavedFileName = fileName;

            return instance;
        }

        [XmlIgnore]
        public string SavedFileName { get; set; }

        public virtual void SaveToFile()
        {
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
