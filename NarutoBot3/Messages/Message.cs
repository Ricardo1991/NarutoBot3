namespace NarutoBot3
{
    abstract public class Message
    {
        public string header;
        public string body;
        public string destinatary;
        public string footer = "\r\n";

        abstract public string toString();

        abstract public bool isValid();
    }


}
