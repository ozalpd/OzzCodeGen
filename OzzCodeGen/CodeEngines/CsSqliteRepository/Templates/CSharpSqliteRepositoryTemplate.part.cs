using OzzCodeGen.CodeEngines.CsDbRepository.Templates;
using OzzUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteRepositoryTemplate
    {
        public CSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting) : base(codeEngine, entitySetting) { }


        public override string GetDefaultFileName()
        {
            return $"{EntitySetting.Name}Repository.cs";
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>()
            {
                "Microsoft.Data.Sqlite",
                $"{CodeEngine.NamespaceName}.Extensions"
            };
            var modelClassEngine = CodeEngine.ModelClassCodeEngine;
            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
                if (modelClassEngine.GenerateValidator)
                    namespaces.Add(modelClassEngine.ValidatorNamespaceName);
            }

            if (CodeEngine.HasDifferentNamespaceForContracts)
            {
                namespaces.Add(CodeEngine.InfrastructureNamespaceName);
            }

            if (EntitySetting.GenerateGetPaged && !string.IsNullOrWhiteSpace(CodeEngine.QueryParamNamespaceName))
            {
                namespaces.Add(CodeEngine.QueryParamNamespaceName);
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }

        protected WriteColumnsModel GetWriteColumnsModel(SqliteRepositoryPropertySetting column, bool forInsert = false)
        {
            return GetWriteColumnsModel(new[] { column }, forInsert);
        }

        protected WriteColumnsModel GetWriteColumnsModel(IEnumerable<SqliteRepositoryPropertySetting> columns, bool forInsert = false)
        {
            var pkey = GetPrimaryKey();
            var createdAtCol = GetCreatedAtColumn();
            var updatedAtCol = GetUpdatedAtColumn();

            var writeModel = new WriteColumnsModel
            {
                PKey = pkey as SqliteRepositoryPropertySetting,
                PKeyValue = pkey.Name.ToCamelCase(),
                CreatedAtCol = createdAtCol,
                UpdatedAtCol = updatedAtCol
            };

            foreach (var column in columns)
            {
                writeModel.Columns.Add(column);
                writeModel.ValueList.Add(column.Name.ToCamelCase());
            }

            if (updatedAtCol != null)
            {
                writeModel.Columns.Add(updatedAtCol);
                writeModel.ValueList.Add(updatedAtCol?.Name.ToCamelCase());
            }

            if (forInsert && createdAtCol != null)
            {
                writeModel.Columns.Add(createdAtCol);
                writeModel.ValueList.Add(createdAtCol?.Name.ToCamelCase());
            }

            return writeModel;
        }

        public override bool WriteToFile(string FilePath, bool overwriteExisting)
        {
            bool allWritten = base.WriteToFile(FilePath, overwriteExisting);
            if (CodeEngine.HasDifferentFolderForContracts)
            {
                var contractsTemplate = new CsDbRepositoryContractsTemplate(CodeEngine, EntitySetting, SignatureList);
                FilePath = Path.Combine(CodeEngine.TargetInfrastructureDirectory, contractsTemplate.GetDefaultFileName());
                allWritten &= contractsTemplate.WriteToFile(FilePath, overwriteExisting);
            }

            return allWritten;
        }
    }
}