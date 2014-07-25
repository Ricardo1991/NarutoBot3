using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    public delegate void UserListChangedEventHandler(object sender, EventArgs e);
    public delegate void ConnectedChangedEventHandler(object sender, EventArgs e);
    public delegate void ReturnMessageChanged(object sender, EventArgs e);

    public class Bot
    {
        string mode;

        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        string newNick;

        public string NewNick
        {
            get { return newNick; }
            set { newNick = value; }
        }

        string who;

        public string Who
        {
            get { return who; }
            set { who = value; }
        }

        string wholeft;

        public string Wholeft
        {
            get { return wholeft; }
            set { wholeft = value; }
        }

        bool conneceted = false;

        public bool isConnected
        {
            get { return conneceted; }
            set { conneceted = value; }
        }

        public event UserListChangedEventHandler Created;
        public event UserListChangedEventHandler Joined;
        public event UserListChangedEventHandler Left;
        public event UserListChangedEventHandler NickChanged;
        public event UserListChangedEventHandler ModeChanged;
        public event UserListChangedEventHandler Kicked;
        public event ReturnMessageChanged MessageReturned;
        public event ConnectedChangedEventHandler Connected;


        protected virtual void OnCreate(EventArgs e)
        {
            if (Created != null)
                Created(this, e);
        }
        protected virtual void OnJoin(EventArgs e)
        {
            if (Joined != null)
                Joined(this, e);
        }
        protected virtual void OnLeave(EventArgs e)
        {
            if (Left != null)
                Left(this, e);
        }

        protected virtual void OnNickChange(EventArgs e)
        {
            if (NickChanged != null)
                NickChanged(this, e);
        }

        protected virtual void OnConenct(EventArgs e)
        {
            if (Connected != null)
                Connected(this, e);
        }

        protected virtual void OnModeChange(EventArgs e)
        {
            if (ModeChanged != null)
                ModeChanged(this, e);
        
        }

        protected virtual void OnKick(EventArgs e)
        {
            if (Kicked != null)
                Kicked(this, e);
        
        }

        protected virtual void OnMessageReturned(EventArgs e)
        {
            if (MessageReturned != null)
                MessageReturned(this, e);
        
        }


        public string BotMessage(string message, /*out string returnmessage,*/ ref List<string> userList)
        {
            Who = "";
            Wholeft = "";
            NewNick = "";

            string prefix;
            string command;
            string[] parameters;
            string completeParameters;
            //returnmessage = "";
            List<string> userTemp = new List<string>();

            bool found;

            if (message.Contains("PING :"))
            {
                var prefixend = message.IndexOf(":");
                string pingcmd = message.Substring(prefixend+1);
                return  "PONG :" + pingcmd + "\r\n";
            }

            else
            {
                parseMessage(message, out prefix, out command, out parameters, out completeParameters);

                switch (command)
                {
                    case ("353"): //USERLIST
                       
                        foreach(string s in parameters[3].Split(' '))
                        {
                            found = false;
                            foreach(string u in userList)
                                if (s == u) found = true;
                            if (!found) userList.Add(s);
                        }
                            
                        userList.Sort();
                        OnCreate(EventArgs.Empty);
                        break;

                    case ("376"): //END OF MOTD

                        if(completeParameters.Contains("End of /MOTD command."))
                        {
                            isConnected = true;
                            OnConenct(EventArgs.Empty); 
                        }
                            
                        break;

                    case ("JOIN"):
                        Who = prefix.Substring(0, prefix.IndexOf("!"));
                        found = false;

                        foreach(string s in userList)
                        { if (s == Who) found = true; }

                        if(!found)userList.Add(Who);

                        userList.Sort();
                        OnJoin(EventArgs.Empty);
                        break;


                    case ("PART"):
                        Wholeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string user in userList)
                        {
                            if (user.Replace("@", string.Empty).Replace("+", string.Empty) != Wholeft.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(user);
                            }
                        }
                        userList.Clear();

                        foreach (string user in userTemp)
                        {
                            userList.Add(user);
                        }
                        userList.Sort();
                        userTemp.Clear();

                        OnLeave(EventArgs.Empty);
                        break;


                    case ("QUIT"): 
                        
                        Wholeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string user in userList)
                        {
                            if (user.Replace("@", string.Empty).Replace("+", string.Empty) != Wholeft.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(user);
                            }
                        }
                        userList.Clear();

                        foreach (string user in userTemp)
                        {
                            userList.Add(user);
                        }
                        userList.Sort();
                        userTemp.Clear();
                        OnLeave(EventArgs.Empty);
                        break;


                    case ("NICK"):
                        string oldnick = prefix.Substring(0, prefix.IndexOf("!"));
                        string newnick = completeParameters;
                        char mode = getUserMode(oldnick, userList);

                        if (mode != '0')
                            newnick = mode + newnick;

                        userTemp = new List<string>();

                        foreach (string user in userList)
                        {
                            if (user.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)
                                !=
                                oldnick.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)))
                            {
                                userTemp.Add(user);
                            }
                        }
                        userList.Clear();

                        foreach (string user in userTemp)
                        {
                            userList.Add(user);
                        }
                        userList.Add(newnick);
                        userList.Sort();


                        NewNick = newnick;
                        Who = oldnick;

                        OnNickChange(EventArgs.Empty);
                        userTemp.Clear();
                        break;


                    case ("MODE"):

                        userTemp = new List<string>();
                        string modechange = parameters[1];

                        if (parameters.Length < 3)
                            break;
                        string affectedUser = parameters[2];

                        foreach (string user in userList)
                        {
                            if (user.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)
                                !=
                                affectedUser.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(user);
                            }
                        }
                        userList.Clear();

                        foreach (string user in userTemp)
                        {
                            userList.Add(user);
                        }

                        switch (modechange)
                        {
                            case ("+o"):
                                userList.Add("@" + affectedUser);
                                break;
                            case ("-o"):
                                userList.Add(affectedUser);
                                break;
                            case ("+v"):
                                userList.Add("+" + affectedUser);
                                break;
                            case ("-v"):
                                userList.Add(affectedUser);
                                break;
                            case ("+h"):
                                userList.Add("%" + affectedUser);
                                break;
                            case ("-h"):
                                userList.Add(affectedUser);
                                break;
                            case ("+q"):
                                userList.Add("~" + affectedUser);
                                break;
                            case ("-q"):
                                userList.Add(affectedUser);
                                break;
                            case ("+a"):
                                userList.Add("&" + affectedUser);
                                break;
                            case ("-a"):
                                userList.Add(affectedUser);
                                break;
                        }

                        Who = affectedUser;
                        OnModeChange(EventArgs.Empty);
                        userList.Sort();
                        userTemp.Clear();

                        break;


                    case ("KICK"): ;

                        userTemp = new List<string>();
                        string kickedUser = parameters[1];

                          foreach (string user in userList)
                        {
                            if (user.Replace("@", string.Empty).Replace("+", string.Empty) != kickedUser.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(user);
                            }
                        }
                        userList.Clear();

                        foreach (string user in userTemp)
                        {
                            userList.Add(user);
                        }
                        userList.Sort();

                        userTemp.Clear();
                        Who = kickedUser;
                        OnKick(EventArgs.Empty);
                        break;

                    case ("PRIVMSG"): 
                        
                        ;
                        break;


                    case ("NOTICE"): ;
                        break;
                        
                }
                return "";
            }
        
        }

        void parseMessage(string message, out string prefix, out string command, out string[] parameters, out string completeParameters)
        {
            int prefixEnd = -1, trailingStart = message.Length;
            string trailing = null;
            prefix = command = String.Empty;
            parameters = new string[] { };
            completeParameters="";

            if (message.StartsWith(":"))
            {
                prefixEnd = message.IndexOf(" ");
                prefix = message.Substring(1, prefixEnd - 1);
            }

            trailingStart = message.IndexOf(" :");
            if (trailingStart >= 0)
                trailing = message.Substring(trailingStart + 2);
            else
                trailingStart = message.Length;

            var commandAndParameters = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');

            command = commandAndParameters.First();
            if (commandAndParameters.Length > 1)
                parameters = commandAndParameters.Skip(1).ToArray();



            if (!String.IsNullOrEmpty(trailing))
                parameters = parameters.Concat(new string[] { trailing }).ToArray();

            completeParameters = "";
            foreach (string s in parameters)
                completeParameters = completeParameters + s + " ";

        }

        private char getUserMode(string user, List<string> userList)
        {
            foreach (string u in userList)
            {
                if (u.Contains(user))
                {
                    switch (u.Substring(0, 1))
                    {
                        case "@":
                            return '@';
                        case "+":
                            return '+';
                        case "%":
                            return '%';
                        case "~":
                            return '~';
                        case "&":
                            return '&';
                        default:
                            return '0';
                    }
                }
            }
            return '0';
        }
    }
}
