namespace OzzCodeGen.CodeEngines.DataLayer.Templates
{
    public partial class MvcServerVM : MetadataClass
    {
        public override string GetDefaultFileName()
        {
            return EntitySetting.Name + "ViewModel.g.cs";
        }
    }
}
