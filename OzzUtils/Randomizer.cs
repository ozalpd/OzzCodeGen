using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzUtils
{
    public static class Randomizer
    {
        static int seed;

        public static int RandomNr(int minValue, int maxValue)
        {
            if (seed == ((int)DateTime.Now.Ticks & 0x0000FFFF))
            {
                seed = (int)DateTime.Now.Ticks & 0x0000F8FF;
            }
            else
            {
                seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            }
            Random rnd = new Random(seed);
            int k = rnd.Next(5, 50);
            for (int i = 0; i < k; i++) { rnd.Next(1, k); } //Zarlari iyice sallayalim.

            return rnd.Next(minValue, maxValue);
        }
    }
}
