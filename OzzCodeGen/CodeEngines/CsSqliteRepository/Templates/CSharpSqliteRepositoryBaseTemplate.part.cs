using System;
using System.Collections.Generic;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteRepositoryBaseTemplate
    {
        public CSharpSqliteRepositoryBaseTemplate(CSharpSqliteRepositoryEngine codeEngine) : base(codeEngine) { }

        public override string GetDefaultFileName()
        {
            return $"{CodeEngine.BaseRepositoryClassName}.cs";
        }
    }
}
