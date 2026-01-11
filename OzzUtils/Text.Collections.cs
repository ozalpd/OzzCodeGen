using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzUtils
{
    public static partial class Text
    {
        public static ICollection<string> AddUnique(this ICollection<string> collection, string item)
        {
            if (!string.IsNullOrEmpty(item) && !collection.Contains(item))
                collection.Add(item);
            return collection;
        }

        public static ICollection<string> AddUnique(this ICollection<string> collection, params string[] items)
        {
            foreach (var item in items)
            {
                collection.AddUnique(item);
            }

            return collection;
        }
    }
}
