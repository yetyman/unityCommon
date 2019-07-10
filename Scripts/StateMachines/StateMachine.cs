using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.StateMachines
{
    public delegate bool TransitionCheckEventHandler(ITransition t);
    public class StateMachine<T> : StateMachine
    {
        public StateMachine(MonoBehaviour parent, List<string> stateHistory) : base(parent, stateHistory) { }

        protected override void SafeAddState(string name, object target = null)
        {
            if(!(target is T))
            {
                throw new Exception("You may only use objects of type T");
            }
            if (!States.ContainsKey(name))
            {
                if (target != null && target is T)
                    States.Add(name, new State<T>() { Name = name, Target = (T)target });
                else
                    States.Add(name, new State<T>() { Name = name });
            }
        }
    }
    public class StateMachine : IStateMachine
    {

        public MonoBehaviour Parent { get; set; }
        public TransitionCheckEventHandler DefaultTransitionCheckEvent { get; set; }
        public MilestoneEventHandler DefaultTransitionBeginning { get; set; }
        public MilestoneEventHandler DefaultTransitionEnding { get; set; }
        public MilestoneEventHandler Transitioning { get; set; }
        private ITransition ActiveTransition = null;
        public Dictionary<string, IState> States { get; set; } 
        
        protected Dictionary<string, Dictionary<string, ITransition>> FromTo = new Dictionary<string, Dictionary<string, ITransition>>();

        protected Dictionary<Direction, Dictionary<string, List<string>>> ReverseLookup = new Dictionary<Direction, Dictionary<string, List<string>>>();

        public IState CurrentState
        {
            get { return States[CurrentStateName]; }
        }

        public string CurrentStateName
        {
            get { return History(0); }
        }
        public string ApproachingStateName
        {
            get {
                if (ActiveTransition != null)
                    return ActiveTransition.To;
                return CurrentStateName;
            }
        }
        public string PreviousState
        {
            get { return History(1); }
        }

        private List<string> StateHistory = new List<string>();
        public StateMachine(MonoBehaviour parent, List<string> stateHistory)
        {
            Parent = parent;
            StateHistory = stateHistory;
            States = new Dictionary<string, IState>();
        }

        protected void SafeAdd(string from, string to, Direction direction, float timeSpan = 0, object target = null)
        {
            SafeDictionaryAdd(direction, Direction.Advance, from, to);
            SafeDictionaryAdd(direction, Direction.Switch, from, to);
            SafeDictionaryAdd(direction, Direction.Revert, from, to);

            SafeAddTransition(from, to, direction, timeSpan);

            SafeAddState(from);
            SafeAddState(to, target);
        }
        private void SafeAddTransition(string from, string to, Direction direction, float timeSpan)
        {
            if (!FromTo.ContainsKey(from))
                FromTo.Add(from, new Dictionary<string, ITransition>());
            if (!FromTo[from].ContainsKey(to))
            {
                var t = new Transition(this, from, to, direction, timeSpan);
                t.Ended += TransitionFinished;

                FromTo[from].Add(to, t);
            }
            else FromTo[from][to].Direction |= direction;
        }
        private void SafeAddTransition(string from, string to, ITransition transition)
        {
            if (!FromTo.ContainsKey(from))
                FromTo.Add(from, new Dictionary<string, ITransition>());
            if (!FromTo[from].ContainsKey(to))
            {
                FromTo[from].Add(to, transition);
            }
            else
            {
                //TODO:big issue, if already has transition and we add a switch transition the other way. then when switch adds here the old transition wont get any of the new switch transition's events. you need to rework things so that this isnt an issue to begin with. ah damnit. we're fucked on a lot of things right now. rework statemachine at some point. not too worried about it atm.
                FromTo[from][to].Direction |= transition.Direction;
            }
        }
        protected virtual void SafeAddState(string name, object target = null)
        {
            if (!States.ContainsKey(name))
            {
                if(target != null)
                    States.Add(name, new State() { Name = name, Target = target });
                else
                    States.Add(name, new State() { Name = name });
            }
        }
        private void SafeDictionaryAdd(Direction currentDirection, Direction targetDirection, string from, string to)
        {
            if ((currentDirection & targetDirection) > 0)
            {
                if (!ReverseLookup.ContainsKey(targetDirection)) ReverseLookup.Add(targetDirection, new Dictionary<string, List<string>>());
                if (!ReverseLookup[targetDirection].ContainsKey(from)) ReverseLookup[targetDirection].Add(from, new List<string>());
                if (!ReverseLookup[targetDirection][from].Contains(to) && (currentDirection | targetDirection) > 0) ReverseLookup[targetDirection][from].Add(to);
            }
        }

        public ITransition GetTransition(string to, Direction? direction = null)
        {
            return GetTransition(CurrentStateName, to, direction);
        }
        public ITransition GetTransition(string from, string to, Direction? direction = null)
        {
            if (FromTo[from].ContainsKey(to) && !direction.HasValue)
                return FromTo[from][to];
            else if (FromTo[from].ContainsKey(to) && direction.HasValue && ((FromTo[from][to].Direction & direction) > 0))
                return FromTo[from][to]; 
            else
                return null;
        }

        public string History(int depth)
        {
            return (depth >= 0 && depth <= StateHistory.Count - 1) ? StateHistory[StateHistory.Count - depth - 1] : null;
        }
        public List<string> HistoryList(int length)
        {
            return (length >= 0 && length <= StateHistory.Count) ? StateHistory.GetRange(StateHistory.Count - length, length) : null;
        }

        public bool Advance()
        {
            var to = PreferredOption(Direction.Advance);
            if (to != null)
                return To(to, Direction.Advance);
            else return false;
        }
        public bool Revert(int maxHistoryDepth = 2)
        {
            var to = PreferredOption(Direction.Revert, maxHistoryDepth);
            if (to != null)
                return To(to, Direction.Revert);
            else return false;
        }
        public bool Switch()
        {
            var to = PreferredOption(Direction.Switch);
            if (to != null)
                return To(PreferredOption(Direction.Switch), Direction.Switch);
            else return false;
        }
        public bool AdvanceTo(string to)
        {
            return To(to, Direction.Advance);
        }
        public bool RevertTo(string to, int maxHistoryDepth = 2)
        {
            return To(to, Direction.Revert, maxHistoryDepth);
        }
        public bool SwitchTo(string to)
        {
            return To(to, Direction.Switch);
        }
        private string PreferredOption(Direction dir, int maxHistoryDepth = 2)
        {
            var history = HistoryList(maxHistoryDepth);
            var options = Options(dir, dir == Direction.Revert, maxHistoryDepth);
            List<string> results = new List<string>();

            if(history!=null)
                results = options.Where(x=>!history.Contains(x)).ToList();
            if (results.Count == 0)
                results = options;
            return options.FirstOrDefault();
        }
        private List<string> Options(Direction dir, bool inHistoryRequired = false, int maxHistoryDepth = 1)
        {
            var options = ReverseLookup[dir][CurrentStateName];
            if (inHistoryRequired)
                options = options.Intersect(HistoryList(maxHistoryDepth) ?? new List<string>()).ToList();

            return options;
        }
        public bool CanRevert(string to, int? maxHistoryDepth = null) {
            return HistoryList(maxHistoryDepth ?? StateHistory.Count).Contains(to);
        }
        public bool CanRevert(int maxHistoryDepth) {
            return ReverseLookup[Direction.Revert][CurrentStateName].Intersect(HistoryList(maxHistoryDepth)).Count() > 0;
        }
        public bool CanAdvance(string to) {
            return ReverseLookup[Direction.Advance][CurrentStateName].FirstOrDefault(x => x == to) != null;
        }
        public bool CanAdvance() {
            return ReverseLookup[Direction.Advance][CurrentStateName].Count > 0;
        }
        public bool CanSwitch(string to) {
            return ReverseLookup[Direction.Switch][CurrentStateName].FirstOrDefault(x => x == to) != null;
        }
        public bool CanSwitch() {
            return ReverseLookup[Direction.Switch][CurrentStateName].Count > 0;
        }
        public bool Can(Direction? dir = null, string to = "", int maxHistoryDepth = 2)
        {
            if (dir == null)
                return FromTo[CurrentStateName].Count > 0;
            else if (to == "")
            {
                if (dir == Direction.Advance) return CanAdvance();
                if (dir == Direction.Revert) return CanRevert(maxHistoryDepth);
                if (dir == Direction.Switch) return CanSwitch();
            }else
            {
                if (dir == Direction.Advance) return CanAdvance(to);
                if (dir == Direction.Revert) return CanRevert(to, maxHistoryDepth);
                if (dir == Direction.Switch) return CanSwitch(to);
            }
            return false;
        }

        public bool To(string to, Direction? dir = null, int maxHistoryDepth = 2)
        {
            var cstate = CurrentStateName;
            var tran = GetTransition(cstate, to, dir);
            if (tran != null && Can(dir, to, maxHistoryDepth))
            {
                return ChangeStateTo(cstate, to, tran);
            }
            else return false;
        }

        private bool ChangeStateTo(string cstate, string state, Direction transition = Direction.Advance)
        {
            return ChangeStateTo(cstate, state, GetTransition(cstate, state, transition));
            
        }
        private bool ChangeStateTo(string cstate, string state, ITransition t)
        {
            if (!t.EndedHasListeners)
                t.Ended += TransitionFinished;

            if (ActiveTransition != t)
                ActiveTransition = t;


            AsyncDelegateInvoke(Transitioning);
            t.Start();
            return true;
        }

        protected IEnumerator AsyncDelegateInvoker(StateTransitionEventHandler stateTransitionEventHandler, ITransition transition, IState state)
        {
            yield return new WaitForSeconds(0);
            if(stateTransitionEventHandler!=null)
                stateTransitionEventHandler.Invoke(transition, state);
        }
        protected void AsyncDelegateInvoke(StateTransitionEventHandler stateTransitionEventHandler, ITransition transition, IState state)
        {
            Parent.StartCoroutine(AsyncDelegateInvoker(stateTransitionEventHandler, transition, state));
        }

        protected IEnumerator AsyncDelegateInvoker(MilestoneEventHandler milestoneEventHandler)
        {
            yield return new WaitForSeconds(0);
            if(milestoneEventHandler!=null) 
                milestoneEventHandler.Invoke(ActiveTransition);
        }
        protected void AsyncDelegateInvoke(MilestoneEventHandler milestoneEventHandler)
        {
            Parent.StartCoroutine(AsyncDelegateInvoker(milestoneEventHandler));
        }

        private void TransitionFinished(IPlayable transition)
        {
            AsyncDelegateInvoke(CurrentState.Exiting, (ITransition)transition, CurrentState);

            var t = transition as ITransition;
            switch (t.Direction)
            {
                case Direction.Advance:
                    AdvanceStateHistory(ApproachingStateName);
                    break;
                case Direction.Switch:
                    SwitchStateHistory(CurrentStateName, ApproachingStateName);
                    break;
                case Direction.Revert:
                    RevertStateHistory(ApproachingStateName);
                    break;
            }
            ActiveTransition = null;

            AsyncDelegateInvoke(CurrentState.Entered, t, CurrentState);
        }
        private void AdvanceStateHistory(string to)
        {
            StateHistory.Add(to);
        }
        private void SwitchStateHistory(string from, string to)
        {
            if(CurrentStateName == to)
            {
                var t = from;
                from = to;
                to = t;
            }
            StateHistory.RemoveAt(StateHistory.Count - 1);
            StateHistory.Add(to);
        }

        private void RevertStateHistory(string to)
        {
            var index = StateHistory.LastIndexOf(to);
            //you can only revert if you've been there and you can only revert if its directly in your transitions.
            if (index >= 0)
            {
                StateHistory.RemoveRange(index + 1, StateHistory.Count - index - 1);
            }
        }


        public List<ITransition> SetCyclesThrough(IEnumerable<string> states, Direction direction = Direction.Advance, float timeSpan = 0)
        {
            var transitions = new HashSet<ITransition>();

            var statesList = states.ToList();
            for (int i = 1; i < statesList.Count(); i++)
                transitions.Add(SetTransition(statesList[i - 1], statesList[i], direction));
            transitions.Add(SetTransition(statesList.Last(), statesList.First(), direction));

            return transitions.ToList();
        }

        public ITransition SetTransition(string from, string to, Direction direction, float timeSpan = 0)
        {
            SafeAdd(from, to, direction, timeSpan);
            var transition = GetTransition(from, to);

            if ((direction & Direction.Switch) > 0)
            {

                SafeAdd(to, from, direction, timeSpan);

            }

            return transition;
        }
    }
}
