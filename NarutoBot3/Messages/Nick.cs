using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Nick : Message
    {

        public Nick(string body)
        {
            this.body = body;
            this.header = "NICK";
        }

        public override string toString()
        {
            return header + " " + body + footer;
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