namespace IrcClient.Messages
{
    public class Nick : IrcMessage
    {
        public Nick(string body)
        {
            this.body = body;
            this.header = "NICK";
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