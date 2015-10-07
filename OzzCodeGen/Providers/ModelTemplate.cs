using OzzUtils;

namespace OzzCodeGen.Providers
{
    public class ModelTemplate
    {
        public ModelTemplate() { }
        public ModelTemplate(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }

        public override string ToString()
        {
            return FileName.PascalCaseToTitleCase();
        }
    }
}
