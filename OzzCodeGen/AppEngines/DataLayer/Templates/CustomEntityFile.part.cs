using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.DataLayer.Templates
{
    public partial class CustomEntityFile : BaseDalTemplate
    {

        public override string GetDefaultFileName()
        {
            return EntitySetting.Name + ".cust.cs";
        }
    }
}
