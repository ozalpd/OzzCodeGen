using OzzUtils.Savables;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace OzzUtils.Wpf.Savables
{
    public abstract class SavableCollection<T> : ObservableCollection<T>, ISavable
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


        public static void OpenFileInToList(string fileName, SavableCollection<T> list)
        {
            if (!File.Exists(fileName)) return;

            StreamReader reader = new StreamReader(fileName);
            XmlSerializer x = new XmlSerializer(list.GetType());
            var newInstance = x.Deserialize(reader) as ObservableCollection<T>;
            reader.Close();

            list.Clear();
            foreach (var item in newInstance)
            {
                list.Add(item);
            }
            newInstance.Clear();
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
