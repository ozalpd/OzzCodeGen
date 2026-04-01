using System;
using System.Collections.Generic;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteMetadataRepositoryTemplate
    {
        public CSharpSqliteMetadataRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine) : base(codeEngine) { }

        public override string GetDefaultFileName()
        {
            return "MetadataRepository.cs";
        }
    }
}
