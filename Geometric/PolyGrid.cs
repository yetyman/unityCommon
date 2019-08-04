using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    public class PolyGrid<T> : Dictionary<Vector3Int, PolyGridNode<T>>, IPolyGrid
    {
        public DirBase Directions;
        public static implicit operator PolyGrid<T>(T[,] yxGrid)
        {
            var poly = new PolyGrid<T>(4);
            for (int y = 0; y < yxGrid.GetLength(0); y++)
                for (int x = 0; x < yxGrid.GetLength(1); x++)
                    poly.Add(new Vector3Int(x, y, 0), yxGrid[y, x]);
            return poly;
        }
        public new T this[Vector3Int pos]
        {
            get
            {
                if (base.TryGetValue(pos, out PolyGridNode<T> retVal))
                    return retVal.Value;
                else return default(T);
            }
            set
            {
                if (base.ContainsKey(pos))
                    base[pos].Value = value;
                else Add(pos, value);
            }
        }
        public T this[int x, int y]
        {
            get
            {
                return this[new Vector3Int(x, y, 0)];
            }
            set
            {
                this[new Vector3Int(x, y, 0)] = value;
            }
        }
        public int DirCnt => Directions.Dirs.Count;

        public Vector3 GridToTransform(PolyGridNode<T> node)
        {
            return Directions.GridToTransform(node.GridPos) + node.VisualOffset;
        }

        Vector3Int MinBounds = new Vector3Int();
        Vector3Int MaxBounds = new Vector3Int();
        public int GetContinuousLength(PolyGridNode<T> node, int dir)
        {
            int count = 0;
            var iterator = getDirectionPath(node, dir);
            while (iterator.MoveNext())
                count++;
            return count;
        }
        public void Add(Vector3Int pos, T item)
        {
            Add(pos, new PolyGridNode<T>(this, pos, item));
        }
        public new void Add(Vector3Int pos, PolyGridNode<T> item)
        {
            UpdateBounds(pos);
            base.Add(pos, item);
        }

        private void UpdateBounds(Vector3Int pos)
        {
            if (pos.x < MinBounds.x) MinBounds.x = pos.x;
            if (pos.y < MinBounds.y) MinBounds.y = pos.y;
            if (pos.z < MinBounds.z) MinBounds.z = pos.z;

            if (pos.x > MaxBounds.x) MaxBounds.x = pos.x;
            if (pos.y > MaxBounds.y) MaxBounds.y = pos.y;
            if (pos.z > MaxBounds.z) MaxBounds.z = pos.z;
        }
        public Box GetCartesianBounds()
        {
            return new Box(MinBounds, MaxBounds);
        }

        public PolyGrid(int dirCnt)
        {
            Directions = Poly.Dirs[dirCnt];
        }
        public PolyGridNode<T> GetNode(Vector3Int pos)
        {
            if (base.TryGetValue(pos, out PolyGridNode<T> retVal))
                return retVal;
            else return null;
        }
        public PolyGridNode<T> GetRandom()
        {
            return base.Values.ToList()[UnityEngine.Random.Range(0, Count - 1)];
        }

        public PolyGridNode<T> Adjacent(Vector3Int pos, Direction direction)
        {
            if (ContainsKey(pos + direction.GridOffset))
                return base[pos + direction.GridOffset];
            else return null;
        }
        public PolyGridNode<T> Adjacent(Vector3Int pos, int direction)
        {
            return base[pos + Directions[direction].GridOffset];
        }
        //TODO: when you get back implement PathIterators(they'll contain a circular list of steps to achieve a larger direction
        //also implement SingleUnitCycle with a starting direction and a clockwise counter clockwise boolean. default to clockwise. have an enum.

        public PolyPathIterator<T> getDirectionPath(PolyGridNode<T> start, int direction)
        {
            List<Direction> pathSegment = Directions.GetPath(direction);
            var path = new PolyPathIterator<T>(start, pathSegment);
            return path;
        }
        public PolyPathIterator<T> getSuperPath(PolyGridNode<T> start, float direction)
        {
            List<Direction> pathSegment = Directions.GetSuperPath(direction);
            var path = new PolyPathIterator<T>(start, pathSegment);
            return path;
        }

    }
}
