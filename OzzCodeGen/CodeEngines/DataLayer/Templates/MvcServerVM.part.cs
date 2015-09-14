using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.DataLayer.Templates
{
    public partial class MvcServerVM : MetadataClass
    {
        public override string GetDefaultFileName()
        {
            return EntitySetting.Name + "ViewModel.gen.cs";
        }
    }
}
