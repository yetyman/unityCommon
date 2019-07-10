using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.StateMachines
{
    public class Transition : Pausable, ITransition
    {
        private TransitionCheckEventHandler transitionCheckEvent;
        public TransitionCheckEventHandler TransitionCheckEvent {
            get {
                return (transitionCheckEvent == null && DefaultTransitions) ? SM.DefaultTransitionCheckEvent : transitionCheckEvent;
            }
            set {
                transitionCheckEvent = value;
            }
        }
        public IStateMachine sm;
        public bool EndedHasListeners => EndedHasExternalListeners;
        public bool DefaultTransitions { get; set; }
        private string from;
        private string to;

        public Direction Direction { get; set; }
        public IStateMachine SM { get { return sm; } }
        public string From { get { return from; } }
        public string To { get { return to; } }

        public Transition(IStateMachine sm, string from, string to, Direction transitionDirection, float timeSpanSeconds = 0) : base()
        {
            DefaultTransitions = true;
            this.sm = sm;
            this.Parent = sm.Parent;
            this.from = from;
            this.to = to;
            this.Direction = transitionDirection;
            SetTimeSpan(timeSpanSeconds);
        }

        protected override bool CanStart()
        {
            var transitionCheck = TransitionCheckEvent;
            if (transitionCheck != null)
            {
                return transitionCheck.Invoke(this);
            }
            return true;
        }


        protected override void StartThis()
        {
            if (!BeginningHasExternalListeners && SM.DefaultTransitionBeginning != null)
                AsyncDelegateInvoke(SM.DefaultTransitionBeginning);
        }
        public void SetDoesDefault(bool doesDefault)
        {
            DefaultTransitions = doesDefault;
        }

        protected override void EndThis()
        {
            //ended will always have one invocation internal to statemachine. check for any others.
            if (!EndedHasExternalListeners && SM.DefaultTransitionEnding != null)
                AsyncDelegateInvoke(SM.DefaultTransitionEnding);

        }
        protected override bool CanSlow(float scale)
        {
            return true;
        }

        protected override bool CanCreate()
        {
            return true;
        }

        protected override bool CanFinishEarly()
        {
            return true;
        }

        protected override bool CanCancel()
        {
            return true;
        }

        protected override bool CanUndo()
        {
            return true;
        }



        protected override bool CanRepeat()
        {
            return false;
        }

        protected override void SlowThis(float scale)
        {
        }

        protected override void CreateThis()
        {
        }

        protected override void FinishThisEarly()
        {
        }

        protected override void CancelThis()
        {
        }

        protected override void UndoThis()
        {
        }

        protected override void RepeatThis()
        {
        }


        protected override void PauseThis()
        {
        }

        protected override bool CanPause()
        {
            return true;
        }

        protected override void ResumeThis()
        {
        }

        protected override bool CanResume()
        {
            return true;
        }

    }
}
