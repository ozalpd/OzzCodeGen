using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.CsSqliteRepository;
using OzzCodeGen.Templates.Cs;
using OzzUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.CsDbRepository.Templates
{
    public abstract partial class BaseCsDbRepositoryTemplate : CsClassBase
    {
        protected BaseCsDbRepositoryTemplate(BaseAppInfraCodeEngine codeEngine, BaseEntitySetting entitySetting = null)
        {
            CodeEngine = codeEngine;
            EntitySetting = entitySetting;
        }

        public virtual BaseAppInfraCodeEngine CodeEngine { get; }
        public virtual BaseEntitySetting EntitySetting { get; }

        protected void AddSignature(string signature)
        {
            if (!SignatureList.Contains(signature))
                SignatureList.Add(signature);
        }

        protected virtual List<string> SignatureList { get; } = new List<string>();


        protected abstract IEnumerable<BaseCsDbRepositoryPropertySetting> GetAutoLoadProperties();

        protected abstract string GetEntityTypeName(bool isNullable);

        protected abstract IEnumerable<BaseCsDbRepositoryPropertySetting> GetForeignKeyProperties();

        protected abstract ModelClassEntitySetting GetModelClassEntitySetting();

        protected abstract BaseCsDbRepositoryPropertySetting GetPrimaryKey();

        protected abstract IEnumerable<ModelPropertySetting> GetSearchableNonRangeProperties();

        protected abstract BaseCsDbRepositoryPropertySetting GetUniqueIndexed();

        protected abstract bool HasIsActiveProperty();


        protected string GetSignature(MethodType methodType, SqliteRepositoryPropertySetting column = null)
        {
            var pkey = GetPrimaryKey();
            var unique = GetUniqueIndexed();
            ModelClassEntitySetting modelEntity = GetModelClassEntitySetting();
            CSharpModelClassCodeEngine modelClassEngine = modelEntity?.CodeEngine as CSharpModelClassCodeEngine;
            string queryParamClassName = string.Empty;
            if (modelEntity != null && modelClassEngine != null)
                queryParamClassName = modelEntity.GenerateQueryParam
                                    ? $"{EntitySetting.Name}{modelClassEngine.QueryParamClassName}"
                                    : modelClassEngine.QueryParamClassName;
            bool hasIsActive = HasIsActiveProperty();
            bool needIsActive = hasIsActive
                             && !GetSearchableNonRangeProperties()
                                             .Any(p => p.Name.Equals("IsActive", StringComparison.InvariantCultureIgnoreCase));
            switch (methodType)
            {
                case MethodType.Create:
                    return $"Task<{pkey.GetTypeName()}> CreateAsync({EntitySetting.Name} {EntitySetting.Name.ToCamelCase()})";

                case MethodType.DeleteByPKey:
                    return $"Task<bool> DeleteAsync({pkey.GetTypeName()} {pkey.Name.ToCamelCase()})";

                case MethodType.DeleteByUniqueIndex:
                    if (unique != null)
                        return $"Task<bool> DeleteAsync({unique.GetTypeName()} {unique.Name.ToCamelCase()})";
                    break;

                case MethodType.GetAll:
                    return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetAllAsync({(HasIsActiveProperty() ? "bool? isActive = null" : "")})";

                case MethodType.GetByPKey:
                    return $"Task<{GetEntityTypeName(isNullable: true)}> GetBy{pkey.Name}Async({pkey.GetTypeName()}? {pkey.Name.ToCamelCase()})";

                case MethodType.GetByUnique:
                    if (unique != null)
                        return $"Task<{GetEntityTypeName(isNullable: true)}> GetBy{unique.Name}Async({unique.GetTypeName()}? {unique.Name.ToCamelCase()})";
                    break;

                case MethodType.GetByForeignKey:
                    if (column != null)
                        return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetBy{column.Name}Async({column.GetTypeName()} {column.Name.ToCamelCase()}{(hasIsActive ? ", bool? isActive = null" : "")})";
                    break;

                case MethodType.GetPaged:
                    if (string.IsNullOrWhiteSpace(queryParamClassName))
                        return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetPagedAsync(int pageNumber, int pageSize, string sortBy = null, bool ascending = true{(hasIsActive ? ", bool? isActive = null" : "")})";
                    else
                        return $"Task<IReadOnlyList<{EntitySetting.Name}>> GetPagedAsync({queryParamClassName} queryParameters{(needIsActive ? ", bool? isActive = null" : "")})";

                case MethodType.UpdateEntity:
                    return $"Task<bool> UpdateAsync({EntitySetting.Name} {EntitySetting.Name.ToCamelCase()})";

                case MethodType.UpdateSingleColumnById:
                    if (column != null)
                        return $"Task<bool> Update{column.Name}Async({pkey.GetTypeName()} {pkey.Name.ToCamelCase()}, {column.GetTypeName()} {column.Name.ToCamelCase()})";
                    break;

                default:
                    break;
            }

            return string.Empty;
        }
    }

    public enum MethodType
    {
        Create,
        DeleteByPKey,
        DeleteByUniqueIndex,
        GetAll,
        GetPaged,
        GetByPKey,
        GetByUnique,
        GetByForeignKey,
        UpdateEntity,
        UpdateSingleColumnById,
    }
}
