using System;

namespace IrcClient.Messages
{
    public abstract class IrcMessage : ICloneable
    {
        public string header;
        public string body;
        public string destinatary;
        public string footer = "\r\n";

        public abstract string toString();

        public abstract bool isValid();

        public abstract object Clone();
    }
}