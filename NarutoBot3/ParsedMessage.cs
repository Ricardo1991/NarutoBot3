using System;
using System.Linq;

namespace NarutoBot3
{
    internal class ParsedMessage
    {
        private string sender;          //Who sends the message. Nick, or server

        public string Sender
        {
            get { return sender; }
        }

        private string type;            //Type of message (PRIVMSG, PING, 233, etc)

        public string Type
        {
            get { return type; }
        }

        private string source;          //Source of the message, usually the channel

        public string Source
        {
            get { return source; }
        }

        private string[] splitMessage;  //message split per words

        public string[] SplitMessage
        {
            get { return splitMessage; }
        }

        private string completeMessage; //String with the message

        public string CompleteMessage
        {
            get { return completeMessage; }
        }

        public ParsedMessage(string message)
        {
            string trailing = null;
            sender = type = completeMessage = string.Empty;
            splitMessage = new string[] { };

            int prefixEnd = -1, trailingStart = message.Length;

            if (message.StartsWith(":"))
            {
                prefixEnd = message.IndexOf(" ");
                sender = message.Substring(1, prefixEnd - 1);
            }

            trailingStart = message.IndexOf(" :");

            if (trailingStart >= 0)
                trailing = message.Substring(trailingStart + 2);
            else
                trailingStart = message.Length;

            var typeAndSource = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');

            type = typeAndSource.First();

            if (typeAndSource.Length > 1)
            {
                splitMessage = typeAndSource.Skip(1).ToArray();
                source = typeAndSource[1];
            }

            if (!String.IsNullOrEmpty(trailing))
                splitMessage = splitMessage.Concat(new string[] { trailing }).ToArray();

            foreach (string s in splitMessage)
                completeMessage = completeMessage + s + " ";
        }
    }
}