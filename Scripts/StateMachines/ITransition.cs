using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.StateMachines
{
    [Flags]
    public enum Direction : byte { Switch = 1, Advance = 2, Revert = 4 };

    public interface ITransition : IPausable
    {
        bool EndedHasListeners { get; }
        TransitionCheckEventHandler TransitionCheckEvent { get; set; }
        Direction Direction { get; set; }
        bool DefaultTransitions { get; }
        string From { get; }
        string To { get; }
        IStateMachine SM { get; }

        void SetDoesDefault(bool doesDefault);
    }
}
