using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.Definitions
{
    //TODO: Add EnumPropertyType 
    public class SimpleProperty : BaseProperty
    {
        public bool IsKey
        {
            get { return isKey; }
            set
            {
                if (isKey == value) return;
                isKey = value;
                RaisePropertyChanged("IsKey");
            }
        }
        private bool isKey;


        public bool IsForeignKey
        {
            get { return isForeignKey; }
            set
            {
                if (isForeignKey == value) return;
                isForeignKey = value;
                RaisePropertyChanged("IsForeignKey");
            }
        }
        private bool isForeignKey;


        public bool IsNullable
        {
            get { return isNullable; }
            set
            {
                if (isNullable == value) return;
                isNullable = value;
                RaisePropertyChanged("IsNullable");
            }
        }
        private bool isNullable;


        public ComplexProperty GetDependent()
        {
            var dependent = from p in EntityDefinition.Properties.OfType<ComplexProperty>()
                            where p.DependentPropertyName == Name
                            select p;

            return dependent.FirstOrDefault();
        }

        public override bool IsTypeNullable()
        {
            return IsTypeNullable(TypeName);
        }

        public override bool IsTypeNumeric()
        {
            return IsTypeNumeric(TypeName);
        }

        public override bool IsTypeDateTime()
        {
            return IsTypeDateTime(TypeName);
        }

        public override bool IsTypeBoolean()
        {
            return IsTypeBoolean(TypeName);
        }

        public override bool IsTypeString()
        {
            return IsTypeString(TypeName);
        }

        public override bool IsTypeGuid()
        {
            return IsTypeGuid(TypeName);
        }

        public override DefinitionType DefinitionType
        {
            get { return DefinitionType.Simple; }
        }

        public override BaseProperty Clone()
        {
            var clone = (SimpleProperty)base.Clone();
            clone.IsForeignKey = this.IsForeignKey;
            clone.IsKey = this.IsKey;
            clone.IsNullable = this.IsNullable;

            return clone;
        }

        public override List<string> GetUsableTypeNames()
        {
            var typeNames = new List<string>();
            typeNames.Add("int");       //System.Int32
            typeNames.Add("uint");      //System.UInt32
            typeNames.Add("bool");      //System.Boolean
            typeNames.Add("DateTime");
            typeNames.Add("decimal");   //System.Decimal
            typeNames.Add("double");    //System.Double
            typeNames.Add("float");     //System.Single
            typeNames.Add("byte");      //System.Byte
            typeNames.Add("sbyte");     //System.SByte
            typeNames.Add("short");     //System.Int16
            typeNames.Add("ushort");    //System.UInt16
            typeNames.Add("long");      //System.Int64
            typeNames.Add("ulong");     //System.UInt64

            typeNames.Add("Guid");  //System.Guid this and the DateTime are struct types, not a simple type

            return typeNames;
        }
    }
}
