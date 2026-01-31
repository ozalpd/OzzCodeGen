namespace OzzCodeGen.CodeEngines.Storage.Templates.MsSql
{
    public partial class CreateMsSqlDb : AbstractStorageTemplate
    {
        public CreateMsSqlDb(StorageCodeEngine engine) : base(engine) { }

        public override string GetDefaultFileName()
        {
            return string.Format("_Create_{0}.bat", CodeEngine.DatabaseName);
        }
    }
}
