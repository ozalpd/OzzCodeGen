using OzzUtils.Savables;
using System.Linq;

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
