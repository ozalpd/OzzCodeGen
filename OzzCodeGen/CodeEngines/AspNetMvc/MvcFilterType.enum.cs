using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public enum MvcFilterType
    {
        [Description("All Properties")]
        AllProperties,
        [Description("Index View Properties")]
        IndexViewProperties,
        [Description("Details View Properties")]
        DetailsViewProperties,
        [Description("Create View Properties")]
        CreateViewProperties,
        [Description("Edit View Properties")]
        EditViewProperties
    }
}
