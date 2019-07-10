using Assets.Scripts.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;

namespace Assets.GenericClasses
{
    public static class Vector2Extensions
    {
        public static List<ControlledPt> InterpolateBezier(this IEnumerable<Vector2> pts)
        {
            return InterpolateBezier(pts.Select(x => new ControlledPt() { Anchor = x }).ToList());
        }

        public static Vector2 Average(this IEnumerable<Vector2> target)
        {
            return new Vector2(
                target.Average(x => x.x),
                target.Average(x => x.y)
                );
        }
        public static List<ControlledPt> InterpolateBezier(this List<ControlledPt> anchors)
        {
            anchors = anchors.Where(x => x != null).ToList();
            for (int i = 0; i < anchors.Count; i++)
            {
                var leftAnchor = i > 0 ? anchors[i - 1] : anchors[i];
                var rightAnchor = i + 1 < anchors.Count ? anchors[i + 1] : anchors[i];
                Vector2 leftControl, rightControl;

                if (leftAnchor == anchors[i])
                    leftControl = anchors[i].Anchor;
                if (rightAnchor == anchors[i])
                    rightControl = anchors[i].Anchor;

                if (rightAnchor == leftAnchor && rightAnchor == anchors[i])
                    continue;

                var leftD = Vector2.Distance(anchors[i].Anchor, leftAnchor.Anchor);
                var rightD = Vector2.Distance(anchors[i].Anchor, rightAnchor.Anchor);

                var leftR = leftD / (leftD + rightD);
                var rightR = rightD / (leftD + rightD);

                var h = rightAnchor.Anchor.x - leftAnchor.Anchor.x;
                var v = rightAnchor.Anchor.y - leftAnchor.Anchor.y;

                leftControl.x = anchors[i].Anchor.x + leftR * h;
                leftControl.y = anchors[i].Anchor.y + leftR * v;

                rightControl.x = anchors[i].Anchor.x + rightR * h;
                rightControl.y = anchors[i].Anchor.y + rightR * v;

                anchors[i].LeftControl = anchors[i].LeftControl ?? leftControl;
                anchors[i].RightControl = anchors[i].RightControl ?? rightControl;
            }

            return anchors;
        }

        public static Vector2[] RotateAround(this Vector2[] list, Vector2 center, float angle, bool copy = false)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            if (copy)
                list = list.Select(v => new Vector2(v.x, v.y)).ToArray();

            for (int i = 0; i < list.Count(); i++)
                list[i] = ((Vector2)(rotation * (list[i] - center))) + center;

            return list;
        }
        public static List<Vector2> RotateAround(this List<Vector2> list, Vector2 center, float angle, bool copy = false)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            if (copy)
                list = list.Select(v => new Vector2(v.x, v.y)).ToList();

            for (int i = 0; i < list.Count(); i++)
                list[i] = ((Vector2)(rotation * (list[i] - center))) + center;

            return list;
        }

    }
}
