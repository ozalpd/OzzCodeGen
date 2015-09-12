using OzzUtils.Wpf;
using System.Linq;
using System.Xml.Serialization;

namespace OzzCodeGen.Definitions
{
    public class EnumMemberList : SavableList<EnumMember>
    {
        public override string ToString()
        {
            return string.Format("[{0}]", Count);
        }

        [XmlIgnore]
        public EnumDefinition EnumDefinition { get; set; }

        public new void Add(EnumMember member)
        {
            member.EnumDefinition = this.EnumDefinition;
            base.Add(member);
        }

        public void Add(EnumMember member, bool distinctly)
        {
            if (distinctly && this.Any(p => p.Name == member.Name)) return;
            this.Add(member);
        }
    }
}
