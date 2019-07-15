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
    public struct Direction
    {
        public float Angle;
        public int SequencePos;
        public int SequenceSize;
        public Vector3 TransformOffset;
        public Vector3Int GridOffset;
        public float Distance;

        public Direction AddRelative(Direction relative)
        {
            return Directions.AddRelative(this, relative);
        }
        public static Vector3 operator +(Direction dir, Vector3 other)
        {
            return other + dir.TransformOffset;
        }
        public static Vector3Int operator +(Direction dir, Vector3Int other)
        {
            return other + dir.GridOffset;
        }
        public static int operator +(Direction dir, int other)
        {
            return other + dir.SequencePos;
        }
        public static float operator +(Direction dir, float distance)
        {
            return distance + dir.Distance;
        }
        public static float operator +(float distance, Direction dir)
        {
            return distance + dir.Distance;
        }
        public static float operator -(float distance, Direction dir)
        {
            return distance - dir.Distance;
        }

        public static Direction operator /(Direction dir, float distance)
        {
            return new Direction()
            {
                SequenceSize = dir.SequenceSize,
                Angle = dir.Angle,
                Distance = dir.Distance / distance,
                SequencePos = dir.SequencePos,
                GridOffset = dir.GridOffset,
                TransformOffset = dir.TransformOffset / distance
            };
        }
        public static Direction operator *(Direction dir, Vector3 scale)
        {
            return new Direction()
            {
                SequenceSize = dir.SequenceSize,
                Angle = dir.Angle,
                Distance = dir.Distance * scale.magnitude,
                SequencePos = dir.SequencePos,
                GridOffset = dir.GridOffset,
                TransformOffset = Vector3.Scale(dir.TransformOffset, scale)
            };
        }
        public static Direction operator *(Direction dir, Quaternion rotation)
        {
            return new Direction()
            {
                SequenceSize = dir.SequenceSize,
                Angle = dir.Angle,//angle is ther for iteration not absolutes. don't multiply it.
                Distance = dir.Distance,
                SequencePos = dir.SequencePos,
                GridOffset = dir.GridOffset,
                TransformOffset = (rotation * dir.TransformOffset).normalized * dir.Distance
            };
        }
    }
    public static class Poly
    {
        public static Directions Dirs = new Directions();
    }
    public class Directions
    {
        public static Direction AddRelative(Direction baseDirection, Direction relativeDirection)
        {
            return Poly.Dirs[baseDirection.SequenceSize][baseDirection.SequencePos + relativeDirection.SequencePos];
        }

        public bool Supports(int dirCnt)
        {
            return Dirs.ContainsKey(dirCnt);
        }
        public DirBase this[int dirCnt]
        {
            get
            {
                switch (dirCnt)
                {
                    case 4:
                        return Dirs[4];
                    case 6:
                        return Dirs[6];
                    case 8:
                        return Dirs[8];
                    default:
                        return null;
                }
            }
        }
        private static Dictionary<int, DirBase> Dirs = new Dictionary<int, DirBase>
        {
            { 4 , new Dir4() }, //-|-
            { 6 , new Dir6() }, //>|<
            { 8 , new Dir8() }  //->|<-
        };

    }
    public abstract class DirBase
    {

        protected Vector3 GridToTransformScale;
        public float NestingScale;
        public virtual float MinimumAngle => Dirs[1].Angle;
        protected DirBase(DirBase other)
        {
            var dirArr = new Direction[other.Dirs.Count];
            other.Dirs.CopyTo(dirArr);
            Dirs.AddRange(dirArr);

            NestingScale = other.NestingScale;
            GridToTransformScale = other.GridToTransformScale;
        }

        protected DirBase()
        {
        }
        public abstract DirBase Clone();
        public void Transform(Quaternion rotation, Vector3 scale)
        {
            for (int i = 0; i < Dirs.Count(); i++)
            {
                Dirs[i] = Dirs[i] * rotation * scale;
            }
            GridToTransformScale = GridToTransformScale.Multiply(scale);
        }
        public Vector3 GridToTransform(Vector3Int gridPos)
        {
            //Use entire transform matrix here. should be the correct move. adjust everything else accordingly since that matrix is key to converting from the grid to the transform. maybe gridtotransformscale doesnt get scaled with direction or we divide by scale here.
            return Vector3.Scale(gridPos, GridToTransformScale);
        }
        public CircularList<Direction> GetBounds()
        {
            //TODO: i can't imagine many situations where you would only get bounds once. cache the value internally and check if rotation and scale have changed at all.

            var shapeLimits = new CircularList<Direction>();

            //gotta user the original shape because the math works out better
            var Dirs = Poly.Dirs[this.Dirs.Count].Dirs;

            var centerAxis = Vector3.Cross(Dirs[0].TransformOffset, Dirs[1].TransformOffset);
            //if(centerAxis != Vector3.Cross(Dirs[1].TransformOffset, Dirs[2].TransformOffset))
            //{
            //    //3d. oooooh. has a hole. not dealing with the situation where the average of all of them is 0
            //    centerAxis = Dirs.Select(x => x.TransformOffset).Average();
            //    if(centerAxis == Vector3.zero)
            //    {
            //        //eh....
            //        //TODO: deal with this.
            //    }
            //}

            var shapeRotation = Quaternion.AngleAxis(MinimumAngle / 2, centerAxis);
            var cornerDistance = Dirs[0].Distance / 2 / Mathf.Cos(MinimumAngle / 180 * Mathf.PI / 2);

            foreach (Direction d in Dirs)
            {
                var dir = new Direction();
                dir.SequenceSize = Dirs.Count;
                dir.Distance = cornerDistance;
                dir.Angle = d.Angle + MinimumAngle / 2;
                dir.TransformOffset = (shapeRotation * d.TransformOffset).normalized * dir.Distance;
                dir.SequencePos = d.SequencePos;
                shapeLimits.Add(dir);
            }

            //else cornerDistance = Mathf.Sqrt(Mathf.Pow(Mathf.Tan(MinimumAngle/180*Mathf.PI/2) * Dirs[0].Distance/2,2) + Mathf.Pow(Dirs[0].Distance/2,2));

            
            return shapeLimits;
        }
        public Direction this[int i]
        {
            get
            {
                return GetDirection(i);
            }
        }
        public Direction this[Vector2Int gridOffset]
        {
            get
            {
                return GetAdjacentGridPos(gridOffset);
            }
        }
        public CircularList<Direction> Dirs = new CircularList<Direction>();

        public Direction GetDirection(int i)
        {
            return Dirs[i];
        }

        public List<Direction> GetPath(int generalDirection)
        {
            return new List<Direction>() { GetDirection(generalDirection) };
                
            //don't account for nonContinuous here. only a dir3 needs to worry about that and it will make the code confusing in all likelihood. just override in dir3 and anything else that needs it
        }
        public List<Direction> GetSuperPath(float generalDirection, bool favorCounterClockwise = false)
        {
            List<Direction> directions = new List<Direction>();
            float actualDirection = Mathf.RoundToInt(generalDirection - (favorCounterClockwise ? .001f : 0));
            Direction nextDirection;
            do
            {
                if (actualDirection >= generalDirection)
                    nextDirection = Dirs[Mathf.FloorToInt(generalDirection)];
                else
                    nextDirection = Dirs[Mathf.CeilToInt(generalDirection)];
                actualDirection += nextDirection.SequencePos;
                directions.Add(nextDirection);
            } while (Mathf.Abs(actualDirection/directions.Count() - generalDirection) > .001);

            return directions;
        }

        public Direction GetAngle(int angle)
        {
            return Dirs[Mathf.RoundToInt(((angle % 360 + 360) % 360) / MinimumAngle)];
        }

        public Direction GetAdjacentObjPos(Vector2 relativePos)
        {
            return Dirs.First(x => x.TransformOffset.Equals(relativePos));
        }
        public Direction GetAdjacentGridPos(Vector2Int relativePos)
        {
            return Dirs.First(x => x.GridOffset.Equals(relativePos));
        }
    }
    public class Dir4 : DirBase
    {
        private Dir4(Dir4 other) : base(other)
        {

        }
        public override DirBase Clone()
        {
            return new Dir4(this);
        }
        public Dir4()
        {
            NestingScale = .5f;
            GridToTransformScale = Vector3.one;

            Dirs = new CircularList<Direction>
            {
                new Direction(){ SequenceSize = 4, Angle = 0, SequencePos = 0, TransformOffset = new Vector3(0,1), GridOffset = new Vector3Int(0,1,0), Distance = 1 },
                new Direction(){ SequenceSize = 4, Angle = 90, SequencePos = 1, TransformOffset = new Vector3(1,0), GridOffset = new Vector3Int(1,0,0), Distance = 1 },
                new Direction(){ SequenceSize = 4, Angle = 180, SequencePos = 2, TransformOffset = new Vector3(0,-1), GridOffset = new Vector3Int(0,-1,0), Distance = 1 },
                new Direction(){ SequenceSize = 4, Angle = 270, SequencePos = 3, TransformOffset = new Vector3(-1,0), GridOffset = new Vector3Int(-1,0,0), Distance = 1 },
            };
            Dirs.ForEach(x => x.SequenceSize = Dirs.Count);
        }

        public Direction Up() => GetDirection(0);
        public Direction Right() => GetDirection(1);
        public Direction Down() => GetDirection(2);
        public Direction Left() => GetDirection(3);
    }
    public class Dir8 : DirBase//squares including corners
    {

        private Dir8(Dir8 other) : base(other)
        {

        }
        public override DirBase Clone()
        {
            return new Dir8(this);
        }
        public Dir8()
        {
            NestingScale = .5f;
            GridToTransformScale = Vector3.one;

            Dirs = new CircularList<Direction>
            {
                new Direction(){ SequenceSize = 8, Angle = 0, SequencePos = 0, TransformOffset = new Vector3(0,1), GridOffset = new Vector3Int(0,1,0), Distance = 1 },
                new Direction(){ SequenceSize = 8, Angle = 45, SequencePos = 1, TransformOffset = new Vector3(1,1), GridOffset = new Vector3Int(1,1,0), Distance = Mathf.Sqrt(2) },
                new Direction(){ SequenceSize = 8, Angle = 90, SequencePos = 2, TransformOffset = new Vector3(1,0), GridOffset = new Vector3Int(1,0,0), Distance = 1 },
                new Direction(){ SequenceSize = 8, Angle = 135, SequencePos = 3, TransformOffset = new Vector3(1,-1), GridOffset = new Vector3Int(1,-1,0), Distance = Mathf.Sqrt(2) },
                new Direction(){ SequenceSize = 8, Angle = 180, SequencePos = 4, TransformOffset = new Vector3(0,-1), GridOffset = new Vector3Int(0,-1,0), Distance = 1 },
                new Direction(){ SequenceSize = 8, Angle = 225, SequencePos = 5, TransformOffset = new Vector3(-1,-1), GridOffset = new Vector3Int(-1,-1,0), Distance = Mathf.Sqrt(2) },
                new Direction(){ SequenceSize = 8, Angle = 270, SequencePos = 6, TransformOffset = new Vector3(-1,0), GridOffset = new Vector3Int(-1,0,0), Distance = 1 },
                new Direction(){ SequenceSize = 8, Angle = 315, SequencePos = 7, TransformOffset = new Vector3(-1,1), GridOffset = new Vector3Int(-1,1,0), Distance = Mathf.Sqrt(2) },
            };
            Dirs.ForEach(x => x.SequenceSize = Dirs.Count);
        }

        public Direction Up() => GetDirection(0);
        public Direction UpRight() => GetDirection(1);
        public Direction Right() => GetDirection(2);
        public Direction DownRight() => GetDirection(3);
        public Direction Down() => GetDirection(4);
        public Direction DownLeft() => GetDirection(5);
        public Direction Left() => GetDirection(6);
        public Direction UpLeft() => GetDirection(7);
    }
    public class Dir6 : DirBase//represents triangular grid, not hexagonal!
    {

        private Dir6(Dir6 other) : base(other)
        {

        }
        public override DirBase Clone()
        {
            return new Dir6(this);
        }
        public Dir6()
        {
            NestingScale = .5f;
            GridToTransformScale = new Vector3(Mathf.Sqrt(3) / 2, .5f, 1);

            Dirs = new CircularList<Direction>
            {
                //Grid Offset Note. while 0,.5,1 may feel more intuitive, 0,1,2 contributes greatly to even odd calculations making code much easier to write. i think this makes up for the lack of intuitiveness
                new Direction(){ SequenceSize = 6, Angle = 0, SequencePos = 0, TransformOffset = new Vector3(0, 1), GridOffset = new Vector3Int(0,2,0), Distance = 1 },
                new Direction(){ SequenceSize = 6, Angle = 60, SequencePos = 1, TransformOffset = new Vector3(Mathf.Sqrt(3)/2, .5f), GridOffset = new Vector3Int(1,1,0), Distance = 1},
                new Direction(){ SequenceSize = 6, Angle = 120, SequencePos = 2, TransformOffset = new Vector3(Mathf.Sqrt(3)/2, -.5f), GridOffset = new Vector3Int(1,-1,0), Distance = 1 },
                new Direction(){ SequenceSize = 6, Angle = 180, SequencePos = 3, TransformOffset = new Vector3(0, -1), GridOffset = new Vector3Int(0,-2,0), Distance = 1 },
                new Direction(){ SequenceSize = 6, Angle = 240, SequencePos = 4, TransformOffset = new Vector3(-Mathf.Sqrt(3)/2, -.5f), GridOffset = new Vector3Int(-1,-1,0), Distance = 1 },
                new Direction(){ SequenceSize = 6, Angle = 300, SequencePos = 5, TransformOffset = new Vector3(-Mathf.Sqrt(3)/2, .5f), GridOffset = new Vector3Int(-1, 1,0), Distance = 1 },
            };
            Dirs.ForEach(x => x.SequenceSize = Dirs.Count);
        }

        public Direction Up() => GetDirection(0);
        public Direction UpRight() => GetDirection(1);
        public Direction DownRight() => GetDirection(2);
        public Direction Down() => GetDirection(3);
        public Direction DownLeft() => GetDirection(4);
        public Direction UpLeft() => GetDirection(5);
    }
    public class Dir3 : DirBase//represents hexagonal, still in progress!
    {

        private Dir3(Dir3 other) : base(other)
        {

        }
        public override DirBase Clone()
        {
            return new Dir3(this);
        }
        public Dir3()
        {
            Dirs = new CircularList<Direction>
            {
                //Grid Offset Note. while 0,.5,1 may feel more intuitive, 0,1,2 contributes greatly to even odd calculations making code much easier to write. i think this makes up for the lack of intuitiveness
                new Direction(){ SequenceSize = 3, Angle = 0, SequencePos = 0, TransformOffset = new Vector3(0, 1), GridOffset = new Vector3Int(0,2,0), Distance = 1 },
                new Direction(){ SequenceSize = 3, Angle = 120, SequencePos = 1, TransformOffset = new Vector3(Mathf.Sqrt(2), -.5f), GridOffset = new Vector3Int(2,-1,0), Distance = 1 },
                new Direction(){ SequenceSize = 3, Angle = 240, SequencePos = 2, TransformOffset = new Vector3(-Mathf.Sqrt(2), -.5f), GridOffset = new Vector3Int(-2,-1,0), Distance = 1 },
            };
        }

        public Direction Up() => GetDirection(0);
        public Direction DownRight() => GetDirection(1);
        public Direction DownLeft() => GetDirection(2);
    }
}
