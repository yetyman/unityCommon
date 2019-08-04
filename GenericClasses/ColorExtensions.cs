using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class ColorExtensions
    {
        public static Color Average(this Color one, Color two, float weight = .5f)
        {
            return new Color(one.r.Average(two.r,weight), one.g.Average(two.g,weight), one.b.Average(two.b,weight), one.a.Average(two.a,weight));
        }
    }
}
