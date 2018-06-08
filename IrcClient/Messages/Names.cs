using System;

namespace IrcClient.Messages
{
    public class Names : IrcMessage
    {
        public Names(string channel)
        {
            this.body = channel;
            this.header = "NAMES";
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool isValid()
        {
            return !String.IsNullOrWhiteSpace(this.body);
        }

        public override string toString()
        {
            return header + " " + body;
        }
    }
}