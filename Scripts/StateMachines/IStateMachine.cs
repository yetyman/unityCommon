using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.StateMachines
{
    public interface IStateMachine
    {

        TransitionCheckEventHandler DefaultTransitionCheckEvent { get; set; }
        MilestoneEventHandler DefaultTransitionBeginning { get; set; }
        MilestoneEventHandler DefaultTransitionEnding { get; set; }
        MilestoneEventHandler Transitioning { get; set; }

        string CurrentStateName { get; }
        string PreviousState { get; }
        MonoBehaviour Parent { get; set; }
        IState CurrentState { get; }
        string ApproachingStateName { get; }



        bool Advance();
        bool AdvanceTo(string to);
        bool Can(Direction? dir = null, string to = "", int maxHistoryDepth = 2);
        bool CanAdvance();
        bool CanAdvance(string to);
        bool CanRevert(int maxHistoryDepth);
        bool CanRevert(string to, int? maxHistoryDepth = null);
        bool CanSwitch();
        bool CanSwitch(string to);
        ITransition GetTransition(string to, Direction? direction = null);
        ITransition GetTransition(string from, string to, Direction? direction = null);
        string History(int depth);
        List<string> HistoryList(int length);
        bool Revert(int maxHistoryDepth = 2);
        bool RevertTo(string to, int maxHistoryDepth = 2);
        List<ITransition> SetCyclesThrough(IEnumerable<string> states, Direction direction = Direction.Advance, float timeSpan = 0);
        ITransition SetTransition(string from, string to, Direction direction, float timeSpan = 0);
        Dictionary<string, IState> States { get; set; }
        bool Switch();
        bool SwitchTo(string to);
        bool To(string to, Direction? dir = null, int maxHistoryDepth = 2);
    }
}