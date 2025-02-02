using OzzCodeGen.CodeEngines.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.TechDocument.Templates
{
    public partial class TechDocTemplate : AbstractTemplate
    {
        public TechDocTemplate(TechDocEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
        }

        public TechDocEntitySetting EntitySetting { get; private set; }

        public override string GetDefaultFileName()
        {
            return EntitySetting.Name + ".txt";
        }
    }
}
