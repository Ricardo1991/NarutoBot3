namespace IrcClient.Messages
{
    internal class Quit : IrcMessage
    {
        public Quit(string body)
        {
            this.body = body;
            this.header = "QUIT";
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