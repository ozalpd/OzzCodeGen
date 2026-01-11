using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OzzUtils.Savables
{
    [Serializable()]
    public abstract class SavableList<T> : List<T>, ISavable
    {
        public void ChangeIndex(int newIndex, T item)
        {
            Remove(item);
            Insert(newIndex, item);
        }

        public virtual void MoveTop(T item)
        {
            ChangeIndex(0, item);
        }

        public virtual void MoveUp(T item)
        {
            int newIndex = IndexOf(item) - 1;
            if (newIndex < 0) return;
            ChangeIndex(newIndex, item);
        }

        public virtual void MoveDown(T item)
        {
            int newIndex = IndexOf(item) + 1;
            if (newIndex < Count)
            {
                ChangeIndex(newIndex, item);
            }
        }

        public virtual void MoveBottom(T item)
        {
            Remove(item);
            Add(item);
        }

        public virtual void SaveToFile(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            StreamWriter writer = new StreamWriter(fileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }
    }
}
