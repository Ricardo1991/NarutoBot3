using NarutoBot3.Properties;
using Newtonsoft.Json;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using TweetSharp;

namespace NarutoBot3
{
    public class Bot : IDisposable
    {
        private List<string> ops = new List<string>();
        private List<string> rls = new List<string>();
        private List<string> hlp = new List<string>();
        private List<string> ban = new List<string>();
        private List<string> tri = new List<string>();
        private List<string> kill = new List<string>();
        private List<Greetings> greet = new List<Greetings>();
        private List<string> nickGenStrings;
        private List<pastMessage> pastMessages = new List<pastMessage>();

        string eta = Settings.Default.eta;

        string mode;
        public string Mode{ get { return mode; } set { mode = value; } }

        string newNick;
        public string NewNick{get { return newNick; }set { newNick = value; }}

        string who;
        public string Who{get { return who; }set { who = value; }}

        string whoLeft;
        public string WhoLeft{get { return whoLeft; }set { whoLeft = value; }}

        bool conneceted = false;
        public bool IsConnected{get { return conneceted; }set { conneceted = value; }}

        public event EventHandler<EventArgs> DuplicatedNick;

        public event EventHandler<EventArgs> Created;
        public event EventHandler<EventArgs> Joined;
        public event EventHandler<EventArgs> Left;
        public event EventHandler<EventArgs> NickChanged;
        public event EventHandler<EventArgs> ModeChanged;
        public event EventHandler<EventArgs> Kicked;

        public event EventHandler<EventArgs> Timeout;
        public event EventHandler<EventArgs> PongReceived;

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> ConnectedWithServer;

        public event EventHandler<EventArgs> BotNickChanged;

        public event EventHandler<EventArgs> BotSilenced;
        public event EventHandler<EventArgs> BotUnsilenced;

        public event EventHandler<EventArgs> Quit;

        public event EventHandler<EventArgs> TopicChange;

        private System.Timers.Timer dTime;

        private bool waitingForPong = false;

        private TimeSpan timeDifference;

        IRC_Client Client;
        RichTextBox Output2;
        string botVersion = "NarutoBot3 by Ricardo1991, compiled on " + getCompilationDate.RetrieveLinkerTimestamp();
        private Reddit reddit;

        string topic;

        public string Topic
        {
            get { return topic; }
            set { topic = value; }
        }

        private TwitterService service;

        private RedditSharp.Things.AuthenticatedUser user;
        
        public TimeSpan TimeDifference
        {
            get { return timeDifference; }
            set { timeDifference = value; }
        }

        public bool WaitingForPong
        {
            get { return waitingForPong; }
            set { waitingForPong = value; }
        }

        protected virtual void OnTopicChange(EventArgs e)
        {
            if (TopicChange != null)
                TopicChange(this, e);
        
        }
        protected virtual void OnPongReceived(EventArgs e)
        {
            if (PongReceived != null)
                PongReceived(this, e);
        }

        protected virtual void OnTimeout(EventArgs e)
        {
            if (Timeout != null)
                Timeout(this, e);
        }

        protected virtual void OnConnectedWithServer(EventArgs e)
        {
            if (ConnectedWithServer != null)
                ConnectedWithServer(this, e);
        }

        protected virtual void OnDuplicatedNick(EventArgs e)
        {
            if (DuplicatedNick != null)
                DuplicatedNick(this, e);
        }

        protected virtual void OnUnsilence(EventArgs e)
        {
            if (BotUnsilenced != null)
                BotUnsilenced(this, e);
        }

        protected virtual void OnSilence(EventArgs e)
        {
            if (BotSilenced != null)
                BotSilenced(this, e);
        }

        protected virtual void OnQuit(EventArgs e)
        {
            if (Quit != null)
                Quit(this, e);
        }

        protected virtual void OnBotNickChanged(EventArgs e)
        {
            if (BotNickChanged != null)
                BotNickChanged(this, e);
        }

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

        protected virtual void OnConnect(EventArgs e)
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
        
        public Bot(ref IRC_Client client, ref RichTextBox output2)
        {
            Client = client;
            Output2 = output2;
        }

        ~Bot()
        {
            Dispose(false);
            return;
        }

        public void LoadSettings()
        {
            ReadOps();                  //operators
            ReadBan();                  //banned users
            ReadHelp();                 //help text
            ReadTrivia();               //trivia strings
            ReadGreetings();            //read greetings
            ReadKills();                //Read the killstrings
            LoadNickGenStrings();       //For the nick generator

            reddit = new Reddit();

            if (Settings.Default.redditUserEnabled)
            {
                try { 
                    user = reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass, true); 
                }
                catch { }
            }

            if (Settings.Default.twitterEnabled)
            {
                try {
                    service = new TwitterService(Settings.Default.twitterConsumerKey, Settings.Default.twitterConsumerKeySecret);

                    service.AuthenticateWith(Settings.Default.twitterAccessToken, Settings.Default.twitterAccessTokenSecret);
                }

                catch {
                    Settings.Default.twitterEnabled = false;
                    Settings.Default.Save();
                }
            
            }
        }

