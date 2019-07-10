using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{

    public class PauseInstance
    {
        //adapt the state machine transitions to be pausable. handle it like in the animation library, check the time elapsed in the callback and if its paused or its too early, then re invoke. 
        //be sure not to fire the transition end event until the actual end.
        private float timeStopped;
        private bool localPause = false;
        private bool globalPause = false;
        public bool Paused { get { return LocalPause || GlobalPause; } set { } }
        public float TimePaused
        {
            get
            {
                if (Paused)
                    return Time.fixedTime - timeStopped;
                else return 0;
            }
            set { }
        }
        public float TimeStopped
        {
            get
            {
                return timeStopped;
            }
        }

        public bool LocalPause
        {
            get
            {
                return localPause;
            }

            set
            {
                if (value && !Paused)
                    SetTimeStopped();
                localPause = value;
            }
        }

        public bool GlobalPause
        {
            get
            {
                return globalPause;
            }

            set
            {
                if (value && !Paused)
                    SetTimeStopped();
                globalPause = value;
            }
        }

        private void SetTimeStopped()
        {
            timeStopped = Time.fixedTime;
        }

        public bool ResumeLocal()
        {
            bool starting = !GlobalPause && LocalPause;
            if (starting) BaseResume();

            LocalPause = false;

            return starting;
        }
        public bool ResumeGlobal()
        {
            if (!Pausable.GlobalPause)
            {
                bool starting = GlobalPause && !LocalPause;
                if (starting) BaseResume();

                GlobalPause = false;

                return starting;
            }
            else return false;
        }
        public bool ResumeAbsolute()
        {
            return ResumeLocal() || ResumeGlobal();
        }
        private void BaseResume()
        {
            timeStopped = -1;
        }
    }
}
