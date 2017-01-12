namespace NarutoBot3.Messages
{
    public class Kick : Message
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