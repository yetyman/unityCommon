using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GenericClasses
{
    public static class Vector3Extensions
    {
        public static Vector3 Multiply(this Vector3 target, Vector3 other)
        {
            return new Vector3(target.x * other.x, target.y * other.y, target.z * other.z);
        }
        public static Vector3 Divide(this Vector3 target, Vector3 other)
        {
            return new Vector3(target.x / other.x, target.y / other.y, target.z / other.z);
        }
        public static Vector3 Average(this Vector3 target, Vector3 other, float weight = .5f)
        {
            return new Vector3(
                target.x.Average(other.x, weight),
                target.y.Average(other.y, weight),
                target.z.Average(other.z, weight));
        }

        public static Vector3 Average(this IEnumerable<Vector3> target)
        {
            return new Vector3(
                target.Average(x => x.x),
                target.Average(x => x.y),
                target.Average(x => x.z)
                );
        }
        public static float Hypotenuse(this Vector3 one, Vector3 two)
        {
            return Vector3.Distance(one, two);
        }
    }
}
