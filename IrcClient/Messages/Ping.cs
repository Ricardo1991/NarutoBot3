﻿namespace IrcClient.Messages
{
    public class Ping : IrcMessage
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="body">Message to return</param>
        public Ping(string body)
        {
            this.body = body;
            this.header = "PING";
        }

        public override string toString()
        {
            return header + " :" + body + footer;
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