using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    //TODO: fun project, you can totally add paths to your animations. just need a function for the path. probably a bezier curve with variable numbers of points. make it an optionally overridden function. FUN!

    public class LocationAnimator : AnimationInstanceVec3<GameObject>
    {
        public static void setter(GameObject go, Vector3 localPosition)
        {
            go.transform.localPosition = localPosition;
        }
        public static Vector3 getter(GameObject go)
        {
            return go.transform.localPosition;
        }
        public LocationAnimator(GameObject target, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In)
            : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
            
        {
            
        }
    }
    /// <summary>
    /// you should be able to cache multiple animation instances of the same object and property, you should only be able to run one of them at a time.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public class AnimationInstanceVec3<TTarget> : AnimationInstanceS<TTarget, Vector3>
    {
        public EasingCurves xCurve;
        public EasingCurves yCurve;
        public EasingCurves zCurve;
        public EasingPatterns xPattern;
        public EasingPatterns yPattern;
        public EasingPatterns zPattern;
        public AnimationInstanceVec3(TTarget target, Action<TTarget, Vector3> setter, Func<TTarget, Vector3> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
            xCurve = yCurve = zCurve = easingCurve;
            xPattern = yPattern = zPattern = easingPattern;
        }

        public override Vector3 Tween()
        {
            return new Vector3(
                SingleTween(From.x, To.x, xCurve, xPattern),
                SingleTween(From.y, To.y, yCurve, yPattern),
                SingleTween(From.z, To.z, zCurve, zPattern));
        }
    }
    public class AnimationInstanceVec2<TTarget> : AnimationInstanceS<TTarget, Vector2>
    {
        public AnimationInstanceVec2(TTarget target, Action<TTarget, Vector2> setter, Func<TTarget, Vector2> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
        }

        public override Vector2 Tween()
        {
            return new Vector2(
                SingleTween(From.x, To.x),
                SingleTween(From.y, To.y));
        }
    }
    public class AnimationInstanceQuat<TTarget> : AnimationInstanceS<TTarget, Quaternion>
    {
        public AnimationInstanceQuat(TTarget target, Action<TTarget, Quaternion> setter, Func<TTarget, Quaternion> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
        }

        public override Quaternion Tween()
        {
            return Quaternion.SlerpUnclamped(From, To, SingleTween(PercentageCompleted.Value));
        }
    }
    public class AnimationInstanceFloat<TTarget> : AnimationInstanceS<TTarget, float>
    {
        public AnimationInstanceFloat(TTarget target, Action<TTarget, float> setter, Func<TTarget, float> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
        }

        public override float Tween()
        {
            return SingleTween(From, To);
        }
    }
    public class AnimationInstanceString<TTarget> : AnimationInstanceC<TTarget, string>
    {
        public AnimationInstanceString(TTarget target, Action<TTarget, string> setter, Func<TTarget, string> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
        }

        public override string Tween()
        {
            var len = SingleTween(PercentageCompleted.Value);
            return To.Substring(0, Mathf.RoundToInt(len * To.Length));
        }
    }

}