        public void BotMessage(string message)
        {
            if (String.IsNullOrEmpty(message)) return;

            Who = "";
            WhoLeft = "";
            NewNick = "";

            string prefix;
            string command;
            string[] parameters;
            string completeParameters;
            List<string> userTemp = new List<string>();

            bool found;

            if (message.Contains("PING :"))
            {
                var prefixend = message.IndexOf(":");
                string pingcmd = message.Substring(prefixend+1);
                Client.messageSender("PONG :" + pingcmd + "\r\n");

                #if DEBUG
                WriteMessage(message);
                #endif
            }

            else
            {
                parseMessage(message, out prefix, out command, out parameters, out completeParameters);

                switch (command)
                {
                    case ("004"): //server used for connection

                        Client.HOST_SERVER = parameters[1];
                        WriteMessage("* " + command + " " + completeParameters);
                        
                        break;

                    case ("433"): //Nickname is already in use.
                        OnDuplicatedNick(EventArgs.Empty);
                        WriteMessage("* " + command + " " + completeParameters);

                        break;
                    case ("333"):   //Topic author and time

                        break;
                    case ("366"): //End of /NAMES

                        break;
                    case ("353"): //USERLIST
                       
                        foreach(string s in parameters[3].Split(' '))
                        {
                            found = false;
                            foreach(string u in Client.userList)
                                if (s == u) found = true;
                            if (!found) Client.userList.Add(s);
                        }
                            
                        Client.userList.Sort();
                        OnCreate(EventArgs.Empty);
                        break;

                    case ("376"): //END OF MOTD

                        if(completeParameters.Contains("End of /MOTD command."))
                        {
                            IsConnected = true;
                            OnConnect(EventArgs.Empty);

                            if (!String.IsNullOrEmpty(Client.HOST_SERVER))
                                OnConnectedWithServer(EventArgs.Empty);

                            pingSever();
                        }
                            
                        break;
                    case ("332"):   //TOPIC

                        Topic = completeParameters.Split(new char[] {' '}, 3)[2];
                        OnTopicChange(EventArgs.Empty);
                        break;

                    case ("PONG"):
                        string[] split = message.Split(':');
                        string pongcmd = split[2];

                        #if DEBUG
                        WriteMessage(message);
                        #endif

                        if (WaitingForPong)
                        {
                            string currentStamp = GetTimestamp(DateTime.Now);
                            string format = "mmssffff";

                            DateTime now, then;

                            now = DateTime.ParseExact(currentStamp, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
                            then = DateTime.ParseExact(pongcmd, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));

                            TimeDifference = now.Subtract(then);

                            WaitingForPong = false;
                            dTime.Stop();

                            OnPongReceived(EventArgs.Empty);
                        }

                        break;

                    case ("JOIN"):
                        Who = prefix.Substring(0, prefix.IndexOf("!"));
                        found = false;

                        foreach(string s in Client.userList)
                        { if (s == Who) found = true; }

                        if(!found)Client.userList.Add(Who);

                        Client.userList.Sort();

                        OnJoin(EventArgs.Empty);

                        foreach (Greetings g in greet)
                        {
                            if (g.Nick == Who.Replace("@", string.Empty).Replace("+", string.Empty) && g.Enabled == true)
                            {
                                string messagess = Privmsg(Client.HOME_CHANNEL, g.Greetingg);
                                Client.messageSender(messagess);
                            }
                        }

                        break;


                    case ("PART"):
                        WhoLeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string userP in Client.userList)
                        {
                            if (removeUserMode(userP) != removeUserMode(WhoLeft))
                            {
                                userTemp.Add(userP);
                            }
                        }
                        Client.userList.Clear();

                        foreach (string userO in userTemp)
                        {
                            Client.userList.Add(userO);
                        }

                        Client.userList.Sort();
                        userTemp.Clear();

                        OnLeave(EventArgs.Empty);
                        break;


                    case ("QUIT"): 
                        
                        WhoLeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string userB in Client.userList)
                        {
                            if (removeUserMode(userB) != removeUserMode(WhoLeft))
                                userTemp.Add(userB);
                        }
                        Client.userList.Clear();

                        foreach (string userN in userTemp)
                            Client.userList.Add(userN);

                        Client.userList.Sort();
                        userTemp.Clear();
                        OnLeave(EventArgs.Empty);
                        break;


                    case ("NICK"):
                        string oldnick = prefix.Substring(0, prefix.IndexOf("!"));
                        string newnick = completeParameters;
                        char mode = getUserMode(oldnick, Client.userList);

                        if (mode != '0')
                            newnick = mode + newnick;

                        userTemp = new List<string>();

                        foreach (string userC in Client.userList)
                        {
                            if (removeUserMode(userC) != removeUserMode(oldnick))
                                userTemp.Add(userC);
                        }
                        Client.userList.Clear();

                        foreach (string use in userTemp)
                        {
                            Client.userList.Add(use);
                        }
                        Client.userList.Add(newnick);
                        Client.userList.Sort();


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

                        foreach (string userD in Client.userList)
                        {
                            if (removeUserMode(userD)!= removeUserMode(affectedUser))
                                userTemp.Add(userD);
       
                        }
                        Client.userList.Clear();

                        foreach (string userD in userTemp)
                        {
                            Client.userList.Add(userD);
                        }

                        switch (modechange)
                        {
                            case ("+o"):
                                Client.userList.Add("@" + affectedUser);
                                break;
                            case ("-o"):
                                Client.userList.Add(affectedUser);
                                break;
                            case ("+v"):
                                Client.userList.Add("+" + affectedUser);
                                break;
                            case ("-v"):
                                Client.userList.Add(affectedUser);
                                break;
                            case ("+h"):
                                Client.userList.Add("%" + affectedUser);
                                break;
                            case ("-h"):
                                Client.userList.Add(affectedUser);
                                break;
                            case ("+q"):
                                Client.userList.Add("~" + affectedUser);
                                break;
                            case ("-q"):
                                Client.userList.Add(affectedUser);
                                break;
                            case ("+a"):
                                Client.userList.Add("&" + affectedUser);
                                break;
                            case ("-a"):
                                Client.userList.Add(affectedUser);
                                break;
                        }

                        Who = affectedUser;
                        OnModeChange(EventArgs.Empty);
                        Client.userList.Sort();
                        userTemp.Clear();

                        break;

                    case ("KICK"): ;

                        userTemp = new List<string>();
                        string kickedUser = parameters[1];

                        foreach (string userR in Client.userList)
                        {
                            if (removeUserMode(userR) != removeUserMode(kickedUser))
                                userTemp.Add(userR);
                        }

                        Client.userList.Clear();

                        foreach (string userT in userTemp)
                        {
                            Client.userList.Add(userT);
                        }
                        Client.userList.Sort();

                        userTemp.Clear();
                        Who = kickedUser;
                        OnKick(EventArgs.Empty);
                        break;

                    case ("PRIVMSG"):
                        string user = prefix.Substring(0, prefix.IndexOf("!")); //Nick of the Sender
                        string whoSent = parameters[0];                         //Who sent is the source of the Message. (The Channel, or User if private Message)
                        string msg = parameters[1].Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                        string cmd = msg.Split(' ')[0];
                        string arg = "";

                        if (msg.Length - 1 > cmd.Length)
                            arg = msg.Substring(cmd.Length+1); //the rest of msg

                        //Write Message on Console
                        if (whoSent == Client.NICK)
                        {
                            if (user.Length > 14)
                                WriteMessage(user.Truncate(15) + " : " + msg, Color.DodgerBlue);
                            else if (user.Length >= 8)                       //Write the Message on the bot console
                                WriteMessage(user + "\t: " + msg, Color.DodgerBlue);
                            else
                                WriteMessage(user + "\t\t: " + msg, Color.DodgerBlue);
                        }
                        else if (msg.ToLower().Contains(Client.NICK.ToLower()))
                        {
                            if (user.Length > 14)
                                WriteMessage(user.Truncate(15) + " : " + msg, Color.LightGreen);
                            else if (user.Length >= 8)                       //Write the Message on the bot console
                                WriteMessage(user + "\t: " + msg, Color.LightGreen);
                            else
                                WriteMessage(user + "\t\t: " + msg, Color.LightGreen);
                        }
                        else
                        {
                            if (user.Length > 14)
                                WriteMessage(user.Truncate(15) + " : " + msg);
                            else if (user.Length >= 8)                       //Write the Message on the bot console
                                WriteMessage(user + "\t: " + msg);
                            else
                                WriteMessage(user + "\t\t: " + msg);
                        }

                        //StartParsing
                        if ((String.Compare(cmd, "hello", true) == 0
                                || String.Compare(cmd, "hi", true) == 0
                                || String.Compare(cmd, "hey", true) == 0)
                                && arg.ToLower().Contains(Client.NICK.ToLower()))
                            {
                                WriteMessage("* Received a hello from " + user, Color.Pink);
                                hello(whoSent, user);
                            }

                        else if (msg == "!anime best anime ever")
                            {
                                Client.messageSender(Privmsg(whoSent, "[25 episodes] [8,88 / 10] : Code Geass: Hangyaku no Lelouch -> http://myanimelist.net/anime/1575/Code_Geass:_Hangyaku_no_Lelouch"));
                            }

                        else if (String.Compare(cmd, Client.NICK+",", true) == 0 && !String.IsNullOrWhiteSpace(arg) && arg[arg.Length-1]=='?')
                            {
                                WriteMessage("* Received a question from " + user, Color.Pink);
                                parseQuestion(Client.HOME_CHANNEL, user, arg);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "help", true) == 0)
                            {
                                WriteMessage("* Received a help request from " + user, Color.Pink);
                                help(user);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "rules", true) == 0)
                            {
                                WriteMessage("* Received a rules request from " + user, Color.Pink);
                                rules(whoSent, user);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "eta", true) == 0)
                            {
                                WriteMessage("* Received a eta request from " + user, Color.Pink);
                                mangaETA(whoSent, user);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "quit", true) == 0)
                            {
                                WriteMessage("* Received a quit request from " + user, Color.Pink);
                                if(quitIRC(user)) OnQuit(EventArgs.Empty);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "oplist", true) == 0)
                            {
                                WriteMessage("* Received a oplist request from " + user, Color.Pink);
                                opList(user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "roll", true) == 0)
                            {
                                WriteMessage("* Received a roll request from " + user, Color.Pink);
                                roll(whoSent, user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "say", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a say request from " + user, Color.Pink);
                                say(Client.HOME_CHANNEL, arg, user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "greetme", true) == 0)
                            {
                                if (String.IsNullOrEmpty(arg))
                                {
                                    WriteMessage("* Received a greet TOOGLE request from " + user, Color.Pink);
                                    GreetToogle(user);
                                }
                                else
                                {
                                    WriteMessage("* Received a greet request from " + user, Color.Pink);
                                    AddGreetings(arg, user);
                                }
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "me", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a me request from " + user, Color.Pink);
                                me(Client.HOME_CHANNEL, arg, user);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "claims", true) == 0 || String.Compare(cmd, Client.SYMBOL + "c", true) == 0)
                            {
                                WriteMessage("* Received a claims request from " + user, Color.Pink);
                                claims(whoSent, user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "assignments", true) == 0 || String.Compare(cmd, Client.SYMBOL + "a", true) == 0)
                            {
                                WriteMessage("* Received a assignments request from " + user, Color.Pink);
                                assignments(whoSent, user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "silence", true) == 0)
                            {
                                WriteMessage("* Received a silence request from " + user, Color.Pink);
                                silence(user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "rename", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a rename request from " + user, Color.Pink);
                                if (isOperator(user)) changeNick(arg);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "op", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a op request from " + user, Color.Pink);
                                if (addBotOP(user, arg)) SaveOps();
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "deop", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a deop request from " + user, Color.Pink);
                                if (removeBotOP(user, arg)) SaveOps();
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "toF", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a temp. conversion to F request from " + user, Color.Pink);
                                toFahrenheit(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "toC", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a temp. conversion to C request from " + user, Color.Pink);
                                toCelcius(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "time", true) == 0)
                            {
                                WriteMessage("* Received a time request from " + user, Color.Pink);
                                time(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "wiki", true) == 0)
                            {
                                WriteMessage("* Received a explain request from " + user, Color.Pink);
                                explain(Client.HOME_CHANNEL, user, arg);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "anime", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a animeSearch request from " + user, Color.Pink);
                                animeSeach(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "poke", true) == 0)
                            {
                                WriteMessage("* Received a poke request from " + user, Color.Pink);
                                poke(Client.HOME_CHANNEL, user);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "trivia", true) == 0)
                            {
                                WriteMessage("* Received a trivia request from " + user, Color.Pink);
                                trivia(Client.HOME_CHANNEL, user);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "nick", true) == 0)
                            {
                                WriteMessage("* Received a nickname request from " + user, Color.Pink);
                                nickGen(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "kill", true) == 0)
                            {
                                WriteMessage("* Received a kill request from " + user, Color.Pink);
                                killUser(Client.HOME_CHANNEL, user, arg);
                            }
                        else if (msg.Contains("youtube") && msg.Contains("watch") && (msg.Contains("?v=") || msg.Contains("&v=")))
                            {
                                WriteMessage("* Detected an youtube video from  " + user, Color.Pink);
                                youtube(whoSent, user, msg, false);
                            }

                        else if (msg.Contains("youtu.be") && (msg.Contains("?v=") == false && msg.Contains("&v=") == false))
                            {
                                WriteMessage("* Detected a short youtube video from  " + user, Color.Pink);
                                youtube(whoSent, user, msg, true);
                            }

                        else if (msg.Contains("vimeo.f"))
                            {
                                WriteMessage("* Detected an vimeo video from  " + user, Color.Pink);
                                vimeo(whoSent, user, msg);
                            }

                        else if (msg.Contains("reddit.com") && msg.Contains("/r/") && msg.Contains("/comments/"))
                            {
                                WriteMessage("* Detected a reddit link from  " + user, Color.Pink);
                                redditLink(whoSent, user, msg);
                            }

                        else if (msg.Contains("twitter.com") && msg.Contains("/status/"))
                            {
                                WriteMessage("* Detected a twitter link from  " + user, Color.Pink);
                                twitter(whoSent, user, msg);
                            }

                        else if (message.Contains("\x01"))
                            {
                                if (cmd.Contains("VERSION"))
                                {
                                    WriteMessage("* Received a CTCP version request from " + user, Color.Pink);
                                    ctcpVersion(user);
                                }

                                if (cmd.Contains("TIME"))
                                {
                                    WriteMessage("* Received a CTCP time request from " + user, Color.Pink);
                                    ctcpTime(user);
                                }
                                if (cmd.Contains("PING"))
                                {
                                    WriteMessage("* Received a CTCP ping request from " + user, Color.Pink);
                                    ctcpPing(user, arg);
                                }
                            }

                        else //No parsing, just a normal Message
                            {
                                if (whoSent == Client.HOME_CHANNEL && msg != null)//Add to past messages
                                {
                                    pastMessage p = new pastMessage(user, msg);
                                    pastMessages.Add(p);
                                }
                            }

                        break;

                    case ("NOTICE"):
                        if (message.Contains("\x01"))
                        {
                            string userr;
                            if (prefix.Contains('!'))
                                userr = prefix.Substring(0, prefix.IndexOf("!"));   //Nick of the Sender
                            else userr = prefix;
                            //string whoSentt = parameters[0];                        //Who sent is the source of the Message. (The Channel, or User if private Message)
                            string msgg = parameters[1].Replace("\r", string.Empty).Replace("\n", string.Empty);
                            msgg=msgg.Trim();
                            string cmdd = msgg.Split(' ')[0];
                            string argg;
                            if (msgg.Length - 1 > cmdd.Length)
                                argg = msgg.Substring(cmdd.Length);                 //the rest of msg
                            else argg = "";
                            //string commandS = msgg.Substring(msgg.IndexOf("\x01") + 1, msgg.Length - 2);

                            if (cmdd.Contains("VERSION"))
                            {
                                WriteMessage("* Received a CTCP version request from " + userr, Color.Pink);
                                ctcpVersion(userr);
                            }

                            if (cmdd.Contains("TIME"))
                            {
                                WriteMessage("* Received a CTCP time request from " + userr, Color.Pink);
                                ctcpTime(userr);
                            }
                            if (cmdd.Contains("PING"))
                            {
                                WriteMessage("* Received a CTCP ping request from " + userr, Color.Pink);
                                
                                ctcpPing(userr, argg);
                            }
                        }
                        else WriteMessage(message, Color.DodgerBlue);
                        
                        break;

                    default:
                        WriteMessage("* " + command + " " + completeParameters);
                        break;
                }
            }
        }

        static void parseMessage(string message, out string prefix, out string command, out string[] parameters, out string completeParameters)
        {
            int prefixEnd = -1, trailingStart = message.Length;
            string trailing = null;
            prefix = command = String.Empty;
            parameters = new string[] { };

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

        static private char getUserMode(string user, List<string> userList)
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

        static private string removeUserMode(string user)
        {
            char[] usermodes = { '@', '+', '%', '~', '&'};
            
            if (user.Contains(user))
            {
                if (usermodes.Any((s) => Convert.ToChar(user.Substring(0, 1)).Equals(s)))
                {
                    return user.Substring(1).Trim();
                }
                else return user.Trim();
            }
            return "";
        }

        public void ReadKills()
        {
            kill.Clear();
            try
            {
                StreamReader sr = new StreamReader("TextFiles/kills.txt");
                while (sr.Peek() >= 0)
                {
                    kill.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void SaveOps()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/ops.txt", false))
            {
                foreach (string op in ops)
                {
                    newTask.WriteLine(op);
                }
            }
        }

        public void ReadOps()
        {
            ops.Clear();
            try
            {
                StreamReader sr = new StreamReader("TextFiles/ops.txt");
                while (sr.Peek() >= 0)
                {
                    ops.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void SaveBan()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/banned.txt", false))
            {
                foreach (string rl in ban)
                {
                    newTask.WriteLine(rl);
                }
            }
        }
        public void ReadBan()
        {
            ban.Clear();
            try
            {
                StreamReader sr = new StreamReader("TextFiles/banned.txt");
                while (sr.Peek() >= 0)
                {
                    ban.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void ReadRules()
        {
            rls.Clear();
            try
            {
                StreamReader sr = new StreamReader("TextFiles/rules.txt");
                while (sr.Peek() >= 0)
                {
                    rls.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void ReadHelp()
        {
            hlp.Clear();
            try
            {
                StreamReader sr = new StreamReader("TextFiles/help.txt");
                while (sr.Peek() >= 0)
                {
                    hlp.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void ReadTrivia() //Reads the trivia stuff
        {
            tri.Clear();

            try
            {
                StreamReader sr = new StreamReader("TextFiles/trivia.txt");
                while (sr.Peek() >= 0)
                {
                    tri.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void ReadGreetings()
        {
            string nick, greeting, line;
            string[] split;
            bool enabled = false;
            bool found = false;

            if (File.Exists("TextFiles/greetings.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/greetings.txt");
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        split = line.Split(new char[] { ':' }, 3);
                        nick = split[0];
                        greeting = split[2];

                        switch (split[1])
                        {
                            case "False":
                                enabled = false;
                                break;
                            case "True":
                                enabled = true;
                                break;
                            default:
                                enabled = false;
                                break;
                        }

                        foreach (Greetings g in greet)//avoid adding duplicate nicks
                        {
                            if (g.Nick == nick)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            Greetings g = new Greetings(nick, greeting, enabled);
                            greet.Add(g);
                        }

                    }
                    sr.Close();
                }
                catch
                {
                }

            }
        }


        public void AddGreetings(string args, string nick)
        {
            bool found = false;

            foreach (Greetings g in greet)
            {
                if (g.Nick == nick)
                {
                    found = true;
                    g.Greetingg = args;
                    SaveGreetings();
                }
            }

            if (!found)
            {
                Greetings g = new Greetings(nick, args, true);
                greet.Add(g);
                SaveGreetings();
            }


        }

        public void SaveGreetings()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/greetings.txt", false))
            {
                foreach (Greetings gg in greet)
                {
                    newTask.WriteLine(gg.Nick + ":" + gg.Enabled.ToString() + ":" + gg.Greetingg);
                }
            }

        }

        void GreetToogle(string nick)
        {
            string message = Notice(nick, "You didn't set a greeting yet"); ;
            string state = "disabled";
            bool found = false;


            foreach (Greetings g in greet)
            {
                if (g.Nick == nick && !found)
                {
                    found = true;
                    g.Enabled = !g.Enabled;
                    if (g.Enabled) state = "enabled";

                    message = Notice(nick, "Your greeting is now " + state);
                    SaveGreetings();
                }
            }

            Client.messageSender(message);
        }

        public void LoadNickGenStrings()//These are for the nick gen
        {
            nickGenStrings = new List<string>();
            nickGenStrings.Clear();

            try
            {
                StreamReader sr = new StreamReader("TextFiles/nickGen.txt");
                while (sr.Peek() >= 0)
                {
                    nickGenStrings.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends a Message to the destinatary
        /// </summary>
        /// <param Name="destinatary">string with either a User or a channel, it's where the Message will be sent</param>
        /// <param Name="Message">String of text with the Message that will be delivered</param>
        public string Privmsg(string destinatary, string message)
        {
            string result;

            result = "PRIVMSG " + destinatary + " :" + message + "\r\n";

            if (Client.NICK.Length > 15)
            {
                WriteMessage(Client.NICK.Truncate(16) + ":" + message);
            }
                
            else if (Client.NICK.Length >= 8)                       //Write the Message on the bot console
            {
                WriteMessage(Client.NICK + "\t: " + message);
            }
            else {
                WriteMessage(Client.NICK + "\t\t: " + message);
            }
                

            return result;
        }

        /// <summary>
        /// Sends a notice to the destinatary
        /// </summary>
        /// <param Name="destinatary">string with either a User or a channel, it's where the Message will be sent</param>
        /// <param Name="Message">String of text with the Message that will be delivered</param>
        public string Notice(string destinatary, string message)
        {
            string result;

            result = "NOTICE " + destinatary + " :" + message + "\r\n";

            if (Client.NICK.Length > 15){
                WriteMessage(Client.NICK.Truncate(16) + ":" + message);
            }
                
            else if (Client.NICK.Length >= 8)                       //Write the Message on the bot console
            {
                WriteMessage(Client.NICK + "\t: " + message);
            }
                
            else
            {
                WriteMessage(Client.NICK + "\t\t: " + message);
            }
                

            return result;
        }

        public void pokeUser(string nick)
        {
            string message = Privmsg(Client.HOME_CHANNEL, "\x01" + "ACTION stabs " + nick + " with a sharp knife" + "\x01");
            Client.messageSender(message);

        }
        public void whoisUser(string nick)
        {
            string message = "WHOIS " + nick + "\n";
            Client.messageSender(message);

        }

        public void randomTextSender(object source, ElapsedEventArgs e)
        {
            pastMessages.Sort((p, q) => p.WordsCount.CompareTo(q.WordsCount));
            List<pastMessage> pastTemp = new List<pastMessage>();

            foreach (pastMessage p in pastMessages)//remove messages with less than 8 Words
            {
                if (p.WordsCount >= 8)
                    pastTemp.Add(p);
            }
            pastMessages.Clear();
            foreach (pastMessage p in pastTemp)
            {
                pastMessages.Add(p);
            }

            if (pastMessages.Count < 10 || !Settings.Default.randomTextEnabled) return;

            string message;
            string randomWords;
            StringBuilder g = new StringBuilder();
            Random rd = new Random();

            int numberSamples = rd.Next(2, 4);      //Number of messages to sample from
            int count;
            int n;
            int d;

            for (count = 0; count <= numberSamples; count++)
            {

                n = rd.Next(pastMessages.Count);

                int start = rd.Next(pastMessages[n].WordsCount - 5);
                int end;
                do { end = rd.Next(pastMessages[n].WordsCount); } while (end <= start);

                string[] words = pastMessages[n].GetWords();
                for (d = start; d < end; d++)
                {
                    g.Append(words[d] + " ");

                }
                pastMessages.Remove(pastMessages[n]);
            }
            randomWords = g.ToString();

            message = Privmsg(Client.HOME_CHANNEL, "\x02" + randomWords + "\x02");
            Client.messageSender(message);

            //pastMessages.Clear();

        }

        ///////////////////////////////////
        //       Commands Functions      //
        ///////////////////////////////////

        /// <summary>
        /// Adds a nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be made bot operator</param>
        bool addBotOP(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!isOperator(nick))
                return false;

            if (giveOps(targetUser))
            {
                message = Notice(nick, targetUser + " was added as a bot operator!");
                Client.messageSender(message);
                return true;
            }
            else
            {
                message = Notice(nick, "Error: " + targetUser + " is already a bot operator!");
                Client.messageSender(message);
                return false;
            }
        }
        /// <summary>
        /// Removes a nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be removed from the bot operator list</param>
        bool removeBotOP(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!isOperator(nick))
                return false;

            if (takeOps(targetUser))
            {
                message = Notice(nick, targetUser + " was removed as a bot operator!");
                Client.messageSender(message);
                return true;
            }
            else
            {
                message = Notice(nick, "Error: " + targetUser + " was not a bot operator!");
                Client.messageSender(message);
                return false;
            }
        }
        void opList(string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = Notice(nick, "Bot operators:");
                Client.messageSender(message);
                foreach (string p in ops)
                {
                    message = Notice(nick, nick + " :->" + p);
                    Client.messageSender(message);
                }
            }
        }

        void say(string CHANNEL, string args, string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = Privmsg(CHANNEL, args);
                Client.messageSender(message);
                return;
            }
        }

        void me(string CHANNEL, string args, string nick)
        {
            string message;
            if (isOperator(nick))
            {
                message = Privmsg(CHANNEL, "\x01" + "ACTION " + args + "\x01");
                Client.messageSender(message);
                return;

            }

        }
        public void silence(string nick)
        {
            string message;
            if (isOperator(nick))
            {
                if (Settings.Default.silence == true)
                {

                    OnUnsilence(EventArgs.Empty);
                    message = Notice(nick, "The bot was unmuted");
                }
                else
                {
                    OnSilence(EventArgs.Empty);
                    message = Notice(nick, "The bot was muted");
                }

                Client.messageSender(message);
                return;
            }
        }

        void hello(string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.hello_Enabled == true && Settings.Default.silence == false)
            {
                string message = Privmsg(CHANNEL, "Hello " + nick + "!");
                Client.messageSender(message);
            }
        }

        void help(string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.help_Enabled == true)
            {
                foreach (string h in hlp)
                {
                    message = Notice(nick, h.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);

                }
            }
        }
        void rules(string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.rules_Enabled == true)
            {
                if (isOperator(nick))
                {
                    foreach (string h in rls)
                    {
                        message = Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                        Client.messageSender(message);

                    }
                    return;
                }

            }
            else if (Settings.Default.rules_Enabled == true)
            {
                foreach (string h in ops)
                {
                    message = Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);
                    return;
                }
            }
        }
        void mangaETA(string CHANNEL, string nick)
        {
            if (Settings.Default.eta_Enabled == true)
            {
                if (isMuted(nick)) return;

                string message;
                if (Settings.Default.silence == true)
                {
                    message = Notice(nick, "The average time for the chapter release is " + eta + ".");
                    Client.messageSender(message);
                }
                else
                {
                    message = Privmsg(CHANNEL, "The average time for the chapter release is " + eta + ".");
                    Client.messageSender(message);
                }
            }
        }

        void roll(string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.roll_Enabled == true)
            {
                Random random = new Random();
                int number = random.Next(0, 100);

                nick = nick.Replace("\r", "");
                string message = Privmsg(CHANNEL, nick + " rolled a " + number);
                Client.messageSender(message);
            }
        }
        private void poke(string CHANNEL, string nicks)
        {
            string message;
            int userNumber = 0;
            Random rnd = new Random();

            if (isMuted(nicks)) return;

            do
            {
                userNumber = rnd.Next((Client.userList.Count - 1));
            }

            while (removeUserMode(Client.userList[userNumber]) == nicks);

            if (Settings.Default.silence == false && Settings.Default.pokeEnabled == true)
            {
                message = Privmsg(CHANNEL, "\x01" + "ACTION " + "pokes " + Client.userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) + "\x01");
                Client.messageSender(message);
            }

        }

        void assignments(string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.assign_Enabled == true)
            {
                if (isOperator(nick))
                {
                    message = Privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.assign_Enabled == true)
            {
                message = Privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
                Client.messageSender(message);

            }
        }
        void claims(string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.claims_Enabled == true)
            {

                if (isOperator(nick))
                {
                    message = Privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.claims_Enabled == true)
            {

                message = Privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
                Client.messageSender(message);

            }
        }

        void toFahrenheit(string CHANNEL, string nick, string args)
        {
            string message;
            double f = 0;
            double cc = 0;

            try
            {
                cc = Convert.ToDouble(args);
                f = ((9.0 / 5.0) * cc) + 32;
            }
            catch
            {
                message = Privmsg(CHANNEL, "Could not parse arguments");
                Client.messageSender(message);
                return;
            }

            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = Privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
                    Client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.conversionEnabled == true)
            {
                message = Privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
                Client.messageSender(message);

            }
        }
        void toCelcius(string CHANNEL, string nick, string args)
        {
            string message;
            double cc = 0;
            double f = 0;

            if (isMuted(nick)) return;

            try
            {
                f = Convert.ToDouble(args);
                cc = (5.0 / 9.0) * (f - 32);
            }
            catch
            {
                message = Privmsg(CHANNEL, "Could not parse arguments");
                Client.messageSender(message);
                return;
            }

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = Privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                    Client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.conversionEnabled == true)
            {

                message = Privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                Client.messageSender(message);

            }
        }

        public void trivia(string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;
            if (tri.Count == 0) return;
            if (Settings.Default.silence == false && Settings.Default.triviaEnabled == true)
            {
                string message;
                Random rnd = new Random();
                int rt = rnd.Next(tri.Count - 1);

                message = Privmsg(CHANNEL, tri[rt]);
                Client.messageSender(message);

            }
        }

        void time(string CHANNEL, string nick, string args)
        {
            string message;
            string timezoneS;
            string location = "";
            bool wantUTC = false;
            bool sensor = false;
            bool invalid = false;
            string requestURL;

            DateTime convertedTime;

            GoogleTimeZone g = new GoogleTimeZone();
            string json;

            if (isMuted(nick)) return;

            args = args.Replace("\r", string.Empty);
            args = args.Replace("\n", string.Empty);
            args = args.Replace(" ", string.Empty);

            string IST = "23.7833, 85.9667";            //Bokaro, india
            string MSK = "55.74941,37.614441";          //Moscow
            string FET = "53.895311,27.563324";         //Minsk
            string EET = "44.4476304,26.0860545";       //bucharest
            string CET = "48.8588589,2.3470599";        //paris

            string WER = "38.7436266,-9.1602038";       //lisbon
            string GMT = "51.5232391,-0.1166146";       //london

            string BRT = "-23.5778896,-46.6096585";     //sao paulo
            string ART = "-34.6158526,-58.4332985";     //buenos aires
            string AST = "53.3215407,-60.3542792";      //Happy+Valley-Goose+Bay
            string VET = "10.4683917,-66.8903658";      //caracas
            string EST = "40.7056308,-73.9780035";      //NYC
            string CST = "39.091919,-94.5757195";       //kansas city
            string MST = "40.7609881,-111.8936263";     //salt lake city
            string PST = "34.0469605,-118.2621293";     //LA
            string AKDT = "61.1878492,-149.8158133";    //Anchorage

            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;

            DateTime currentUTC = localZone.ToUniversalTime(currentDate);

            double timestamp = ConvertToUnixTimestamp(currentUTC);

            if (Settings.Default.silence == false && Settings.Default.timeEnabled == true)
            {
                switch (args.Replace("\r", string.Empty).ToLower())
                {
                    case "ist": location = IST; timezoneS = "IST";
                        break;
                    case "msk": location = MSK; timezoneS = "MSK";
                        break;
                    case "fet": location = FET; timezoneS = "FET";
                        break;

                    case "eet": location = EET; timezoneS = "EET";
                        break;
                    case "stillbutterfly": location = EET; timezoneS = "EET";
                        break;
                    case "romania": location = EET; timezoneS = "EET";
                        break;

                    case "cet": location = CET; timezoneS = "CET";
                        break;
                    case "wer": location = WER; timezoneS = "WER";
                        break;

                    case "gmt": location = GMT; timezoneS = "GMT";
                        break;
                    case "utc": wantUTC = true; timezoneS = "UTC";
                        break;
                    case "masterrace": location = GMT; timezoneS = "GMT";
                        break;

                    case "brt": location = BRT; timezoneS = "BRT";
                        break;
                    case "art": location = ART; timezoneS = "ART";
                        break;
                    case "ast": location = AST; timezoneS = "AST";
                        break;
                    case "vet": location = VET; timezoneS = "VET";
                        break;
                    case "est": location = EST; timezoneS = "EST";
                        break;
                    case "cst": location = CST; timezoneS = "CST";
                        break;

                    case "mst": location = MST; timezoneS = "MST";
                        break;
                    case "jhoudiey": location = MST; timezoneS = "MST";
                        break;

                    case "pst": location = PST; timezoneS = "PST";
                        break;
                    case "akdt": location = AKDT; timezoneS = "AKDT";
                        break;
                    case "": location = GMT; timezoneS = "GMT";
                        break;
                    default: location = GMT; timezoneS = "GMT"; invalid = true;
                        break;
                }

                if (!wantUTC)
                {
                    requestURL = "https://maps.googleapis.com/maps/api/timezone/json?location=" + location + "&timestamp=" + timestamp + "&sensor=" + sensor.ToString().ToLower() + "&key=" + Settings.Default.apikey;
                    var webClient = new WebClient();
                    webClient.Encoding = Encoding.UTF8;
                    try
                    {
                        json = webClient.DownloadString(requestURL);
                        JsonConvert.PopulateObject(json, g);

                    }
                    catch { }

                    convertedTime = ConvertFromUnixTimestamp(timestamp + g.DSTOffset + g.RawOffset);

                }
                else
                    convertedTime = currentUTC;

                if (invalid)
                    if (args.Replace("\r", string.Empty).ToLower() == "2blaze" || args.Replace("\r", string.Empty).ToLower() == "2blaze1" || args.Replace("\r", string.Empty).ToLower() == "toblaze")
                        message = Privmsg(CHANNEL, "4:20");
                    else if (args.Replace("\r", string.Empty).ToLower() == "alan_jackson" || args.Replace("\r", string.Empty).ToLower() == "alan" || args.Replace("\r", string.Empty).ToLower() == "alanjackson")
                        message = Privmsg(CHANNEL, "5:00");
                    else
                        message = Privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS + ". \"" + args + "\" is an invalid argument");
                else
                    message = Privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS);
                Client.messageSender(message);

            }
        }

        public void youtube(string CHANNEL, string nick, string line, bool isShort)
        {
            if (String.IsNullOrEmpty(line)) return;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.youtube_Enabled == true)
            {
                string[] bah;
                string ID;
                string message;

                if (!isShort)
                {
                    if (line.Contains("?v="))
                    {
                        ID = getBetween(line, "?v=", "&");
                    }
                    else
                    {
                        ID = getBetween(line, "&v=", "&");
                    }
                }
                else {

                    ID = getBetween(line, "youtu.be/", "?t");
                }


                bah = ID.Split(new char[] { ' ' }, 2);
                ID = bah[0];

                string title, duration;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int temp = 0;
                string URLString = "http://gdata.youtube.com/feeds/api/videos/" + ID;

                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string readHtml = webClient.DownloadString(URLString);

                title = getBetween(readHtml, "<title type='text'>", "</title>");
                duration = getBetween(readHtml, "<yt:duration seconds='", "'/>");

                title = WebUtility.HtmlDecode(title);

                temp = Convert.ToInt32(duration);

                while (temp >= 60)
                {
                    minutes++;
                    temp = temp - 60;

                }
                seconds = temp;

                if (minutes >= 60)
                {
                    temp = minutes;
                    while (temp >= 60)
                    {
                        hours++;
                        temp = temp - 60;

                    }
                    minutes = temp;
                    message = Privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message); ;
                }
                else
                {
                    message = Privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);
                }
            }
        }
        public void twitter(string CHANNEL, string nick, string line)
        {
            string author, tweet, message;

            if (isMuted(nick)) return;

            if (Settings.Default.silence == true || Settings.Default.twitterEnabled == false) return;
            else
            {

                string ID = getBetween(line, "/status/", "/");
                long tweetID = Convert.ToInt64(ID);

                TwitterStatus tweetResult = service.GetTweet(new GetTweetOptions { Id = tweetID });

                author = tweetResult.Author.ScreenName;
                tweet = tweetResult.Text.Replace("\n", " ");


                message = Privmsg(CHANNEL, "Tweet by @" + author + " : " + tweet);
                
                Client.messageSender(message);
            }
        }

        public void animeSeach(string CHANNEL, string nick, string line)
        {
            string message="";

            if (isMuted(nick))
            {
                WriteMessage(nick + " is ignored", Color.BlanchedAlmond);
                return;
            }
            if (Settings.Default.silence == true || Settings.Default.aniSearchEnabled == false)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(line)) return;

            GoogleSeach g = new GoogleSeach();
            anime a = new anime();

            string jsonGoogle;
            string jsonAnime;
            bool user = false;

            if (line.Contains("-u") || line.Contains("-User")) user = true;

            line = line.Replace(" ", "%20").Replace(" -User", "%20").Replace(" -u", "%20");


            string getString = "https://www.googleapis.com/customsearch/v1?key=" + Settings.Default.apikey + "&cx=" + Settings.Default.cxKey + "&q=" + line;

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            webClient.Credentials = new NetworkCredential(Settings.Default.malUser, Settings.Default.malPass);
            webClient.Headers.Add("User-agent", "NarutoBot3");

            string name = "";

            try
            {
                jsonGoogle = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonGoogle, g);
            }
            catch { }

            if (g.items == null) message = Privmsg(CHANNEL, "Could not find anything, try http://myanimelist.net/anime.php?q=" + line);
            else
            {
                int i_max = 0; int i = 0; bool found = false;

                if (g.items.Length < 4)
                    i_max = g.items.Length - 1;
                else i_max = 4;

                while (i <= i_max && found == false)
                {
                    if (!user)
                    {
                        if (g.items[i].link.Contains("http://myanimelist.net/anime/"))
                        {
                            found = true;
                            string[] split = g.items[i].link.Split('/');
                            if (split.Length <= 5)
                                name = g.items[i].link + "/" + line;
                            else
                                name = g.items[i].link;
                        }
                        else i++;
                    }
                    else
                    {
                        if (g.items[i].link.Contains("http://myanimelist.net/profile/"))
                        {
                            found = true;
                        }
                        else i++;
                    }


                }

                if (!found) message = Privmsg(CHANNEL, g.items[0].link);
                else
                    if (!user)
                    {
                        string[] animeName = name.Replace("http://myanimelist.net/anime/", string.Empty).Replace(" ", "+").Replace("_", "+").Split('/');
                        getString = "http://myanimelist.net/api/anime/search.xml?q=" + animeName[1];

                        try
                        {
                            jsonAnime = webClient.DownloadString(getString);
                            XmlSerializer serializer = new XmlSerializer(typeof(anime));
                            using (StringReader reader = new StringReader(jsonAnime))
                            {
                                a = (anime)(serializer.Deserialize(reader));
                            }
                        }
                        catch { }

                        if (a == null)
                        {
                            #if DEBUG
                                message = Privmsg(CHANNEL, "Error: a=null");
                            #endif

                        }
                        else
                        {
                            string score = a.entry[0].score.ToString();
                            string episodes = a.entry[0].episodes.ToString();
                            string title = a.entry[0].title;

                            if (episodes == "0")
                                episodes = "?";

                            message = Privmsg(CHANNEL, "[" + episodes + " episodes] " + "[" + score + " / 10] : " + "\x02" + title + "\x02" + " -> " + g.items[i].link);
                        }

                    }
                    else
                    {
                        string readHtml = webClient.DownloadString(g.items[i].link.Replace("recommendations", string.Empty).Replace("reviews", string.Empty).Replace("clubs", string.Empty).Replace("friends", string.Empty));

                        string profile = getBetween(readHtml, "<title>", "'s Profile - MyAnimeList.net</title>");

                        string completed = getBetween(readHtml, ">Completed</span></td>", "<td><div style=");
                        completed = getBetween(completed, "<td align=\"center\">", "</td>");


                        message = Privmsg(CHANNEL, "[" + profile + "] " + "Completed " + completed + " animes" + " -> " + g.items[i].link.Replace("recommendations", string.Empty).Replace("reviews", string.Empty).Replace("clubs", string.Empty).Replace("friends", string.Empty));
                    }
            }

            Client.messageSender(message);
        }

        public void vimeo(string CHANNEL, string nick, string line)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.vimeoEnabled == true)
            {
                string title, duration;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int temp = 0;

                string message;
                string ID = getBetween(line, "vimeo.com/", "/");
                string URLString = "http://vimeo.com/api/v2/video/" + ID.Replace("\r", "").Replace("\n", "") + ".xml";

                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string readHtml = webClient.DownloadString(URLString);

                title = getBetween(readHtml, "<title>", "</title>");
                duration = getBetween(readHtml, "<duration>", "</duration>");

                temp = Convert.ToInt32(duration);

                while (temp >= 60)
                {
                    minutes++;
                    temp = temp - 60;

                }
                seconds = temp;

                if (minutes >= 60)
                {
                    temp = minutes;
                    while (temp >= 60)
                    {
                        hours++;
                        temp = temp - 60;

                    }
                    minutes = temp;
                    message = Privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);

                }
                else
                {
                    message = Privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);
                }

            }
        }
        public void killUser(string CHANNEL, string nick, string args)
        {
            if (String.IsNullOrEmpty(nick)) return;
            Random r = new Random();
            if (isMuted(nick)) return;
            string target;

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                string message;
                if (args.ToLower() == "la kill".Trim())
                {
                    message = Privmsg(CHANNEL, nick + " lost his way");
                }
                
                else
                {
                    if (String.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                        target = removeUserMode(Client.userList[r.Next((Client.userList.Count - 1))]);
                    else
                        target = args.Trim();

                    string killString = kill[r.Next(kill.Count)].Replace("<target>", target).Replace("<user>", nick.Trim());

                    if (killString.ToLower().Contains("<normal>"))
                        message = Privmsg(CHANNEL, killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty));
                    else
                        message = Privmsg(CHANNEL, "\x01" + "ACTION " + killString + "\x01");
                }
                Client.messageSender(message);

            }
        }

        private void parseQuestion(string CHANNEL, string user, string arg)
        {
            string message="";
            Random r = new Random();
            StreamWriter w = new StreamWriter("TextFiles/questions.txt", true);

            if (isMuted(user)) return;
            if (!Settings.Default.questionEnabled) return;

            arg = arg.ToLower().Replace("?", string.Empty).TrimStart(new char[]{' '});
            string[] split = arg.Split(new char[] { ' ' });

            if (split.Length >= 1)
            {

                if (String.Compare(split[0], "how", true) == 0)
                {
                    if (split.Length >= 2)
                    {
                        if (String.Compare(split[1], "many", true) == 0)
                        {
                            if (arg == "how many killstrings do you have" || arg == "how many kills do you have")
                            {
                                message = Privmsg(CHANNEL, "I have " + kill.Count + " killstrings loaded in.");
                            
                            }
                            else if (arg == "how many fucks do you give")
                            {
                                message = Privmsg(CHANNEL, "I always give 0 fucks.");

                            }
                            else
                            {
                                message = Privmsg(CHANNEL, "I dont know, maybe something like " + r.Next(50));
                                w.WriteLine(arg);
                            }
                                

                        }
                        else if (String.Compare(split[1], "are", true) == 0)
                        {
                            if (arg == "how are you" || arg == "how are you doing")
                            {
                                message = Privmsg(CHANNEL, "I'm fine, thanks for asking. And you?");

                            }
                            else
                            {
                                message = Privmsg(CHANNEL, "I dont know yet, ask later");
                                w.WriteLine(arg);
                            }

                        }
                        else if (String.Compare(split[1], "is", true) == 0)
                        {
                            string[] howIs = {"fine", "not fine", "lost", "being retarded again"};
                            if (split.Length == 3)
                            {
                                string subject = split[2];
                                message = Privmsg(CHANNEL, subject+" is "+howIs[r.Next(howIs.Length-1)]);
                            }
                            else
                            {
                                w.WriteLine(arg);
                            }

                        }
                        else w.WriteLine(arg);
                    }
                    else
                    {
                        message = Privmsg(CHANNEL, user + ", no idea...");
                        w.WriteLine(arg);
                        Client.messageSender(message);

                    }

                }

                if (String.Compare(split[0], "how's", true) == 0)
                {
                    message = Privmsg(CHANNEL, user + ", no idea...");
                    w.WriteLine(arg);
                    Client.messageSender(message);
                }
                else if (String.Compare(split[0], "why", true) == 0)
                {
                    string[] because = { "was lost", "is stupid", "asked me to", "was asked to", "has an inferiority complex", "is a terrible person"};

                    if (split.Length >= 2)
                    {
                        message = Privmsg(CHANNEL, "Because " + removeUserMode(Client.userList[r.Next(Client.userList.Count)-1]) + " " + because[r.Next(because.Length - 1)]);
                    }
                    else
                    {
                        w.WriteLine(arg);
                    }
                }
                else if (String.Compare(split[0], "is", true) == 0)
                {
                    string[] whyY = { "Im not sure if", "Yeah", "Yes", "Correct" };
                    string[] whyN = { "Nope", "No" };
                    bool yes = false;
                    if (r.Next(1, 3) == 1)
                        yes = true;

                    if (split.Length >= 2)
                    {
                        string subject = split[1];
                        string rest = "";

                        for (int i = 2; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ');

                        string replaced = questionsRegex(rest);

                        if (yes)
                        {

                            message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)] + " " + subject.Replace("your","my") + " is " + replaced);
                            
                        }
                        else
                        {
                            message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)] + ", " + subject.Replace("your","my")  + " isn't " + replaced);
                        }

                    }
                    else
                    {
                        if (yes)
                            message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)]);
                        else
                            message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)]);

                    }
                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "when", true) == 0)
                {
                    string[] when = { "maybe next week", "a few days ago", "last year", "yesterday" , "tomorrow", "in a few hours", "nobody knows" };

                    message = Privmsg(CHANNEL, when[r.Next(when.Length - 1)]);

                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "are", true) == 0)
                {
                    if (arg == "are you real")
                    {
                        message = Privmsg(CHANNEL, "Yes, i am real");

                    }
                    else if (arg == "are you a real person" || arg == "are you a real human" || arg == "are you human")
                    {
                        message = Privmsg(CHANNEL, "No, i'm a bot");

                    }
                    else
                    {
                        string[] are = { "I dont know, maybe...", "Yeah..", "Nope.", "Yes.", "No." , "Probably"};
                        message = Privmsg(CHANNEL, are[r.Next(are.Length - 1)]);
                        w.WriteLine(arg);
                    }
                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "can", true) == 0)
                {
                    if (arg == "can you give me a nick" || arg == "can you make me a nick" || arg == "can you generate a nick" || arg == "can you create a nick" || arg == "can you make me a new nick")
                    {
                        message = Privmsg(CHANNEL, "Yes, here it is: " + NickGen.NickG(nickGenStrings, nickGenStrings.Count, false, false, false, false));

                    }
                    else if (arg.Contains("can you kill "))
                    {
                        killUser(CHANNEL, user, getBetween(arg, "can you kill ", ""));
                    }
                    else
                    {
                        string[] why = { "I dont know, maybe...", "Yeah..", "Nope.", "Yes.", "No.", "Probably" };
                        message = Privmsg(CHANNEL, why[r.Next(why.Length - 1)]);
                        w.WriteLine(arg);
                    }
                }
                else if (String.Compare(split[0], "would", true) == 0)
                {
                    if (arg == "would you make me a nick" || arg == "would you generate a nick" || arg == "would you create a nick" || arg == "would you make me a new nick")
                    {
                        message = Privmsg(CHANNEL, "Yes, here it is: " + NickGen.NickG(nickGenStrings, nickGenStrings.Count, false, false, false, false));

                    }
                    else
                    {
                        string[] why = { "I dont know, maybe...", "Yeah..", "Nope.", "Yes.", "No.", "Probably" };
                        message = Privmsg(CHANNEL, why[r.Next(why.Length - 1)]);
                        w.WriteLine(arg);
                    }
                }
                else if (String.Compare(split[0], "where", true) == 0)
                {

                    string[] where = { "somewhere in a far away land" , "on the youtube datacenter", "behind you", "in your house", "in Europe", "near Lygs", "that special place" , "in outer space" };

                    message = Privmsg(CHANNEL, where[r.Next(where.Length - 1)]);

                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "who", true) == 0)
                {
                    if (arg == "who are you")
                    {
                        message = Privmsg(CHANNEL, "I'm a bot!");

                    }
                    else
                    {
                        string randomUser = removeUserMode(Client.userList[r.Next(Client.userList.Count) - 1]);
                        string[] who = { "probabbly", "maybe it was", "i'm sure it was", "it wasn't" };

                        message = Privmsg(CHANNEL, who + " " + randomUser);

                        w.WriteLine(arg);
                    }

                }
                else if (String.Compare(split[0], "what", true) == 0)
                {
                    if (arg == "what are you")
                    {
                        message = Privmsg(CHANNEL, "I'm a bot!");

                    }

                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "what's", true) == 0)
                {

                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "if", true) == 0)
                {

                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "do", true) == 0)
                {
                    string[] whyY = { "Im not sure if", "Yeah", "Yes", "Correct" };
                    string[] whyN = { "Nope", "No" };
                    bool yes = false;
                    if (r.Next(1, 3) == 1)
                        yes = true;

                    if (split.Length >= 2)
                    {
                        string subject = split[1];
                        string rest = "";

                        for (int i = 2; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ');

                        string replaced = questionsRegex(rest);

                        if (split[1] == "you")
                        {
                            
                            if (yes)
                            {

                                

                                message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)] + " " + "I do " + replaced);
                            }
                            else
                            {
                                message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)] + ", " + "I don't " + replaced);
                            }

                        }
                    }
                    else
                    {
                        if (yes)
                            message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)]);
                        else
                            message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)]);
                    
                    }
                    
                    
                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "did", true) == 0)
                {

                    string[] why = { "I dont know, maybe...", "Yeah..", "Nope.", "Yes.", "No.", "Probably" };
                    message = Privmsg(CHANNEL, why[r.Next(why.Length - 1)]);
                    w.WriteLine(arg);
                }
                else if (String.Compare(split[0], "does", true) == 0)
                {
                    string[] whyY = { "Im not sure if", "Yeah", "Yes", "Correct" };
                    string[] whyN = { "Nope", "No" };
                    bool yes = false;
                    if (r.Next(1, 3) == 1)
                        yes = true;

                    if (split.Length >= 2)
                    {
                        string subject = split[1];
                        string rest = "";

                        for (int i = 2; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ');

                        string replaced = questionsRegex(rest);

                        if (yes)
                        {

                            message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)] + " " + subject + " does " + replaced);
                            
                        }
                        else
                        {
                            message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)] + ", " + subject + " does not " + replaced);
                        }

                    }
                    else
                    {
                        if (yes)
                            message = Privmsg(CHANNEL, whyY[r.Next(whyY.Length - 1)]);
                        else
                            message = Privmsg(CHANNEL, whyN[r.Next(whyN.Length - 1)]);

                    }

                }
            }
            else
            {
                message = Privmsg(CHANNEL, user + ", what?");
                w.WriteLine(arg);
            
            }

            if(!String.IsNullOrWhiteSpace(message)) Client.messageSender(message);
            w.Close();
        }

        public void nickGen(string CHANNEL, string nick, string args)
        {
            if (String.IsNullOrEmpty(nick)) return;
            Random rnd = new Random();

            bool randomnumber = false;
            bool randomUpper = false;
            bool switchLetterNumb = false;
            bool Ique = false;
            string message;

            if (isMuted(nick)) return;

            if (nickGenStrings.Count < 2)
            {
                message = Privmsg(CHANNEL, "Nicks was not initialized properly");
                Client.messageSender(message);
                return;
            }

            if (Settings.Default.silence == false && Settings.Default.nickEnabled == true)
            {
                if (args.Contains("random"))
                {
                    if (rnd.Next(0, 100) <= 30)
                        switchLetterNumb = true;

                    if (rnd.Next(0, 100) <= 30)
                        randomnumber = true;

                    if (rnd.Next(0, 100) <= 30)
                        randomUpper = true;

                    if (rnd.Next(0, 100) <= 10)
                        Ique = true;
                }
                else
                {
                    if (args.Contains("SL") == true)
                        switchLetterNumb = true;

                    if (args.Contains("RN") == true)
                        randomnumber = true;

                    if (args.Contains("RU") == true)
                        randomUpper = true;

                    if (args.Contains("IQ") == true)
                        Ique = true;

                }

                //gen = new NickGen();
                string nick_ = NickGen.NickG(nickGenStrings, nickGenStrings.Count, randomnumber, randomUpper, switchLetterNumb, Ique);

                message = Privmsg(CHANNEL, nick + " generated the nick " + nick_);
                Client.messageSender(message);
            }
        }

        public void redditLink(string CHANNEL, string nick, string line)
        {
            if (String.IsNullOrEmpty(line) || String.IsNullOrEmpty(nick)) return;

            string[] temp = line.Split(' ');
            string subreddit = "";
            string url = "";
            string message;

            RedditSharp.Things.Post post;
            RedditSharp.Things.Subreddit sub;
            RedditSharp.Things.Comment comment;

            if (isMuted(nick)) return;

            foreach (string st in temp)
            {
                if (st.Contains("reddit.com") && st.Contains("/r/") && st.Contains("/comments/"))
                {
                    url = st;
                    url = url.Replace("http://", string.Empty).Replace("https://", string.Empty);
                }
                else return;
            }

            subreddit = getBetween(url, "/r/", "/");

            try
            {
                sub = reddit.GetSubreddit("/r/" + subreddit);

                string[] linkParse = url.Replace("\r", string.Empty).Split('/');

                if (linkParse.Length >= 7 && !String.IsNullOrEmpty(linkParse[6]))    //With Comment
                {
                    string postName = linkParse[4];
                    string commentName = linkParse[6].Split(new char[] { '?' }, 2)[0];

                    post = (RedditSharp.Things.Post)reddit.GetThingByFullname("t3_" + postName);
                    comment = reddit.GetComment(sub.Name, "t1_" + commentName, "t3_" + postName);

                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                    Client.messageSender(message);

                    if (comment.Body.ToString().Length > 250)
                        message = Privmsg(CHANNEL, "\x02" + "Comment: " + comment.Body.ToString().Truncate(250).Replace("\r", " ").Replace("\n", " ") + "(...)" + "\x02");
                    else
                        message = Privmsg(CHANNEL, "\x02" + "Comment: " + comment.Body.ToString().Replace("\r", " ").Replace("\n", " ") + "\x02");

                    Client.messageSender(message);

                }
                else
                {                                               //No comment link
                    post = (RedditSharp.Things.Post)reddit.GetThingByFullname("t3_" + linkParse[4]);

                    if (post.IsSelfPost)
                    {
                        message = Privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                        Client.messageSender(message);
                    }
                    else
                    {
                        message = Privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "]" + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02" + " :" + " \x033" + post.Url + "\x03");
                        Client.messageSender(message);
                    }
                }
            }

            catch   //403 error
            {
                subreddit = getBetween(url, "/r/", "/");

                message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Replace(" ", string.Empty) + "] " + "this subreddit is private or the link was invalid" + "\x02");
                Client.messageSender(message);
                return;
            }
        }

        void explain(string CHANNEL, string nick, string args)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.wikiEnabled == true)
            {
                string message = Privmsg(CHANNEL, "Here's a wiki for \"" + args + "\": " + "http://en.wikipedia.org/w/index.php?title=Special:Search&search=" + args.Replace(" ", "%20"));
                Client.messageSender(message);
            }
        }
        public bool quitIRC(string nick)
        {
            string message;

            foreach (string n in ops)
            {
                if (String.Compare(n, nick, true) == 0)
                {
                    dTime.Stop();
                    waitingForPong = false;
                    message = "QUIT :Goodbye everyone!\n";
                    Client.messageSender(message);

                    return true;
                }
            }

            return false;
        }

        ///////////////////////////////////
        //     Commands Functions End    //
        ///////////////////////////////////

        //CTCP replies
        public void ctcpTime(string u)
        {
            DateTime dateValue = new DateTime();
            dateValue = DateTime.Now;
            string week = dateValue.ToString("ddd", new CultureInfo("en-US"));
            string month = dateValue.ToString("MMM", new CultureInfo("en-US"));
            string day = DateTime.Now.ToString("dd");
            string hour = DateTime.Now.ToString("HH:mm:ss");


            string complete = week + " " + month + " " + day + " " + hour;

            string message = Notice(u, "\x01" + "TIME " + complete + "\x01");
            Client.messageSender(message);
        }
        public void ctcpVersion(string u)
        {
            string message = Notice(u, "\x01" + "VERSION " + botVersion + "\x01");
            Client.messageSender(message);
        }

        public void ctcpPing(string u, string stamp)
        {
            string message = Notice(u, "\x01" + "PING " + stamp + "\x01");
            Client.messageSender(message);
        }
        ////

        public bool isMuted(string nick)
        {
            foreach (string u in ban)
            {
                if (String.Compare(u, nick, true) == 0)
                    return true;
            }

            return false;
        }
        public bool muteUser(string nick)
        {
            if (isMuted(nick))
                return false;
            else
            {
                ban.Add(nick);
                return true;
            }
        }
        public bool unmuteUSer(string nick)
        {
            if (isMuted(nick))
            {
                ban.Remove(nick);
                return true;
            }
            else return false;

        }
        public bool isOperator(string nick)
        {
            foreach (string u in ops)
            {
                if (String.Compare(u, nick, true) == 0)
                    return true;
            }

            return false;

        }
        public bool giveOps(string nick)
        {
            if (!isOperator(nick))
            {
                ops.Add(nick);
                return true;
            }
            else return false;

        }
        public bool takeOps(string nick)
        {
            if (isOperator(nick))
            {
                ops.Remove(nick);
                return true;
            }
            else return false;
        }

        public bool changeNick(string nick)
        {
            Client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();
            OnBotNickChanged(EventArgs.Empty);

            //do nick change to server
            if (Client.isConnected)
            {
                string message = "NICK " + Client.NICK + "\n";
                Client.messageSender(message);
                return true;
            }
            else return false;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (String.IsNullOrEmpty(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            
            else if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End < 0) End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
        }

        /// <summary>
        /// Writes a Message on the output window
        /// </summary>
        /// <param name="Message">A sting with the Message to write</param>

        public void WriteMessage(String message) //Writes Message on the TextBox (bot console)
        {
            if (Output2.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message);
                    Output2.Invoke(invoker);
                }
                catch { }
            }
            else
            {
                this.Output2.AppendText(message + "\n");
            }

            //also, should make a log

        }
        public void WriteMessage(String message, Color color) //Writes Message on the TextBox (bot console)
        {
            if (Output2.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message, color);
                    Output2.Invoke(invoker);
                }
                catch { }
            }
            else
            {
                this.Output2.AppendText(message + "\n", color);
            }

            //also, should make a log

        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("mmssffff");
        }

        public void pingSever()
        {
            if (!WaitingForPong)
            {
                string timeStamp = GetTimestamp(DateTime.Now);
                string message = "PING " + timeStamp;

                #if DEBUG
                    WriteMessage("PING " + timeStamp);
                #endif
                    Client.messageSender(message);

                WaitingForPong = true;

                dTime = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000);
                dTime.Elapsed += (sender, e) => checkIfTimeout(sender, e);
                dTime.Enabled = true;
            }
        }

        public void kickUser(string userToBeKicked)
        {
            string message = "KICK " + Client.HOME_CHANNEL + " " + userToBeKicked;
            Client.messageSender(message);
        }

        void checkIfTimeout(object sender, EventArgs e)
        {
            if (WaitingForPong)
            {
                OnTimeout(EventArgs.Empty);
                WaitingForPong = false;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(dTime!=null) dTime.Close();
            }
            Client.Disconnect();
            Output2.Clear();
        }

        private string questionsRegex(string rest)
        {
            var someVariable1 = "you";
            var someVariable2 = "me";
            var someVariable3 = "you are";
            var someVariable4 = "mine";
            var someVariable5 = "your";
            var someVariable6 = "myself";
            var someVariable7 = "yourself";

            var replacements = new Dictionary<string, string>()
            {
                    {"me",someVariable1},
                    {"you",someVariable2},
                    {"i am", someVariable3},
                    {"i'm", someVariable3},
                    {"your", someVariable4},
                    {"mine", someVariable5},
                    {"yourself", someVariable6},
                    {"myself", someVariable7},
                    {"i", someVariable1}
            };

            var regex = new Regex("(?i)(\\b" + String.Join("\\b|\\b", replacements.Keys) + "\\b)");
            var replaced = regex.Replace(rest, m => replacements[m.Value]);

            return replaced;
        
        }

        public void TwitterLogin()
        {
            if (Settings.Default.twitterEnabled)
            {
                try
                {
                    service = new TwitterService(Settings.Default.twitterConsumerKey, Settings.Default.twitterConsumerKeySecret);

                    service.AuthenticateWith(Settings.Default.twitterAccessToken, Settings.Default.twitterAccessTokenSecret);
                }

                catch
                {
                    Settings.Default.twitterEnabled = false;
                    Settings.Default.Save();
                }

            }

            
            
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        internal void redditLogin(string userName, string password)
        {
            user = reddit.LogIn(userName, password);
        }
    }
}
