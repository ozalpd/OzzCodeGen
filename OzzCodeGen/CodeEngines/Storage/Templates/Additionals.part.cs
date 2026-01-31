namespace OzzCodeGen.CodeEngines.Storage.Templates
{
    public partial class Additionals : AbstractStorageTemplate
    {
        public Additionals(StorageCodeEngine engine) : base(engine) { }

        public override string GetDefaultFileName()
        {
            return "_FinishingSetup.sql";
        }
    }
}
