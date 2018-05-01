using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    public class UserList : IEnumerable<User>
    {
        private List<User> users = new List<User>();

        public List<User> Users
        {
            get { return users; }
        }

        public void SaveData()
        {
            TextWriter WriteFileStream = new StreamWriter("data.json", false);

            WriteFileStream.Write(JsonConvert.SerializeObject(users, Formatting.Indented));

            WriteFileStream.Close();
        }

        public void LoadData()
        {
            try
            {
                TextReader stream = new StreamReader("data.json");
                string json = stream.ReadToEnd();

                JsonSerializerSettings jss = new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Populate
                };
                JsonConvert.PopulateObject(json, users, jss);
                stream.Close();
            }
            catch
            {
                System.IO.File.Move("data.json", "data.json.bak");
                users = new List<User>();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True for Online, False for Offline</param>
        public void SetUserOnlineStatus(string nick, bool status)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.IsOnline = status;
                u.LastSeen = DateTime.UtcNow;
            }
            else users.Add(new User(nick, status));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True to give Operator, False to remove Operator</param>
        public void SetUserOperatorStatus(string nick, bool status)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.IsOperator = status;
            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim())
                {
                    IsOperator = status
                };
                users.Add(u);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nick">Name of the User</param>
        /// <param name="status">True to mute, False to unmute</param>
        public void SetUserMuteStatus(string nick, bool status)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.IsMuted = status;
            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim())
                {
                    IsMuted = status
                };
                users.Add(u);
            }
        }

        public void SetGreeting(string nick, string greeting, bool enabled)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.Greeting = greeting;
                u.GreetingEnabled = enabled;
            }
            else
            {
                u = new User(nick.Replace("@", string.Empty).Replace("+", string.Empty).Trim())
                {
                    Greeting = greeting,
                    GreetingEnabled = enabled
                };
                users.Add(u);
            }
        }

        public bool UserIsOperator(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null && u.IsOperator) return true;

            return false;
        }

        public bool UserIsMuted(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null && u.IsMuted) return true;

            return false;
        }

        public bool UserHasChannelOP(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null && u.HasOP) return true;

            return false;
        }

        public bool UserHasChannelVoice(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null && u.HasVoice) return true;

            return false;
        }

        public void SetUserChannelVoice(string nick, bool status)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.HasVoice = status;
            }
            return;
        }

        public char GetUserMode(string nick)
        {
            User u = GetUserByName(nick);

            if (u == null) return (char)0;

            return u.UserMode;
        }

        public void SetUserMode(string nick, char mode)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.UserMode = mode;
            }
            return;
        }

        public void SetUserChannelOP(string nick, bool status)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.HasOP = status;
            }
        }

        public bool UserIsMirrored(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null && u.MirrorMode)
                return true;

            return false;
        }

        public bool ToogleMirror(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.MirrorMode = !u.MirrorMode;
                return u.MirrorMode;
            }
            return false;
        }

        public int UserMessageCount(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                if (u.DeliveredMessages != null)
                    return u.DeliveredMessages.Count;
            }

            return 0;
        }

        public UserMessage GetUserMessage(string nick, int index)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                if (u.DeliveredMessages.Count >= index + 1)
                    return u.DeliveredMessages[index];
            }

            return null;
        }

        public DateTime GetUserSeenUTC(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                return u.LastSeen;
            }

            return new DateTime(0);
        }

        public void MarkUserSeen(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.LastSeen = DateTime.UtcNow;
            }
        }

        public void AddUserMessage(string destinatary, string sender, string message)
        {
            User u = GetUserByName(destinatary);

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

        public bool ClearUserMessages(string nick)
        {
            User u = GetUserByName(nick);

            if (u != null)
            {
                u.DeliveredMessages.Clear();
                return true;
            }

            return false;
        }

        public bool ClearUserMessages(string nick, string arg)
        {
            if (string.IsNullOrWhiteSpace(nick)) return false;

            int messageNumber;
            try
            {
                messageNumber = Int32.Parse(arg);
            }
            catch
            {
                return false;
            }

            User u = GetUserByName(nick);

            if (u != null)
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

        public User GetUserByName(string name)
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

        public List<User> GetAllOnlineUsers()
        {
            List<User> ul = new List<User>();

            foreach (User u in Users)
            {
                if (u.IsOnline)
                    ul.Add(u);
            }

            return ul;
        }

        public IEnumerator<User> GetEnumerator()
        {
            return new UserListEnumerator(this.Users);
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }
    }

    public class UserListEnumerator : IEnumerator<User>
    {
        private List<User> _userlist;
        private User _current;
        private int index = 0;

        public UserListEnumerator(List<User> ul)
        {
            _userlist = ul;
        }

        public User Current
        {
            get
            {
                if (_userlist == null || _current == null)
                {
                    throw new InvalidOperationException();
                }

                return _current;
            }
        }

        private object Current1
        {
            get { return this.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current1; }
        }

        public bool MoveNext()
        {
            try
            {
                _current = _userlist[index++];
            }
            catch
            {
                _current = null;
            }

            if (_current == null)
                return false;
            return true;
        }

        public void Reset()
        {
            _current = null;
            index = 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class User : IComparer<User>, IComparable<User>, IEquatable<User>
    {
        private List<UserMessage> deliveredMessages;

        public List<UserMessage> DeliveredMessages
        {
            get { return deliveredMessages; }
            set { deliveredMessages = value; }
        }

        private char userMode = '0';
        private string nick = "User";
        private string greeting = "";
        private bool isOperator = false;
        private bool isMuted = false;
        private bool greetingEnabled = false;
        private bool mirrorMode = false;
        private DateTime lastSeen = new DateTime(0);

        public bool MirrorMode
        {
            get { return mirrorMode; }
            set { mirrorMode = value; }
        }

        [JsonIgnore]
        private bool isOnline = false;

        [JsonIgnore]
        private bool hasOP = false;

        [JsonIgnore]
        public bool HasOP
        {
            get { return hasOP; }
            set { hasOP = value; }
        }

        [JsonIgnore]
        private bool hasVoice = false;

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
            LastSeen = DateTime.UtcNow;

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
            LastSeen = DateTime.UtcNow;

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
            LastSeen = DateTime.UtcNow;

            deliveredMessages = new List<UserMessage>();
        }

        public override string ToString()
        {
            if (UserMode == '0')
                return Nick;
            else return UserMode + Nick;
        }

        public static int Compare1(User x, User y)
        {
            return x.ToString().CompareTo(y.ToString());
        }

        public int Compare(User x, User y)
        {
            return x.ToString().CompareTo(y.ToString());
        }

        public int CompareTo(User other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        public bool Equals(User other)
        {
            return this.ToString().CompareTo(other.ToString()) == 0;
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

        public DateTime LastSeen
        {
            get
            {
                return lastSeen;
            }

            set
            {
                lastSeen = value;
            }
        }

        public char UserMode
        {
            get
            {
                return userMode;
            }

            set
            {
                userMode = value;
            }
        }
    }

    public class UserMessage
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string sender;

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        private DateTime timestamp;

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