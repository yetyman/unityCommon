using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    [Serializable]
    public class Box : IPolygon
    {
        public Vector3 Min;
        public Vector3 Max;
        public Box(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
        public float DeltaX => Max.x - Min.x;
        public float DeltaY => Max.y - Min.y;
        public float DeltaZ => Max.z - Min.z;
        public int DeltaXInt => Mathf.RoundToInt(DeltaX)+1;
        public int DeltaYInt => Mathf.RoundToInt(DeltaY)+1;
        public int DeltaZInt => Mathf.RoundToInt(DeltaZ)+1;//thiis delta is inclusive, inclusive

        public Vector3 Delta { get => new Vector3(DeltaX, DeltaY, DeltaZ);  }

        public bool Within(Vector3 pos)
        {
            return pos.x <= Max.x
                && pos.y <= Max.y
                && pos.z <= Max.z
                && pos.x >= Min.x
                && pos.y >= Min.y
                && pos.z >= Min.z;
        }
    }
}
