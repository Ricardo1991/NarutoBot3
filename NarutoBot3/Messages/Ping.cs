using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Ping : Message
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
