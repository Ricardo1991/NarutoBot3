using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    public class Greeting
    {
        string Nick1;
        string Greeting1;
        bool Enabled1;

        public string Nick
        {
          get { return Nick1; }
          set { Nick1 = value; }
        }        

        public string Greetingg
        {
          get { return Greeting1; }
          set { Greeting1 = value; }
        }
       
        public bool Enabled
        {
          get { return Enabled1; }
          set { Enabled1 = value; }
        }

        public Greeting(string nick, string greeting, bool enabled)
        {
            this.Nick1 = nick;
            this.Greeting1 = greeting;
            this.Enabled1 = enabled;
        }
    }
}
