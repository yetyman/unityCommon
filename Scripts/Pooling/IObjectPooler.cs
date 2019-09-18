using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{

    /// <summary>
    /// Possible alternative
    /// </summary>
    [Serializable]
    public abstract class AObjectPooler : MonoBehaviour
    {
        public abstract GameObject GetUninstantiated(string name = null);
        //public abstract Transform transform { get; }
        public abstract GameObject GetOne(string name = null, Transform newParent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, bool global = false);
        public abstract void Reclaim(GameObject obj);

        public abstract bool CanGet(GameObject viewPrefab);

    }
    //public interface AObjectPooler
    //{
    //    Transform transform { get; }
    //    GameObject GetOne(string name = null, Transform newParent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, bool global = false);
    //    void Reclaim(GameObject obj);
    //}
}