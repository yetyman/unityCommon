using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GenericClasses
{
    [Serializable]
    public class EventedList<T> : List<T>
    {
        public delegate void CollectionEvent(EventedList<T> sender);
        public delegate void ItemEvent(EventedList<T> sender, T item, int index);
        public event ItemEvent Added;
        public event ItemEvent Removed;
        public event CollectionEvent Cleared;
        public event CollectionEvent Reversed;
        public EventedList() : base(){}
        public EventedList(IEnumerable<T> list) : base(list){}
        public EventedList(int capacity) : base(capacity){ }
        public new void Add(T item)
        {
            
            base.Add(item);
            Added?.Invoke(this, item, Count);
        }
        public new bool Remove(T item)
        {
            bool removed = false;
            var index = IndexOf(item);
            if(removed = base.Remove(item))
                Removed?.Invoke(this, item, Count);
            return removed;
        }
        public new void Clear()
        {
            base.Clear();
            Cleared?.Invoke(this);
        }
        public new void Reverse()
        {
            base.Reverse();
            Reversed?.Invoke(this);
        }
    }
}
