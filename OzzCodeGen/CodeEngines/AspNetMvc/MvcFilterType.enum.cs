using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public enum MvcFilterType
    {
        [Description("All Properties")]
        AllProperties = -1,
        [Description("Index View Properties")]
        IndexViewProperties = 0,
        [Description("Details View Properties")]
        DetailsViewProperties = 1,
        [Description("Create View Properties")]
        CreateViewProperties = 2,
        [Description("Edit View Properties")]
        EditViewProperties = 3,
        [Description("Search Properties")]
        SearchProperties = 4
    }
}
