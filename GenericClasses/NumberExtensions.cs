using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GenericClasses
{
    public static class NumberExtensions
    {
        public static int PositiveMod(this int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        public static float PositiveMod(this float x, float m)
        {
            float r = x % m;
            return r < 0 ? r + m : r;
        }
        public static double PositiveMod(this double x, double m)
        {
            double r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
