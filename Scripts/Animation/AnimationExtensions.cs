using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public static class AnimationExtension
    {
        public static AnimationInstanceVec3<GameObject> MoveTo(this GameObject target, Vector3 to, float timeSpanSeconds = 1, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In, AnimationInstanceVec3<GameObject> cachedInstance = null)
        {
            if (cachedInstance == null)
            {
                Action<GameObject, Vector3> setter = (GameObject, Vector3) => { GameObject.transform.localPosition = Vector3; };
                Func<GameObject, Vector3> getter = (GameObject) => { return GameObject.transform.localPosition; };
                cachedInstance = new AnimationInstanceVec3<GameObject>(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern);
            }
            if (IsAnimated(cachedInstance.GetSummary()))
                AnimationsController.CancelAnimation(cachedInstance);
            cachedInstance.Start(to, timeSpanSeconds, easingCurve, easingPattern);
            return cachedInstance;
        }

        public static AnimationInstanceVec3<GameObject> ScaleTo(this GameObject target, Vector3 to, float timeSpanSeconds = 1, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In, AnimationInstanceVec3<GameObject> cachedInstance = null)
        {
            if (cachedInstance == null)
            {
                Action<GameObject, Vector3> setter = (GameObject, Vector3) => { GameObject.transform.localScale = Vector3; };
                Func<GameObject, Vector3> getter = (GameObject) => { return GameObject.transform.localScale; };
                cachedInstance = new AnimationInstanceVec3<GameObject>(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern);
            }
            if (IsAnimated(cachedInstance.GetSummary()))
                AnimationsController.CancelAnimation(cachedInstance);
            cachedInstance.Start(to, timeSpanSeconds, easingCurve, easingPattern);
            return cachedInstance;
        }

        public static AnimationInstanceQuat<GameObject> RotateTo(this GameObject target, Quaternion to, float timeSpanSeconds = 1, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In, AnimationInstanceQuat<GameObject> cachedInstance = null)
        {
            if (cachedInstance == null)
            {
                Action<GameObject, Quaternion> setter = (GameObject, Quaternion) => { GameObject.transform.localRotation = Quaternion; };
                Func<GameObject, Quaternion> getter = (GameObject) => { return GameObject.transform.localRotation; };
                cachedInstance = new AnimationInstanceQuat<GameObject>(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern);
            }
            if (IsAnimated(cachedInstance.GetSummary()))
                AnimationsController.CancelAnimation(cachedInstance);
            cachedInstance.Start(to, timeSpanSeconds, easingCurve, easingPattern);
            return cachedInstance;
        }

        public static bool HasAnimatedValues(this GameObject target)
        {
            return AnimationsController.HasAnimatedValues(target);
        }
        public static bool IsAnimated(this AnimationSummary animation)
        {
            return AnimationsController.IsAnimated(animation);
        }
        public static void AnimateBounds(this RectTransform rt, float moveTime, bool cancel = false, bool append = true, bool chain = false, float? leftpad = null, float? rightpad = null, float? toppad = null, float? bottompad = null, float? leftanchor = null, float? rightanchor = null, float? topanchor = null, float? bottomanchor = null, float? fromleftpad = null, float? fromrightpad = null, float? fromtoppad = null, float? frombottompad = null, float? fromleftanchor = null, float? fromrightanchor = null, float? fromtopanchor = null, float? frombottomanchor = null, EasingCurves? curve = null, EasingPatterns? pattern = null)
        {

            //need to check the animation conroller to see if any of these have to values already. if they do then we need to continue with those to values...
            var aMax = GetAnchorMax(rt);
            var aMin = GetAnchorMin(rt);
            var oMax = GetOffsetMax(rt);
            var oMin = GetOffsetMin(rt);

            var a = new AnimationInstanceVec2<RectTransform>(rt, SetAnchorMax, GetAnchorMax, moveTime, curve.Value, pattern.Value);
            var b = new AnimationInstanceVec2<RectTransform>(rt, SetAnchorMin, GetAnchorMin, moveTime, curve.Value, pattern.Value);
            var c = new AnimationInstanceVec2<RectTransform>(rt, SetOffsetMax, GetOffsetMax, moveTime, curve.Value, pattern.Value);
            var d = new AnimationInstanceVec2<RectTransform>(rt, SetOffsetMin, GetOffsetMin, moveTime, curve.Value, pattern.Value);
            a = (AnimationInstanceVec2 < RectTransform >) AnimationInstance.FindAnimation(a.GetSummary());
            b = (AnimationInstanceVec2 < RectTransform >) AnimationInstance.FindAnimation(a.GetSummary());
            c = (AnimationInstanceVec2 < RectTransform >) AnimationInstance.FindAnimation(a.GetSummary());
            d = (AnimationInstanceVec2 < RectTransform >) AnimationInstance.FindAnimation(a.GetSummary());

            if (append) 
            {
                //if a value is already being animated and we want to maintain that animation and add this one(say a top and a right animation) we should check for it and use previous animated values as a default if it isnt being animated to something else now.
                if (AnimationsController.IsAnimated(a.GetSummary()))
                    aMax = a.To;

                if (AnimationsController.IsAnimated(b.GetSummary()))
                    aMin = b.To;

                if (AnimationsController.IsAnimated(c.GetSummary()))
                    oMax = c.To;

                if (AnimationsController.IsAnimated(d.GetSummary()))
                    oMin = d.To;
            }

            //may need to adjust these incase it becomes an issue like the above.
            var fromaMax = GetAnchorMax(rt);
            var fromaMin = GetAnchorMin(rt);
            var fromoMax = GetOffsetMax(rt);
            var fromoMin = GetOffsetMin(rt);
            oMax.x = -rightpad ?? oMax.x;
            oMin.x = leftpad ?? oMin.x;
            oMax.y = -toppad ?? oMax.y;
            oMin.y = bottompad ?? oMin.y;
            aMax.x = rightanchor ?? aMax.x;
            aMin.x = leftanchor ?? aMin.x;
            aMax.y = topanchor ?? aMax.y;
            aMin.y = bottomanchor ?? aMin.y;

            fromoMax.x = -fromrightpad ?? fromoMax.x;
            fromoMin.x = fromleftpad ?? fromoMin.x;
            fromoMax.y = -fromtoppad ?? fromoMax.y;
            fromoMin.y = frombottompad ?? fromoMin.y;
            fromaMax.x = fromrightanchor ?? fromaMax.x;
            fromaMin.x = fromleftanchor ?? fromaMin.x;
            fromaMax.y = fromtopanchor ?? fromaMax.y;
            fromaMin.y = frombottomanchor ?? fromaMin.y;


            if (!chain)
            {
                if(cancel)
                    AnimationsController.CancelAnimations(rt);

                if ((rightpad ?? toppad) != null)
                {
                    c.CancelImmediately();
                    c.Start(oMax, fromoMax);
                }
                if ((leftpad ?? bottompad) != null)
                {
                    d.CancelImmediately();
                    d.Start(oMin, fromoMin);
                }
                if ((rightanchor ?? topanchor) != null)
                {
                    a.CancelImmediately();
                    a.Start(aMax, fromaMax);
                }
                if ((leftanchor ?? bottomanchor) != null)
                {
                    b.CancelImmediately();
                    b.Start(aMin, fromaMin);
                }
            }
            else
            {
                if ((rightpad ?? toppad) != null) c.Chain(oMax);
                if ((leftpad ?? bottompad) != null) d.Chain(oMin);
                if ((rightanchor ?? topanchor) != null) a.Chain(aMax);
                if ((leftanchor ?? bottomanchor) != null) b.Chain(aMin);
            }
        }
        private static void SetAnchorMax(RectTransform target, Vector2 assignedValue) { target.anchorMax = assignedValue; }
        private static Vector2 GetAnchorMax(RectTransform target) { return target.anchorMax; }
        private static void SetAnchorMin(RectTransform target, Vector2 assignedValue) { target.anchorMin = assignedValue; }
        private static Vector2 GetAnchorMin(RectTransform target) { return target.anchorMin; }

        private static void SetOffsetMax(RectTransform target, Vector2 assignedValue) { target.offsetMax = assignedValue; }
        private static Vector2 GetOffsetMax(RectTransform target) { return target.offsetMax; }
        private static void SetOffsetMin(RectTransform target, Vector2 assignedValue) { target.offsetMin = assignedValue; }
        private static Vector2 GetOffsetMin(RectTransform target) { return target.offsetMin; }
    }
}
