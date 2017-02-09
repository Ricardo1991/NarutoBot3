﻿namespace IrcClient.Messages
{
    public class Action : IrcMessage
    {
        public Action(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body;
            this.header = "PRIVMSG";
        }

        public override string toString()
        {
            return header + " " + destinatary + " :" + "\x01" + "ACTION " + body.Trim() + "\x01" + footer;
        }

        public override bool isValid()
        {
            return !string.IsNullOrWhiteSpace(body);
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}