using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class ObjectListPooler : GenericObjectPooler
    {
        [SerializeField]
        List<GameObject> Items = new List<GameObject>();
        
        protected new void Start()
        {
            base.Start();
            HardCap = Items.Count();
            SoftCap = Items.Count();
            CapIncrement = 0;
            MarginBeforeIncrement = 0;
        }

        protected override GameObject FindNext()
        {
            return Items[PooledObjs.Count() - 1];
        }
    }
}
