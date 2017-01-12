namespace NarutoBot3.Messages
{
    public class Join : Message
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="body">Channel to Join</param>
        public Join(string body)
        {
            this.body = body;
            this.header = "JOIN";
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