using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    [CreateAssetMenu()]
    public class PolyGridSelector : SingletonScriptableObject<PolyGridSelector>
    {
        
        [SerializeField]
        public HashSet<PolyGridInfo> PolyGrids = new HashSet<PolyGridInfo>();
        public IPolyGrid this[string name]
        {
            get { return GetPolyGrid(name); }
        }
        public IPolyGrid GetPolyGrid(string name)
        {
            return PolyGrids.First(x => x.name == name).PolyGrid;
        }
    }
    [Serializable]
    public class SelectPolyGrid<T>
    {
        public string PolyGridName;
        public PolyGridSelector Selector => PolyGridSelector.Instance;
        public PolyGrid<T> CachedPolyGrid;
        public PolyGrid<T> GetPolyGrid()
        {
            if (CachedPolyGrid == null)
                CachedPolyGrid = Selector.GetPolyGrid(PolyGridName) as PolyGrid<T>;
            return CachedPolyGrid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">PolyGrid Type</typeparam>
        /// <returns></returns>
        public PolyGrid<T> GetRandomPolyGrid()
        {
            return Selector.PolyGrids.Where(x=> x.PolyGrid is PolyGrid<T>).Random().PolyGrid as PolyGrid<T>;
        }
    }

    [Serializable]
    public struct PolyGridInfo
    {
        [SerializeField] public string name;

        [SerializeField] public IPolyGrid PolyGrid;

    }
}
