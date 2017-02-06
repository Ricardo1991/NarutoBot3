using System;

namespace IrcClient.Messages
{
    abstract public class IrcMessage : ICloneable
    {
        public string header;
        public string body;
        public string destinatary;
        public string footer = "\r\n";

        abstract public string toString();

        abstract public bool isValid();

        abstract public object Clone();
    }
}