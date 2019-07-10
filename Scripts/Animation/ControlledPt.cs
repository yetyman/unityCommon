using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    //represents a single point of a bezier curve. one point and two anchors.
    public class ControlledPt
    {
        public Vector2 Anchor;
        public Vector2? LeftControl = null;
        public Vector2? RightControl = null;
    }
}
