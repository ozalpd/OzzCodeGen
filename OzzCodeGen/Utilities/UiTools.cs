using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OzzCodeGen.Utilities
{
   public class UiTools
    {
       public static MenuItem CreateMenuItem(string header, string toolTip)
       {
           return new MenuItem()
           {
               Header = header,
               ToolTip = toolTip
           };
       }
    }
}
