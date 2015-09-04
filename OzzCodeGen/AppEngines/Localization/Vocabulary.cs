using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OzzCodeGen.AppEngines.Localization
{
    public class Vocab
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class Vocabulary : SavableList<Vocab>
    {
        public Vocabulary(string cultureCode)
        {
            CultureCode = cultureCode;
        }

        public string CultureCode { get; private set; }

        public static Vocabulary OpenFile(string fileName, string cultureCode)
        {
            List<Vocab> vocabulary = OpenVocabList(fileName);
            Vocabulary instance = new Vocabulary(cultureCode);
            foreach (var item in vocabulary)
            {
                instance.Add(item);
            }

            return instance;
        }

        public static List<Vocab> OpenVocabList(string FileName)
        {
            if (!File.Exists(FileName)) return new List<Vocab>();

            StreamReader reader = new StreamReader(FileName);
            XmlSerializer x = new XmlSerializer(typeof(List<Vocab>));
            List<Vocab> instance = x.Deserialize(reader) as List<Vocab>;
            reader.Close();

            return instance;
        }

        public static Dictionary<string, Vocabulary> OpenVocabularies(string directory)
        {
            Dictionary<string, Vocabulary> vocabularies = new Dictionary<string, Vocabulary>();
            string[] files = Directory.GetFiles(directory, "vocabulary.??.xml");

            foreach (string file in files)
            {
                string[] parts = file.Split('.');
                vocabularies.Add(parts[1], OpenFile(file, parts[1]));
            }

            return vocabularies;
        }
    }
}
