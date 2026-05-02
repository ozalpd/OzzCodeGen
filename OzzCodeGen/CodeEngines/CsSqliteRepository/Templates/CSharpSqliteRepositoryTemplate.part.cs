using OzzCodeGen.CodeEngines.CsDbRepository.Templates;
using OzzUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                //if (modelClassEngine.GenerateValidator)
                //    namespaces.Add(modelClassEngine.ValidatorNamespaceName);
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

        public string GetConstructorParameters()
        {
            var sb = new StringBuilder();
            sb.Append("string databasePath");
            var autoloadCols = GetAutoLoadProperties().ToList();
            if (autoloadCols.Count == 0)
                return sb.ToString();

            string className = EntitySetting.GetRepositoryName();
            var autoloadTypeNames = autoloadCols.Where(p => p.GetTypeName(getReturnType: true) != EntitySetting.Name)
                                                .Select(p => p.GetTypeName(getReturnType: true))
                                                .Distinct()
                                                .ToList();
            int i = 0;
            foreach (var typeName in autoloadTypeNames)
            {
                if (i % 2 == 1)
                {
                    sb.Append(",\r\n");
                    sb.Append(' ', className.Length + 16);
                }
                else
                {
                    sb.Append(", ");
                }

                sb.Append('I');
                sb.Append(GetRepositoryName(typeName));
                sb.Append("? ");

                sb.Append(GetRepositoryName(typeName).ToCamelCase());
                sb.Append(" = null");
                i++;
            }

            return sb.ToString();
        }

        public string GetOnInitializedParameters(bool forMethodCall = false)
        {
            var autoloadCols = GetAutoLoadProperties().ToList();
            if (autoloadCols.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            var autoloadTypeNames = autoloadCols.Where(p => p.GetTypeName(getReturnType: true) != EntitySetting.Name)
                                                .Select(p => p.GetTypeName(getReturnType: true))
                                                .Distinct()
                                                .ToList();
            int i = 0;
            int spacesToAdd = forMethodCall ? 26 : 35;
            foreach (var typeName in autoloadTypeNames)
            {
                if (i % 3 == 0 && i > 0)
                {
                    sb.Append(",\r\n");
                    sb.Append(' ', spacesToAdd);
                }
                else if (i > 0)
                {
                    sb.Append(", ");
                }
                if (forMethodCall)
                {
                    sb.Append(GetRepositoryName(typeName).ToCamelCase());
                    sb.Append(" == null");
                }
                else
                {
                    sb.Append("bool is");
                    sb.Append(GetRepositoryName(typeName));
                }
                i++;
            }
            return sb.ToString();
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