using System;

namespace NarutoBot3.Events
{
    public class PongEventArgs : EventArgs
    {
        private TimeSpan timeDifference;

        public PongEventArgs(TimeSpan time)
        {
            TimeDifference = time;
        }

        public TimeSpan TimeDifference
        {
            get
            {
                return timeDifference;
            }

            set
            {
                timeDifference = value;
            }
        }
    }
}