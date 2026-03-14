using System.Collections.Generic;

namespace OzzCodeGen.Definitions
{
    public class StringProperty : SimpleProperty
    {

        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                if (maxLength == value) return;
                maxLength = value;
                RaisePropertyChanged("MaxLength");
            }
        }
        private int maxLength;
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
            clone.MaxLength = this.MaxLength;

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
