namespace IrcClient.Messages
{
    public class Notice : IrcMessage
    {
        public Notice(string destinatary, string body)
        {
            this.destinatary = destinatary;
            this.body = body;
            this.header = "NOTICE";
        }

        public override string toString()
        {
            return header + " " + destinatary + " :" + body + footer;
        }

        public override bool isValid()
        {
            return (!string.IsNullOrWhiteSpace(body) && !string.IsNullOrWhiteSpace(destinatary));
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}