using System;

namespace NarutoBot3.Events
{
    public class NickChangeEventArgs : EventArgs
    {
        string oldNick;
        string newNick;

        public string OldNick
        {
            get
            {
                return oldNick;
            }

            set
            {
                oldNick = value;
            }
        }

        public string NewNick
        {
            get
            {
                return newNick;
            }

            set
            {
                newNick = value;
            }
        }

        public NickChangeEventArgs(string newNick, string oldNick)
        {
            this.OldNick = oldNick;
            this.NewNick = newNick;
        }
    }
}
