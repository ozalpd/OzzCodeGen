using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public enum IndexViewGeneration
    {
        [Description("HTML Table with Action Menu")]
        HtmlTableWithMenuAction = 101,
        [Description("HTML Table with Action Buttons")]
        HtmlTableWithButtonsAction = 102,
        [Description("agGrid with Action Buttons")]
        AgGridWithMenuAction = 201,
        [Description("agGrid with Action Buttons")]
        AgGridWithButtonsAction = 202
    }
}
