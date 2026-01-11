using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzUtils
{
    public static class Numeric
    {
        public static string ToFileSize(this int i)
        {
            long l = Convert.ToInt64(i);
            return ToFileSize(l);
        }

        public static string ToFileSize(this long l)
        {
            double d = l;
            int gig = (1024 * 1024 * 1024);
            if (l > gig)
            {
                return string.Format("{0:#.#0} GB", d / gig);
            }
            else if (l > (1024 * 1024))
            {
                return string.Format("{0:#.#0} MB", d / (1024 * 1024));
            }
            else if (l > 1024)
            {
                return string.Format("{0:#.#0} KB", d / 1024);
            }
            else
            {
                return string.Format("{0} Bytes", l);
            }
        }
    }
}
