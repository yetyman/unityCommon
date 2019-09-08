using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class UniqueObjectPooler : AObjectPooler
    {
        [SerializeField]
        List<GameObject> Items = new List<GameObject>();
        List<GameObject> InstantiatedItems = new List<GameObject>();

        public static Dictionary<string, UniqueObjectPooler> Pools = new Dictionary<string, UniqueObjectPooler>();

        public List<string> AvailablePrefabs()
        {
            //TODO: for some reason this does not work correctly
            return InstantiatedItems.Where(x => !x.activeSelf).Select(x => x.name).ToList();
        }
        public List<string> AllPrefabs()
        {
            //TODO: for some reason this does not work correctly
            return Items.Select(x => x.name).ToList();
        }
        protected void Start()
        {
            if (!Items.Any())
                Debug.LogWarning("you must provide objects to be pooled, if not this script will do nothing");
            else
            {
                StartCoroutine(Pool());
                Pools.Add(name, this);
            }
        }


        protected virtual void OnInstantiate(GameObject obj)
        {

        }



        IEnumerator Pool()
        {
            foreach (var obj in Items)
            {
                yield return new WaitForEndOfFrame();
                OnInstantiate(MakeOne(obj));
            }
        }

        private GameObject MakeOne(GameObject nextFab)
        {
            if (!InstantiatedItems.Any(x => x.name == (nextFab.name + "(Clone)")))
            {
                var obj = Instantiate(nextFab, transform);
                InstantiatedItems.Add(obj);
                obj.SetActive(false);
                return obj;
            }
            else return null;
        }
        public override void Reclaim(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.parent = transform;
        }

        public override GameObject GetOne(string name, Transform newParent, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, bool global = false)
        {
            GameObject obj = InstantiatedItems.FirstOrDefault(x => x.name == (name + "(Clone)"));
            obj = obj ?? MakeOne(Items.First(x => x.name == name));

            if (obj != null)
            {
                if (global)
                {
                    obj.transform.localPosition = position ?? obj.transform.localPosition;
                    obj.transform.localRotation = rotation ?? obj.transform.localRotation;
                    obj.transform.localScale = scale ?? obj.transform.localScale;
                }
                obj.transform.SetParent(newParent, global);
                if (!global)
                {
                    obj.transform.localPosition = position ?? obj.transform.localPosition;
                    obj.transform.localRotation = rotation ?? obj.transform.localRotation;
                    obj.transform.localScale = scale ?? obj.transform.localScale;
                }
                obj.SetActive(true);
            }
            return obj;
        }

        public override GameObject GetUninstantiated(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("You must specify a specific object for unique object pools");
            return Items.FirstOrDefault(x => x.name == name);
        }

        public override bool CanGet(GameObject viewPrefab)
        {
            return Items.Any(x => x == viewPrefab);
        }
    }
}
