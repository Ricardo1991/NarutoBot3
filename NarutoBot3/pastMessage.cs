using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    class pastMessage
    {
        public string user;
        public string message;
        public int lenght;
        public int wordsCount;
        public string[] words;

        public pastMessage(string userA, string messageA)
        {
            this.lenght = messageA.Length;
            this.message = messageA;
            this.user = userA;
            this.words = message.Split(' ');
            this.wordsCount = words.Length;
        }
    }
}
