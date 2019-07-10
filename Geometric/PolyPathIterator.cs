using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CommonLibrary.Geometric
{
    public class PolyPathIterator<T> : IEnumerator<T>
    {
        public PolyPathIterator(PolyGridNode<T> start, List<Direction> pathSegment)
        {
            Start = start;
            PathSegment = (CircularList<Direction>)pathSegment;
        }
        private CircularList<Direction> PathSegment;

        private PolyGridNode<T> Start;
        private int Index;
        public PolyGridNode<T> CurrentNode;
        private bool IsSuperDirection = false;
        T IEnumerator<T>.Current => CurrentNode.Value;


        public object Current => CurrentNode.Value;

        public bool IsPathMarker => Index % PathSegment.Count == 0;

        public bool MoveNext()
        {
            return MoveNext(false);
        }
        public bool MoveNext(bool AdvanceToPathMarker)
        {
            do
            {
                CurrentNode = CurrentNode.Adjacent(PathSegment[Index++]);

            } while (CurrentNode != null
            && (!IsPathMarker && AdvanceToPathMarker));

            return CurrentNode != null;
        }
        public void Reset()
        {
            Index = 0;
            CurrentNode = Start;
        }

        public void Dispose()
        {
            PathSegment.Clear();
            Start = null;
            CurrentNode = null;
        }

    }
}
