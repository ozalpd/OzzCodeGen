using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzLocalization
{
    public class Vocabularies : Dictionary<string, Vocabulary>
    {

        public static string NotrCode = "notr"; //Notr or default vocabulary code
        public static string FilePattern = "vocabulary.??.xml";
        public static string GetNotrFileName() //should be "vocabulary.notr.xml"
        {
            return FilePattern.Replace("??", NotrCode);
        }

        public Vocabulary GetNotrVocabulary()
        {
            var vocabulary = GetVocabulary(NotrCode);
            if (vocabulary == null)
            {
                return new Vocabulary(NotrCode);
            }
            else
            {
                return vocabulary;
            }
        }

        public Vocabulary GetVocabulary(string cultureCode)
        {
            if (!string.IsNullOrEmpty(cultureCode) && this.ContainsKey(cultureCode))
            {
                return this.FirstOrDefault(v => v.Key.Equals(cultureCode)).Value;
            }
            else
            {
                return null;
            }
        }

        public List<string> GetCultureCodes()
        {
            var codes = new List<string>();
            foreach(var item in this)
            {
                codes.Add(item.Key);
            }
            return codes;
        }

        public void SaveVocabularies()
        {
            foreach (var item in this)
            {
                item.Value.SaveToFile(item.Value.FilePath);
            }
        }

        public static Vocabularies OpenVocabularies(string directory)
        {
            Vocabularies vocabularies = new Vocabularies();

            string notrFile = Path.Combine(directory, GetNotrFileName());
            Vocabulary notrVocabulary = null;
            if (File.Exists(notrFile))
            {
                notrVocabulary = Vocabulary.OpenFile(notrFile, NotrCode);
            }
            else
            {
                notrVocabulary = new Vocabulary(NotrCode);
                notrVocabulary.FilePath = notrFile;
            }
            vocabularies.Add(NotrCode, notrVocabulary);

            string[] files = Directory.GetFiles(directory, FilePattern);
            foreach (string file in files)
            {
                string[] parts = file.Split('.');
                vocabularies.Add(parts[1], Vocabulary.OpenFile(file, parts[1]));
            }

            return vocabularies;
        }
    }
}
