using OzzUtils.Savables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OzzLocalization
{
    public class Vocabulary : SavableList<Vocab>
    {
        public Vocabulary(string cultureCode)
        {
            CultureCode = cultureCode;
        }

        [XmlIgnore]
        public string CultureCode { get; private set; }

        [XmlIgnore]
        public string FilePath { get; set; }

        public void AddUnique(Vocab vocab)
        {
            if (!this.Any(v => v.Name.Equals(vocab.Name)))
            {
                Add(vocab);
            }
        }

        public static Vocabulary OpenFile(string fileName, string cultureCode)
        {
            List<Vocab> vocabulary = OpenVocabList(fileName);
            Vocabulary instance = new Vocabulary(cultureCode);
            instance.FilePath = fileName;

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
