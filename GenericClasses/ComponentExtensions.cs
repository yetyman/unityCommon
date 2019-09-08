using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class ComponentExtensions
    {
        public static T GetComponentAbove<T>(this Component comp) where T : Component
        {
            T retVal = null;
            Transform parent = comp.transform;

            while (!retVal && parent)
                retVal = (parent=parent.parent).GetComponent<T>();

            return retVal;
        }
    }
}
