using System;

namespace NarutoBot3.Events
{
    public class TopicChangedEventArgs : EventArgs
    {
        private string topic;

        public TopicChangedEventArgs(string topic)
        {
            this.Topic = topic;
        }

        public string Topic
        {
            get
            {
                return topic;
            }

            set
            {
                topic = value;
            }
        }
    }
}