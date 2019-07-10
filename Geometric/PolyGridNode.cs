using Assets.CommonLibrary.GenericClasses;
using Assets.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    public class PolyGridNode<T>
    {
        public T Value;
        public Vector3Int GridPos;
        public Vector3 LocalPos => Grid.GridToTransform(this);
        
        public PolyGrid<T> Grid;

        //useful for maps with floating platforms. minimum performance impact. keepin it.
        Dictionary<Direction, PolyGridNode<T>> ExtraDirections;
        public Vector3 VisualOffset;

        public PolyGridNode(PolyGrid<T> parent, Vector3Int pos, T val)
        {
            Value = val;
            GridPos = pos;
            Grid = parent;
        }

        public CircularList<Vector3> NodeBorderCorners()
        {
            
            //TODO: figure out what is wrong. this should return local positions. should not have localtoworldmatreix here
            return Grid.Directions.GetBounds().Select(x => x.TransformOffset + this.LocalPos).ToCircularList();
        }
        public bool Add(Direction dir, T value)
        {
            if (!Has(dir))
            {
                Grid.Add(GridPos + dir.GridOffset, new PolyGridNode<T>(Grid, GridPos + dir.GridOffset, value));
                return true;
            }
            return false;
        }
        public PolyGridNode<T> Adjacent(Direction direction)
        {
            return Grid.Adjacent(GridPos, direction);
        }
        public bool Has(Direction direction)
        {
            return Grid.Adjacent(GridPos, direction) != null;
        }
        public IEnumerable<(Direction Direction, PolyGridNode<T> Node)> GetSurrounding(bool includeNull = false)
        {
            return ExtraDirections == null ?
                Grid.Directions.Dirs
                .Select(x => (x, Adjacent(x)))
                .Where(y => includeNull || (!includeNull && y.Item2 != null)).ToList()
                : Grid.Directions.Dirs.Concat(ExtraDirections?.Keys)
                .Select(x => (x, Adjacent(x)))
                .Where(y => includeNull || (!includeNull && y.Item2 != null)).ToList();
        }

        public Dictionary<PolyGridNode<T>, PolyPathIterator<T>> Sprawl(float distance, bool includePortals = false)
        {
            var dictionary = new Dictionary<PolyGridNode<T>, (float distance, PolyPathIterator<T> pathTo)>();
            return Sprawl(this, new Queue<Direction>(), distance, 0, includePortals, dictionary);
        }
        private Dictionary<PolyGridNode<T>, PolyPathIterator<T>> Sprawl(PolyGridNode<T> start, Queue<Direction> soFar, float distance, float distanceTravelled, bool includePortals, Dictionary<PolyGridNode<T>, (float distanceTravelled, PolyPathIterator<T> pathTo)> sprawledLocations)
        {

            if (sprawledLocations.ContainsKey(this) && sprawledLocations[this].distanceTravelled > distanceTravelled || !sprawledLocations.ContainsKey(this))
                sprawledLocations.SafeSet(this, (distanceTravelled, new PolyPathIterator<T>(start, soFar.ToList())));

            foreach (var adjacent in GetSurrounding())
                if (adjacent.Direction.Distance < distance)
                {
                    soFar.Enqueue(adjacent.Direction);
                    adjacent.Node.Sprawl(start, soFar, distance - adjacent.Direction, distanceTravelled + adjacent.Direction, includePortals, sprawledLocations);
                    soFar.Dequeue();
                }
            if (distance == 0)
                return sprawledLocations.ToDictionary(x => x.Key, x => x.Value.pathTo);
            else return null;
        }
    }


}
