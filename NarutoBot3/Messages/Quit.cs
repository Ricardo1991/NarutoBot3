﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Messages
{
    class Quit : Message
    {

        public Quit(string destinatary, string body)
        {
            this.destinatary = null;
            this.body = body;
            this.header = "QUIT";
        }

        public override string toString()
        {
            return header + " " + body + footer;
        }
        public override bool isValid()
        {
            return !string.IsNullOrWhiteSpace(body);
        }
    }
}