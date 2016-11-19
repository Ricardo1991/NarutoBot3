using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Events
{
    public class ModeChangedEventArgs : EventArgs
    {
        string user;
        string mode;

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
