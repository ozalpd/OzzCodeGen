namespace OzzCodeGen.CodeEngines.Storage.Templates.MsSql
{
    public partial class DropMsSqlDb : AbstractStorageTemplate
    {
        public DropMsSqlDb(StorageCodeEngine engine) : base(engine) { }

        public override string GetDefaultFileName()
        {
            return string.Format("_Drop_Create_{0}.bat", CodeEngine.DatabaseName);
        }
    }
}
