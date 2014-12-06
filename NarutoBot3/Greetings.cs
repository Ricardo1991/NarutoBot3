namespace NarutoBot3
{
    public class Greetings
    {
        string nick;
        string greeting;
        bool enabled;

        public string Nick
        {
          get { return nick; }
          set { nick = value; }
        }        

        public string Greeting
        {
          get { return greeting; }
          set { greeting = value; }
        }
       
        public bool Enabled
        {
          get { return enabled; }
          set { enabled = value; }
        }

        public Greetings(string nick, string greeting, bool enabled)
        {
            this.nick = nick;
            this.greeting = greeting;
            this.enabled = enabled;
        }
    }
}
