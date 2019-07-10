using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CommonLibrary.GenericClasses
{
    public class CircularList<T> : List<T>
    {
        public new T this[int i]
        {
            get
            {
                return base[ToCircularIndex(i)];
            }
            set
            {
                base[ToCircularIndex(i)] = value;
            }
        }

        private int ToCircularIndex(int i)
        {
            return i % Count + (i < 0 ? Count : 0);
        }

        public IEnumerator<T> IterateFrom(int index)
        {
            for (int i = 0; i < Count; i++)
                yield return this[index + i];
        }

        public IEnumerator<T> IterateRangeAround(int index, int indicesAfter, int? indicesBefore = null)
        {
            indicesBefore = indicesBefore??indicesAfter;
            for (int i = -indicesBefore.Value; i <= indicesAfter; i++)
                yield return this[index + i];
        }
    }
}
