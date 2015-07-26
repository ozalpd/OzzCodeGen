using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OzzCodeGen
{
    [Serializable()]
    public abstract class SavableList<T> : List<T>
    {
        public void ChangeIndex(int newIndex, T item)
        {
            Remove(item);
            Insert(newIndex, item);
        }

        public void MoveTop(T item)
        {
            ChangeIndex(0, item);
        }

        public void MoveUp(T item)
        {
            int newIndex = IndexOf(item) - 1;
            if (newIndex < 0) return;
            ChangeIndex(newIndex, item);
        }

        public void MoveDown(T item)
        {
            int newIndex = IndexOf(item) + 1;
            if (newIndex < Count)
            {
                ChangeIndex(newIndex, item);
            }
        }

        public void MoveBottom(T item)
        {
            Remove(item);
            Add(item);
        }

        public virtual void SaveToFile(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(writer, this);
            writer.Close();
        }
    }
}
