using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Definitions
{
    public class EnumDefinitionList : SavableList<EnumDefinition>
    {
        public override string ToString()
        {
            return string.Format("[{0}]", Count);
        }



        public new void Add(EnumDefinition enumDefinition)
        {
            if (!this.Contains(enumDefinition))
            {
                if (enumDefinition.EnumList != this)
                {
                    enumDefinition.EnumList = this;
                }
                base.Add(enumDefinition);
            }
        }

        public void Add(EnumDefinition member, bool distinctly)
        {
            if (distinctly && this.Any(p => p.Name == member.Name)) return;
            this.Add(member);
        }

    }
}
