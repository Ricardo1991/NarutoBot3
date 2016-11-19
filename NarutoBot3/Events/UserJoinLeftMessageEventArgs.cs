using System;

namespace NarutoBot3.Events
{
    public class UserJoinLeftMessageEventArgs : EventArgs
    {
        string who;
        string message;

        public string Who
        {
            get
            {
                return who;
            }

            set
            {
                who = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        public UserJoinLeftMessageEventArgs(string w, string m)
        {

            Message = m;
            Who = w;

        }
    }
}
