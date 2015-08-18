using OzzCodeGen.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OzzCodeGen
{
    public class DataModel : List<EntityDefinition>
    {
        public new void Add(EntityDefinition entity)
        {
            if (!this.Contains(entity)) base.Add(entity);
            if (entity.DataModel == this) return;
            entity.DataModel = this;
        }

        public void ChangeIndex(int newIndex, EntityDefinition entity)
        {
            Remove(entity);
            Insert(newIndex, entity);
            int i = 0;
            foreach (var item in this)
            {
                i++;
                item.DisplayOrder = i;
            }
        }

        public void MoveTop(EntityDefinition entity)
        {
            ChangeIndex(0, entity);
        }

        public void MoveUp(EntityDefinition entity)
        {
            int newIndex = IndexOf(entity) - 1;
            if (newIndex < 0) return;
            ChangeIndex(newIndex, entity);
        }

        public void MoveDown(EntityDefinition entity)
        {
            int newIndex = IndexOf(entity) + 1;
            if (newIndex < Count)
            {
                ChangeIndex(newIndex, entity);
            }
        }

        public void MoveBottom(EntityDefinition entity)
        {
            Remove(entity);
            Add(entity);
        }

        public override string ToString()
        {
            return string.Format("DataModel with {0} EntityDefinition", Count);
        }

        /// <summary>
        /// Reads a project settings file and creates a ProjectSettings instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static DataModel OpenFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            XmlSerializer x = new XmlSerializer(typeof(DataModel));
            DataModel instance = x.Deserialize(reader) as DataModel;
            reader.Close();

            return instance;
        }

        public void SaveToFile(string FileName)
        {
            StreamWriter writer = new StreamWriter(FileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }
    }
}
