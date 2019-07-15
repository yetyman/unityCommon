using Assets.CommonLibrary.GenericClasses;
using Assets.GenericClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CommonLibrary.Geometric
{
    public class PolyGridLoader : MonoBehaviour
    {
        public static Dictionary<string, PolyGrid<TileController>> Grids = new Dictionary<string, PolyGrid<TileController>>();
        public PolyGrid<TileController> Grid;
        public GenericObjectPooler TilePooler;
        public int DirCnt = 6;

        public bool ScaleToBounds;
        public int Size;
        //TODO: scale to bounds option. requires polygon count...

        //public IPolygon Limits = new Box(Vector3.zero, Vector3.zero);
        public Box Limits = new Box(Vector3.zero, Vector3.zero);
        private void OnDrawGizmos()
        {
            
            //Gizmos.DrawCube(Limits.Min.Average(Limits.Max)+transform.localPosition, Limits.Delta);
            if (ScaleToBounds)
            {
                SetScale();
            }
            var grid = CreateGrid();
            if(grid!=null)
                foreach(var node in grid)
                    DrawNode(node.Value);
        }
        public virtual void SetScale()
        {
            var scale = Limits.DeltaZ / (float)Size;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        private void DrawNode(PolyGridNode<TileController> polyGridNode)
        {
            var corners = polyGridNode.NodeBorderCorners().Select(x=> transform.TransformPoint(x)).ToCircularList();
            for (int i = 0; i < corners.Count; i++)
                Gizmos.DrawLine( corners[i], corners[i + 1]);
        }
         
        private PolyGrid<TileController> CreateGrid()
        {
            if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
                if (Poly.Dirs.Supports(DirCnt))
                {
                    Grid = new PolyGrid<TileController>(DirCnt);

                    Grid.Add(new Vector3Int(0, 0, 0), (TileController)null);

                    CreateNodes(Grid.GetNode(Vector3Int.zero));

                    return Grid;
                }
            return null;
        }
        private void CreateNodes(PolyGridNode<TileController> polyGridNode)
        {
            foreach (var node in polyGridNode.GetSurrounding(true))
            {
                var nextNodePos = polyGridNode.LocalPos + node.Direction.TransformOffset;
                if (Limits.Within(transform.localToWorldMatrix * nextNodePos))
                {
                    if (node.Node == null)
                    {
                        if (polyGridNode.Add(node.Direction, (TileController) null ))
                            CreateNodes(polyGridNode.Adjacent(node.Direction));
                    }
                }
            }
        }

        public virtual GameObject GetPooledUnit(PolyGridNode<TileController> node)
        {
            return TilePooler.GetOne(transform, node.LocalPos, Quaternion.identity);
        }
        // Start is called before the first frame update
        void Start()
        {

            if (TilePooler != null)
            {
                foreach (var node in CreateGrid())
                    Grid[node.Key] = GetPooledUnit(node.Value).GetComponent<TileController>();
                Grids.Add(name, Grid);

            }
        }


    }
}