using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.TechDocument
{
    public class TechDocPropertySetting : BasePropertySetting
    {
        [XmlIgnore]
        public TechDocEntitySetting TechDocEntitySetting
        {
            get { return (TechDocEntitySetting)EntitySetting; }
        }

        [XmlIgnore]
        public TechDocumentEngine CodeEngine
        {
            get { return TechDocEntitySetting.CodeEngine; }
        }



    }
}
