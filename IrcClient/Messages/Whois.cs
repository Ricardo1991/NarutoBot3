namespace IrcClient.Messages
{
    public class Whois : IrcMessage
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