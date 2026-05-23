using OzzCodeGen.CodeEngines.CsModelClass;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpQueryParamsVmTemplate
    {
        public CSharpQueryParamsVmTemplate(WpfMvvmCodeEngine codeEngine, WpfMvvmEntitySetting entitySetting = null) : base(codeEngine, entitySetting)
        {

        }


        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();
            if (CodeEngine.ModelClassCodeEngine != null)
            {
                namespaces.Add(CodeEngine.ModelClassCodeEngine.QueryParamNamespaceName);
                if (GenerateQueryParam)
                    namespaces.Add(CodeEngine.ModelClassCodeEngine.NamespaceName);
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }

        public bool GenerateQueryParam => EntitySetting?.GenerateQueryParam == true;

        public string GetBaseClassName()
        {
            if (GenerateQueryParam)
                return $"{CSharpModelClassCodeEngine.QueryParamClassName}VM";

            return CodeEngine.BaseViewModelName;
        }

        public string GetClassName()
        {
            return $"{GetQueryParamClassName()}VM";
        }

        public override string GetDefaultFileName()
        {
            if (GenerateQueryParam
                && !string.IsNullOrWhiteSpace(SubFolder))
                return $"{SubFolder}\\{GetClassName()}.cs";

            return $"{GetClassName()}.cs";
        }



        public string GetNamespace()
        {
            string baseNamespace = string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder)
                                 ? CodeEngine.ViewModelNamespaceName
                                 : $"{CodeEngine.InfrastructureNamespaceName}.{CodeEngine.ViewModelFolder.FolderPathToNamespace()}";
            if (!string.IsNullOrWhiteSpace(SubFolder))
            {
                return $"{baseNamespace}.{SubFolder.FolderPathToNamespace()}";
            }

            return baseNamespace;
        }

        public string GetQueryParamClassName()
        {
            if (EntitySetting != null)
                return EntitySetting.GetQueryParamClassName();

            return CSharpModelClassCodeEngine.QueryParamClassName;
        }

        public string SubFolder => EntitySetting?.SubFolder ?? string.Empty;
    }
}
