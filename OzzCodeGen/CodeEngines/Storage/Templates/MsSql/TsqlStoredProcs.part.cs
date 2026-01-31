namespace OzzCodeGen.CodeEngines.Storage.Templates.MsSql
{
    public partial class TsqlStoredProcs : AbstractStorageTemplate
    {
        public TsqlStoredProcs(StorageEntitySetting tableDefinition)
            : base(tableDefinition)
        {

        }

        public override string GetDefaultFileName()
        {
            return TableDefinition.TableName + "-SP.sql";
        }
    }
}
