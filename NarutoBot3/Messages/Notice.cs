using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Notice : Message
    {

        public Notice(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body;
            this.header = "NOTICE";
        }

        public override string toString()
        {
            return header + " " + destinatary + " :" + body.Trim() + footer;
        }
        public override bool isValid()
        {
            return (!string.IsNullOrWhiteSpace(body) && !string.IsNullOrWhiteSpace(destinatary));
        }
        public override object Clone()
        {

            return this.MemberwiseClone();
        }

    }
}
