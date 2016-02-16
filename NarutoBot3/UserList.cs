using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    public class UserList
    {
        private List<User> users = new List<User>();

        public List<User> Users
        {
            get { return users; }
        }

        public void saveData()
        {
            TextWriter WriteFileStream = new StreamWriter("data.json", false);

            WriteFileStream.Write(JsonConvert.SerializeObject(users, Formatting.Indented));

            WriteFileStream.Close();
        }

        public void loadData()
        {
            try
            {
                TextReader stream = new StreamReader("data.json");
                string json = stream.ReadToEnd();
                JsonConvert.PopulateObject(json, users);
                stream.Close();
            }
            catch
            {
                users = new List<User>();
            }


        }

        public bool userExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            name = name.Replace("@", string.Empty).Replace("+", string.Empty);
            name = name.Trim();

            foreach (User u in Users)
            {
                if (string.Compare(u.Nick, name, true) == 0)
                    return true;
            }

            return false;
        }

        public void setUserOnline(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;

            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();

            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOnline = true;
                }
            }

            else users.Add(new User(n, true));
        }

        public void setUserOffline(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;

            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOnline = false;
                }
            }
            else users.Add(new User(n, false));
        }

        public void opUser(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();

            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOperator = true;
                }
            }
            else
            {
                User u = new User(n);
                u.IsOperator = true;
                users.Add(u);
            }
        }

        public void deopUser(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;

            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOperator = false;
                }
            }
            else
            {
                User u = new User(n);
                u.IsOperator = false;
                users.Add(u);
            }
        }

        public void muteUser(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();

            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsMuted = true;
                }
            }
            else
            {
                User u = new User(n);
                u.IsMuted = true;
                users.Add(u);
            }
        }

        public void unmuteUser(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsMuted = false;
                }
            }
            else
            {
                User u = new User(n);
                u.IsMuted = false;
                users.Add(u);
            }
        }

        public void setGreeting(string n, string greeting, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (userExists(n))
            {
                foreach (User u in Users)
                {
                    if (String.Compare(u.Nick, n, true) == 0)
                    {
                        u.Greeting = greeting;
                        u.GreetingEnabled = enabled;
                    }

                }
            }
            else
            {
                User u = new User(n);
                u.Greeting = greeting;
                u.GreetingEnabled = enabled;
                users.Add(u);
            }
        }

        public bool userIsOperator(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.IsOperator) return true;
            }
            return false;
        }

        public bool userIsMuted(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.IsMuted) return true;
            }
            return false;
        }

        public bool userHasChannelOP(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.HasOP) return true;
            }
            return false;
        }

        public bool userHasChannelVoice(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.HasVoice) return true;
            }
            return false;
        }

        public void setUserChannelVoice(string nick, bool status)
        {
            if (string.IsNullOrWhiteSpace(nick)) return;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                    u.HasVoice = status;
            }
            return;
        }

        public void setUserChannelOP(string nick, bool status)
        {
            if (string.IsNullOrWhiteSpace(nick)) return;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                    u.HasOP = status;
            }
            return;
        }

        public bool userIsMirrored(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.MirrorMode) return true;
            }
            return false;
        }

        public bool toogleMirror(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                {
                    u.MirrorMode = !u.MirrorMode;
                    return u.MirrorMode;
                }
            }
            return false;
        }

        public int userMessageCount(string nick)
        {

            if (string.IsNullOrWhiteSpace(nick)) return 0;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                {
                    if (u.DeliveredMessages != null)
                        return u.DeliveredMessages.Count;
                    else return 0;
                }

            }
            return 0;
        }

        public UserMessage getUserMessage(string nick, int index)
        {
            if (string.IsNullOrWhiteSpace(nick)) return null;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                {
                    if (u.DeliveredMessages.Count > 0)
                    {
                        return u.DeliveredMessages[index];

                    }
                }
            }

            return null;
        }

        public void addUserMessage(string destinatary, string sender, string message)
        {
            if (string.IsNullOrWhiteSpace(destinatary)) return;
            destinatary = destinatary.Replace("@", string.Empty).Replace("+", string.Empty);
            destinatary = destinatary.Trim();

            if (userExists(destinatary))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, destinatary, true) == 0)
                    {
                        if (u.DeliveredMessages == null)
                            u.DeliveredMessages = new List<UserMessage>();

                        u.DeliveredMessages.Add(new UserMessage(message, sender));
                    }
                        
                }
            }
            else
            {
                User u = new User(destinatary);

                if (u.DeliveredMessages == null)
                    u.DeliveredMessages = new List<UserMessage>();

                u.DeliveredMessages.Add(new UserMessage(message, sender));
                users.Add(u);
            }
        }

        public void clearUserMessages(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                {
                    u.DeliveredMessages.Clear();
                }

            }

        }
    }

    public class User
    {
        List<UserMessage> deliveredMessages;

        public List<UserMessage> DeliveredMessages
        {
            get { return deliveredMessages; }
            set { deliveredMessages = value; }
        }

        string nick;
        string greeting;
        bool isOperator;
        bool isMuted;
        bool greetingEnabled;
        bool mirrorMode;

        public bool MirrorMode
        {
            get { return mirrorMode; }
            set { mirrorMode = value; }
        }

        [JsonIgnore]
        bool isOnline=false;

        [JsonIgnore]
        bool hasOP = false;

        [JsonIgnore]
        public bool HasOP
        {
            get { return hasOP; }
            set { hasOP = value; }
        }
        [JsonIgnore]
        bool hasVoice = false;

        [JsonIgnore]
        public bool HasVoice
        {
            get { return hasVoice; }
            set { hasVoice = value; }
        }

        [JsonConstructor]
        private User()
        {

        }

        public User(string n)
        {
            nick = n;
            greeting = string.Empty;
            isOnline = false;
            isOperator = false;
            isMuted = false;
            greetingEnabled = false;
            mirrorMode = false;

            deliveredMessages = new List<UserMessage>();
        }

        public User(string n, bool online)
        {
            nick = n;
            greeting = string.Empty;
            isOnline = online;
            isOperator = false;
            isMuted = false;
            greetingEnabled = false;
            mirrorMode = false;

            deliveredMessages = new List<UserMessage>();
        }

        public User(string n, string g, bool online, bool op, bool mute, bool greet, bool mirror)
        {
            nick = n;
            greeting = g;
            isOnline = online;
            isOperator = op;
            isMuted = mute;
            greetingEnabled = greet;
            mirrorMode = mirror;

            deliveredMessages = new List<UserMessage>();
        }

        public string Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public string Greeting
        {
            get { return greeting; }
            set { greeting = value; }
        }
       
        public bool IsMuted
        {
            get { return isMuted; }
            set { isMuted = value; }
        }

        public bool IsOperator
        {
            get { return isOperator; }
            set { isOperator = value; }
        }

        [JsonIgnore]
        public bool IsOnline
        {
            get { return isOnline; }
            set { isOnline = value; }
        }

        public bool GreetingEnabled
        {
            get { return greetingEnabled; }
            set { greetingEnabled = value; }
        }
    }

    public class UserMessage
    {
        string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        string sender;

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        DateTime timestamp;

        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public UserMessage(string message, string sender)
        {
            this.message = message;
            this.sender = sender;

            timestamp = DateTime.Now.ToUniversalTime();
        }

    }
}
