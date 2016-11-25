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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True for Online, False for Offline</param>
        public void setUserOnlineStatus(string nick, bool status)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.IsOnline = status;

            }
            else users.Add(new User(nick, status));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True to give Operator, False to remove Operator</param>
        public void setUserOperatorStatus(string nick, bool status)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.IsOperator = status;

            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim());
                u.IsOperator = status;
                users.Add(u);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True to mute, False to unmute</param>
        public void setUserMuteStatus(string nick, bool status)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.IsMuted = status;
                
            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim());
                u.IsMuted = status;
                users.Add(u);
            }
        }

        public void setGreeting(string nick, string greeting, bool enabled)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.Greeting = greeting;
                u.GreetingEnabled = enabled;
            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim());
                u.Greeting = greeting;
                u.GreetingEnabled = enabled;

                users.Add(u);
            }
        }

        public bool userIsOperator(string nick)
        {
            User u = getUserByName(nick);

            if (u != null && u.IsOperator) return true;

            return false;
        }

        public bool userIsMuted(string nick)
        {
            User u = getUserByName(nick);

            if (u != null && u.IsMuted) return true;

            return false;
        }

        public bool userHasChannelOP(string nick)
        {
            User u = getUserByName(nick);

            if (u != null && u.HasOP) return true;

            return false;
        }

        public bool userHasChannelVoice(string nick)
        {
            User u = getUserByName(nick);

            if (u != null && u.HasVoice) return true;
            
            return false;
        }

        public void setUserChannelVoice(string nick, bool status)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.HasVoice = status;
            }
            return;
        }

        public void setUserChannelOP(string nick, bool status)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.HasOP = status;
            }
        }

        public bool userIsMirrored(string nick)
        {
            User u = getUserByName(nick);

            if (u != null && u.MirrorMode)
                return true;
           
            return false;
        }

        public bool toogleMirror(string nick)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.MirrorMode = !u.MirrorMode;
                    return u.MirrorMode;
                
            }
            return false;
        }

        public int userMessageCount(string nick)
        {

            User u = getUserByName(nick);

            if (u != null)
            {
                if (u.DeliveredMessages != null)
                        return u.DeliveredMessages.Count;
            }

            return 0;
        }


        public UserMessage getUserMessage(string nick, int index)
        {

            User u = getUserByName(nick);

            if (u != null)
            {
                if (u.DeliveredMessages.Count >= index+1)
                    return u.DeliveredMessages[index];
            }

            return null;
        }

        public void addUserMessage(string destinatary, string sender, string message)
        {
            User u = getUserByName(destinatary);

            if (u != null)
            {
                
                if (u.DeliveredMessages == null)
                    u.DeliveredMessages = new List<UserMessage>();

                u.DeliveredMessages.Add(new UserMessage(message, sender));
                     
            }
            else
            {
                u = new User(destinatary.Replace("@", string.Empty).Replace("+", string.Empty).Trim());

                if (u.DeliveredMessages == null)
                    u.DeliveredMessages = new List<UserMessage>();

                u.DeliveredMessages.Add(new UserMessage(message, sender));
                users.Add(u);
            }
        }

        public bool clearUserMessages(string nick)
        {
            User u = getUserByName(nick);

            if (u != null)
            {
                u.DeliveredMessages.Clear();
                    return true;
            }

            return false;
        }

        public bool clearUserMessages(string nick, string arg)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            int messageNumber = 0;
            try
            {
                messageNumber = Int32.Parse(arg);
            }
            catch
            {
                return false;
            }

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();


            User u = getUserByName(nick);

            if(u != null)
            { 
                try
                {
                    u.DeliveredMessages.RemoveAt(messageNumber - 1);
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
            }

            else return false;
        }




        public User getUserByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            name = name.Replace("@", string.Empty).Replace("+", string.Empty);
            name = name.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, name, true) == 0)
                {
                    return u;
                }
            }

            return null;
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
