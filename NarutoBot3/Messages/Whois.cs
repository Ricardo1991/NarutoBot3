﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    public class Whois : Message
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body">Channel to Join</param>
        public Whois(string body)
        {
            this.body = body;
            this.header = "WHOIS";
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