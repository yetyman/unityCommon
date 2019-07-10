using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.GenericClasses;
using System;
using System.Linq;

namespace Assets.Scripts.Animation
{



    public class AnimationsController : MonoBehaviour
    {
       
        public static Coroutine Animator = null;
        public bool Animating = false;
        private static Dictionary<AnimationSummary, AnimationInstance> ActiveAnimations = new Dictionary<AnimationSummary, AnimationInstance>();
        public static AnimationsController Instance;
        public float FPS { get { return 1 / SPF; } }
        public bool BenchmarkingAtStart = true;
        private UITextElement Benchmark;
        private Dictionary<string, string> Stats = new Dictionary<string, string>();
        public float SPF { get; } = .016666f;
        
        // Use this for initialization
        void Start()
        {
            Instance = this;
            if (BenchmarkingAtStart)
            {
                StartCoroutine(GetBenchmark());
            }
        }
        public IEnumerator GetBenchmark()
        {
            yield return new WaitUntil(() => GameSceneContext.UIElements.ContainsKey("BenchmarkUIElement"));
            Benchmark = GameSceneContext.UIElements["BenchmarkUIElement"];
        }
        // Update is called once per frame
        void Update()
        {

        }
        public static bool GlobalPause
        {
            get
            {
                return Pausable.GlobalPause;
            }

        }

        public static void PauseGlobal()
        {
            Pausable.PauseGlobal();
        }
        public static void ResumeGlobal()
        {
            Pausable.ResumeGlobal();
        }

        public static bool IsAnimated(AnimationInstance instance)
        {
            return IsAnimated(instance?.GetSummary());
        }
        public static bool IsAnimated(AnimationSummary summary)
        {
            return summary == null ? false : ActiveAnimations.ContainsKey(summary);
        }

        public static bool HasAnimatedValues(object target)
        {
            return ActiveAnimations.FirstOrDefault(x => x.Key.target == target).Key != null;
        }
        public static void BeginAnimation(AnimationInstance instance)
        {
            Instance.BeginAnimationInst(instance);
        }

        private void BeginAnimationInst(AnimationInstance instance)
        {
            ActiveAnimations.Add(instance.GetSummary(), instance);
            //if (Animator == null)
            //  Animator = StartCoroutine(AnimationsController.Instance.Animate());
            if(!Animating)
                InvokeRepeating("Animate", 0, SPF);
            Animating = true;
        }

        public static void EndAnimation(AnimationInstance instance)
        {
            RemoveAnimation(instance);
            instance.FinishImmediately();
        }
        public static void RemoveAnimation(AnimationInstance instance)
        {
            ActiveAnimations.Remove(instance.GetSummary());
        }

        public static void CancelAnimations(object target)
        {
            List<AnimationInstance> animations = ActiveAnimations.Where((kv) => kv.Key.target == target).Select(kv => kv.Value).ToList();
            foreach (AnimationInstance ani in animations)
                CancelAnimation(ani);
        }
        public static void CancelAnimation(AnimationInstance instance)
        {
            RemoveAnimation(instance);
            instance.CancelImmediately();
        }
        //public IEnumerator Animate()
        public void Animate()
        {
            //yield return new WaitForEndOfFrame();
            Queue<AnimationSummary> removeQueue = new Queue<AnimationSummary>();
            //while(!GlobalPause && Animations.Count != 0)
            if (!GlobalPause && ActiveAnimations.Count != 0)
            {
                if(Benchmark!=null)
                    UnityEngine.Profiling.Profiler.BeginSample("AnimationLoop");
                int Anicount = 0;
                foreach (var animation in ActiveAnimations)
                {
                    if (!animation.Value.IsPaused)
                    {
                        if (!animation.Value.Animate())
                        {
                            //using an async invocation so that expensive uses of the event can't screw up the animation queue.
                            //can't use begininvoke. it invokes off of the main thread. gotta use coroutine here. that's fine. we don't care about latency on the ended event, critical systems shouldn't be based around animations to begin with...
                            if (animation.Value.EndedHasListeners && animation.Value.IsCancelled)
                                StartCoroutine(AnimationEndedEvent(animation.Value));
                                //animation.Value.Ended.BeginInvoke(animation.Value, null, null);

                            removeQueue.Enqueue(animation.Key);
                        }
                        else Anicount++;
                    }
                }
                Stats.SafeSet("ApL", Anicount+"");

                if (removeQueue.Count != 0)
                    foreach (var animation in removeQueue)
                        ActiveAnimations.Remove(animation);

                if (ActiveAnimations.Count == 0)
                {
                    //lock (Animator)
                    //{
                    //    StopCoroutine(Animator);
                    //    Animator = null;
                    //}

                    CancelInvoke();
                    Animating = false;
                }

                Benchmark?.UpdateText(Stats);

                if (Benchmark != null)
                    UnityEngine.Profiling.Profiler.EndSample();

                //yield return new WaitForSeconds(SPF);
            }
            //Animator = null;
            
        }

        private IEnumerator AnimationEndedEvent(AnimationInstance ani)
        {
            yield return null;
            ani.InvokeEnded();
        }

    }

}
