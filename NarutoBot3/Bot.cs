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
        private List<Greeting> greet = new List<Greeting>();
        private List<string> nickGenStrings;
        private List<pastMessage> pastMessages = new List<pastMessage>();

        string eta = Settings.Default.eta;

        int lineNumber;
        int triviaNumber;

        string mode;
        public string Mode{ get { return mode; } set { mode = value; } }

        string newNick;
        public string NewNick{get { return newNick; }set { newNick = value; }}

        string who;
        public string Who{get { return who; }set { who = value; }}

        string wholeft;
        public string Wholeft{get { return wholeft; }set { wholeft = value; }}

        bool conneceted = false;
        public bool isConnected{get { return conneceted; }set { conneceted = value; }}

        public event EventHandler<EventArgs> DuplicatedNick;

        public event EventHandler<EventArgs> Created;
        public event EventHandler<EventArgs> Joined;
        public event EventHandler<EventArgs> Left;
        public event EventHandler<EventArgs> NickChanged;
        public event EventHandler<EventArgs> ModeChanged;
        public event EventHandler<EventArgs> Kicked;

        public event EventHandler<EventArgs> TimeOut;
        public event EventHandler<EventArgs> PongReceived;

        public event EventHandler<EventArgs> MessageReturned;

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> ConnectedWithServer;

        public event EventHandler<EventArgs> MessageReceived;
        public event EventHandler<EventArgs> MentionReceived;

        public event EventHandler<EventArgs> BotNickChanged;

        public event EventHandler<EventArgs> BotSilenced;
        public event EventHandler<EventArgs> BotUnsilenced;

        public event EventHandler<EventArgs> Quit;

        private System.Timers.Timer dTime;

        private bool waitigForPong = false;

        public TimeSpan timeDifference;




        public bool WaitigForPong
        {
            get { return waitigForPong; }
            set { waitigForPong = value; }
        }

        protected virtual void OnPongReceived(EventArgs e)
        {
            if (PongReceived != null)
                PongReceived(this, e);

        }


        protected virtual void OnTimeOut(EventArgs e)
        {
            if (TimeOut != null)
                TimeOut(this, e);

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
        protected virtual void OnReceiveMessage(EventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        protected virtual void OnReceiveMention(EventArgs e)
        {
            if (MentionReceived != null)
                MentionReceived(this, e);
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

        IrcClient Client;
        RichTextBox Output2;
        string botVersion = "NarutoBot3 by Ricardo1991, compiled on " + getCompilationDate.RetrieveLinkerTimestamp();
        public Reddit reddit;
        public RedditSharp.Things.AuthenticatedUser user;
        
        public Bot(ref IrcClient client, ref RichTextBox output2)
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
            readOPS();              //operators
            readBAN();              //banned users
            readHLP();              //help text
            readTRI();              //trivia strings
            readGREET();            //read greetings
            readKILLS();
            loadNickGenStrings();   //For the nick generator

            if (Settings.Default.redditUserEnabled)
            {
                try { user = reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass); }
                catch { }
            }

            reddit = new Reddit();
        
        }
        public void BotMessage(string message, out string returnmessage)
        {
            Who = "";
            Wholeft = "";
            NewNick = "";

            string prefix;
            string command;
            string[] parameters;
            string completeParameters;
            returnmessage = "";
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

                    //:nova.esper.net 433 * SekiKun :Nickname is already in use.
                    case("433"):
                        OnDuplicatedNick(EventArgs.Empty);
                        WriteMessage("* " + command + " " + completeParameters);

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
                            isConnected = true;
                            OnConenct(EventArgs.Empty);

                            if (!String.IsNullOrEmpty(Client.HOST_SERVER))
                                OnConnectedWithServer(EventArgs.Empty);
                        }
                            
                        break;

                    case ("PONG"):
                        string[] split = message.Split(':');
                        string pongcmd = split[2];

                        #if DEBUG
                        WriteMessage(message);
                        #endif

                        if (WaitigForPong)
                        {
                            string currentStamp = GetTimestamp(DateTime.Now);
                            string format = "mmssffff";

                            DateTime now, then;

                            now = DateTime.ParseExact(currentStamp, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
                            then = DateTime.ParseExact(pongcmd, format, System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));

                            timeDifference = now.Subtract(then);

                            WaitigForPong = false;

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

                        foreach (Greeting g in greet)
                        {
                            if (g.Nick == Who.Replace("@", string.Empty).Replace("+", string.Empty) && g.Enabled == true)
                            {
                                string messagess = privmsg(Client.HOME_CHANNEL, g.Greetingg);
                                Client.messageSender(messagess);
                            }
                        }

                        OnJoin(EventArgs.Empty);
                        break;


                    case ("PART"):
                        Wholeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string userP in Client.userList)
                        {
                            if (userP.Replace("@", string.Empty).Replace("+", string.Empty) != Wholeft.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
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
                        
                        Wholeft = prefix.Substring(0, prefix.IndexOf("!"));

                        userTemp = new List<string>();

                        foreach (string userB in Client.userList)
                        {
                            if (userB.Replace("@", string.Empty).Replace("+", string.Empty) != Wholeft.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(userB);
                            }
                        }
                        Client.userList.Clear();

                        foreach (string userN in userTemp)
                        {
                            Client.userList.Add(userN);
                        }
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
                            if (userC.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)
                                !=
                                oldnick.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)))
                            {
                                userTemp.Add(userC);
                            }
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
                            if (userD.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty)
                                !=
                                affectedUser.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(userD);
                            }
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
                            if (userR.Replace("@", string.Empty).Replace("+", string.Empty) != kickedUser.Replace("@", string.Empty).Replace("+", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty))
                            {
                                userTemp.Add(userR);
                            }
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
                        string whoSent = parameters[0];                         //Who sent is the source of the message. (The Channel, or User if private message)
                        string msg = parameters[1].Replace("\r", string.Empty).Replace("\n", string.Empty);
                        msg=msg.Trim();
                        string cmd = msg.Split(' ')[0];
                        string arg;
                        if (msg.Length - 1 > cmd.Length)
                            arg = msg.Substring(cmd.Length+1); //the rest of msg
                        else arg = "";

                        //Write Message on Console
                        if (msg.ToLower().Contains(Client.NICK.ToLower()))
                        {
                            if (user.Length > 14)
                            {
                                WriteMessage(user.Truncate(15) + " : " + msg, Color.LightGreen);
                            }
                            else if (user.Length >= 8)                       //Write the message on the bot console
                            {
                                WriteMessage(user + "\t: " + msg, Color.LightGreen);
                            }
                            else
                            {
                                WriteMessage(user + "\t\t: " + msg, Color.LightGreen);
                            }
                                
                        }
                        else
                        {
                            if (user.Length > 14)
                            {
                                WriteMessage(user.Truncate(15) + " : " + msg);
                            }

                            else if (user.Length >= 8)                       //Write the message on the bot console
                            {
                                WriteMessage(user + "\t: " + msg);
                            }
                            else
                            {
                                WriteMessage(user + "\t\t: " + msg);
                            }
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
                                Client.messageSender(privmsg(whoSent, "[#15] [8.88 / 10] : "+"\x02"+"Code Geass: Hangyaku no Lelouch " +"\x02"+"-> http://myanimelist.net/anime/1575/Code_Geass:_Hangyaku_no_Lelouch"));
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
                                if (!String.IsNullOrEmpty(arg))
                                {
                                    WriteMessage("* Received a greet TOOGLE request from " + user, Color.Pink);
                                    greetToogle(user);
                                
                                }
                                else
                                {
                                    WriteMessage("* Received a greet request from " + user, Color.Pink);
                                    addGreet(arg.Substring(1), user);
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
                                if (isOperator(user)) changeNick(arg, out returnmessage);
                            }

                        else if (String.Compare(cmd, Client.SYMBOL + "op", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a op request from " + user, Color.Pink);
                                if (addBotOP(user, arg)) SaveOPS();
                            }
                        else if (String.Compare(cmd, Client.SYMBOL + "deop", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a deop request from " + user, Color.Pink);
                                if (removeBotOP(user, arg)) SaveOPS();
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
                                WriteMessage("* Received a time request from " + user, Color.Pink);
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
                        else if (String.Compare(cmd, Client.SYMBOL + "kill", true) == 0 && !String.IsNullOrEmpty(arg))
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
                                WriteMessage("* Received a ctcp version request from " + user, Color.Pink);
                                ctcpVersion(user);
                            }

                            if (cmd.Contains("TIME"))
                            {
                                WriteMessage("* Received a ctcp time request from " + user, Color.Pink);
                                ctcpTime(user);
                            }
                            if (cmd.Contains("PING"))
                            {
                                WriteMessage("* Received a ctcp ping request from " + user, Color.Pink);
                                ctcpPing(user, arg);
                            }
                        }

                        else //No parsing, just a normal message
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
                            //string whoSentt = parameters[0];                        //Who sent is the source of the message. (The Channel, or User if private message)
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
                                WriteMessage("* Received a ctcp version request from " + userr, Color.Pink);
                                ctcpVersion(userr);
                            }

                            if (cmdd.Contains("TIME"))
                            {
                                WriteMessage("* Received a ctcp time request from " + userr, Color.Pink);
                                ctcpTime(userr);
                            }
                            if (cmdd.Contains("PING"))
                            {
                                WriteMessage("* Received a ctcp ping request from " + userr, Color.Pink);
                                
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


        public void readKILLS()
        {
            kill.Clear();
            try
            {
                StreamReader sr = new StreamReader("kills.txt");
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
        public void SaveOPS()
        {
            using (StreamWriter newTask = new StreamWriter("ops.txt", false))
            {
                foreach (string op in ops)
                {
                    newTask.WriteLine(op);
                }
            }
        }

        public void readOPS()
        {
            ops.Clear();
            try
            {
                StreamReader sr = new StreamReader("ops.txt");
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

        public void SaveBAN()
        {
            using (StreamWriter newTask = new StreamWriter("banned.txt", false))
            {
                foreach (string rl in ban)
                {
                    newTask.WriteLine(rl);
                }
            }
        }
        public void readBAN()
        {
            ban.Clear();
            try
            {
                StreamReader sr = new StreamReader("banned.txt");
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

        public void SaveRLS()
        {
            using (StreamWriter newTask = new StreamWriter("rules.txt", false))
            {
                foreach (string rl in rls)
                {
                    newTask.WriteLine(rl);
                }
            }


        }
        public void readRLS()
        {
            rls.Clear();
            try
            {
                StreamReader sr = new StreamReader("rules.txt");
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

        public void SaveHLP()
        {
            using (StreamWriter newTask = new StreamWriter("help.txt", false))
            {
                foreach (string hp in hlp)
                {
                    newTask.WriteLine(hp);
                }
            }


        }
        public void readHLP()
        {
            hlp.Clear();
            try
            {
                StreamReader sr = new StreamReader("help.txt");
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

        public void readTRI()//Reads the trivia stuff
        {
            tri.Clear();
            triviaNumber = 0;

            try
            {
                StreamReader sr = new StreamReader("trivia.txt");
                while (sr.Peek() >= 0)
                {
                    tri.Add(sr.ReadLine());
                    triviaNumber++;
                }
                sr.Close();
            }
            catch
            {
            }
        }

        public void readGREET()
        {
            string nick, greeting, line;
            string[] split;
            bool enabled = false;
            bool found = false;

            if (File.Exists("greetings.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("greetings.txt");
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

                        foreach (Greeting g in greet)
                        {
                            if (g.Nick == nick)
                                found = true;

                        }

                        if (!found)
                        {
                            Greeting g = new Greeting(nick, greeting, enabled);
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


        public void addGreet(string args, string nick)
        {
            bool found = false;

            foreach (Greeting g in greet)
            {
                if (g.Nick == nick)
                {
                    found = true;
                    g.Greetingg = args;
                    SaveGreets();
                }
            }

            if (!found)
            {
                Greeting g = new Greeting(nick, args, true);
                greet.Add(g);
                SaveGreets();
            }


        }

        public void SaveGreets()
        {
            using (StreamWriter newTask = new StreamWriter("greetings.txt", false))
            {
                foreach (Greeting gg in greet)
                {
                    newTask.WriteLine(gg.Nick + ":" + gg.Enabled.ToString() + ":" + gg.Greetingg);
                }
            }

        }

        void greetToogle(string nick)
        {
            string message = notice(nick, "You didn't set a greeting yet"); ;
            string state = "disabled";
            bool found = false;


            foreach (Greeting g in greet)
            {
                if (g.Nick == nick && !found)
                {
                    found = true;
                    g.Enabled = !g.Enabled;
                    if (g.Enabled) state = "enabled";

                    message = notice(nick, "Your greeting is now " + state);
                    SaveGreets();
                }
            }

            Client.messageSender(message);
        }

        public void loadNickGenStrings()//These are for the nick gen
        {
            lineNumber = 0;
            nickGenStrings = new List<string>();
            nickGenStrings.Clear();

            try
            {
                StreamReader sr = new StreamReader("text.txt");
                while (sr.Peek() >= 0)
                {
                    nickGenStrings.Add(sr.ReadLine());
                    lineNumber++;
                }
                sr.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends a message to the destinatary
        /// </summary>
        /// <param Name="destinatary">string with either a User or a channel, it's where the message will be sent</param>
        /// <param Name="message">String of text with the message that will be delivered</param>
        public string privmsg(string destinatary, string message)
        {
            string result;

            result = "PRIVMSG " + destinatary + " :" + message + "\r\n";

            if (Client.NICK.Length > 15)
            {
                WriteMessage(Client.NICK.Truncate(16) + ":" + message);
                OnReceiveMessage(EventArgs.Empty);
            }
                
            else if (Client.NICK.Length >= 8)                       //Write the message on the bot console
            {
                WriteMessage(Client.NICK + "\t: " + message);
                OnReceiveMessage(EventArgs.Empty);
            }
            else {
                WriteMessage(Client.NICK + "\t\t: " + message);
                OnReceiveMessage(EventArgs.Empty);
            }
                

            return result;
        }

        /// <summary>
        /// Sends a notice to the destinatary
        /// </summary>
        /// <param Name="destinatary">string with either a User or a channel, it's where the message will be sent</param>
        /// <param Name="message">String of text with the message that will be delivered</param>
        public string notice(string destinatary, string message)
        {
            string result;

            result = "NOTICE " + destinatary + " :" + message + "\r\n";

            if (Client.NICK.Length > 15){
                WriteMessage(Client.NICK.Truncate(16) + ":" + message);
                OnReceiveMessage(EventArgs.Empty);
            }
                
            else if (Client.NICK.Length >= 8)                       //Write the message on the bot console
            {
                WriteMessage(Client.NICK + "\t: " + message);
                OnReceiveMessage(EventArgs.Empty);
            }
                
            else
            {
                WriteMessage(Client.NICK + "\t\t: " + message);
                OnReceiveMessage(EventArgs.Empty);
            }
                

            return result;
        }

        public void pokeUser(string nick)
        {
            string message = privmsg(Client.HOME_CHANNEL, "\x01" + "ACTION stabs " + nick + " with a sharp knife" + "\x01");
            Client.messageSender(message);

        }
        public void whoisUser(string nick)
        {
            string message = "WHOIS " + nick + "\n";
            Client.messageSender(message);

        }

        public void randomTextSender(object source, ElapsedEventArgs e)
        {
            pastMessages.Sort((p, q) => p.wordsCount.CompareTo(q.wordsCount));
            List<pastMessage> pastTemp = new List<pastMessage>();

            foreach (pastMessage p in pastMessages)//remove messages with less than 8 words
            {
                if (p.wordsCount >= 8)
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

                int start = rd.Next(pastMessages[n].wordsCount - 5);
                int end;
                do { end = rd.Next(pastMessages[n].wordsCount); } while (end <= start);

                for (d = start; d < end; d++)
                {
                    g.Append(pastMessages[n].words[d] + " ");

                }
                pastMessages.Remove(pastMessages[n]);
            }
            randomWords = g.ToString();

            message = privmsg(Client.HOME_CHANNEL, "\x02" + randomWords + "\x02");
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
                message = notice(nick, targetUser + " was added as a bot operator!");
                Client.messageSender(message);
                return true;
            }
            else
            {
                message = notice(nick, "Error: " + targetUser + " is already a bot operator!");
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
                message = notice(nick, targetUser + " was removed as a bot operator!");
                Client.messageSender(message);
                return true;
            }
            else
            {
                message = notice(nick, "Error: " + targetUser + " was not a bot operator!");
                Client.messageSender(message);
                return false;
            }
        }
        int opList(string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = notice(nick, "Bot operators:");
                Client.messageSender(message);
                foreach (string p in ops)
                {
                    message = notice(nick, nick + " :->" + p);
                    Client.messageSender(message);
                }
                return 1;
            }

            return 0;
        }
        void say(string CHANNEL, string args, string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = privmsg(CHANNEL, args);
                Client.messageSender(message);
                return;
            }
        }

        void me(string CHANNEL, string args, string nick)
        {
            string message;
            if (isOperator(nick))
            {
                message = privmsg(CHANNEL, "\x01" + "ACTION " + args + "\x01");
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
                    message = notice(nick, "The bot was unmuted");
                }
                else
                {
                    OnSilence(EventArgs.Empty);
                    message = notice(nick, "The bot was muted");
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
                string message = privmsg(CHANNEL, "Hello " + nick + "!");
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
                    message = notice(nick, h.Replace("\n", "").Replace("\r", ""));
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
                        message = privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                        Client.messageSender(message);

                    }
                    return;
                }

            }
            else if (Settings.Default.rules_Enabled == true)
            {
                foreach (string h in ops)
                {
                    message = privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
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
                    message = notice(nick, "The average time for the chapter release is " + eta + ".");
                    Client.messageSender(message);
                }
                else
                {
                    message = privmsg(CHANNEL, "The average time for the chapter release is " + eta + ".");
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
                string message = privmsg(CHANNEL, nick + " rolled a " + number);
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
            while (Client.userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) == nicks);

            if (Settings.Default.silence == false && Settings.Default.pokeEnabled == true)
            {
                message = privmsg(CHANNEL, "\x01" + "ACTION " + "pokes " + Client.userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) + "\x01");
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
                    message = privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.assign_Enabled == true)
            {
                message = privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
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
                    message = privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
                    Client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.claims_Enabled == true)
            {

                message = privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
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
                message = privmsg(CHANNEL, "Could not parse arguments");
                Client.messageSender(message);
                return;
            }

            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
                    Client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.conversionEnabled == true)
            {
                message = privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
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
                message = privmsg(CHANNEL, "Could not parse arguments");
                Client.messageSender(message);
                return;
            }

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                    Client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.conversionEnabled == true)
            {

                message = privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                Client.messageSender(message);

            }
        }

        public void trivia(string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;
            if (triviaNumber == 0) return;
            if (Settings.Default.silence == false && Settings.Default.triviaEnabled == true)
            {
                string message;
                Random rnd = new Random();
                int rt = rnd.Next(triviaNumber - 1);

                message = privmsg(CHANNEL, tri[rt]);
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

            TimeZoneAPI g = new TimeZoneAPI();
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
                        message = privmsg(CHANNEL, "4:20");
                    else if (args.Replace("\r", string.Empty).ToLower() == "alan_jackson" || args.Replace("\r", string.Empty).ToLower() == "alan" || args.Replace("\r", string.Empty).ToLower() == "alanjackson")
                        message = privmsg(CHANNEL, "5:00");
                    else
                        message = privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS + ". \"" + args + "\" is an invalid argument");
                else
                    message = privmsg(CHANNEL, convertedTime.Hour + ":" + convertedTime.Minute.ToString("00") + " " + timezoneS);
                Client.messageSender(message);

            }
        }

        public void youtube(string CHANNEL, string nick, string line, bool isShort)
        {
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

                title = title.Replace("&lt;", "<");
                title = title.Replace("&rt;", ">");
                title = title.Replace("&amp;", "&");
                title = title.Replace("&quot;", "\"");

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
                    message = privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message); ;
                }
                else
                {
                    message = privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);
                }
            }
        }

        public void twitter(string CHANNEL, string nick, string line)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.twitterEnabled == true)
            {
                string[] bah;
                string url = "";
                string link="";
                string author, tweet;
                bool gotIt = false;

                bah = line.Split(new char[] { ' ' });

                foreach (string ss in bah)
                {
                    if (ss.Contains("twitter.com/"))
                    {
                        url = ss;
                        gotIt = true;
                    }

                    if (gotIt) break;
                }

                if (!gotIt) return;


                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string readHtml = webClient.DownloadString(url);

                author = getBetween(readHtml, "<span class=\"username js-action-profile-name\"><s>@</s><b>", "</b></span>");
                tweet = getBetween(readHtml, "<p class=\"js-tweet-text tweet-text\">", "</p>");
                if(tweet.Contains("<a "))
                {
                    link = getBetween(tweet, "data-expanded-url=\"", "\"");
                    tweet = tweet.Substring(0, tweet.IndexOf("<a ")); 
                    
                }
                    

                author = StripTagsRegex(author);
                tweet = StripTagsRegex(tweet);

                tweet = tweet.Replace("&#39;", "'");
                tweet = tweet.Replace("&lt;", "<");
                tweet = tweet.Replace("&rt;", ">");
                tweet = tweet.Replace("&amp;", "&");
                tweet = tweet.Replace("&quot;", "\"");
                tweet = tweet.Replace("&#10;", " ");
                tweet = tweet.Replace("&nbsp;", " ");

                string message = privmsg(CHANNEL, "Tweet by @" + author + " : " + tweet + " " + link);
                Client.messageSender(message);
            }
        }

        public void animeSeach(string CHANNEL, string nick, string line)
        {
            string message;

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

            string json;
            string jsonAnime;

            bool user = false;


            if (line.Contains("-u") || line.Contains("-User")) user = true;

            string getString = "https://www.googleapis.com/customsearch/v1?key=" + Settings.Default.apikey + "&cx=" + Settings.Default.cxKey + "&q=" + line.Replace(" ", "%20").Replace(" -User", "%20").Replace(" -u", "%20");

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            webClient.Credentials = new NetworkCredential(Settings.Default.malUser, Settings.Default.malPass);
            webClient.Headers.Add("User-agent", "NarutoBot3");

            string name = "";

            try
            {
                json = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(json, g);
            }
            catch { }

            if (g.items == null) message = privmsg(CHANNEL, "Could not find anything, try http://myanimelist.net/anime.php?q=" + line.Replace(" ", "%20").Replace(" -User", "%20").Replace(" -u", "%20"));
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

                if (!found) message = privmsg(CHANNEL, g.items[0].link);
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
                            message = privmsg(CHANNEL, "Error: a=null");
                            #else
                            message = privmsg(CHANNEL, "");
                            
                            #endif

                        }
                        else
                        {
                            string score = a.entry[0].score.ToString();
                            string episodes = a.entry[0].episodes.ToString();
                            string title = a.entry[0].title;

                            message = privmsg(CHANNEL, "[" + episodes + " episodes] " + "[" + score + " / 10] : " + "\x02" + title + "\x02" + " -> " + g.items[i].link);
                        }

                    }
                    else
                    {
                        string readHtml = webClient.DownloadString(g.items[i].link.Replace("recommendations", string.Empty).Replace("reviews", string.Empty).Replace("clubs", string.Empty).Replace("friends", string.Empty));

                        string profile = getBetween(readHtml, "<title>", "'s Profile - MyAnimeList.net</title>");

                        string completed = getBetween(readHtml, ">Completed</span></td>", "<td><div style=");
                        completed = getBetween(completed, "<td align=\"center\">", "</td>");


                        message = privmsg(CHANNEL, "[" + profile + "] " + "Completed " + completed + " animes" + " -> " + g.items[i].link.Replace("recommendations", string.Empty).Replace("reviews", string.Empty).Replace("clubs", string.Empty).Replace("friends", string.Empty));
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
                    message = privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);

                }
                else
                {
                    message = privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    Client.messageSender(message);
                }

            }
        }
        public void killUser(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                string message;
                if (args.ToLower() == "la kill".Trim())
                {
                    message = privmsg(CHANNEL, nick + " lost his way");
                }
                else
                {
                    string killString = kill[r.Next(kill.Count)].Replace("<target>", args.Trim()).Replace("<user>", nick.Trim());

                    if(killString.ToLower().Contains("<normal>"))
                        message = privmsg(CHANNEL, killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty));
                    else
                        message = privmsg(CHANNEL, "\x01" + "ACTION " + killString + "\x01");
                }
                Client.messageSender(message);

            }
        }
        public void nickGen(string CHANNEL, string nick, string args)
        {
            Random rnd = new Random();

            bool randomnumber = false;
            bool randomUpper = false;
            bool switchLetterNumb = false;
            bool Ique = false;
            string message;

            if (isMuted(nick)) return;

            if (lineNumber < 2)
            {
                message = privmsg(CHANNEL, "Nicks was not initialized properly");
                Client.messageSender(message);
                return;
            }

            if (Settings.Default.silence == false && Settings.Default.nickEnabled == true)
            {
                if (args.Contains("random"))
                {
                    if (rnd.Next(0, 100) <= 30)
                        switchLetterNumb = true;
                    else switchLetterNumb = false;

                    if (rnd.Next(0, 100) <= 30)
                        randomnumber = true;
                    else randomnumber = false;

                    if (rnd.Next(0, 100) <= 30)
                        randomUpper = true;
                    else randomUpper = false;

                    if (rnd.Next(0, 100) <= 10)
                        Ique = true;
                    else Ique = false;
                }
                else
                {
                    if (args.Contains("SL") == true)
                    {
                        switchLetterNumb = true;
                    }
                    if (args.Contains("RN") == true)
                    {
                        randomnumber = true;
                    }
                    if (args.Contains("RU") == true)
                    {
                        randomUpper = true;
                    }
                    if (args.Contains("IQ") == true)
                    {
                        Ique = true;
                    }
                }
                if (lineNumber == 0)
                    return;


                //gen = new NickGen();
                string nick_ = NickGen.NickG(nickGenStrings, lineNumber, randomnumber, randomUpper, switchLetterNumb, Ique);

                message = privmsg(CHANNEL, nick + " generated the nick " + nick_);
                Client.messageSender(message);
            }
        }

        public void redditLink(string CHANNEL, string nick, string line)
        {
            string[] temp = line.Split(' ');
            string subreddit = "";
            string url = "";
            bool found = false;
            string[] tempQ;
            string message, message2;

            RedditSharp.Things.Post post;
            RedditSharp.Things.Subreddit sub;
            RedditSharp.Things.Comment com;

            if (isMuted(nick)) return;

            foreach (string st in temp)
            {
                if (st.Contains("reddit.com") && st.Contains("/r/") && st.Contains("/comments/"))
                {
                    url = st;
                    found = true;
                }
            }

            if (!found) return;

            subreddit = getBetween(url, "/r/", "/");

            try
            {
                sub = reddit.GetSubreddit("/r/" + subreddit);

                string[] linkParse = url.Replace("\r", string.Empty).Split('/');

                if (linkParse.Length >= 9 && !String.IsNullOrEmpty(linkParse[8]))    //Com comentário
                {
                    tempQ = url.Split(new char[] { '?' }, 2);
                    url = tempQ[0];
                    Uri urlURI = new Uri(url.Replace("\r", string.Empty));
                    post = reddit.GetPost(urlURI);     //slow

                    string commentID = linkParse[8];
                    tempQ = commentID.Split(new char[] { '?' }, 2);
                    commentID = tempQ[0];

                    com = reddit.GetComment(sub.Name, "t1_" + commentID, "t3_" + linkParse[6]);

                    message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                    Client.messageSender(message);

                    if (com.Body.ToString().Length > 250)
                        message2 = privmsg(CHANNEL, "\x02" + "Comment: " + com.Body.ToString().Truncate(250).Replace("\r", " ").Replace("\n", " ") + "(...)" + "\x02");
                    else
                        message2 = privmsg(CHANNEL, "\x02" + "Comment: " + com.Body.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty) + "\x02");

                    Client.messageSender(message2);

                }
                else
                {                                               //No comment link
                    Uri urlURI = new Uri(url.Replace("\r", string.Empty));
                    post = reddit.GetPost(urlURI);     //slow

                    if (post.IsSelfPost)
                    {
                        message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                        Client.messageSender(message);
                    }
                    else
                    {
                        message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "]" + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submitted by /u/" + post.Author + "\x02" + " :" + " \x033" + post.Url + "\x03");
                        Client.messageSender(message);

                    }

                }

            }

            catch   //403 error
            {
                subreddit = getBetween(url, "/r/", "/");

                message = privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Replace(" ", string.Empty) + "] " + "this subreddit is private" + "\x02");
                Client.messageSender(message);
                return;
            }
        }

        void explain(string CHANNEL, string nick, string args)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.wikiEnabled == true)
            {
                string message = privmsg(CHANNEL, "Here's a wiki for \"" + args + "\": " + "http://en.wikipedia.org/w/index.php?title=Special:Search&search=" + args.Replace(" ", "%20"));
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

            string message = notice(u, "\x01" + "TIME " + complete + "\x01");
            Client.messageSender(message);
        }
        public void ctcpVersion(string u)
        {
            string message = notice(u, "\x01" + "VERSION " + botVersion + "\x01");
            Client.messageSender(message);
        }

        public void ctcpPing(string u, string stamp)
        {
            string message = notice(u, "\x01" + "PING " + stamp + "\x01");
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

        public bool changeNick(string nick, out string returnmessage)
        {
            Client.NICK = Settings.Default.Nick = nick;

            if (!String.IsNullOrEmpty(Client.HOST_SERVER))
                returnmessage = Client.NICK + " @ " + Client.HOME_CHANNEL + " - " + Client.HOST + ":" + Client.PORT + " (" + Client.HOST_SERVER + ")";
            else
                returnmessage = Client.NICK + " @ " + Client.HOME_CHANNEL + " - " + Client.HOST + ":" + Client.PORT;
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
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
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
        /// Writes a message on the output window
        /// </summary>
        /// <param name="message">A sting with the message to write</param>

        public void WriteMessage(String message) //Writes message on the TextBox (bot console)
        {
            if (Output2.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message);
                    Output2.Invoke(invoker);
                    //SetTextCallback d = new SetTextCallback(WriteMessage);
                    //this.Invoke(d, new object[] { message });
                }
                catch { }
            }
            else
            {
                this.Output2.AppendText(message + "\n");
            }

            //also, should make a log

        }
        public void WriteMessage(String message, Color color) //Writes message on the TextBox (bot console)
        {
            if (Output2.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message, color);
                    Output2.Invoke(invoker);

                    //    SetTextCallback d = new SetTextCallback(WriteMessage);
                    //    this.Invoke(d, new object[] { message, color });
                }
                catch { }
            }
            else
            {
                this.Output2.AppendText(message + "\n", color);
            }

            //also, should make a log

        }

        static public string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("mmssffff");
        }

        public void pingSever()
        {
            if (!WaitigForPong)
            {
                string timeStamp = GetTimestamp(DateTime.Now);
                string message = "PING " + timeStamp;
                #if DEBUG
                WriteMessage("PING " + timeStamp);
                #endif
                Client.messageSender(message);

                WaitigForPong = true;

                dTime = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000);
                dTime.Enabled = true;

                dTime.Elapsed += (sender, e) => checkIfTimeout(sender, e);
            }

        }

        public void kickUser(string user)
        {
            string message = "KICK " + Client.HOME_CHANNEL + " " + user;
            Client.messageSender(message);
        
        
        }

        void checkIfTimeout(object sender, EventArgs e)
        {
            if (WaitigForPong)
            {
                OnTimeOut(EventArgs.Empty);
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
    }
}
