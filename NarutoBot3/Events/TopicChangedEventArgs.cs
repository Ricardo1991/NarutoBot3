using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3.Events
{
    public class TopicChangedEventArgs : EventArgs
    {
        string topic;

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
