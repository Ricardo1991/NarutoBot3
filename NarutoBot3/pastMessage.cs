using System;
namespace NarutoBot3
{
    public class pastMessage
    {
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        private int lenght;

        public int Lenght
        {
            get { return lenght; }
            set { lenght = value; }
        }
        private int wordsCount;

        public int WordsCount
        {
            get { return wordsCount; }
            set { wordsCount = value; }
        }
        private string[] words;

        public void Words(string[] wordss)        
        {            
            words = wordss;        
        }         

        public string[] GetWords()        
        {                     
            return (string[])words.Clone();        
        }    

        public pastMessage(string userA, string messageA)
        {
            if (String.IsNullOrEmpty(userA) || String.IsNullOrEmpty(messageA)) return;

            this.lenght = messageA.Length;
            this.message = messageA;
            this.user = userA;
            this.words = message.Split(' ');
            this.wordsCount = words.Length;
        }
    }
}
