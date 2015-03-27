﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NarutoBot3
{
    public class UserList
    {
        private List<User> users = new List<User>();

        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        public bool hasUserByName(String name){
            name = name.Replace("@", string.Empty).Replace("+", string.Empty);
            foreach (User u in Users)
            {
                if (u.Nick == name)
                    return true;
            }
            
            return false;
        }

        public void makeOnline(String n)
        {
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
                        u.IsOnline = true;
                }
            }
            else users.Add(new User(n, true));
        }

        public void makeOffline(String n)
        {
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
                        u.IsOnline = false;
                }
            }
            else users.Add(new User(n, false));
        }

        public void opUser(String n)
        {
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
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
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
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
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
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
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
            if (hasUserByName(n))
            {
                foreach (User u in Users)
                {
                    if (u.Nick == n)
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
            n = n.Replace("@", string.Empty).Replace("+", string.Empty);
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
            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.IsOperator) return true;
            }
            return false;
        }

        public bool userIsMuted(String nick)
        {
            nick = nick.Replace("@", string.Empty).Replace("+", string.Empty);
            foreach (User u in Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.IsMuted) return true;
            }
            return false;
        }

    }

    public class User
    {
        String nick;
        String greeting;
        bool isOperator;
        bool isMuted;
        bool greetingEnabled;

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
        }

        public User(String n, bool online)
        {
            nick = n;
            greeting = string.Empty;
            isOnline = online;
            isOperator = false;
            isMuted = false;
            greetingEnabled = false;
        }

        public User(String n, String g, bool online, bool op, bool mute, bool greet)
        {
            nick = n;
            greeting = g;
            isOnline = online;
            isOperator = op;
            isMuted = mute;
            greetingEnabled = greet;
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