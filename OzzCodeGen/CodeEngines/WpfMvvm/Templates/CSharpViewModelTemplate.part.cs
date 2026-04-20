namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates
{
    public partial class CSharpWpfViewModelTemplate
    {
        public CSharpWpfViewModelTemplate(WpfMvvmEntitySetting entitySetting, bool isEdit) : base(entitySetting.CodeEngine as WpfMvvmCodeEngine, entitySetting)
        {
            IsEdit = isEdit;
        }

        public bool IsEdit { get; }

        public override string GetDefaultFileName()
        {
            return $"{EntitySetting.Name}{(IsEdit ? "Edit" : "Create")}ViewModel.cs";
        }
    }
}
