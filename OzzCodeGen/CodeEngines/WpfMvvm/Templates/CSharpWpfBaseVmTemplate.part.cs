using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpWpfBaseVmTemplate
    {
        public CSharpWpfBaseVmTemplate(WpfMvvmCodeEngine codeEngine, BaseViewModelTypes baseType) : base(codeEngine)
        {
            BaseType = baseType;
        }

        public BaseViewModelTypes BaseType { get; }

        public bool GenerateValidator => CodeEngine.ModelClassCodeEngine != null
                                      && CodeEngine.ModelClassCodeEngine.GenerateValidator;

        public string GetClassDeclaration()
        {
            switch (BaseType)
            {
                case BaseViewModelTypes.BaseViewModel:
                    return "public abstract class AbstractViewModel : INotifyPropertyChanged";

                case BaseViewModelTypes.DataErrorInfoVM:
                    return "public abstract class AbstractDataErrorInfoVM : AbstractViewModel, INotifyDataErrorInfo";

                case BaseViewModelTypes.CollectionVM:
                    return "public abstract class AbstractCollectionVM<T> : AbstractViewModel where T : class";

                case BaseViewModelTypes.CreateEditVM:
                    return "public abstract class AbstractCreateEditVM : AbstractDataErrorInfoVM, IIsDirty";

                default:
                    return string.Empty;
            }
        }

        public string GetClassName()
        {
            switch (BaseType)
            {
                case BaseViewModelTypes.BaseViewModel:
                    return "AbstractViewModel";

                case BaseViewModelTypes.DataErrorInfoVM:
                    return "AbstractDataErrorInfoVM";

                case BaseViewModelTypes.CollectionVM:
                    return "AbstractCollectionVM";

                case BaseViewModelTypes.CreateEditVM:
                    return "AbstractCreateEditVM";

                default:
                    return string.Empty;
            }
        }

        public override string GetDefaultFileName() => $"{GetClassName()}.cs";

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();

            switch (BaseType)
            {
                case BaseViewModelTypes.BaseViewModel:
                    namespaces.Add("System.ComponentModel");
                    break;
                case BaseViewModelTypes.DataErrorInfoVM:
                    if (GenerateValidator)
                    {
                        namespaces.Add(CodeEngine.ModelClassCodeEngine.ValidatorNamespaceName);
                    }
                    namespaces.Add("System.ComponentModel");
                    namespaces.Add("System.Collections");
                    break;
                case BaseViewModelTypes.CollectionVM:
                    namespaces.Add("System.Collections.ObjectModel");
                    break;
                case BaseViewModelTypes.CreateEditVM:
                    break;
                default:
                    break;
            }

            return namespaces;
        }
    }

    public enum BaseViewModelTypes
    {
        BaseViewModel,
        DataErrorInfoVM,
        CollectionVM,
        CreateEditVM
    }
}
