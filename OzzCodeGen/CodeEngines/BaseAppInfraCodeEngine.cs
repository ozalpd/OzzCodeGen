using System.IO;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines
{
    public abstract class BaseAppInfraCodeEngine : BaseCodeEngine
    {
        /// <summary>
        /// Namespace for generated shared MVVM infrastructure types (for example <c>ViewModelBase</c>, <c>RelayCommand</c>, <c>AsyncRelayCommand</c>).
        /// </summary>
        public string BaseClassNamespaceName
        {
            get { return _baseNamespaceName ?? $"{Project.NamespaceName}.Infra"; }
            set
            {
                if (_baseNamespaceName == value) return;
                _baseNamespaceName = value;
                RaisePropertyChanged(nameof(BaseClassNamespaceName));
            }
        }
        private string _baseNamespaceName;


        [XmlIgnore]
        [JsonIgnore]
        public bool HasDifferentNamespaceForContracts => HasDifferentFolderForContracts && !string.IsNullOrWhiteSpace(InfrastructureNamespaceName)
                                                      && !string.Equals(BaseClassNamespaceName, InfrastructureNamespaceName, System.StringComparison.InvariantCulture);
        [XmlIgnore]
        [JsonIgnore]
        public bool HasDifferentFolderForContracts => !string.IsNullOrWhiteSpace(InfrastructureFolder)
                                                   && !string.Equals(TargetFolder, InfrastructureFolder, System.StringComparison.InvariantCulture);

        /// <summary>
        /// Namespace for generated base classes or contract interfaces (for example <c>IViewModel</c>, <c>IAsyncCommand</c>, <c>INavigationService</c>).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>{MvvmNamespaceName}.Contracts</c>.
        /// </remarks>
        public string InfrastructureNamespaceName
        {
            get { return _infrastructureNamespaceName ?? $"{Project.NamespaceName}.Infra"; }
            set
            {
                if (_infrastructureNamespaceName == value) return;
                _infrastructureNamespaceName = value;
                RaisePropertyChanged(nameof(InfrastructureNamespaceName));
            }
        }
        private string _infrastructureNamespaceName;

        /// <summary>
        /// Relative folder (under <see cref="BaseCodeEngine.TargetDirectory"/>) where shared infrastructure files are generated.
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

        [XmlIgnore]
        [JsonIgnore]
        public virtual string TargetInfrastructureDirectory
        {
            get
            {
                if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir) && string.IsNullOrEmpty(InfrastructureFolder))
                {
                    return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, TargetFolder));
                }
                else if (Project != null && !string.IsNullOrEmpty(Project.TargetSolutionDir))
                {
                    return Path.GetFullPath(Path.Combine(Project.TargetSolutionDir, InfrastructureFolder));
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
