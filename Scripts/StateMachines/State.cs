using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.StateMachines
{
    public class State : IState
    {
        public object Target { get; set; }

        public StateTransitionEventHandler Entered { get; set; }
        public StateTransitionEventHandler Exiting { get; set; }
        public string Name { get; set; }
    }
    public class State<T> : State, IState<T>
    {
        public new T Target { get { return (T)base.Target; } set { base.Target = value; } }

    }
}
