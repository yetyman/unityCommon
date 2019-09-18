using Assets.CommonLibrary;
using Assets.CommonLibrary.GenericClasses;
using System;
using System.Linq;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class PrefabFactory //: ISerializationCallbackReceiver
    {
        //add enum for which way to get it and make it a radio button on the editor so that things can be pretty, will change the two function's logic btw
        public string PrefabName;

        //[SerializeField, HideInInspector]
        //private SerializedData data;
        [SerializeField]
        private AObjectPooler prefabPool;
        public GameObject Prefab;

        public AObjectPooler PrefabPool
        {
            get {
                if (prefabPool == null && !String.IsNullOrWhiteSpace(PrefabName))
                    prefabPool = PoolSelector.Instance.GetPool(PrefabName);
                return prefabPool;
            }
        }

        public GameObject GetUninstantiated(string name = null)
        {
            if (PrefabPool != null)
            {
                return PrefabPool.GetUninstantiated(name);
            }
            else if (Prefab)
            {
                return Prefab;
            }
            else
                throw new Exception("you forgot to assign values to this Prefab Factory");
        }

        internal bool CanGet(GameObject viewPrefab)
        {
            if (PrefabPool != null)
            {
                return PrefabPool.CanGet(viewPrefab);
            }
            else if (Prefab)
            {
                return Prefab == viewPrefab;
            }
            else
                throw new Exception("you forgot to assign values to this Prefab Factory");
        }
        public GameObject GetOne(string name = null, Transform parent = null, Vector3 position = default(Vector3), Quaternion rotation = new Quaternion(), Vector3? scale = null, bool global =false)
        {
            if (PrefabPool != null)
            {
                return PrefabPool.GetOne(name, parent, position, rotation, scale, global);
            }
            else if (Prefab)
            {
                var go = GameObject.Instantiate(Prefab, global ? position : Vector3.zero, global ? rotation : Quaternion.identity, parent);
                if (!global)
                {
                    go.transform.localPosition = position;
                    go.transform.localRotation = rotation;
                    if (scale.HasValue)
                        go.transform.localScale = scale.Value;
                }

                return go;
            }
            else
                throw new Exception("you forgot to assign values to this Prefab Factory");
        }
        public void Reclaim(GameObject go)
        {
            if (PrefabPool != null)
            {
                PrefabPool.Reclaim(go);
            }
            else if (Prefab)
            {
                GameObject.Destroy(go);
            }
            else
                throw new Exception("you forgot to assign values to this Prefab Factory");
        }

        internal static AObjectPooler GetRandomPool(string v)
        {
            return PoolSelector.Instance.Pools.Where(x => string.IsNullOrWhiteSpace(v) || x.Key.ToLower().Contains(v.ToLower())).Random().Value;
        }

        //public void OnBeforeSerialize()
        //{
        //    if (prefabPool == null)
        //        return;
        //    data = SerializedData.Serialize(prefabPool);
        //}

        //public void OnAfterDeserialize()
        //{
        //    if (data == null || data.type == null)
        //        return;
        //    prefabPool = (AObjectPooler)SerializedData.Deserialize(data);
        //}
    }
}
