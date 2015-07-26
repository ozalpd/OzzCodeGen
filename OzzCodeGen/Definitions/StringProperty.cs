using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Definitions
{
    public class StringProperty : SimpleProperty
    {

        public int MaxLenght
        {
            get { return maxLenght; }
            set
            {
                if (maxLenght == value) return;
                maxLenght = value;
                RaisePropertyChanged("MaxLenght");
            }
        }
        private int maxLenght;

        public override bool IsTypeNullable() { return true; }
        public override bool IsTypeNumeric() { return false; }
        public override bool IsTypeDateTime() { return false; }
        public override bool IsTypeBoolean() { return false; }
        public override bool IsTypeGuid() { return false; }
        public override bool IsTypeString() { return true; }

        public override DefinitionType DefinitionType
        {
            get { return DefinitionType.String; }
        }

        public override BaseProperty Clone()
        {
            var clone = (StringProperty)base.Clone();
            clone.MaxLenght = this.MaxLenght;

            return clone;
        }

        public override List<string> GetUsableTypeNames()
        {
            var typeNames = new List<string>();
            typeNames.Add("string");

            return typeNames;
        }
    }
}
