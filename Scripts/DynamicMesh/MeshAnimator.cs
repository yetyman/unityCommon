using Assets.GenericClasses;
using Assets.Scripts.Animation;
using Assets.Scripts.DynamicMesh;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;


namespace Assets.Scripts.DynamicMesh
{
    public class MeshValues
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Color[] colors;
    }
    public enum MeshAnimationType { VisibleTriangles, FromTo }
    public class MeshAnimator : AnimationInstanceC<Mesh, MeshValues>
    {
        private static Dictionary<Mesh, MeshAnimator> Instances = new Dictionary<Mesh, MeshAnimator>();
        MeshAnimationType animationType;
        MeshValues Current = new MeshValues();
        public static void UpdateCloud (Mesh target, MeshValues tweenedMesh)
        {
            Instances[target].UpdateCloud(tweenedMesh); 
        }

        private void UpdateCloud(MeshValues tweenedValues)
        {
            Target.Clear();

            if (PercentageCompleted < 1)
            {
                //TODO:reenable this one after we get triangles working to begin with.
                Target.vertices = tweenedValues.vertices;//Current.vertices;
                Target.colors = tweenedValues.colors;
                Target.triangles = tweenedValues.triangles;
            }
            else
            {
                Target.vertices = To.vertices;
                Target.colors = To.colors;
                Target.triangles = To.triangles;
            }
        }

        public static MeshValues GETNOTHIN(Mesh target)
        {
            return new MeshValues() { vertices = target.vertices, colors = target.colors, triangles = target.triangles};
        }

        public MeshAnimator(Mesh target, float timeSpanSeconds, MeshAnimationType animationType = MeshAnimationType.FromTo, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, UpdateCloud, GETNOTHIN, timeSpanSeconds, easingCurve, easingPattern)
        {
            //if (target.vertices.Count() != final.vertices.Count() || target.triangles.Count() != final.triangles.Count() || target.colors.Count() != final.colors.Count())
            //    throw new System.Exception("You cannot animate from variable numbers of vertices, triangles, or colors yet");
            ////TODO: interpolate new vertices,. triangles, and colors...

            this.animationType = animationType; 
            Target = target;
            Target.MarkDynamic();
            Instances.Add(Target, this);

            Current.vertices = new Vector3[target.vertices.Length];
            Current.triangles = new int[target.triangles.Length];
            Current.colors = new Color[target.colors.Length];
            //Current.colors = new Color[From.Colors.Length];

        }
        public override MeshValues Tween()
        {
            float percent = PercentageCompleted.Value;
            //we're gonna set the global variables here. 
            //in the setter we'll apply all the arrays.
            //just figure them out here. never intantiate new, jsut modify. 
            if (animationType == MeshAnimationType.FromTo) {
                for (int i = 0; i < Current.vertices.Count(); i++)
                {
                    Current.vertices[i] = To.vertices[i].Average(From.vertices[i], percent);
                    Current.colors[i] = To.colors[i].Average(From.colors[i], percent);
                }
                for (int i = 0; i < Current.triangles.Count(); i++)
                { 
                    Current.triangles[i] = From.triangles[i];
                }

            }
            else if (animationType == MeshAnimationType.VisibleTriangles) {
                int maxVertex = 3;
                //for now we'll just set the vertices up to the the curent percentage and we'll include every triangle up to there. so all of them that only contain existing vertices. where all triangle indices are less than vertice count.

                //we are setting all visible vertices, so dont worry about setting the rest.
                for (; (maxVertex / (float)From.vertices.Length) <= percent; maxVertex++)
                    Current.vertices[maxVertex] = From.vertices[maxVertex];

                if (maxVertex < From.vertices.Length)
                {
                    //buggy as crap; find the next vertices for the newest triangle and animate those instead
                    var percentTilNextTriangle = ((maxVertex / (float)From.vertices.Length) - percent) * From.vertices.Length;
                    Current.vertices[maxVertex] = From.vertices[maxVertex - 1].Average(From.vertices[maxVertex], percentTilNextTriangle);
                    maxVertex++;
                }


                for (int i = 0; i < From.triangles.Length - 3; i += 3)
                {
                    if (From.triangles[i] < maxVertex && From.triangles[i + 1] < maxVertex && From.triangles[i + 2] < maxVertex)
                    {
                        Current.triangles[i] = From.triangles[i];
                        Current.triangles[i + 1] = From.triangles[i + 1];
                        Current.triangles[i + 2] = From.triangles[i + 2];
                    }
                    else
                    {
                        //this will make a zero area triangle, good enough.
                        Current.triangles[i] = 0;
                        Current.triangles[i + 1] = 0;
                        Current.triangles[i + 2] = 0;
                    }

                }

            }
            return new MeshValues() { vertices = Current.vertices, colors = Current.colors, triangles = Current.triangles};
            //return the point cloud for the graphics card here.
            ///soooooo much optimisation will happen here in the future
        }
    }
   
}
