using Assets;
using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



//Not for all pooler uses. generally you should just directly link poolers. this is for multi pooler selection. the upgrade menu for instance will be easier to manage with a pool selector than a list of all the different type of units pools linked individually.
[CreateAssetMenu()]
public class PoolSelector : SingletonScriptableObject<PoolSelector>
{
    [SerializeField]
    public Dictionary<string, GenericObjectPooler> Pools = new Dictionary<string, GenericObjectPooler>();
    public GenericObjectPooler this[string name]{
        get { return GetPool(name); }
    }
    public GenericObjectPooler GetPool(string name)
    {
        return Pools.SafeGet(name.ToLower());
    }
}
//[Serializable]
//public class PrefabFactory
//{
//    public string PoolName;
//    public PoolSelector Selector => PoolSelector.Instance;
//    public GenericObjectPooler CachedPool;
//    public GenericObjectPooler GetPool()
//    {
//        if (CachedPool == null)
//            CachedPool = Selector.GetPool(PoolName);
//        return CachedPool;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="category">matching tags will be in the same category</param>
//    /// <returns></returns>
//    public GenericObjectPooler GetRandomPool(string category = "")
//    {
//        return Selector.Pools.Where(x => x.Value.tag.Contains(category)).Random().Value;
//    }
//}

