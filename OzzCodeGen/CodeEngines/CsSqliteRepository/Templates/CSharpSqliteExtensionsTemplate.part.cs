namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteExtensionsTemplate
    {
        public CSharpSqliteExtensionsTemplate(CSharpSqliteRepositoryEngine codeEngine) : base(codeEngine) { }

        public override string GetDefaultFileName()
        {
            return $"Extensions\\{CodeEngine.SqliteExtensionsClassName}.cs";
        }
    }
}
