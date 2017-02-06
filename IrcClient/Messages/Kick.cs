namespace IrcClient.Messages
{
    public class Kick : IrcMessage
    {
        public Kick(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body;
            this.header = "KICK";
        }

        public override string toString()
        {
            return header + " " + destinatary + " " + body + footer;
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