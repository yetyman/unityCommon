using Assets.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public enum EasingPatterns
    {
        In,
        Out,
        InOut,
        OutIn
    }
    public enum EasingCurves
    {
        //simple linear tweening - no easing, no acceleration
        Linear,
        Quad,
        Cubic,
        Quart,
        Quint,
        Sine,
        Expo,
        Circ,
        Bounce,
        Back,
        CustomBezier,
        CustomFunc
    }
    public abstract class AnimationInstance : Pausable
    {
        private EasingCurves? easingCurve = EasingCurves.Linear;
        private EasingPatterns? easingPattern = EasingPatterns.In;
        protected AnimationSummary summary;
        public List<ControlledPt> CustomEaseCurvePts = new List<ControlledPt>();
        public List<Vector2> SplineEaseCurvePts
        {
            get
            {
                return CustomEaseCurvePts.Select(x => x.Anchor).ToList();
            }
            private set { }
        }

        /// <summary>
        /// Return a value [0,1], you will receive a value [0,1]
        /// </summary>
        public Func<float, float> CustomEase = null;

        public EasingCurves? EasingCurve
        { get { return easingCurve; } }

        public EasingPatterns? EasingPattern
        { get { return easingPattern; } }

        public bool EndedHasListeners => EndedHasExternalListeners;
        public void InvokeEnded()
        {
            InvokeEnded();
        }
        protected override bool CanCreate()
        {
            return true;
        }
        protected override void CreateThis()
        {
            Parent = AnimationsController.Instance;
        }
        protected override bool CanSlow(float scale)
        {
            return true;
        }
        protected override void SlowThis(float scale)
        {
        }
        protected override bool CanPause()
        {
            return true;
        }
        protected override void PauseThis()
        {
        }
        protected override bool CanResume()
        {
            return true;
        }
        protected override void ResumeThis()
        {
        }
        public float SingleTween(float from, float to)
        {
            //This is definitely not an item for immediate consideration, but in the future you could pretty easily preprocess the calculations to make sure that the data is ready to be assigned as soon as the frame comes around.
            float weight = SingleTween(PercentageCompleted.Value);

            return from * (1 - weight) + to * weight;
        }
        public void SetEase(EasingCurves easingCurve, EasingPatterns easingPattern)
        {
            this.easingCurve = easingCurve;
            this.easingPattern = easingPattern;
        }

        //can't wait to constantly come back to this function trying to figure out whats wrong and adjust.
        public float SingleTween(float t)
        {
            return SingleTween(t, EasingCurve.Value, EasingPattern.Value);
        }
        public float SingleTween(float t, EasingCurves easingCurve, EasingPatterns easingPattern)
        {
            //TODO: add an amplitude option for how extreme the curve is. cannot be generically controlled you idiot.
            //this is where we will have all our different kinds of tween patterns. you should implement a bounce immediately to make sure we have everything we need.
            switch (easingPattern)
            {
                case EasingPatterns.In:
                    switch (easingCurve)
                    {
                        default:
                        case EasingCurves.Linear:
                            break;
                        case EasingCurves.Quad:
                            t = t * t;
                            break;
                        case EasingCurves.Cubic:
                            t = t * t * t;
                            break;
                        case EasingCurves.Quart:
                            t = t * t * t * t;
                            break;
                        case EasingCurves.Quint:
                            t = t * t * t * t * t;
                            break;
                        case EasingCurves.Sine:
                            t = -Mathf.Cos(t * (Mathf.PI / 2)) + 1;
                            break;
                        case EasingCurves.Expo:
                            t = Mathf.Pow(2, 10 * (t - 1));
                            break;
                        case EasingCurves.Circ:
                            t = -(Mathf.Sqrt(1 - t * t) - 1);
                            break;
                        case EasingCurves.Back:
                            t = t * t * t - t * Mathf.Sin(t * Mathf.PI);///3;
                            break;
                        case EasingCurves.Bounce:
                            //TODO:FILL THIS IN
                            break;
                        case EasingCurves.CustomBezier:
                            t = InterpretBezier(t);
                            break;
                        case EasingCurves.CustomFunc:
                            t = CustomEase.Invoke(t);
                            break;
                    }
                    break;
                case EasingPatterns.Out:
                    t = -SingleTween(1 - t, easingCurve, EasingPatterns.In) + 1;

                    break;
                case EasingPatterns.InOut:
                    t *= 2;
                    if (t < 1)
                        t = SingleTween(t, easingCurve, EasingPatterns.In);
                    else
                        t = SingleTween(t - 1, easingCurve, EasingPatterns.Out) + 1f;
                    t *= .5f;
                    break;
                case EasingPatterns.OutIn:
                    t *= 2;
                    if (t < 1)
                        t = SingleTween(t, easingCurve, EasingPatterns.Out);
                    else
                        t = SingleTween(t - 1, easingCurve, EasingPatterns.In) + 1f;
                    t *= .5f;
                    break;
            }
            return t;
        }

        private float InterpretBezier(float t)
        {
            if (CustomEaseCurvePts == null || CustomEaseCurvePts.Count < 2)
            {
                Console.Write(new Exception("You cannot user a custom bezier tween without setting bezier pts..."));
                return t;
            }

            float segmentPercent = t * (CustomEaseCurvePts.Count - 1);
            int index = Mathf.RoundToInt(segmentPercent + .500001f);
            index = index < CustomEaseCurvePts.Count ? index : CustomEaseCurvePts.Count - 1;

            var a = CustomEaseCurvePts[index];
            var b = CustomEaseCurvePts[index - 1];

            return InterpretBezier(a, b, 1 - index + segmentPercent);

        }

        private float InterpretBezier(ControlledPt a, ControlledPt b, float t)
        {
            //only grab y component
            var o = 1 - t;
            return o * o * o * a.Anchor.y
               + 3 * o * o * t * a.RightControl.Value.y
               + 3 * o * t * t * b.LeftControl.Value.y
                   + t * t * t * b.Anchor.y;
        }

        public void SetCustomCurve(List<Vector2> pts, bool interpolateBezier = true)
        {
            if (interpolateBezier)
                CustomEaseCurvePts = pts.InterpolateBezier();

            else CustomEaseCurvePts = pts.Select(x => new ControlledPt() { Anchor = x }).ToList();
        }

        public void SetCustomCurve(List<ControlledPt> pts, bool interpolateBezier = true)
        {
            if (interpolateBezier)
                CustomEaseCurvePts = pts.InterpolateBezier();

            else CustomEaseCurvePts = pts.Where(x => x != null).ToList();
        }


        protected static Dictionary<AnimationSummary, AnimationInstance> Animations = new Dictionary<AnimationSummary, AnimationInstance>();

        public static AnimationInstance FindAnimation(AnimationSummary summary)
        {
            if (!Animations.ContainsKey(summary)) return null;
            return Animations[summary];
        }
        public abstract bool Animate();
        public abstract AnimationSummary GetSummary();
    }
    public abstract class AnimationInstance<TTarget, TAssign> : AnimationInstance
    {
        public TTarget Target;


        protected Action<TTarget, TAssign> Setter;
        protected Func<TTarget, TAssign> Getter;


        protected AnimationC<TAssign> ReadiedAnimation = null;
        protected Queue<AnimationC<TAssign>> QueuedAnimations = new Queue<AnimationC<TAssign>>();

        protected AnimationC<TAssign>[] LoopedAnimations;

        public void Loop()
        {
            LoopedAnimations = new AnimationC<TAssign>[QueuedAnimations.Count()];
            QueuedAnimations.CopyTo(LoopedAnimations, 0);
        }
        public void EndLoop()
        {
            LoopedAnimations = null;
        }
        public override AnimationSummary GetSummary()
        {
            if (summary == null)
                summary = new AnimationSummary()
                {
                    target = Target,
                    setter = Setter.Method.Name,
                    getter = Getter.Method.Name,
                };
            return summary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">unfortunately, taken on trust. this is the larger object your Setter is related to.</param>
        /// <param name="setter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="PlayOnInitialization"></param>
        public AnimationInstance(TTarget target, Action<TTarget, TAssign> setter, Func<TTarget, TAssign> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In)
        {

            Target = target;
            Setter = setter;
            Getter = getter;

            //TODO:next time you consider reducing the parameter to a single lambda rather than getter and setter, consider this, if the solution is fast enough then its okay to store a list of short lambdas to make the ultimate overall call.
            //https://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo/16222886
            //remember that single level doesnt really work well since we need to be able to run separate instances of the same kind of animation for multiple of a give kind of object without interefering with eachother.


            SetEase(easingCurve, easingPattern);
            Create(timeSpanSeconds);
        }
        protected TAssign to;
        protected TAssign from;

        public TAssign From
        {
            get
            {
                return from;
            }
        }

        public TAssign To
        {
            get
            {
                return to;
            }
        }

        public abstract void Start(TAssign to, TAssign from, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null);
        public abstract void Start(TAssign to, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null);


        public abstract TAssign Tween();


        public override bool Animate()
        {
            lock (TimeSpanLock)
            {
                if (!IsCancelled)
                {
                    var timeSpanned = TimeSpanned;
                    if (timeSpanned < TimeSpanSeconds)
                        Setter(Target, Tween());

                    else
                    {
                        Setter(Target, To);
                        return false;
                    }
                    return true;
                }
                else return false;
            }
        }

        protected override bool CanCancel()
        {
            return true;
        }




        protected override bool CanStart()
        {

            var instance = GetActiveInstance();
            if (this != instance)
                Debug.LogWarning("Please cache animation instances used repeatedly");


            if (!TimeSpanSeconds.HasValue)
                throw new Exception("TimeSpan must be set before an animation can be played");

            return !AnimationsController.IsAnimated(GetSummary());
        }


        protected override void UndoThis()
        {
            AnimationsController.RemoveAnimation(this);

            Setter(Target, From);
        }
        protected override bool CanUndo()
        {
            return true;
        }

        protected override bool CanFinishEarly()
        {
            return true;
        }
        protected override void FinishThisEarly()
        {
            AnimationsController.RemoveAnimation(this);
            lock (TimeSpanLock)
            {
                timeSpanSeconds = TimeSpanned;
            }
            Animate();

        }
        private void StartNextQueuedAnimation()
        {
            var next = QueuedAnimations.Dequeue();
            Start(next.To, next.Time, next.EasingCurve, next.EasingPattern);
        }
        protected override void EndThis()
        {
            AnimationsController.RemoveAnimation(this);
            Animate();
            if (LoopedAnimations != null && QueuedAnimations.Count() == 0)
            {
                foreach (AnimationC<TAssign> animation in LoopedAnimations)
                    QueuedAnimations.Enqueue(animation);
            }
            if (QueuedAnimations.Count() > 0)
            {
                StartNextQueuedAnimation();
            }
        }

        protected override void RepeatThis()
        {
        }
        protected override bool CanRepeat()
        {
            return true;
        }
        protected override void CancelThis()
        {
            AnimationsController.RemoveAnimation(this);
            lock (TimeSpanLock)//not sure how unity deals with parallelism, but it does happen even if its all on one thread. not sure how that works. so lets leave this here for now.
            {
                QueuedAnimations.Clear();
            }
        }

        private AnimationInstance<TTarget, TAssign> GetActiveInstance()
        {
            if (!Animations.ContainsKey(GetSummary()))
                Animations.Add(GetSummary(), this);
            return (AnimationInstance<TTarget, TAssign>)Animations[GetSummary()];
        }
        public void ClearChain()
        {
            QueuedAnimations.Clear();
            EndLoop();
        }
        public void FinishChain()
        {
            var finalChain = QueuedAnimations.LastOrDefault();
            if (finalChain != null)
            {
                ClearChain();
                Start(finalChain.To);
                FinishImmediately();
            }
        }
        public void Chain(TAssign to, bool startImmediately = false, bool clearChain = false, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            if (clearChain)
                ClearChain();

            QueuedAnimations.Enqueue(new AnimationC<TAssign>()
            {
                To = to,
                Time = timeSpanSeconds,
                EasingCurve = easingCurve,
                EasingPattern = easingPattern
            });
            if (startImmediately && !AnimationsController.IsAnimated(GetSummary()))
            {
                StartNextQueuedAnimation();
            }

        }
        public bool HasChainedAnimation()
        {
            return QueuedAnimations.Count > 0;
        }
       
    }
    public abstract class AnimationInstanceS<TTarget, TAssign> : AnimationInstance<TTarget, TAssign>
        where TAssign : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">unfortunately, taken on trust. this is the larger object your Setter is related to.</param>
        /// <param name="setter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="PlayOnInitialization"></param>
        public AnimationInstanceS(TTarget target, Action<TTarget, TAssign> setter, Func<TTarget, TAssign> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In) 
            : base(target, setter, getter, timeSpanSeconds, easingCurve, easingPattern)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">if set to NULL, will use current value</param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="easingCurve"></param>
        /// <param name="easingPattern"></param>
        public void Start(TAssign to, TAssign? from, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            ReadiedAnimation = new AnimationS<TAssign>()
            {
                From = from,
                To = to,
                Time = timeSpanSeconds,
                EasingCurve = easingCurve,
                EasingPattern = easingPattern
            };

            Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">if set to NULL, will use current value</param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="easingCurve"></param>
        /// <param name="easingPattern"></param>
        public override void Start(TAssign to, TAssign from, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            Start(to, from, timeSpanSeconds, easingCurve, easingPattern);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">if set to NULL, will use current value</param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="easingCurve"></param>
        /// <param name="easingPattern"></param>
        public override void Start(TAssign to, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            Start(to, null, timeSpanSeconds, easingCurve, easingPattern);
        }

        protected override void StartThis()
        {
            SetTimeSpan(ReadiedAnimation.Time);

            var readiedAnimation = (AnimationS<TAssign>)ReadiedAnimation;
            this.from = readiedAnimation.From.HasValue ? readiedAnimation.From.Value : Getter(Target);

            this.to = ReadiedAnimation.To;

            SetEase((ReadiedAnimation.EasingCurve ?? EasingCurve).Value, (ReadiedAnimation.EasingPattern ?? EasingPattern).Value);

            AnimationsController.BeginAnimation(this);
        }
    }
    public abstract class AnimationInstanceC<TTarget, TAssign> : AnimationInstance<TTarget, TAssign>
        where TAssign : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">unfortunately, taken on trust. this is the larger object your Setter is related to.</param>
        /// <param name="setter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="PlayOnInitialization"></param>
        public AnimationInstanceC(TTarget target, Action<TTarget, TAssign> setter, Func<TTarget, TAssign> getter, float timeSpanSeconds, EasingCurves easingCurve = EasingCurves.Linear, EasingPatterns easingPattern = EasingPatterns.In)
        :base(target,setter,getter,timeSpanSeconds,easingCurve,easingPattern){
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">if set to NULL, will use current value</param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="easingCurve"></param>
        /// <param name="easingPattern"></param>
        public override void Start(TAssign to, TAssign from, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            ReadiedAnimation = new AnimationC<TAssign>()
            {
                From = from,
                To = to,
                Time = timeSpanSeconds,
                EasingCurve = easingCurve,
                EasingPattern = easingPattern
            };

            
            Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">if set to NULL, will use current value</param>
        /// <param name="to"></param>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="easingCurve"></param>
        /// <param name="easingPattern"></param>
        public override void Start(TAssign to, float? timeSpanSeconds = null, EasingCurves? easingCurve = null, EasingPatterns? easingPattern = null)
        {
            Start(to, null, timeSpanSeconds, easingCurve, easingPattern);
        }

        protected override void StartThis()
        {
            SetTimeSpan(ReadiedAnimation.Time);
            this.from = ReadiedAnimation.From ?? Getter(Target);

            this.to = ReadiedAnimation.To;

            SetEase((ReadiedAnimation.EasingCurve ?? EasingCurve).Value, (ReadiedAnimation.EasingPattern ?? EasingPattern).Value);

            AnimationsController.BeginAnimation(this);
        }
    }
}
