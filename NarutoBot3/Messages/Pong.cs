namespace NarutoBot3.Messages
{
    public class Pong : Message
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="body">Message to return</param>
        public Pong(string body)
        {
            this.body = body;
            this.header = "PONG";
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