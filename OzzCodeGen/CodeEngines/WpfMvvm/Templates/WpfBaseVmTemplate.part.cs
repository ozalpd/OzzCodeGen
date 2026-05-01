using OzzCodeGen.CodeEngines.Mvvm;
using System.Collections.Generic;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    /// <summary>
    /// Template for generating base ViewModel classes in a WPF MVVM application. This template can generate different types of base ViewModel classes based on the specified BaseViewModelTypes enum value.
    /// The generated classes include AbstractViewModel, AbstractDataErrorInfoVM, AbstractCollectionVM, and AbstractCreateEditVM, each with its own set of implemented interfaces and functionality.
    /// The template also determines the necessary using namespaces based on the type of base ViewModel being generated and whether a validator is needed.
    /// Partial class of WpfBaseVmTemplate.tt's code-behind file.
    /// </summary>
    public partial class WpfBaseVmTemplate
    {
        public WpfBaseVmTemplate(WpfMvvmCodeEngine codeEngine, BaseVM baseType) : base(codeEngine)
        {
            BaseType = baseType;
        }

        public BaseVM BaseType { get; }

        public bool GenerateValidator => CodeEngine.ModelClassCodeEngine != null
                                      && CodeEngine.ModelClassCodeEngine.GenerateValidator;

        public string GetClassDeclaration()
        {
            switch (BaseType)
            {
                case BaseVM.BaseViewModel:
                    return "public abstract class AbstractViewModel : INotifyPropertyChanged";

                case BaseVM.DataErrorInfoVM:
                    return "public abstract class AbstractDataErrorInfoVM : AbstractViewModel, INotifyDataErrorInfo";

                case BaseVM.CollectionVM:
                    return "public abstract class AbstractCollectionVM<T> : AbstractViewModel where T : class";

                case BaseVM.CreateEditVM:
                    return "public abstract class AbstractCreateEditVM : AbstractDataErrorInfoVM, IIsDirty";

                default:
                    return string.Empty;
            }
        }

        public string GetClassName()
        {
            switch (BaseType)
            {
                case BaseVM.BaseViewModel:
                    return "AbstractViewModel";

                case BaseVM.DataErrorInfoVM:
                    return "AbstractDataErrorInfoVM";

                case BaseVM.CollectionVM:
                    return "AbstractCollectionVM";

                case BaseVM.CreateEditVM:
                    return "AbstractCreateEditVM";

                default:
                    return string.Empty;
            }
        }

        public override string GetDefaultFileName() => $"{GetClassName()}.cs";

        public string GetNamespace()
        {
            if (string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
            {
                return CodeEngine.ViewModelNamespaceName;
            }
            return $"{CodeEngine.InfrastructureNamespaceName}.{GetFolderToNamespace(CodeEngine.ViewModelFolder)}";
        }

        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();

            switch (BaseType)
            {
                case BaseVM.BaseViewModel:
                    namespaces.Add("System.ComponentModel");
                    break;
                case BaseVM.DataErrorInfoVM:
                    if (GenerateValidator)
                    {
                        namespaces.Add(CodeEngine.ModelClassCodeEngine.ValidatorNamespaceName);
                    }
                    namespaces.Add("System.ComponentModel");
                    namespaces.Add("System.Collections");
                    break;
                case BaseVM.CollectionVM:
                    namespaces.Add("System.Collections.ObjectModel");
                    break;
                case BaseVM.CreateEditVM:
                    break;
                default:
                    break;
            }

            return namespaces;
        }
    }
}
