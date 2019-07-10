using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GenericClasses
{
    [Serializable]
    public class Vec3 : SNullable<Vector3>
    {
        public static implicit operator Vector3? (Vec3 d)
        {
            Vector3? newVector3 = d.Value;
            if (!d.HasValue)
                newVector3 = null;
            return newVector3;
        }
        public static implicit operator Vec3(Vector3? d)
        {
            return new Vec3() { HasValue = d.HasValue, Value = d ?? default };
        }
        public static implicit operator Vector3(Vec3 d)
        {
            return d.Value;
        }
        public static implicit operator Vec3(Vector3 d)
        {
            return new Vec3() { HasValue = true, Value = d };
        }
    }
    public class SNullable {
        [SerializeField]
        public bool HasValue;
    }
    [Serializable]
    public class SNullable<T> : SNullable where T : struct
    {

        [SerializeField]
        T value;
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new Exception("This has no value");
                return value;
            }
            set
            {
                HasValue = true;
                this.value = value;
            }
        }
        public static implicit operator T? (SNullable<T> d)
        {
            T? newT = d.value;
            if (!d.HasValue)
                newT = null;
            return newT;
        }
        public static implicit operator SNullable<T>(T? d)
        {
            return new SNullable<T>() { HasValue = d.HasValue, value = d ?? default };
        }
        public static implicit operator T(SNullable<T> d)
        {
            return d.value;
        }
        public static implicit operator SNullable<T>(T d)
        {
            return new SNullable<T>() { HasValue = true, value = d };
        }
        public override string ToString()
        {
            return HasValue ? Value.ToString() : "null";
        }
    }
}
