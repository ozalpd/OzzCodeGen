using System;
using System.Collections.Generic;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteRepositoryTemplate
    {
        public CSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting) : base(codeEngine, entitySetting) { }


        public override string GetDefaultFileName()
        {
            return $"{EntitySetting.Name}Repository.cs";
        }
    }
}
