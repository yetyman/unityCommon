using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Animation
{
    public class AnimationC<TAssign>
    {
        public TAssign From;
        public TAssign To;
        public float? Time = null;
        public EasingCurves? EasingCurve = null;
        public EasingPatterns? EasingPattern = null;
    }
    public class AnimationS<TAssign> : AnimationC<TAssign> where TAssign:struct
    {
        public new TAssign? From;
    }


}
