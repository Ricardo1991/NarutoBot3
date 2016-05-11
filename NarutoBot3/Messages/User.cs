using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class User : Message
    {

        public User(string destinatary, string body)
        {
            this.destinatary = null;
            this.body = body;
            this.header = "USER";
        }

        public override string toString()
        {
            return header + " " +body + footer;
        }
        public override bool isValid()
        {
            return !string.IsNullOrWhiteSpace(body);
        }
    }
}