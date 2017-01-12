using System;

namespace NarutoBot3.Events
{
    public class UserKickedEventArgs : EventArgs
    {
        private string kickedUser;

        public string KickedUser
        {
            get
            {
                return kickedUser;
            }

            set
            {
                kickedUser = value;
            }
        }

        public UserKickedEventArgs(string user)
        {
            KickedUser = user;
        }
    }
}