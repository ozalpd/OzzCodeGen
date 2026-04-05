using OzzUtils;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.CsSqliteRepository.Templates
{
    public partial class CSharpSqliteRepositoryTemplate
    {
        public CSharpSqliteRepositoryTemplate(CSharpSqliteRepositoryEngine codeEngine, SqliteRepositoryEntitySetting entitySetting) : base(codeEngine, entitySetting) { }


        public override string GetDefaultFileName()
        {
            return $"{EntitySetting.Name}Repository.cs";
        }

        protected WriteColumnsModel GetWriteColumnsModel(SqliteRepositoryPropertySetting column, bool forInsert = false)
        {
            var pkey = GetPrimaryKey();
            var createdAtCol = GetCreatedAtColumn();
            var updatedAtCol = GetUpdatedAtColumn();

            var writeModel = new WriteColumnsModel
            {
                PKey = pkey,
                PKeyValue = pkey.Name.ToCamelCase(),
                CreatedAtCol = createdAtCol,
                UpdatedAtCol = updatedAtCol
            };
            writeModel.Columns.Add(column);
            writeModel.ValueList.Add(column.Name.ToCamelCase());

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
    }
}