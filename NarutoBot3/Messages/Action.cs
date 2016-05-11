using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Action : Message
    {

        public Action(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body ;
            this.header = "PRIVMSG";
        }

        public override string toString()
        {
            return header + " " + destinatary + " :"+"\x01" + "ACTION" + body.Trim() + "\x01" + footer;
        }
        public override bool isValid()
        {
            return !string.IsNullOrWhiteSpace(body);
        }

    }
}
