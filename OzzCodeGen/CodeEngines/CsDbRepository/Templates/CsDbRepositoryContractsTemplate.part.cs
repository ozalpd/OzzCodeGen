using OzzCodeGen.CodeEngines.CsModelClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.CsDbRepository.Templates
{
    public partial class CsDbRepositoryContractsTemplate : BaseCsDbRepositoryTemplate
    {

        public CsDbRepositoryContractsTemplate(BaseAppInfraCodeEngine codeEngine, BaseEntitySetting entitySetting, List<string> signatureList) : base(codeEngine, entitySetting)
        {
            _signatureList = signatureList;
        }

        protected override List<string> SignatureList => _signatureList;
        private readonly List<string> _signatureList;

        public override string GetDefaultFileName()
        {
            return $"I{EntitySetting.Name}Repository.cs";
        }

        protected override IEnumerable<BaseCsDbRepositoryPropertySetting> GetAutoLoadProperties()
        {
            throw new NotImplementedException();
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            var project = CodeEngine.Project;
            var modelClassEngine = project.GetCodeEngine(EngineTypes.CsModelClassCodeEngineId) as CSharpModelClassCodeEngine;

            if (modelClassEngine != null)
            {
                namespaces.Add(modelClassEngine.NamespaceName);
                namespaces.Add(modelClassEngine.QueryParamNamespaceName);
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }

        protected CSharpModelClassCodeEngine GetModelClassEngine()
        {
            var project = CodeEngine.Project;
            return project.GetCodeEngine(EngineTypes.CsModelClassCodeEngineId) as CSharpModelClassCodeEngine;
        }

        protected override string GetEntityTypeName(bool isNullable)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BaseCsDbRepositoryPropertySetting> GetForeignKeyProperties()
        {
            throw new NotImplementedException();
        }

        protected override ModelClassEntitySetting GetModelClassEntitySetting()
        {
            var modelClassEngine = GetModelClassEngine();
            if (modelClassEngine != null)
            {
                return modelClassEngine.EntitySettings
                                       .OfType<ModelClassEntitySetting>()
                                       .FirstOrDefault(e => e.Name == EntitySetting.Name);
            }

            return null;
        }

        protected override BaseCsDbRepositoryPropertySetting GetPrimaryKey()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ModelPropertySetting> GetSearchableNonRangeProperties()
        {
            throw new NotImplementedException();
        }

        protected override BaseCsDbRepositoryPropertySetting GetUniqueIndexed()
        {
            throw new NotImplementedException();
        }

        protected override bool HasIsActiveProperty()
        {
            throw new NotImplementedException();
        }
    }
}
