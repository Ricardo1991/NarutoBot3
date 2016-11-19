using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Events
{
    public class UserKickedEventArgs : EventArgs
    {
        string kickedUser;

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
