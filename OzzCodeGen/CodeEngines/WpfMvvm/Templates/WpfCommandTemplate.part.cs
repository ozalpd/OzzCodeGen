using OzzCodeGen.CodeEngines.Mvvm;
using OzzUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class WpfCommandTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCommandTemplate"/> class for collection-based command generation.
        /// </summary>
        /// <param name="entitySetting">The WPF MVVM entity setting.</param>
        /// <param name="pageCommand">The page command type.</param>
        public WpfCommandTemplate(WpfMvvmEntitySetting entitySetting, PageCommand pageCommand) : this(entitySetting, MvvmTemplate.Collection)
        {
            PageCommand = pageCommand;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCommandTemplate"/> class for generating WPF command
        /// implementations.
        /// </summary>
        /// <param name="entitySetting">The entity setting containing the model and generation configuration.</param>
        /// <param name="templateType">The MVVM template type to generate.</param>
        public WpfCommandTemplate(WpfMvvmEntitySetting entitySetting, MvvmTemplate templateType)
            : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting, templateType: templateType)
        {
            PageCommand = PageCommand.LoadCommand;
        }

        /// <summary>
        /// Determines load command type if TemplateType is Collection, in other template types it has no effect.
        /// </summary>
        public PageCommand PageCommand { get; }


        public string BaseCommandName => GetBaseCommandName();

        private string GetBaseCommandName()
        {
            // All commands inherit from the same base command
            // except NextPageCommand and PrevPageCommand.
            if (PageCommand == PageCommand.LoadCommand)
                return CodeEngine.BaseCommandName;

            PassViewModelToBase = true;
            return EntitySetting.GetPageCommandName(PageCommand.LoadCommand);
        }

        public bool PassViewModelToBase { get; private set; }

        public string GetClassName() => EntitySetting.GetCommandName(TemplateType, PageCommand);

        public override string GetDefaultFileName()
        {
            if (string.IsNullOrWhiteSpace(EntitySetting.SubFolder))
                return $"{GetClassName()}.cs";

            return $"{EntitySetting.SubFolder}\\{GetClassName()}.cs";
        }

        public string GetNamespace() => EntitySetting.GetCommandsNamespaceName();

        public bool HasDlgService => IsCreateOrEdit || IsDetail;

        public bool IsDisplayOnly => TemplateType == MvvmTemplate.Collection || TemplateType == MvvmTemplate.Detail;

        public string GetConstructorParameters()
        {
            return GetCommandConstructorParams("viewModel", isDeclaration: true, hasDlgService: HasDlgService, isDisplayOnly: IsDisplayOnly);
        }

        public string GetSvcShowParameters()
        {
            var sb = new StringBuilder();
            sb.Append("owner");
            if (IsEdit)
            {
                sb.Append(", _viewModel.Selected");
                sb.Append(EntitySetting.Name);
            }
            if (!IsCreateOrEdit)
                return sb.ToString();

            var foreignLookupEntities = GetForeignLookupEntities();
            foreach (var lookupEntity in foreignLookupEntities)
            {
                sb.Append(", _");
                sb.Append(lookupEntity.GetLookupName(LookupTemplate.RunTimeClass).ToCamelCase());
            }

            if (TemplateType == MvvmTemplate.Create)
            {
                var preselectProperties = GetPreselectProperties();
                foreach (var property in preselectProperties)
                {
                    sb.Append(", Preselected");
                    sb.Append(property.Name);
                }
            }

            return sb.ToString();
        }


        public override List<string> DefaultUsingNamespaceList()
        {
            var namespaces = new List<string>();

            if (!IsDisplayOnly || HasDlgService)
            {
                var resxEngine = CodeEngine.ResxEngine;
                if (resxEngine != null)
                    namespaces.Add(resxEngine.NamespaceName);
                namespaces.Add("System.Windows");
            }

            if (HasDlgService)
            {
                namespaces.Add(CodeEngine.ServicesNamespaceName);
            }

            if (!string.IsNullOrEmpty(EntitySetting.CommandVmNamespace))
            {
                namespaces.Add(EntitySetting.CommandVmNamespace);
            }

            if (!string.IsNullOrWhiteSpace(CodeEngine.InfrastructureFolder))
                namespaces.Add(CodeEngine.BaseCommandNamespaceName);

            if (TemplateType == MvvmTemplate.Create
                && !namespaces.Contains(CodeEngine.LookupNamespaceName)
                && GetForeignLookupEntities().Any())
            {
                namespaces.Add(CodeEngine.LookupNamespaceName);
            }

            if (TemplateType == MvvmTemplate.Create && GetPreselectProperties().Any())
            {
                var modelClassEngine = CodeEngine.ModelClassCodeEngine;
                if (modelClassEngine != null)
                {
                    namespaces.Add(modelClassEngine.NamespaceName);
                }
            }

            return namespaces.OrderBy(ns => ns).ToList();
        }
    }
}
