using Assets.Scripts.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{

    //TODO:Separate into Pausable:Playable
    public abstract class Pausable : Playable, IPausable
    {
        /// <summary>
        /// using multitons boiiiis
        /// </summary>
        public static HashSet<Pausable> AllOfEm = new HashSet<Pausable>(); 

        public static bool globalPause {get; set;}
        private PauseInstance PauseParameters = new PauseInstance();

        public MilestoneEventHandler Pausing { get; set; }
        public MilestoneEventHandler Slowing { get; set; }
        public MilestoneEventHandler Resuming { get; set; }

        public new void ClearEvents()
        {
            base.ClearEvents();
            Resuming = null;
            Pausing = null;
            Slowing = null;
        }
        public Pausable()
        {
            AllOfEm.Add(this);
            //TODO:you need to define how things will get removed from this multiton list. i recommend that it be handled with an abstract function since different implementations could have different times when they really end. could restart!
        }

        public void Slow(float scale)
        {
            if (CanSlow(scale))
            {
                var timeElapsed = TimeSpanned;
                StartTime = Time.fixedTime - timeElapsed * scale;
                timeSpanSeconds *= scale;
                SlowThis(scale);
                RefreshInvokeEnd();
                if (Slowing != null)
                    AsyncDelegateInvoke(Slowing);
            }
            //should cause percentage completed to be equal to what it already was.
        }
        protected abstract void SlowThis(float scale);
        protected abstract bool CanSlow(float scale);

        public bool IsPaused
        {
            get
            {
                return PauseParameters.Paused;
            }
            set { }
        }

        public void Create(float timeSpanInSeconds, bool playOnInitialization)
        {
            base.Create(timeSpanInSeconds);
            if (!playOnInitialization)
                Pause();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="global">This parameter is only intended for use by the Animation Controller. and will do nothing when called here.</param>
        /// <returns></returns>
        public bool Pause(bool global = false)
        {
            if (CanPause())
            {
                if (global && globalPause)
                {
                    PauseParameters.GlobalPause = true;
                }

                else if (!global)
                    PauseParameters.LocalPause = true;

                PauseThis();

                if (RunningThis != null)
                    Stop();

                if (Pausing != null)
                    AsyncDelegateInvoke(Pausing);

                else return false;
                return true;
            }
            else return false;
        }

        protected abstract void PauseThis();
        protected abstract bool CanPause();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="global">This parameter is only intended for use by the Animation Controller. and will do nothing when called here.</param>
        /// <returns>a value indicating if the animation is now playing.</returns>
        public bool Resume(bool global = false)
        {
            var timePaused = PauseParameters.TimePaused;
            if (CanResume())
            {
                if (global && PauseParameters.ResumeGlobal() || !global && PauseParameters.ResumeLocal())
                {
                    ResumeThis();
                    StartTime += timePaused;//TODO:I agree that this is weird. separate start time into and original start time and a MathStartTime that is private. that should fix the issue.

                    if (Pausing != null)
                        AsyncDelegateInvoke(Resuming);

                    RefreshInvokeEnd();
                }
            }
            return IsPaused;
        }

        protected abstract void ResumeThis();
        protected abstract bool CanResume();

        public static bool GlobalPause
        {
            get
            {
                return globalPause;
            }
        }



        public static void PauseGlobal()
        {
            globalPause = true;
            foreach (var instance in AllOfEm)
                instance.Pause(true);
        }
        public static void ResumeGlobal()
        {
            globalPause = false;
            foreach (var instance in AllOfEm)
                instance.Resume(true);
        }
    }
}
