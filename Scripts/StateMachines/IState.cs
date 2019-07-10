using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.StateMachines
{
    public delegate void StateTransitionEventHandler(ITransition transition, IState current);
    public interface IState
    {
        StateTransitionEventHandler Entered { get; set; }
        StateTransitionEventHandler Exiting { get; set; }
        string Name { get; set; }
        object Target { get; set; }
    }
    public interface IState<T> :IState
    {
        new T Target { get; set; }
    }
}

