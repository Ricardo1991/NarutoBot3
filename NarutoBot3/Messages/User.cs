namespace NarutoBot3.Messages
{
    public class User : Message
    {
        public User(string body)
        {
            this.body = body;
            this.header = "USER";
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