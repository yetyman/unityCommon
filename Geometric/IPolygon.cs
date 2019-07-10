using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    public interface IPolygon
    {
        bool Within(Vector3 pos);
    }
}
