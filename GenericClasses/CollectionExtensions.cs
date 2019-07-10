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
    }
}
