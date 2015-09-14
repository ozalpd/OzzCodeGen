using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public enum MvcViewType
    {
        [Description("All Views")]
        AllViews,
        [Description("Index View")]
        IndexView,
        [Description("Details View")]
        DetailsView,
        [Description("Create View")]
        CreateView,
        [Description("Edit View")]
        EditView
    }
}
