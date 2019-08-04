﻿using Assets;
using Assets.CommonLibrary.GenericClasses;
using Assets.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericObjectPooler : MonoBehaviour
{
    public PoolSelector Pools => PoolSelector.Instance;

    public SelectPrefab SelectPrefab;
    [BeginHorizontal]
    public bool Unlimited;
    [EndHorizontal]
    public int HardCap = 80;
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

        Pools.Pools.SafeSet(new PoolInfo()
        {
            Pool = this,
            name = SelectPrefab.GetPrefab().name
        });
        if (SelectPrefab.GetPrefab() == null)
            Debug.LogWarning("you must provide an object to be pooled, if not this script will do nothing");
        else
        {
            StartPooling();
            
        }
    }

    IEnumerator Pool()
    {
        while (PooledObjs.Count < SoftCap)
        {
            if(PoolOne() == null)
                break;

            if (IterTime>0)
                yield return new WaitForSeconds(IterTime);
        }
    }
    private GameObject PoolOne()
    {
        GameObject obj = null;
        if (Unlimited || PooledObjs.Count < HardCap)
        {
            if ((SoftCap - CountInUse) < MarginBeforeIncrement)
                SoftCap += CapIncrement;
            if (!Unlimited)
                if (SoftCap > HardCap)
                    SoftCap = HardCap;
            if (PooledObjs.Count < SoftCap)
            {
                OnInstantiate(obj = MakeOne());
            }
        }
        return obj;
    }
    protected virtual GameObject FindNext()
    {
        return SelectPrefab.GetPrefab();
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
        return GetOne(newParent, position, rotation, null, global);
    }
    public GameObject GetOne(Transform newParent, Vector3 position, Quaternion rotation, Vector3? scale, bool global = false)
    {
        var obj = PooledObjs.FirstOrDefault(x => !x.activeSelf);
        if (obj == null && (CountInUse < HardCap || Unlimited))
        {
            if(IterTime!=0)
                Debug.LogWarning($"Consider lowering the Iter Time or increasing Cap Increment on {SelectPrefab.GetPrefab().name}'s object pooler. it is not pooling as fast as the demand.");
            obj = PoolOne();
        }
        if (obj != null)
        {
            scale = scale ?? obj.transform.localScale;

            if (global)
            {
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
                obj.transform.localScale = scale.Value;//this is for convenience but could change later.
            }
            obj.transform.parent = newParent;
            if (!global)
            {
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
                obj.transform.localScale = scale.Value;//this is for convenience but could change later.
            }
            obj.SetActive(true);

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
            if (obj == null && (PooledObjs.Count() < HardCap || Unlimited))
                obj = PoolOne();
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
