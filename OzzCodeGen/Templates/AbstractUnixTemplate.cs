using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Templates
{
    public abstract class AbstractUnixTemplate : AbstractTemplate
    {
        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            if (!overwriteExisting && File.Exists(FilePath))
            {
                return false;
            }
            string dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            StreamWriter sw = new StreamWriter(FilePath);
            string s = TransformText().Replace("\r\n", "\n");
            sw.Write(s);
            sw.Close();
            return true;
        }
    }
}
