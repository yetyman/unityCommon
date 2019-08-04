using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class CollectionExtensions
    {
        public static CircularList<T> ToCircularList<T>(this IEnumerable<T> enumerable)
        {
            var list = new CircularList<T>();
            list.AddRange(enumerable);
            return list;
        }
        public static bool SafeAdd<TValue>(this ICollection<TValue> collection, TValue value)
        {
            if (!collection.Contains(value))
            {
                collection.Add(value);
                return true;
            }
            return false;
        }
        public static bool SafeSet<TValue>(this ICollection<TValue> collection, TValue value)
        {
            if (collection.Contains(value))
            {
                collection.Remove(value);
                collection.Add(value);
                return false;
            }
            else
            {
                collection.Add(value);
                return true;
            }
        }

        public static T Random<T>(this ICollection<T> collection)
        {
            var random = UnityEngine.Random.Range(0, collection.Count());
            return collection.Skip(random).First();
        }
        public static T Random<T>(this IEnumerable<T> collection)
        {
            var random = UnityEngine.Random.Range(0, collection.Count());
            return collection.Skip(random).First();
        }
    }
}
