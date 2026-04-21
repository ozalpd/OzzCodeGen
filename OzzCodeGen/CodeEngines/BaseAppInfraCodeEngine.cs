namespace OzzCodeGen.CodeEngines
{
    public abstract class BaseAppInfraCodeEngine : BaseCodeEngine
    {
        /// <summary>
        /// Namespace for generated shared MVVM infrastructure types (for example <c>ViewModelBase</c>, <c>RelayCommand</c>, <c>AsyncRelayCommand</c>).
        /// </summary>
        public string BaseClassNamespaceName
        {
            get { return _baseNamespaceName ?? $"{NamespaceName}.Infra"; }
            set
            {
                if (_baseNamespaceName == value) return;
                _baseNamespaceName = value;
                RaisePropertyChanged(nameof(BaseClassNamespaceName));
            }
        }
        private string _baseNamespaceName;

        /// <summary>
        /// Namespace for generated MVVM contract interfaces (for example <c>IViewModel</c>, <c>IAsyncCommand</c>, <c>INavigationService</c>).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>{MvvmNamespaceName}.Contracts</c>.
        /// </remarks>
        public string ContractsNamespaceName
        {
            get { return _contractsNamespaceName ?? $"{NamespaceName}.Infra"; }
            set
            {
                if (_contractsNamespaceName == value) return;
                _contractsNamespaceName = value;
                RaisePropertyChanged(nameof(ContractsNamespaceName));
            }
        }
        private string _contractsNamespaceName;

        /// <summary>
        /// Relative folder (under <see cref="BaseCodeEngine.TargetDirectory"/>) where shared MVVM infrastructure files are generated.
        /// </summary>
        /// <remarks>
        /// This folder is intentionally platform-agnostic so the generated base/contracts can be reused by future engines (for example, MAUI)
        /// with minimal duplication. If set to empty or whitespace, generated base/contracts will be placed in the related commands, views or
        /// view models folder.
        /// </remarks>
        public string InfrastructureFolder
        {
            get { return _infrastructureFolder ?? "Infrastructure"; }
            set
            {
                if (_infrastructureFolder == value) return;
                _infrastructureFolder = value;
                RaisePropertyChanged(nameof(InfrastructureFolder));
                RaisePropertyChanged(nameof(TargetInfrastructureDirectory));
            }
        }
        private string _infrastructureFolder;

        public abstract string TargetInfrastructureDirectory { get; }
    }
}
