using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public delegate void MilestoneEventHandler(IPlayable instance);
    public abstract class Playable : IPlayable
    {
        public MonoBehaviour Parent { get; set; }
        protected Coroutine RunningThis = null;

        public object TimeSpanLock = new object();
        protected float? OriginalTimeSpanSeconds = 1;
        protected float? timeSpanSeconds = 1, StartTime = 0;
        public bool IsCancelled { get; private set; }

        public float? TimeSpanned
        { get { return (StartTime == null) ? null : Time.fixedTime - StartTime; } set { } }
        public float? EndTime
        { get { return (StartTime == null) ? null : StartTime + TimeSpanSeconds; } set { } }
        public float? TimeRemaining
        { get { return (StartTime == null) ? null : TimeSpanSeconds - TimeSpanned; } set { } }
        public float? PercentageCompleted
        { get { return (StartTime == null) ? null : TimeSpanned / TimeSpanSeconds; } set { } }
        public float? TimeSpanSeconds
        { get { return timeSpanSeconds; } }

        public event MilestoneEventHandler TimeSpanChanged;
        public event MilestoneEventHandler Created;
        public event MilestoneEventHandler Beginning;
        public event MilestoneEventHandler Ended; 
        public event MilestoneEventHandler Cancelled; 
        public event MilestoneEventHandler Undone;

        protected bool BeginningHasExternalListeners => Beginning.GetInvocationList().Count() > 0;
        protected bool EndedHasExternalListeners => (Beginning?.GetInvocationList()?.Count() ?? 0) > 1;
        protected void InvokeEnded()
        {
            Ended(this);
        }
        public void Start()
        {
            if (CanStart())
            {
                ResetTime();
                StartThis();

                if (Beginning != null)
                    AsyncDelegateInvoke(Beginning);
                InvokeEnd(TimeRemaining.Value);
            }
        }

        protected abstract void StartThis();
        protected abstract bool CanStart();

        public void Create(float timeSpanInSeconds)
        {
            SetTimeSpan(timeSpanInSeconds);


            if (CanCreate())
            {
                CreateThis();
                if (Created != null)
                    AsyncDelegateInvoke(Created);
            }
        }
        protected abstract void CreateThis();
        protected abstract bool CanCreate();

        public void SetTimeSpan(float? timeSpanInSeconds)
        {
            try
            {
                if (timeSpanInSeconds.HasValue)
                {
                    if (RunningThis != null)
                        throw new Exception("You cannot change the timespan while this playing");

                    lock (TimeSpanLock)
                    {
                        OriginalTimeSpanSeconds = timeSpanInSeconds;
                        this.timeSpanSeconds = timeSpanInSeconds;
                    }

                    if (TimeSpanChanged != null)
                        AsyncDelegateInvoke(TimeSpanChanged);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// Finish Journey, go immediately to the end.
        /// </summary>
        public void FinishImmediately()
        {
            if (CanFinishEarly())
            {
                FinishThisEarly();
                lock (TimeSpanLock)
                {
                    timeSpanSeconds = TimeSpanned;
                }
                End();
                //RefreshInvokeEnd();
            }
        }
        protected abstract void FinishThisEarly();
        protected abstract bool CanFinishEarly();

        public void ResetTime()
        {
            StartTime = Time.fixedTime;
            IsCancelled = false;
            lock (TimeSpanLock)
            {
                timeSpanSeconds = OriginalTimeSpanSeconds;
            }
        }
        /// <summary>
        /// cancel the journey, stop dead in its tracks, do not pass go. should still fire the ending event.
        /// </summary>
        public void CancelImmediately()
        {

            if (CanCancel())
            {
                Stop();
                CancelThis();
                IsCancelled = true;
                if (Cancelled != null)
                    AsyncDelegateInvoke(Cancelled);
            }
        }
        protected abstract void CancelThis();
        protected abstract bool CanCancel();

        public void UndoImmediately()
        {
            if (CanUndo())
            {
                Stop();
                UndoThis();
                if (Undone != null)
                    AsyncDelegateInvoke(Undone);
            }
        }
        protected abstract void UndoThis();
        protected abstract bool CanUndo();

        protected IEnumerator AsyncDelegateInvoker(MilestoneEventHandler milestoneEventHandler)
        {
            yield return new WaitForSeconds(0);
            milestoneEventHandler.Invoke(this);
        }
        protected void AsyncDelegateInvoke(MilestoneEventHandler milestoneEventHandler)
        {
            Parent.StartCoroutine(AsyncDelegateInvoker(milestoneEventHandler));
        }

        protected void RefreshInvokeEnd()
        {
            if (RunningThis != null)
            {
                Stop();
                InvokeEnd(TimeRemaining.Value);
            }
        }
        private void InvokeEnd(float time)
        {
            //invoke EndTransition

            RunningThis = Parent.StartCoroutine(End());
        }
        private IEnumerator End()
        {
            while (TimeRemaining > 0)
                yield return new WaitForSeconds(TimeRemaining.Value);

            if (TimeRemaining <= 0)
            {
                RunningThis = null;

                EndThis();
                if (Ended != null)
                    AsyncDelegateInvoke(Ended);
            }
        }

        protected abstract void EndThis();
        protected void Stop()
        {
            if (RunningThis != null)
                Parent.StopCoroutine(RunningThis);//TODO: i don't necessarily like that every playable has its own coroutine. we may want to add an option to manage the stopping ourselve for batche playables like animaiton.... we'll have to find out what the overhead is actually like.
            RunningThis = null;
        }

        protected void EndRepeat()
        {
            Ended -= RepeatAction;
        }
        protected void Repeat()
        {
            if (Ended.GetInvocationList().FirstOrDefault(x => x == (MulticastDelegate)new MilestoneEventHandler(RepeatAction)) == null)
            {
                if (CanRepeat())
                {
                    RepeatThis();
                    Ended += RepeatAction;
                }
            }
        }

        private void RepeatAction(IPlayable x)
        {
            x.Start();
        }

        protected abstract void RepeatThis();
        protected abstract bool CanRepeat();
    }
}
