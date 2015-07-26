using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Providers.SourceCode
{
    public class CodeFile
    {
        FileInfo fileInfo;

        public CodeFile(string fileName)
        {
            fileInfo = new FileInfo(fileName);
        }


        public SourceCodeBlock FileContent { get; set; }

        public DirectoryInfo Directory
        {
            get { return fileInfo.Directory; }
        }

        public bool Exists
        {
            get { return fileInfo.Exists; }
        }

        public string FileName
        {
            get { return fileInfo.Name; }
        }

        public string FullPath
        {
            get { return fileInfo.FullName; }
        }


        public virtual string ReadAll()
        {
            var rd = GetStreamReader();
            string s = rd.ReadToEnd();
            rd.Close();

            return s;
        }

        public virtual List<string> ReadLines()
        {
            var rd = GetStreamReader();
            var lines = new List<string>();
            do
            {
                string s = rd.ReadLine();
                lines.Add(s);

            } while (!rd.EndOfStream);
            rd.Close();

            return lines;
        }

        protected virtual StreamReader GetStreamReader()
        {
            if (!Exists)
            {
                throw new Exception("File not found!");
            }

            return new StreamReader(FullPath);
        }

        public override string ToString()
        {
            return FullPath;
        }
    }
}
