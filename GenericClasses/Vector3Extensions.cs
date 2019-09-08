using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class Vector3Extensions
    {
        public static Vector3 Take(this Vector3 target, bool x, bool y, bool z)
        {
            return new Vector3(x ? target.x : 0, y ? target.y : 0, z ? target.z : 0);
        }
        public static Vector3 Mix(this Vector3 target, Vector3 other, bool x, bool y, bool z)
        {
            return new Vector3(x ? target.x : other.x, y ? target.y : other.y, z ? target.z : other.z);
        }
        public static Vector3 Add(this Vector3 target, float x, float y, float z)
        {
            return new Vector3(target.x +x,target.y +y, target.z +z);
        }
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
