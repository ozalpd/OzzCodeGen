namespace OzzCodeGen.Definitions
{
    public abstract class BaseClassProperty : BaseProperty
    {
        public string DependentPropertyName { get; set; }
        public string DependentPropertyType { get; set; }

        public override bool IsTypeNullable() { return true; }
        public override bool IsTypeNumeric() { return false; }
        public override bool IsTypeDateTime() { return false; }
        public override bool IsTypeBoolean() { return false; }
        public override bool IsTypeString() { return false; }
        public override bool IsTypeGuid() { return false; }
    }
}
