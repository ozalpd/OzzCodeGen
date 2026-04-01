using System;
using System.Collections.Generic;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteBaseRepositoryTemplate
    {
        public CSharpSqliteBaseRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine) : base(codeEngine) { }

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.BaseRepositoryClassName}.cs";
        }
    }
}
