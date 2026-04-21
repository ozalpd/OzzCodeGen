using System.Linq;
using System.Windows.Controls;

namespace OzzCodeGen.Utilities
{
   public class DataGridTools
    {
       public static void ReorderDataGridColumn(DataGrid dataGrid, string columnHeader, int index)
       {
           DataGridColumn c = dataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == columnHeader);
           if (c != null)
           {
               dataGrid.Columns.Remove(c);
               dataGrid.Columns.Insert(index, c);
           }
       }
    }
}
