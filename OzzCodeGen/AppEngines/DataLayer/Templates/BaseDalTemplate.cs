using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;
using OzzCodeGen.Templates.Cs;

namespace OzzCodeGen.AppEngines.DataLayer.Templates
{
    public abstract class BaseDalTemplate : CsClassBase
    {
        public DalEntitySetting EntitySetting
        {
            get { return _entitySetting; }
            set
            {
                _entitySetting = value;
            }
        }
        DalEntitySetting _entitySetting;

        public string ViewModelNamespace { get; set; }
    }
}
