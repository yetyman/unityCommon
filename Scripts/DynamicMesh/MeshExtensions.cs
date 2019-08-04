using Assets.CommonLibrary.GenericClasses;
using Assets.Scripts.Animation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.Scripts.DynamicMesh
{
    public static class MeshExtensions
    {
        public static MeshAnimator AnimateInitialization(this Mesh mesh, float timespan, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In)
        {
            var ani = new MeshAnimator(mesh, timespan, MeshAnimationType.VisibleTriangles, easingCurve, easingPattern);
            ani.Start();
            return ani;
        }

        public static Mesh DrawMesh(this MonoBehaviour behaviour, Mesh mesh)
        {
            mesh.MarkDynamic();

            // Set up game object with mesh;
            var meshRenderer = behaviour.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

            var filter = behaviour.gameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            return mesh;
        }

        public static Mesh MakeMesh(this Vector2[] outlineVertices, Rect bounds)
        {
            for (int i = 0; i < outlineVertices.Length; i++)
            {
                //TODO: this can slowly change to centering and scaling the point cloud to the container size. slow? of fuckin course. but i wanna know that i can do everything i'm doing here and that it all works the way i think it does.
                outlineVertices[i] -= bounds.min;
                outlineVertices[i].Scale(Vector2.one / bounds.size);
                outlineVertices[i] += bounds.min / bounds.size;

            }

            var vertices3D = System.Array.ConvertAll<Vector2, Vector3>(outlineVertices, v => v);

            // Use the triangulator to get indices for creating triangles
            var triangulator = new Triangulator(outlineVertices);
            var indices = triangulator.Triangulate();

            // Generate a color for each vertex
            var colors = Enumerable.Range(0, vertices3D.Length)
                .Select(i => Random.ColorHSV())
                .ToArray();

            // Create the mesh
            var mesh = new Mesh
            {
                vertices = vertices3D,
                triangles = indices,
                colors = colors
            };

            //http://www.html5gamedevs.com/topic/33987-whats-the-difference-between-mesh-and-submesh/
            //https://stackoverflow.com/questions/41784790/how-to-update-vertex-buffer-data-frequently-every-frame-opengl/41784937
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public class DirtyInt
        {
            public int value;
            public bool dirty = false;
            public int Value
            {
                get
                {
                    return value;
                }
                set
                {
                    this.value = value;
                    dirty = true;
                }
            }
            public void Decrement()
            {
                if(!dirty)
                    value--;
            }
        }

        public static void CombineVertice(int originalVecIndex, int newVecIndex, ref List<Vector3> vertices, ref List<int> oldTriangleIndices, ref List<DirtyInt> newTriangleIndices, ref List<Color> colors)
        {
            vertices.RemoveAt(originalVecIndex);
            colors.RemoveAt(originalVecIndex);
            for(int i = 0; i < oldTriangleIndices.Count; i++)
            {
                if (oldTriangleIndices[i] > originalVecIndex)
                    newTriangleIndices[i].Decrement();
                if (oldTriangleIndices[i] == originalVecIndex)
                    newTriangleIndices[i].Value = newVecIndex;
            }
        }

        public static Mesh AddMesh(this Mesh bigMesh, Mesh smallMesh, Matrix4x4 gridPositionTransform)
        {
            CombineInstance[] instances = new CombineInstance[2];
            instances[0] = new CombineInstance();

            instances[0].mesh = bigMesh;

            instances[1].mesh = smallMesh;
            instances[1].transform = gridPositionTransform;
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(instances,false);
            return newMesh;
        }

        //public static void AddMesh(this Mesh bigMesh, Mesh smallMesh, out Vector3[] sectionVertices, out int[] sectionTriangleIndices, out Color[] sectionColors)
        //{

        //    var smallTriangles = smallMesh.triangles.ToList();
        //    var smallVertices = smallMesh.vertices.ToList();
        //    var smallColors = smallMesh.colors.ToList();

        //    var triangles = bigMesh.triangles.ToList();
        //    var vertices = bigMesh.vertices.ToList();
        //    var colors = bigMesh.colors.ToList();
        //    Dictionary<int, int> VerticesCombined = new Dictionary<int, int>();
        //    for (int j = 0; j < smallVertices.Count(); j++)
        //    {
        //        for (int i = 0; i < vertices.Count(); i++)
        //        {
        //            if (vertices[i] == smallVertices[j])
        //            {
        //                VerticesCombined.Add(j, i);
        //                break;
        //            }
        //        }
        //    }

        //    int[] replacedIndexes = new int[smallVertices.Count];
        //    var replacedTriangles = smallTriangles.Select(x => new DirtyInt() { value = x }).ToList();
        //    foreach (var kv in VerticesCombined.Reverse())
        //    {
        //        CombineVertice(kv.Key, kv.Value, ref smallVertices, ref smallTriangles, ref replacedTriangles, ref smallColors);
        //    }
        //    foreach(var dirtyInt in replacedTriangles)
        //    {
        //        if(!dirtyInt.dirty)
        //            dirtyInt.Value = dirtyInt.Value + vertices.Count();
        //    }
        //    smallTriangles = replacedTriangles.Select(x => x.Value).ToList();

        //    vertices.AddRange(smallVertices);
        //    colors.AddRange(smallColors);
        //    triangles.AddRange(smallTriangles);

        //    bigMesh.Clear();

        //    bigMesh.vertices = vertices.ToArray();
        //    bigMesh.colors = colors.ToArray();
        //    bigMesh.triangles = triangles.ToArray();

        //    sectionVertices = smallVertices.ToArray();
        //    sectionTriangleIndices = smallTriangles.ToArray();
        //    sectionColors = smallColors.ToArray();

        //}
    }
}
