using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Maps
{
    public interface IMap {
        Vector3 Size { get; }
    }
    public interface IMap<Vals> : IMap
    {
        Vals Values { get; }
    }
}
