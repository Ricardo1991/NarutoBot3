using System;

namespace IrcClient
{
    public class WorkerIsBusyException : Exception
    {
        public WorkerIsBusyException()
        {
        }

        public WorkerIsBusyException(string message) : base(message)
        {
        }
    }
}