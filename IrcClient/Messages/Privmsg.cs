namespace IrcClient.Messages
{
    public class Privmsg : IrcMessage
    {
        public Privmsg(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body;
            this.header = "PRIVMSG";
        }

        public override bool isValid()
        {
            return (!string.IsNullOrWhiteSpace(body) && !string.IsNullOrWhiteSpace(destinatary));
        }

        public override string toString()
        {
            return header + " " + destinatary + " :" + body.Trim() + footer;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}