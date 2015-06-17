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

            WriteFileStream.Write(JsonConvert.SerializeObject(users));

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

        public bool hasUserByName(String name)
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

        public void makeOnline(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOnline = true;
                }
            }
            else users.Add(new User(n, true));
        }

        public void makeOffline(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;

            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, n, true) == 0)
                        u.IsOnline = false;
                }
            }
            else users.Add(new User(n, false));
        }

        public void opUser(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
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

        public void deopUser(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;

            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
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

        public void muteUser(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();

            if (hasUserByName(n))
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

        public void unmuteUser(String n)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
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

        public void setGreeting(String n, String greeting, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(n)) return;
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            n = n.Trim();
            if (hasUserByName(n))
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

        public bool userIsOperator(String nick)
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

        public bool userIsMuted(String nick)
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

        public bool userIsMirrored(String nick)
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

        public bool toogleMirror(String nick)
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

        public int userMessageCount(String nick)
        {

            if (string.IsNullOrWhiteSpace(nick)) return 0;

            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            nick = nick.Trim();

            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0)
                {
                    return u.DeliveredMessages.Count;
                }

            }
            return 0;
        }

        public String getUserMessage(String nick, int index)
        {
            if (string.IsNullOrWhiteSpace(nick)) return string.Empty;

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

            return string.Empty;
        }

        public void addUserMessage(String destinatary, String sender, String message)
        {
            if (string.IsNullOrWhiteSpace(destinatary)) return;
            destinatary = destinatary.Replace("@", string.Empty).Replace("+", string.Empty);
            destinatary = destinatary.Trim();

            if (hasUserByName(destinatary))
            {
                foreach (User u in Users)
                {
                    if (string.Compare(u.Nick, destinatary, true) == 0)
                        u.DeliveredMessages.Add("From <" + sender + "> : " + message);
                }
            }
            else
            {
                User u = new User(destinatary);
                u.DeliveredMessages.Add("From <" + sender + "> : " + message);
                users.Add(u);
            }
        }

        public void clearUserMessages(String nick)
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
        List<String> deliveredMessages;

        public List<String> DeliveredMessages
        {
            get { return deliveredMessages; }
            set { deliveredMessages = value; }
        }
        String nick;
        String greeting;
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

        [JsonConstructor]
        private User()
        {

        }

        public User(String n)
        {
            nick = n;
            greeting = string.Empty;
            isOnline = false;
            isOperator = false;
            isMuted = false;
            greetingEnabled = false;
            mirrorMode = false;

            deliveredMessages = new List<string>();
        }

        public User(String n, bool online)
        {
            nick = n;
            greeting = string.Empty;
            isOnline = online;
            isOperator = false;
            isMuted = false;
            greetingEnabled = false;
            mirrorMode = false;

            deliveredMessages = new List<string>();
        }

        public User(String n, String g, bool online, bool op, bool mute, bool greet, bool mirror)
        {
            nick = n;
            greeting = g;
            isOnline = online;
            isOperator = op;
            isMuted = mute;
            greetingEnabled = greet;
            mirrorMode = mirror;

            deliveredMessages = new List<string>();
        }

        public String Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public String Greeting
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
}
