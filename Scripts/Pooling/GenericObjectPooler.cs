using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericObjectPooler : MonoBehaviour
{
    private static Dictionary<string, GenericObjectPooler> Pools = new Dictionary<string, GenericObjectPooler>();
    public GameObject PooledObj;
    public int HardCap= 80;
    public int SoftCap = 20;
    public int CountInUse
    {
        get {
            return PooledObjs.Count(x => x.activeSelf);
        }
    }
    public int CountNotInUse
    {
        get {
            return PooledObjs.Count() - CountInUse;
        }
    }
    public int CapIncrement=10;
    public float MarginBeforeIncrement = 5;
    public float IterTime;
    protected List<GameObject> PooledObjs = new List<GameObject>();
    private Coroutine Pooler;

    protected virtual void OnInstantiate(GameObject obj)
    {

    }

    // Start is called before the first frame update
    protected void Start()
    {

        if (PooledObj == null)
            Debug.LogWarning("you must provide an object to be pooled, if not this script will do nothing");
        else
        {
            StartPooling();
            Pools.Add(PooledObj.name, this);
        }
    }

    IEnumerator Pool()
    {
        while (transform.childCount < HardCap && SoftCap < HardCap)
        {
            if ((SoftCap - CountInUse) < MarginBeforeIncrement)
                SoftCap += CapIncrement;
            if (SoftCap > HardCap) SoftCap = HardCap;
            if (PooledObjs.Count() < SoftCap)
            {
                OnInstantiate(MakeOne());
            }
            else break;

            if (IterTime>0)
                yield return new WaitForSeconds(IterTime);
        }
    }
    protected virtual GameObject FindNext()
    {
        return PooledObj;
    }
    private GameObject MakeOne()
    {
        GameObject nextFab = FindNext();
        var obj = Instantiate(nextFab, transform);
        PooledObjs.Add(obj);
        obj.SetActive(false);
        return obj;
    }
    public void Reclaim(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.parent = transform;
    }

    public GameObject GetOne(Transform newParent, Vector3 position, Quaternion rotation, bool global = false)
    {
        var obj = PooledObjs.FirstOrDefault(x => !x.activeSelf);
        if (obj == null && CountInUse < HardCap)
        {
            Debug.LogWarning($"Consider lowering the Iter Time or increasing Cap Increment on {PooledObj.name}'s object pooler. it is not pooling as fast as the demand.");
            obj = MakeOne();
        }
        if (obj != null)
        {
            var scale = obj.transform.localScale;

            if (global)
            {
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
                obj.transform.localScale = scale;//this is for convenience but could change later.
            }
            obj.transform.parent = newParent;
            if (!global)
            {
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
                obj.transform.localScale = scale;//this is for convenience but could change later.
            }
            obj.SetActive(true);
            if ((SoftCap - CountInUse) < MarginBeforeIncrement)
                StartPooling();
        }
        return obj;
    }
    public List<GameObject> GetMany(int count, Transform newParent, Vector3[] position, Quaternion[] rotation)
    {
        var objs = new List<GameObject>();
        for(int i = 0; i < count; i++)
        {
            var obj = PooledObjs.FirstOrDefault(x => !x.activeSelf);
            if (obj == null && PooledObjs.Count() < HardCap)
                obj = MakeOne();
            if (obj != null)
            {
                obj.transform.parent = newParent;
                obj.transform.localPosition = position[i];
                obj.transform.localRotation = rotation[i];
                obj.SetActive(true);
            }
            objs.Add(obj);
        }
        if ((SoftCap - CountInUse) < MarginBeforeIncrement)
            StartPooling();
        return objs;
    }
    public void StartPooling()
    {
        if(Pooler!=null)
            StopCoroutine(Pooler);
        Pooler = StartCoroutine(Pool());
    }


}
