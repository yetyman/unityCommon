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
    public GenericObjectPooler GetPool(string name, bool allowContaining = true)
    {
        var val = Pools.SafeGet(name.ToLower());
        if (val == null && !string.IsNullOrWhiteSpace(name) && allowContaining)
            val = Pools.FirstOrDefault(x => x.Key.Contains(name.ToLower())).Value;
        return val;
    }
}

