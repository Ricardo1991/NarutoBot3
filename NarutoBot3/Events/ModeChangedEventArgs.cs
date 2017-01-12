using System;

namespace NarutoBot3.Events
{
    public class ModeChangedEventArgs : EventArgs
    {
        private string user;
        private string mode;

        public string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public string Mode
        {
            get
            {
                return mode;
            }

            set
            {
                mode = value;
            }
        }

        public ModeChangedEventArgs(string user, string mode)
        {
            this.User = user;
            this.Mode = mode;
        }
    }
}