using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GenericClasses
{
    public static class FloatExtensions
    {
        public static float Average(this float target, float other, float weight)
        {
            return target * weight + other * (1 - weight);
        }
    }
}
