using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Kick : Message
    {

        public Kick(string destinatary, string body)
        {
            this.destinatary = null;
            this.body = body;
            this.header = "KICK";
        }

        public override string toString()
        {
            return header + " " + destinatary + " "+ body + footer;
        }
        public override bool isValid()
        {
            return !string.IsNullOrWhiteSpace(body);
        }
    }
}

