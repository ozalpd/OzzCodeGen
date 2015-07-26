using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.Providers
{
    [Serializable()]
    public class EntitySource
    {
        public string SourceName { get; set; }
        public string SourcePath { get; set; }
    }
}
