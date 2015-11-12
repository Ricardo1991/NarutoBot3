﻿using GiphySearch;
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
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TextMarkovChains;
using ChatterBotAPI;
using TweetSharp;

namespace NarutoBot3
{
    public class Bot : IDisposable
    {

        Questions qq;

        public TextMarkovChain tmc = new TextMarkovChain();
        public TextMarkovChain killgen = new TextMarkovChain();
        int tmcCount = 0;


        ChatterBotFactory factory = new ChatterBotFactory();
        ChatterBot bot1;
        ChatterBotSession bot1session;


        private List<string> rls = new List<string>();
        private List<string> hlp = new List<string>();
        private List<string> tri = new List<string>();
        private List<string> kill = new List<string>();
        private List<string> facts = new List<string>();
        private List<string> quotes = new List<string>();
        private List<string> funk = new List<string>();
        private List<string> nickGenStrings;


        public List<string> userList = new List<string>();

        public UserList ul = new UserList();

        private StatsManager stats = new StatsManager();

        string mode;
        public string Mode{ get { return mode; } set { mode = value; } }

        string newNick;
        public string NewNick{get { return newNick; }set { newNick = value; }}

        string who;
        public string Who{get { return who; }set { who = value; }}

        string whoLeft;
        public string WhoLeft{get { return whoLeft; }set { whoLeft = value; }}

        string quitMessage;
        public string QuitMessage{get { return quitMessage; } set { quitMessage = value; }}

        string joinMessage;

        public string JoinMessage
        {
            get { return joinMessage; }
            set { joinMessage = value; }
        }

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
        public event EventHandler<EventArgs> EnforceMirrorChanged;

        private System.Timers.Timer timeoutTimer;

        private bool waitingForPong = false;

        private TimeSpan timeDifference;

        IRC_Client Client;
        RichTextBox OutputBox;
        ColorScheme currentColorScheme = new ColorScheme();
        string botVersion = "NarutoBot3 by Ricardo1991, compiled on " + getCompilationDate.RetrieveLinkerTimestamp();

        private Reddit reddit;

        string topic;

        private List<int> killsUsed = new List<int>();
        private List<int> factsUsed = new List<int>();

        private TwitterService service;

        public string Topic
        {
            get { return topic; }
            set { topic = value; }
        }
        
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

        protected virtual void OnEnforceMirrorChanged(EventArgs e)
        {
            if (EnforceMirrorChanged != null)
                EnforceMirrorChanged(this, e);
        }
        
        public Bot(ref IRC_Client client, ref RichTextBox output, ColorScheme color)
        {

            qq = new Questions();

            Client = client;
            OutputBox = output;
            currentColorScheme = color;

            ReadHelp();                 //Help text
            ReadTrivia();               //Trivia strings
            ReadKills();                //Read the killstrings
            ReadFacts();                //Read the factStrings
            ReadNickGen();              //For the Nick generator
            ReadQuotes();
            ReadFunk();
            ReadRules();

            ul.loadData();

            bot1 = factory.Create(ChatterBotType.CLEVERBOT);
            bot1session = bot1.CreateSession();


            if (File.Exists("textSample.xml"))
            {
                XmlDocument d = new XmlDocument();
                d.Load("textSample.xml");
                tmc.feed(d);
            }
            
            

            reddit = new Reddit(false);

            if (Settings.Default.redditUserEnabled)
            {
                try
                {
                    reddit.User = reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass, true);
                }
                catch { }
            }

            if (Settings.Default.twitterEnabled)
                TwitterLogin();
        }

        ~Bot()
        {
            Dispose(false);
            return;
        }

        public void updateTheme(ColorScheme newColorScheme){
            if(newColorScheme != null)
                currentColorScheme = newColorScheme;
        }

        public void processMessage(string message)
        {
            List<string> userTemp = new List<string>();
            bool found;
            Message messageObject;

            Who = "";
            WhoLeft = "";
            NewNick = "";

            if (String.IsNullOrEmpty(message)) return;

            messageObject = new Message(message);

            switch (messageObject.Type)
            {
                case ("PING"):

                    Client.sendMessage("PONG :" + messageObject.SplitMessage[0] + "\r\n");

                    #if DEBUG
                        WriteMessage(message);
                    #endif
                    break;

                case ("001"):
                case ("002"):
                case ("003"):

                    WriteMessage(messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1]);
                    break;

                case ("004"): //server used for connection

                    Client.HOST_SERVER = messageObject.SplitMessage[1];
                    break;

                case ("005"):
                case ("250"):
                case ("251"):
                case ("252"):
                case ("254"):
                case ("255"):
                case ("265"):
                case ("266"):
                case ("333"): //Topic author and Time
                case ("366"): //End of /NAMES
                case ("375"): //START OF MOTD

                    break;

                case ("332"): //TOPIC

                    topic = messageObject.CompleteMessage.Split(new char[] { ' ' }, 3)[2];
                    OnTopicChange(EventArgs.Empty); //Tell the ui the topic changed
                    break;

                case ("353"): //USERLIST

                    foreach (string s in messageObject.SplitMessage[3].Split(' '))
                    {
                        found = false;
                        foreach (string u in userList)
                            if (string.Compare(s, u, true) == 0 ) found = true;

                        if (!found) 
                            userList.Add(s);

                        ul.makeOnline(removeUserMode(s));

                        if (s[0] == '@')
                            ul.setUserChannelOP(s, true);
                        else if (s[0] == '+')
                            ul.setUserChannelVoice(s, true);
                    }
                            
                    userList.Sort();
                    OnCreate(EventArgs.Empty);
                    break;


                case ("372"): //MOTD

                    string motd = messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1];
                    WriteMessage(motd, currentColorScheme.Motd);
                    break;

                case ("376"): //END OF MOTD

                    IsConnected = true;
                    OnConnect(EventArgs.Empty);

                    if (!String.IsNullOrEmpty(Client.HOST_SERVER))
                        OnConnectedWithServer(EventArgs.Empty);

                    break;

                case ("433"): //Nickname is already in use.

                    OnDuplicatedNick(EventArgs.Empty);
                    WriteMessage("* " + messageObject.Type + " " + messageObject.CompleteMessage);
                    break;

                case ("TOPIC"):   //TOPIC

                    Topic = messageObject.CompleteMessage.Split(new char[] { ' ' }, 2)[1];
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
                        timeoutTimer.Stop();

                        OnPongReceived(EventArgs.Empty);
                    }

                    break;

                case ("JOIN"):

                    if (messageObject.Sender.Contains("!"))
                    {
                        Who = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                        JoinMessage = messageObject.Sender.Substring(messageObject.Sender.IndexOf("!") + 1);
                    }
                    else {
                        Who = messageObject.Sender;
                        JoinMessage = "";
                    }

                    found = false;

                    foreach(string s in userList)
                        if (string.Compare(s, Who, true) == 0) 
                            found = true; 

                    if (!found)
                        userList.Add(Who);

                    userList.Sort();

                    OnJoin(EventArgs.Empty);

                    ul.makeOnline(removeUserMode(Who));

                    greetUser(removeUserMode(Who));

                    messageDelivery(removeUserMode(Who));

                    break;


                case ("PART"):

                    if (messageObject.Sender.Contains("!"))
                        WhoLeft = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                    else WhoLeft = messageObject.Sender;
                    quitMessage = messageObject.CompleteMessage;

                    userTemp = new List<string>();

                    foreach (string userP in userList)
                        if (string.Compare(removeUserMode(userP), removeUserMode(WhoLeft), true) != 0)
                            userTemp.Add(userP);
                        
                    userList.Clear();

                    foreach (string userO in userTemp)
                        userList.Add(userO);

                    userList.Sort();
                    userTemp.Clear();

                    ul.makeOffline(WhoLeft);

                    OnLeave(EventArgs.Empty);
                    break;


                case ("QUIT"):
                    if (messageObject.Sender.Contains("!"))
                        WhoLeft = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                    else WhoLeft = messageObject.Sender;

                    quitMessage = messageObject.CompleteMessage;

                    userTemp = new List<string>();

                    foreach (string userB in userList)
                        if (string.Compare(removeUserMode(userB), removeUserMode(WhoLeft), true) != 0)
                            userTemp.Add(userB);

                    userList.Clear();

                    foreach (string userN in userTemp)
                        userList.Add(userN);

                    userList.Sort();
                    userTemp.Clear();

                    ul.makeOffline(WhoLeft);


                    OnLeave(EventArgs.Empty);
                    break;


                case ("NICK"):

                    string oldnick = messageObject.Sender;
                    if (messageObject.Sender.Contains("!"))
                        oldnick = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));
                    string newnick = messageObject.CompleteMessage;
                    char userMode = getUserMode(oldnick, userList);

                    if (userMode != '0')
                        newnick = userMode + newnick;

                    userTemp = new List<string>();

                    foreach (string userC in userList)
                        if (string.Compare(removeUserMode(userC) , removeUserMode(oldnick), true) != 0)
                            userTemp.Add(userC);
                    userList.Clear();

                    foreach (string use in userTemp)
                        userList.Add(use);

                    userList.Add(newnick);
                    userList.Sort();


                    NewNick = newnick;
                    Who = oldnick;

                    OnNickChange(EventArgs.Empty);

                    ul.makeOffline(oldnick);
                    ul.makeOnline(removeUserMode(newnick));


                    messageDelivery(removeUserMode(newnick));

                    userTemp.Clear();
                    break;


                case ("MODE"):

                    userTemp = new List<string>();
                    string modechange = messageObject.SplitMessage[1];

                    if (messageObject.SplitMessage.Length < 3)
                        break;

                    string affectedUser = messageObject.SplitMessage[2];

                    affectedUser = affectedUser.Split(new char[] { '!' }, 2)[0];

                    if (string.Compare(affectedUser, "*") == 0) return;

                    foreach (string userD in userList)
                        if (string.Compare(removeUserMode(userD), removeUserMode(affectedUser), true) != 0)
                            userTemp.Add(userD);
       
                    userList.Clear();

                    foreach (string userD in userTemp)
                        userList.Add(userD);

                    
                    switch (modechange)
                    {
                        case ("+o"):
                            ul.setUserChannelOP(affectedUser, true);

                            ul.setUserChannelOP(affectedUser, true);
                            userList.Add("@" + affectedUser);
                            break;
                        case ("+v"):
                            if (ul.userHasChannelOP(affectedUser))
                                userList.Add("@" + affectedUser);
                            else
                                userList.Add("+" + affectedUser);

                            ul.setUserChannelVoice(affectedUser, true);
                            break;
                        case ("+h"):
                            if (ul.userHasChannelOP(affectedUser))
                                userList.Add("@" + affectedUser);
                            else
                                userList.Add("%" + affectedUser);
                            break;              
                        case ("+a"):
                            userList.Add("&" + affectedUser);
                            break;
                        case ("+q"):
                            if (ul.userHasChannelOP(affectedUser))
                                userList.Add("@" + affectedUser);
                            else
                                userList.Add("~" + affectedUser);
                            break;

                        
                        case ("-q"):
                            userList.Add(affectedUser);
                            break;

                        case ("-o"):
                            ul.setUserChannelOP(affectedUser, false);
                            if (ul.userHasChannelVoice(affectedUser))
                                userList.Add("+" + affectedUser);
                            else
                                userList.Add(affectedUser);

                            break;
                        case ("-a"):
                        
                        case ("-h"):
                            if (ul.userHasChannelOP(affectedUser))
                                userList.Add("@" + affectedUser);
                            else if (ul.userHasChannelVoice(affectedUser))
                                userList.Add("+" + affectedUser);
                            else
                                userList.Add(affectedUser);
                            break;
                        case ("-v"):
                            if (ul.userHasChannelOP(affectedUser))
                                userList.Add("@" + affectedUser);
                            else
                                userList.Add(affectedUser);
                            ul.setUserChannelVoice(affectedUser, false);
                            break;


                        case ("+b"):
                        case ("-b"):
                        case ("+m"):
                        case ("-m"):
                            break;
                        default:
                            userList.Add(affectedUser);
                            break;

                    }
                    

                    Who = affectedUser;
                    OnModeChange(EventArgs.Empty);
                    userList.Sort();
                    userTemp.Clear();

                    break;

                case ("KICK"):

                    userTemp = new List<string>();
                    string kickedUser = messageObject.SplitMessage[1];

                    foreach (string userR in userList)
                        if (string.Compare(removeUserMode(userR) , removeUserMode(kickedUser),true) != 0)
                            userTemp.Add(userR);

                    userList.Clear();

                    foreach (string userT in userTemp)
                        userList.Add(userT);

                    userList.Sort();

                    userTemp.Clear();
                    Who = kickedUser;

                    ul.makeOffline(kickedUser);

                    OnKick(EventArgs.Empty);
                    break;

                case ("PRIVMSG"):

                    string user;
                    string whoSent = messageObject.Source;                         //Who sent is the Source of the Message. (The Channel, or User if private Message)
                    string msg = messageObject.SplitMessage[1].Replace("\r", String.Empty).Replace("\n", String.Empty).Trim();
                    string cmd = msg.Split(' ')[0];
                    string arg = "";

                    if (messageObject.Sender.Contains("!"))
                        user = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!")); //Nick of the Sender
                    else
                        user = messageObject.Sender;

                    user = removeUserMode(user);

                    if (String.Compare(whoSent, Client.NICK, true) == 0)
                    {
                        //If its a user sending, check if it has mirror mode
                        
                        if (ul.userIsMirrored(user) && (ul.userIsOperator(user) || Settings.Default.enforceMirrorOff == false)) //If enforce off is true and user is operator, let it bypass the enforce
                            whoSent = Client.HOME_CHANNEL;
                        else
                            whoSent = user;
                    }

                    if (msg.Length - 1 > cmd.Length)
                        arg = msg.Substring(cmd.Length+1); //the rest of msg

                    //Write Message on Console
                    if (msg.Contains("\x01"+"ACTION "))
                    {
                        msg = msg.Replace("\x01" + "ACTION ", String.Empty).Replace("\x01", String.Empty);

                        if (String.Compare(whoSent, Client.NICK, true) == 0)
                            WriteMessage("             * : " + user + " " + msg, currentColorScheme.Notice);
                        
                        else if (msg.ToLower().Contains(Client.NICK.ToLower()))
                            WriteMessage("             * : " + user + " " + msg, currentColorScheme.Mention);
                        
                        else
                            WriteMessage("             * : " + user + " " + msg);
                    }
                    else
                    {
                        if (String.Compare(whoSent, Client.NICK, true) == 0)
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg, currentColorScheme.Notice);
                        }
                        else if (msg.ToLower().Contains(Client.NICK.ToLower()))
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg, currentColorScheme.Mention);
                        }
                        else
                        {
                            string alignedNick = user.Truncate(13);
                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + msg);
                        }

                    }


                    //StartParsing
                    if ((String.Compare(cmd.Replace(",", String.Empty), "hello", true) == 0
                            || String.Compare(cmd.Replace(",", String.Empty), "hi", true) == 0
                            || String.Compare(cmd.Replace(",", String.Empty), "hey", true) == 0)
                            && arg.ToLower().Contains(Client.NICK.ToLower()))
                        {
                            WriteMessage("* Received a hello from " + user, currentColorScheme.BotReport);
                            hello(whoSent, user);
                        }

                    else if (String.Compare(cmd, Client.NICK + ",", true) == 0 && !String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("* Received a Question from " + user, currentColorScheme.BotReport);
                            parseQuestion(whoSent, user, arg);
                        }
                    else if (cmd[0] == Client.SYMBOL)   //Bot Command
                        {
                            cmd = cmd.Substring(1);

                            if (String.Compare(msg, "!anime best anime ever", true) == 0)
                            {
                                Client.sendMessage(Privmsg(whoSent, "Code Geass: Hangyaku no Lelouch : [Finished Airing] [25 episodes] [8,86 / 10] -> http://myanimelist.net/anime/1575/Code_Geass:_Hangyaku_no_Lelouch"));
                            }
                        else if (String.Compare(cmd, "help", true) == 0)
                            {
                                WriteMessage("* Received a Help request from " + user, currentColorScheme.BotReport);
                                help(user);
                            }

                        else if (String.Compare(cmd, "mirror", true) == 0)
                            {
                                WriteMessage("* Received a mirror toogle request from " + user, currentColorScheme.BotReport);
                                toogleMirror(user);
                            }
                        else if (String.Compare(cmd, "enforcemirror", true) == 0)
                            {
                                WriteMessage("* Received an enforce mirror off toogle request from " + user, currentColorScheme.BotReport);
                                toogleEnforceOff(user);
                            }

                        else if (String.Compare(cmd, "rules", true) == 0)
                            {
                                WriteMessage("* Received a Rules request from " + user, currentColorScheme.BotReport);
                                rules(whoSent, user);
                            }

                        else if (String.Compare(cmd, "quit", true) == 0)
                            {
                                WriteMessage("* Received a quit request from " + user, currentColorScheme.BotReport);
                                if(quitIRC(user)) OnQuit(EventArgs.Empty);
                            }
                        else if (String.Compare(cmd, "oplist", true) == 0)
                            {
                                WriteMessage("* Received a oplist request from " + user, currentColorScheme.BotReport);
                                opList(user);
                            }
                        else if (String.Compare(cmd, "roll", true) == 0)
                            {
                                WriteMessage("* Received a Roll request from " + user, currentColorScheme.BotReport);
                                roll(whoSent, user);
                            }
                        else if (String.Compare(cmd, "say", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a say request from " + user, currentColorScheme.BotReport);
                                say(Client.HOME_CHANNEL, arg, user);
                            }
                        else if (String.Compare(cmd, "greetme", true) == 0)
                            {
                                if (String.IsNullOrEmpty(arg))
                                {
                                    WriteMessage("* Received a Greet TOOGLE request from " + user, currentColorScheme.BotReport);
                                    GreetToogle(user);
                                }
                                else
                                {
                                    WriteMessage("* Received a Greet request from " + user, currentColorScheme.BotReport);
                                    AddGreetings(arg, user);
                                }
                            }
                        else if (String.Compare(cmd, "greetmenow", true) == 0)
                            {
                                    WriteMessage("* Received a Greet me now request from " + user, currentColorScheme.BotReport);
                                    greetUser(user);
                            }
                        else if (String.Compare(cmd, "me", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a me request from " + user, currentColorScheme.BotReport);
                                me(Client.HOME_CHANNEL, arg, user);
                            }

                        else if (String.Compare(cmd, "silence", true) == 0)
                            {
                                WriteMessage("* Received a silence request from " + user, currentColorScheme.BotReport);
                                silence(user);
                            }
                        else if (String.Compare(cmd, "rename", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Rename request from " + user, currentColorScheme.BotReport);
                                if (ul.userIsOperator(user)) changeNick(arg);
                            }

                        else if (String.Compare(cmd, "op", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a op request from " + user, currentColorScheme.BotReport);
                                addBotOP(user, arg);
                            }
                        else if (String.Compare(cmd, "deop", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a deop request from " + user, currentColorScheme.BotReport);
                                removeBotOP(user, arg);
                            }
                        else if (String.Compare(cmd, "mute", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a mute request from " + user, currentColorScheme.BotReport);
                                muteUser(user, arg);
                            }
                        else if (String.Compare(cmd, "unmute", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a unmute request from " + user, currentColorScheme.BotReport);
                                unmuteUser(user, arg);
                            }
                        else if (String.Compare(cmd, "toF", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a temp. conversion to F request from " + user, currentColorScheme.BotReport);
                                toFahrenheit(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "toC", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a temp. conversion to C request from " + user, currentColorScheme.BotReport);
                                toCelcius(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "time", true) == 0)
                            {
                                WriteMessage("* Received a Time request from " + user, currentColorScheme.BotReport);
                                time(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "wiki", true) == 0)
                            {
                                WriteMessage("* Received a explain request from " + user, currentColorScheme.BotReport);
                                wiki(whoSent, user, arg);
                            }

                        else if (String.Compare(cmd, "anime", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a animeSearch request from " + user, currentColorScheme.BotReport);
                                animeSearch(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "youtube", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a youtubeSearch request from " + user, currentColorScheme.BotReport);
                                youtubeSearch(whoSent, user, arg);
                            }
                        else if ((String.Compare(cmd, "giphy", true) == 0 || String.Compare(cmd, "g", true) == 0) && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Giphy request from " + user, currentColorScheme.BotReport);
                                giphySearch(whoSent, user, arg);
                            }

                        else if (String.Compare(cmd, "poke", true) == 0)
                            {
                                WriteMessage("* Received a Poke request from " + user, currentColorScheme.BotReport);
                                poke(whoSent, user);
                            }

                        else if (String.Compare(cmd, "trivia", true) == 0)
                            {
                                WriteMessage("* Received a Trivia request from " + user, currentColorScheme.BotReport);
                                trivia(whoSent, user);
                            }
                        else if (String.Compare(cmd, "nick", true) == 0)
                            {
                                WriteMessage("* Received a nickname request from " + user, currentColorScheme.BotReport);
                                nickGen(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "kill", true) == 0)
                            {
                                WriteMessage("* Received a Kill request from " + user, currentColorScheme.BotReport);
                                killUser(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "fact", true) == 0 || String.Compare(cmd, "facts", true) == 0)
                            {
                                WriteMessage("* Received a Fact request from " + user, currentColorScheme.BotReport);
                                factUser(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "lastkill", true) == 0)
                            {
                                WriteMessage("* Received a lastkill request from " + user, currentColorScheme.BotReport);
                                lastKill(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "rkill", true) == 0)
                            {
                                WriteMessage("* Received a lastkill request from " + user, currentColorScheme.BotReport);
                                randomKill(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "quote", true) == 0 || String.Compare(cmd, "q", true) == 0)
                            {
                                WriteMessage("* Received a Quote request from " + user, currentColorScheme.BotReport);

                                if (string.Compare(arg.ToLower().Split(new char[] { ' ' }, 2)[0], "add") == 0)  //add
                                {
                                    addQuote(arg, user);
                                }
                                else //lookup or random
                                {
                                    printQuote(whoSent, arg, user);
                                }

                            }
                        else if ((String.Compare(cmd, "choose", true) == 0 || String.Compare(cmd, "c", true) == 0) && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Choose request from " + user, currentColorScheme.BotReport);
                                choose(whoSent, user, arg);
                            }
                        else if ((String.Compare(cmd, "shuffle", true) == 0 || String.Compare(cmd, "s", true) == 0) && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Shuffle request from " + user, currentColorScheme.BotReport);
                                shuffle(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "funk", true) == 0 || String.Compare(cmd, "f", true) == 0)
                            {
                                WriteMessage("* Received a Funk request from " + user, currentColorScheme.BotReport);

                                if (String.IsNullOrEmpty(arg)) //lookup or random
                                    printFunk(whoSent, arg, user);

                                else
                                    addFunk(arg, user);
                            }
                        else if (String.Compare(cmd, "reload", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a reload request from " + user, currentColorScheme.BotReport);
                                reloadTexts(user, arg);
                            }
                        else if (String.Compare(cmd, "stats", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a stats request from " + user, currentColorScheme.BotReport);
                                statsPrint(whoSent, user, arg);
                            }
                        else if (String.Compare(cmd, "set", true) == 0 && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a set options request from " + user, currentColorScheme.BotReport);
                                setSetting(user, arg);
                            }

                            else if (String.Compare(cmd, "acknowledge", true) == 0 || String.Compare(cmd, "a", true) == 0)
                            {
                                WriteMessage("* Received a acknowledge request from " + user, currentColorScheme.BotReport);
                                ul.clearUserMessages(user);
                            }
                        else if ((String.Compare(cmd, "tell", true) == 0 || String.Compare(cmd, "message", true) == 0) && !String.IsNullOrEmpty(arg))
                            {
                                WriteMessage("* Received a Tell request from " + user, currentColorScheme.BotReport);
                                tell(user, arg);
                            }

                        
                        }


                    else if ((msg.Contains("youtu.be") && (msg.Contains("?v=") == false && msg.Contains("&v=") == false)) 
                        || (msg.Contains("youtube") && msg.Contains("watch") && (msg.Contains("?v=") || msg.Contains("&v="))))
                        {
                            WriteMessage("* Detected a Youtube video from " + user, currentColorScheme.BotReport);
                            youtube(whoSent, user, msg);
                        }

                    else if (msg.Contains("vimeo.com"))
                        {
                            WriteMessage("* Detected an vimeo video from " + user, currentColorScheme.BotReport);
                            vimeo(whoSent, user, msg);
                        }

                    else if (msg.Contains("reddit.com") && msg.Contains("/r/") && msg.Contains("/comments/"))
                        {
                            WriteMessage("* Detected a reddit link from " + user, currentColorScheme.BotReport);
                            redditLink(whoSent, user, msg);
                        }

                    else if (msg.Contains("twitter.com") && msg.Contains("/status/"))
                        {
                            WriteMessage("* Detected a twitter link from " + user, currentColorScheme.BotReport);
                            twitter(whoSent, user, msg);
                        }
                    else if (msg.Contains("http://") || msg.Contains("https://"))
                        {
                            WriteMessage("* Detected an url from " + user, currentColorScheme.BotReport);
                            urlTitle(whoSent, user, msg);
                        }

                    else if (message.Contains("\x01"))
                        {
                            if (cmd.Contains("VERSION")){
                                WriteMessage("* Received a CTCP version request from " + user, currentColorScheme.BotReport);
                                ctcpVersion(user);
                            }

                            else if (cmd.Contains("TIME")){
                                WriteMessage("* Received a CTCP Time request from " + user, currentColorScheme.BotReport);
                                ctcpTime(user);
                            }

                            else if (cmd.Contains("PING")){
                                WriteMessage("* Received a CTCP ping request from " + user, currentColorScheme.BotReport);
                                ctcpPing(user, arg);
                            }
                        }

                    else //No parsing, just a normal Message
                        {
                            if (whoSent == Client.HOME_CHANNEL && msg != null) //Add to past messages
                            {
                                tmc.feed(msg);
                                tmcCount++;
                            }
                        }

                    break;

                case ("NOTICE"):
                    if (message.Contains("\x01"))
                    {
                        string userN;
                        string argg;
                        string msgN = messageObject.SplitMessage[1].Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                        string cmdN = msgN.Split(' ')[0];

                        if (messageObject.Sender.Contains('!'))
                                userN = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));   //Nick of the Sender
                        else userN = messageObject.Sender;

                        if (msgN.Length - 1 > cmdN.Length)
                            argg = msgN.Substring(cmdN.Length);                 //the rest of msg
                        else argg = "";

                        if (cmdN.Contains("VERSION"))
                        {
                            WriteMessage("* Received a CTCP version request from " + userN, currentColorScheme.BotReport);
                            ctcpVersion(userN);
                        }

                        else if (cmdN.Contains("TIME"))
                        {
                            WriteMessage("* Received a CTCP Time request from " + userN, currentColorScheme.BotReport);
                            ctcpTime(userN);
                        }

                        else if (cmdN.Contains("PING"))
                        {
                            WriteMessage("* Received a CTCP ping request from " + userN, currentColorScheme.BotReport);

                            ctcpPing(userN, argg);
                        }
                    }
                    else {
                        string alignedNick = messageObject.Sender;

                        if (!string.IsNullOrWhiteSpace(alignedNick))
                        { 
                            try {
                                if(messageObject.Sender.Contains("!"))
                                    alignedNick = messageObject.Sender.Substring(0, messageObject.Sender.IndexOf("!"));  //Nick of the Sender
                            }
                            catch { }
                            finally {
                                alignedNick = alignedNick.Truncate(13);
                            }

                            int tab = 15 - alignedNick.Length;

                            for (int i = 0; i < tab; i++)
                                alignedNick = alignedNick + " ";
                            WriteMessage(alignedNick + ": " + messageObject.SplitMessage[1], currentColorScheme.Notice);
                        }
                    }
                        
                    break;

                default:
                    WriteMessage("* " + messageObject.Type + " " + messageObject.CompleteMessage);
                    break;
            }
        }


        static public char getUserMode(string user, List<string> userList)
        {
            if (string.IsNullOrWhiteSpace(user)) return '0';

            user = user.Trim();

            foreach (string u in userList)
            {
                if (String.Compare(u.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty), user, true) == 0)
                {
                    switch (u[0])
                    {
                        case '@':
                        case '+':
                        case '%':
                        case '~':
                        case '&':
                            return u[0];
                        default:
                            return '0';
                    }
                }
            }
            return '0';
        }

        static public string removeUserMode(string user)
        {
            char[] usermodes = { '@', '+', '%', '~', '&'};
            
            if (usermodes.Any((s) => Convert.ToChar(user.Substring(0, 1)).Equals(s)))
                return user.Substring(1).Trim();

            else return user.Trim();
        }

        public void ReadKills()
        {
            kill.Clear();
            killsUsed.Clear();
            killgen = new TextMarkovChain();


            if (File.Exists("TextFiles/kills.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/kills.txt");
                    while (sr.Peek() >= 0)
                    {
                        string killS = sr.ReadLine();

                        if(killS.Length > 1 && !(killS[0] == '/' && killS[1] == '/'))
                        {
                            kill.Add(killS);
                            killgen.feed(killS);
                        }
                            
                    }
                        

                    sr.Close();
                }
                catch
                {
                }
                
            }
            else
            {
                Settings.Default.killEnabled = false;
                Settings.Default.Save();
            }
        }

        public void ReadFacts()
        {
            facts.Clear();
            factsUsed.Clear();

            if (File.Exists("TextFiles/facts.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/facts.txt");
                    while (sr.Peek() >= 0)
                    {
                        string factS = sr.ReadLine();

                        if (factS.Length > 1 && !(factS[0] == '/' && factS[1] == '/'))
                        {
                            facts.Add(factS);
                        }

                    }


                    sr.Close();
                }
                catch
                {
                }

            }
            else
            {
                Settings.Default.factsEnabled = false;
                Settings.Default.Save();
            }
        }

        public void ReadRules()
        {
            rls.Clear();
            if (File.Exists("TextFiles/rules.txt"))
            {
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
            else
            {
                Settings.Default.rules_Enabled = false;
                Settings.Default.Save();
            }
        }

        public void ReadHelp()
        {
            hlp.Clear();
            if (File.Exists("TextFiles/help.txt"))
            {
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
            else
            {
                Settings.Default.help_Enabled = false;
                Settings.Default.Save();
            }
        }

        public void ReadTrivia() //Reads the Trivia stuff
        {
            tri.Clear();

            if (File.Exists("TextFiles/trivia.txt"))
            {
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
            else
            {
                Settings.Default.triviaEnabled = false;
                Settings.Default.Save();
            }
        }

        public void AddGreetings(string args, string nick)
        {
            ul.setGreeting(nick, args, true);
            ul.saveData();
        }

        void GreetToogle(string nick)
        {
            string message;
            string state = "disabled";

            foreach (User u in ul.Users)
            {
                if (u.Nick == nick)
                {
                    u.GreetingEnabled = !u.GreetingEnabled;

                    if(u.GreetingEnabled) state = "enabled";
                    message = string.Empty;
                    message = Notice(nick, "Your Greeting is now " + state);

                    ul.saveData();

                    Client.sendMessage(message);
                    return;
                }
            }

            Client.sendMessage(Notice(nick, "You didn't set a Greeting yet"));
        }

        public void ReadNickGen()//These are for the Nick gen
        {
            nickGenStrings = new List<string>();
            nickGenStrings.Clear();
            if (File.Exists("TextFiles/nickGen.txt"))
            {
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
            else
            {
                Settings.Default.nickEnabled = false;
                Settings.Default.Save();
            }
        }

        public void ReadQuotes()
        {
            quotes = new List<string>();
            quotes.Clear();

            if (File.Exists("TextFiles/quotes.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/quotes.txt");
                    while (sr.Peek() >= 0)
                    {
                        quotes.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        public void saveQuotes(){
            using (StreamWriter newTask = new StreamWriter("TextFiles/quotes.txt", false))
            {
                foreach (string q in quotes)
                {
                    newTask.WriteLine(q);
                }
            }
        }

        public void ReadFunk()
        {
            funk = new List<string>();
            funk.Clear();

            if (File.Exists("TextFiles/funk.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/funk.txt");
                    while (sr.Peek() >= 0)
                    {
                        funk.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        public void saveFunk()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/Funk.txt", false))
            {
                foreach (string q in funk)
                {
                    newTask.WriteLine(q);
                }
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

            result = "PRIVMSG " + destinatary + " :" + message.Trim() + "\r\n";

            if (message.Contains("\x01"+"ACTION "))
            {
                message = Client.NICK + " " + message.Trim().Replace("\x01" + "ACTION ", string.Empty).Replace("\x01", string.Empty);

                WriteMessage("             * : " + message, currentColorScheme.OwnMessage);
            }
            else
            {
                string alignedNick = Client.NICK.Truncate(13);
                int tab = 15 - alignedNick.Length;

                for (int i = 0; i < tab; i++)
                    alignedNick = alignedNick + " ";

                WriteMessage(alignedNick + ": " + message, currentColorScheme.OwnMessage);

            }
            return result;
        }

        /// <summary>
        /// Sends a Notice to the destinatary
        /// </summary>
        /// <param Name="destinatary">string with either a User or a channel, it's where the Message will be sent</param>
        /// <param Name="Message">String of text with the Message that will be delivered</param>
        public string Notice(string destinatary, string message)
        {
            string result;

            result = "NOTICE " + destinatary + " :" + message.Trim() + "\r\n";

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
            Client.sendMessage(message);

        }
        public void whoisUser(string nick)
        {
            string message = "WHOIS " + nick + "\n";
            Client.sendMessage(message);

        }

        public void randomTextSender(object source, ElapsedEventArgs e)
        {
            string message;

            if (!Settings.Default.randomTextEnabled || tmcCount<8 || !tmc.readyToGenerate()) return;

            message = Privmsg(Client.HOME_CHANNEL, "\x02" +  tmc.generateSentence() + "\x02");
            Client.sendMessage(message);

        }

        ///////////////////////////////////
        //       Commands Functions      //
        ///////////////////////////////////

        /// <summary>
        /// Adds a Nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="Nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be made bot operator</param>
        /// 

        void reloadTexts(string user, string arg)
        {
            if (!ul.userIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });

            foreach (string option in options)
            {
                switch (option)
                {
                    case "all":
                        ReadFunk();
                        ReadQuotes();
                        ReadKills();
                        ReadFacts();
                        ReadTrivia();
                        ReadNickGen();
                        ReadHelp();
                        ReadRules();
                        Client.sendMessage(Notice(user, "Reloaded everything!"));
                        break;
                    case "rules":
                    case "rule":
                        ReadRules();
                        Client.sendMessage(Notice(user, "Reloaded rules!"));
                        break;
                    case "help":
                        ReadHelp();
                        Client.sendMessage(Notice(user, "Reloaded help!"));
                        break;
                    case "nick":
                    case "nicks":
                        ReadNickGen();
                        Client.sendMessage(Notice(user, "Reloaded nicks!"));
                        break;
                    case "trivia":
                    case "trivias":
                        ReadTrivia();
                        Client.sendMessage(Notice(user, "Reloaded trivia!"));
                        break;
                    case "kills":
                    case "kill":
                        ReadKills();
                        Client.sendMessage(Notice(user, "Reloaded kills!"));
                        break;

                    case "facts":
                    case "fact":
                        ReadFacts();
                        Client.sendMessage(Notice(user, "Reloaded facts!"));
                        break;
                    case "quotes":
                    case "quote":
                        ReadQuotes();
                        Client.sendMessage(Notice(user, "Reloaded quotes!"));
                        break;
                    case "funk":
                        ReadFunk();
                        Client.sendMessage(Notice(user, "Reloaded funk!"));
                        break;
                    default:
                        break;
                }
            }
        }

        void statsPrint(string whoSent, string user, string arg)
        {
            if (!ul.userIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });

            foreach (string option in options)
            {
                switch (option)
                {
                    case "roll":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getRoll()[0] + " Lifetime: " + stats.getRoll()[1]));
                        break;
                    case "anime":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getAnime()[0] + " Lifetime: " + stats.getAnime()[1]));
                        break;
                    case "help":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getHelp()[0] + " Lifetime: " + stats.getHelp()[1]));
                        break;
                    case "rules":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getRules()[0] + " Lifetime: " + stats.getRules()[1]));
                        break;
                    case "greet":
                    case "greetings":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getGreet()[0] + " Lifetime: " + stats.getGreet()[1]));
                        break;
                    case "time":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getTime()[0] + " Lifetime: " + stats.getTime()[1]));
                        break;
                    case "temperature":
                    case "temperatures":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getTemperature()[0] + " Lifetime: " + stats.getTemperature()[1]));
                        break;
                    case "wiki":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getWiki()[0] + " Lifetime: " + stats.getWiki()[1]));
                        break;
                    case "poke":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getPoke()[0] + " Lifetime: " + stats.getPoke()[1]));
                        break;
                    case "trivia":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getTrivia()[0] + " Lifetime: " + stats.getTrivia()[1]));
                        break;
                    case "quote":
                    case "quotes":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getQuote()[0] + " Lifetime: " + stats.getQuote()[1]));
                        break;
                    case "choose":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getChoose()[0] + " Lifetime: " + stats.getChoose()[1]));
                        break;
                    case "shuffle":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getShuffle()[0] + " Lifetime: " + stats.getShuffle()[1]));
                        break;
                    case "funk":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getFunk()[0] + " Lifetime: " + stats.getFunk()[1]));
                        break;
                    case "giphy":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getGiphy()[0] + " Lifetime: " + stats.getGiphy()[1]));
                        break;
                    case "nick":
                    case "nicks":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getNick()[0] + " Lifetime: " + stats.getNick()[1]));
                        break;
                    case "question":
                    case "questions":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getQuestion()[0] + " Lifetime: " + stats.getQuestion()[1]));
                        break;
                    case "youtube":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getYoutube()[0] + " Lifetime: " + stats.getYoutube()[1]));
                        break;
                    case "kill":
                    case "kills":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getKill()[0] + " Lifetime: " + stats.getKill()[1]));
                        break;
                    case "fact":
                    case "facts":
                        Client.sendMessage(Privmsg(whoSent, "Session: " + stats.getFact()[0] + " Lifetime: " + stats.getFact()[1]));
                        break;


                    default: break;
                }
            }

        }
        void setSetting(string user, string arg)
        {
            if (!ul.userIsOperator(user)) return;

            string[] options = arg.ToLower().Trim().Split(new char[] { ' ' });
            bool status = false;

            if (options.Length < 2)
            {
                Client.sendMessage(Notice(user, "Not enought arguments"));
                return;
            }

            if (options[1] != "off")
                if (options[1] == "on")
                    status = true;
                else { Client.sendMessage(Notice(user, "Invalid Status")); return; }

            switch (options[0])
            {
                case "rules":
                    Settings.Default.rules_Enabled = status;
                    Client.sendMessage(Notice(user, "Rules is now " + (status ? "enabled" : "disabled")));
                    break;
                case "help":
                    Settings.Default.help_Enabled = status;
                    Client.sendMessage(Notice(user, "Help is now " + (status ? "enabled" : "disabled")));
                    break;
                case "time":
                    Settings.Default.timeEnabled = status;
                    Client.sendMessage(Notice(user, "Time is now " + (status ? "enabled" : "disabled")));
                    break;
                case "conversion":
                case "conversions":
                    Settings.Default.conversionEnabled = status;
                    Client.sendMessage(Notice(user, "Conversions is now " + (status ? "enabled" : "disabled")));
                    break;
                case "wiki":
                    Settings.Default.wikiEnabled = status;
                    Client.sendMessage(Notice(user, "Wiki is now " + (status ? "enabled" : "disabled")));
                    break;
                case "anime":
                    Settings.Default.aniSearchEnabled = status;
                    Client.sendMessage(Notice(user, "Anime Search is now " + (status ? "enabled" : "disabled")));
                    break;
                case "youtube":
                    Settings.Default.youtubeSearchEnabled = status;
                    Client.sendMessage(Notice(user, "Youtube Search is now " + (status ? "enabled" : "disabled")));
                    break;
                case "choose":
                    Settings.Default.chooseEnabled = status;
                    Client.sendMessage(Notice(user, "Choose is now " + (status ? "enabled" : "disabled")));
                    break;
                case "shuffle":
                    Settings.Default.shuffleEnabled = status;
                    Client.sendMessage(Notice(user, "Shuffle is now " + (status ? "enabled" : "disabled")));
                    break;
                case "roll":
                    Settings.Default.roll_Enabled = status;
                    Client.sendMessage(Notice(user, "Roll is now " + (status ? "enabled" : "disabled")));
                    break;
                case "hello":
                    Settings.Default.hello_Enabled = status;
                    Client.sendMessage(Notice(user, "Hello is now " + (status ? "enabled" : "disabled")));
                    break;
                case "nick":
                case "nicks":
                    Settings.Default.nickEnabled = status;
                    Client.sendMessage(Notice(user, "Nickname Generator is now " + (status ? "enabled" : "disabled")));
                    break;
                case "text":
                    Settings.Default.randomTextEnabled = status;
                    Client.sendMessage(Notice(user, "Random Text is now " + (status ? "enabled" : "disabled")));
                    break;
                case "poke":
                    Settings.Default.pokeEnabled = status;
                    Client.sendMessage(Notice(user, "Poke is now " + (status ? "enabled" : "disabled")));
                    break;
                case "trivia":
                    Settings.Default.triviaEnabled = status;
                    Client.sendMessage(Notice(user, "Trivia is now " + (status ? "enabled" : "disabled")));
                    break;
                case "kill":
                case "kills":
                    Settings.Default.killEnabled = status;
                    Client.sendMessage(Notice(user, "Kill is now " + (status ? "enabled" : "disabled")));
                    break;

                case "fact":
                case "facts":
                    Settings.Default.factsEnabled = status;
                    Client.sendMessage(Notice(user, "Fact is now " + (status ? "enabled" : "disabled")));
                    break;
                case "questions":
                case "question":
                    Settings.Default.questionEnabled = status;
                    Client.sendMessage(Notice(user, "Questions are now " + (status ? "enabled" : "disabled")));
                    break;
                case "greetings":
                case "greet":
                case "greets":
                    Settings.Default.greetingsEnabled = status;
                    Client.sendMessage(Notice(user, "Greetings are now " + (status ? "enabled" : "disabled")));
                    break;
                case "quotes":
                case "quote":
                    Settings.Default.quotesEnabled = status;
                    Client.sendMessage(Notice(user, "Quotes are now " + (status ? "enabled" : "disabled")));
                    break;
                case "funk":
                    Settings.Default.funkEnabled = status;
                    Client.sendMessage(Notice(user, "Funk is now " + (status ? "enabled" : "disabled")));
                    break;
                case "giphy":
                    Settings.Default.giphyEnabled = status;
                    Client.sendMessage(Notice(user, "Giphy is now " + (status ? "enabled" : "disabled")));
                    break;

                case "reddit":
                case "reddittitle":
                    Settings.Default.redditEnabled = status;
                    Client.sendMessage(Notice(user, "Reddit Titles are now " + (status ? "enabled" : "disabled")));
                    break;

                case "vimeo":
                case "vimeotitle":
                    Settings.Default.vimeoEnabled = status;
                    Client.sendMessage(Notice(user, "Vimeo Titles are now " + (status ? "enabled" : "disabled")));
                    break;

                case"twitter":
                    Settings.Default.twitterEnabled = status;
                    Client.sendMessage(Notice(user, "Tweets are now " + (status ? "enabled" : "disabled")));
                    break;

                case "youtubetitle":
                    Settings.Default.youtube_Enabled = status;
                    Client.sendMessage(Notice(user, "Youtube Info is now " + (status ? "enabled" : "disabled")));
                    break;

                case "url":
                case "urltitle":
                    Settings.Default.urlTitleEnabled = status;
                    Client.sendMessage(Notice(user, "URL Info is now " + (status ? "enabled" : "disabled")));
                    break;

                case "tell":
                    Settings.Default.tellEnabled = status;
                    Client.sendMessage(Notice(user, "Tell is now " + (status ? "enabled" : "disabled")));
                    break;

                default: break;
            }
            Settings.Default.Save();
        }


        bool addBotOP(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!ul.userIsOperator(nick))
                return false;

            ul.opUser(targetUser);
            message = Notice(nick, targetUser + " was set as a bot operator!");
            Client.sendMessage(message);
            return true;

        }

        bool muteUser(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

            if (!ul.userIsOperator(nick))
                return false;

            ul.muteUser(targetUser);
            message = Notice(nick, targetUser + " was muted!");
            Client.sendMessage(message);
            return true;

        }

        bool unmuteUser(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

            if (!ul.userIsOperator(nick))
                return false;

            ul.unmuteUser(targetUser);
            message = Notice(nick, targetUser + " was unmuted!");
            Client.sendMessage(message);
            return true;

        }
        /// <summary>
        /// Removes a Nick Name to the Bot Operator list
        /// </summary>
        /// <param Name="Nick">the User that called the command</param>
        /// <param Name="targetUser">the User to be removed from the bot operator list</param>
        bool removeBotOP(string nick, string targetUser)
        {
            string message;

            targetUser = targetUser.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!ul.userIsOperator(nick))
                return false;

            takeOps(targetUser);

            message = Notice(nick, targetUser + " was removed as a bot operator!");
            Client.sendMessage(message);
            return true;
        }

        void opList(string nick)
        {
            string message;

            if (ul.userIsOperator(nick))
            {
                message = Notice(nick, "Bot operators:");
                Client.sendMessage(message);
                foreach (User u in ul.Users)
                {
                    if (u.IsOperator)
                        Client.sendMessage(Notice(nick, "     -> " + u.Nick));
                }
            }
        }

        void say(string CHANNEL, string args, string nick)
        {
            if (ul.userIsOperator(nick))
            {
                Client.sendMessage(Privmsg(CHANNEL, args));
            }
        }

        void me(string CHANNEL, string args, string nick)
        {
            if (ul.userIsOperator(nick))
                Client.sendMessage(Privmsg(CHANNEL, "\x01" + "ACTION " + args + "\x01"));
        }

        public void silence(string nick)
        {
            string message;
            if (ul.userIsOperator(nick))
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

                Client.sendMessage(message);
                return;
            }
        }

        void hello(string CHANNEL, string nick)
        {
            if (!ul.userIsMuted(nick) && Settings.Default.hello_Enabled == true && Settings.Default.silence == false)
            {
                string message = Privmsg(CHANNEL, "Hello " + nick + "!");
                Client.sendMessage(message);
            }
        }

        void help(string nick)
        {
            string message;

            if (!ul.userIsMuted(nick) && Settings.Default.help_Enabled == true)
            {
                foreach (string h in hlp)
                {
                    message = Notice(nick, h.Replace("\n", "").Replace("\r", ""));
                    Client.sendMessage(message);

                }
                stats.help();
            }
        }

        void rules(string CHANNEL, string nick)
        {
            string message;
            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.rules_Enabled == true)
            {
                if (ul.userIsOperator(nick))
                {
                    foreach (string h in rls)
                    {
                        if (!string.IsNullOrWhiteSpace(h))
                        {
                            message = Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                            Client.sendMessage(message);  
                        }
  
                    }
                    stats.rules();
                    return;
                }

            }
            else if (Settings.Default.rules_Enabled == true)
            {
                foreach (string h in rls)
                {
                    if (!string.IsNullOrWhiteSpace(h))
                    {
                        message = Privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                        Client.sendMessage(message);
                    }
                }
                stats.rules();
                return;
            }
        }

        void toogleMirror(string nick)
        {
            string message;
            bool mirror = false;

            mirror = ul.toogleMirror(nick);
            
            message = Notice(nick, "MirrorMode is now " + (mirror ? "enabled" :"disabled"));
            Client.sendMessage(message);  
        }

        void toogleEnforceOff(string nick)
        {
            string message;

            if (!ul.userIsOperator(nick)) return;

            Settings.Default.enforceMirrorOff = !Settings.Default.enforceMirrorOff;

            Settings.Default.Save();

            message = Notice(nick, "Enforce Mirror Mode Off is now " + (Settings.Default.enforceMirrorOff ? "enabled" : "disabled"));
            Client.sendMessage(message);

            OnEnforceMirrorChanged(EventArgs.Empty);
        }
        

        void roll(string CHANNEL, string nick)
        {
            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.roll_Enabled == true)
            {
                Random random = new Random();
                int number = random.Next(0, 100);

                nick = nick.Replace("\r", "");
                string message = Privmsg(CHANNEL, nick + " rolled a " + number);
                Client.sendMessage(message);
                stats.roll();
            }
        }

        private void poke(string CHANNEL, string nick)
        {
            string message;
            int userNumber = 0;
            Random rnd = new Random();

            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.pokeEnabled == true)
            {
                do
                {
                    userNumber = rnd.Next((userList.Count));
                }
                while (removeUserMode(userList[userNumber]) == nick);

                message = Privmsg(CHANNEL, "\x01" + "ACTION " + "pokes " + userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) + "\x01");
                Client.sendMessage(message);
                stats.poke();
            }
        }

        void toFahrenheit(string CHANNEL, string nick, string args)
        {
            string message;
            double f = 0;
            double c = 0;

            if (ul.userIsMuted(nick) || Settings.Default.silence || !Settings.Default.conversionEnabled) return;

            try
            {
                c = Convert.ToDouble(args);
                f = ((9.0 / 5.0) * c) + 32;
            }
            catch
            {
                message = Privmsg(CHANNEL, "Could not parse arguments");
                Client.sendMessage(message);
                return;
            }

            message = Privmsg(CHANNEL, c + " C is " + Math.Round(f, 2) + " F");
            Client.sendMessage(message);
            stats.temperature();
            return;

        }

        void toCelcius(string CHANNEL, string nick, string args)
        {
            string message;
            double c = 0;
            double f = 0;

            if (ul.userIsMuted(nick) || Settings.Default.silence || !Settings.Default.conversionEnabled) return;

            try
            {
                f = Convert.ToDouble(args);
                c = (5.0 / 9.0) * (f - 32);
            }
            catch
            {
                message = Privmsg(CHANNEL, "Could not parse arguments");
                Client.sendMessage(message);
                return;
            }

            message = Privmsg(CHANNEL, f + " F is " + Math.Round(c, 2) + " C");
            Client.sendMessage(message);
            stats.temperature();
            return;

        }

        public void trivia(string CHANNEL, string nick)
        {
            if (tri == null || ul.userIsMuted(nick) || tri.Count == 0 || Settings.Default.silence || !Settings.Default.triviaEnabled) return;

            Random rnd = new Random();

            string message = Privmsg(CHANNEL, tri[rnd.Next(tri.Count)]);
            Client.sendMessage(message);
            stats.trivia();
            return;
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

            GoogleTimeZone.GoogleTimeZone g = new GoogleTimeZone.GoogleTimeZone();
            string json;

            if (ul.userIsMuted(nick)) return;

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

                    case "eet":
                    case "stillbutterfly":
                    case "romania": location = EET; timezoneS = "EET";
                        break;

                    case "cet": location = CET; timezoneS = "CET";
                        break;
                    case "wer": location = WER; timezoneS = "WER";
                        break;

                    case "gmt":
                    case "masterrace": location = GMT; timezoneS = "GMT";
                        break;

                    case "utc": wantUTC = true; timezoneS = "UTC";
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

                    case "mst":
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
                Client.sendMessage(message);
                stats.time();

            }
        }

        public void youtube(string CHANNEL, string nick, string line)
        {
            if (String.IsNullOrEmpty(line)) return;
            if (ul.userIsMuted(nick)) return;

            if (!Settings.Default.silence && Settings.Default.youtube_Enabled)
            {
                string id = YoutubeUseful.getYoutubeIdFromURL(line);

                string result = YoutubeUseful.getYoutubeInfoFromID(id);

                string message = Privmsg(CHANNEL, result);
                Client.sendMessage(message);
            }
        }

        public void twitter(string CHANNEL, string nick, string line)
        {
            string author, tweet, message;

            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == true || Settings.Default.twitterEnabled == false) return;
            else
            {

                string ID = Useful.getBetween(line, "/status/", "?");
                long tweetID = Convert.ToInt64(ID);

                TwitterStatus tweetResult = service.GetTweet(new GetTweetOptions { Id = tweetID });

                author = HttpUtility.HtmlDecode(tweetResult.Author.ScreenName);
                tweet = HttpUtility.HtmlDecode(tweetResult.Text.Replace("\n", " "));


                message = Privmsg(CHANNEL, "Tweet by @" + author + " : " + tweet);
                
                Client.sendMessage(message);
            }
        }

        public void urlTitle(string CHANNEL, string nick, string line)
        {
            string title, message, url=null, html;
            string[] split;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == true || Settings.Default.urlTitleEnabled == false) return;

            else
            {
                split = line.Split(new char[] { ' ' });

                foreach (string s in split){
                    if (s.Contains("http"))
                    {
                        url = s;
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    WebRequest webRequest = HttpWebRequest.Create(url);

                    webRequest.Method = "HEAD";

                    webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-gb;q=0.8, en-us;q=0.7, en;q=0.6");
                    try
                    {
                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            foreach (string header in webResponse.Headers)
                                headers.Add(header, webResponse.Headers[header]);
                        }
                    }
                    catch
                    {

                    }

                    if (headers.ContainsKey("Content-Type"))
                    {
                        if (headers["Content-Type"].Contains("text/html"))
                        {
                            WebRequest request = WebRequest.Create(url);
                            request.Proxy = null;
                            request.Timeout = 30000;
                            
                            try
                            {
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);

                                html = reader.ReadToEnd();

                                if (!html.Contains("<title")) return;

                                string temp = Useful.getBetween(html, "<title", "</title>");
                                title = Useful.getBetween(temp, ">", "</title>");
                                
                                if (!string.IsNullOrWhiteSpace(title))
                                {

                                    title = title.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim();
                                    title = HttpUtility.HtmlDecode(title);
                                    if (title.ToLower().Contains("gyazo")) return;    //avoid those pages

                                    message = Privmsg(CHANNEL, "[title] " + title);
                                    Client.sendMessage(message);
                                }
                                

                            }
                            catch { }
                        }
                        else
                        {
                            try {
                                string[] sizes = { "B", "KB", "MB", "GB" };
                                double len = Convert.ToDouble(headers["Content-Length"]);
                                int order = 0;
                                while (len >= 1024 && order + 1 < sizes.Length)
                                {
                                    order++;
                                    len = len / 1024;
                                }

                                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                                // show a single decimal place, and no space.
                                string result = String.Format("{0:0.##} {1}", len, sizes[order]);


                                message = Privmsg(CHANNEL, "[" + headers["Content-Type"] + "] " + result);
                                Client.sendMessage(message);
                            }
                            catch{ }

                            

                        }
                    }
                }
            }
        }

        public void animeSearch(string CHANNEL, string nick, string query)
        {
            string message = "";
            GoogleSearch.GoogleSearch g = new GoogleSearch.GoogleSearch();
            anime a = new anime();
            string jsonGoogle;
            string xmlAnime;
            bool user = false;
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);
            string name = "";
            string id = "";
            int i_max = 4;
            int i = 0;
            bool found = false;

            if (ul.userIsMuted(nick) || Settings.Default.silence|| !Settings.Default.aniSearchEnabled || String.IsNullOrWhiteSpace(query))  
                return;

            query = query.ToLower();

            if (query.Contains("-u") || query.Contains("-user")) 
                user = true;

            query = query.Replace(" ", "%20").Replace(" -user", "%20").Replace(" -u", "%20");


            string getString = "https://www.googleapis.com/customsearch/v1?key=" + Settings.Default.apikey + "&cx=" + Settings.Default.cxKey + "&q=" + query;

            try
            {
                jsonGoogle = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonGoogle, g);
            }
            catch {
                message = Privmsg(CHANNEL, "Error while searching for results");
                Client.sendMessage(message);
                return;
            }

            webClient.Credentials = new NetworkCredential(Settings.Default.malUser, Settings.Default.malPass);

            if (g.items == null) {
                message = Privmsg(CHANNEL, "Could not find anything, try http://myanimelist.net/anime.php?q=" + query);
                Client.sendMessage(message);
                return;
            }
                

            if (g.items.Length < 4)
                i_max = g.items.Length - 1;

            while (i <= i_max && found == false)
            {
                if (!user)
                {
                    if (g.items[i].link.Contains("http://myanimelist.net/anime/"))
                    {
                        found = true;
                        string[] split = g.items[i].link.Split('/');

                        if (split.Length <= 5)
                            name = split[5];
                        else
                            name = query;

                        if (split.Length >= 5)
                            id = split[4];
                    }
                    else i++;
                }
                else
                {
                    if (g.items[i].link.Contains("http://myanimelist.net/profile/"))
                        found = true;
                    
                    else i++;
                }
            }

            if (!found)
                message = Privmsg(CHANNEL, g.items[0].link);

            else
            {
                if (!user)
                {
                    string animeName = name.Replace(" ", "+").Replace("_", "+").Replace("%20", "+");
                    getString = "http://myanimelist.net/api/anime/search.xml?q=" + animeName;

                    xmlAnime = webClient.DownloadString(getString);

                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(anime));
                        using (StringReader reader = new StringReader(xmlAnime))
                        {
                            a = (anime)(serializer.Deserialize(reader));
                        }
                    }
                    catch { }

                    if (a.entry == null)
                    {
                        string score = Useful.getBetween(xmlAnime, "<score>", "</score>");
                        string episodes = Useful.getBetween(xmlAnime, "<episodes>", "</episodes>");
                        string title = Useful.getBetween(xmlAnime, "<title>", "</title>");
                        string status = Useful.getBetween(xmlAnime, "<status>", "</status>");

                        if (episodes == "0" || episodes == string.Empty)
                            episodes = "?";
                        if (score == string.Empty)
                            score = "?";
                        if (title == string.Empty)
                            title = "?";
                        if (status == string.Empty)
                            status = "?";

                        message = Privmsg(CHANNEL, "\x02" + title + "\x02 : " + "[" + status + "] " + "[" + episodes + " episode" + (episodes == "1" ? "" : "s") + "] " + "[" + score + " / 10] " + "-> " + g.items[i].link);

                    }
                    else
                    {
                        int index = 0;

                        for (int o = 0; o < a.entry.Length; o++)
                        {
                            if (a.entry[o].id.ToString() == id)
                            {
                                index = o;
                                break;
                            }
                        }

                        string score = a.entry[index].score.ToString();
                        string episodes = a.entry[index].episodes.ToString();
                        string title = a.entry[index].title;
                        string status = a.entry[index].status;

                        if (episodes == "0")
                            episodes = "?";

                        message = Privmsg(CHANNEL, "\x02" + title + "\x02 : " + "[" + status + "] " + "[" + episodes + " episode" + (episodes == "1" ? "" : "s") + "] " + "[" + score + " / 10] " + "-> " + g.items[i].link);
                    }

                }
                else
                {
                    string xmlUser = webClient.DownloadString("http://myanimelist.net/malappinfo.php?u=" + query.Replace("%20", string.Empty).Replace("-u",string.Empty)).Trim();

                    myanimelist u = new myanimelist();

                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(myanimelist));
                        using (StringReader reader = new StringReader(xmlUser))
                        {
                            u = (myanimelist)(serializer.Deserialize(reader));
                        }
                    }
                    catch { }

                    if(u == null || u.myinfo == null|| xmlUser.Contains("<error>Invalid username</error>"))
                        message = Privmsg(CHANNEL, "Error fetching user stats");
                    else
                        message = Privmsg(CHANNEL, "[" + u.myinfo.user_name + "] " + "[Completed: " + u.myinfo.user_completed + " | Curretly Watching: " + u.myinfo.user_watching + "]" + " -> http://myanimelist.net/profile/"+ u.myinfo.user_name);
                }

            }
            Client.sendMessage(message);
            stats.anime();
        }

        public void youtubeSearch(string CHANNEL, string nick, string query)
        {
            string message = "";
            string jsonYoutube, title, duration;
            YoutubeSearch.YoutubeSearch y = new YoutubeSearch.YoutubeSearch();
            YoutubeVideoInfo.YoutubeVideoInfo youtubeVideo = new YoutubeVideoInfo.YoutubeVideoInfo();

            if (ul.userIsMuted(nick)) return;
            if (Settings.Default.silence == true || Settings.Default.youtubeSearchEnabled == false) return;
            if (String.IsNullOrWhiteSpace(query)) return;

            //query = query.Replace(" ", "%20");

            string getString = "https://www.googleapis.com/youtube/v3/search" + "?key=" + Settings.Default.apikey + "&part=id,snippet" + "&q=" +
                HttpUtility.UrlEncode(query) + "&maxresults=10&type=video&safeSearch=none";

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            try
            {
                jsonYoutube = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonYoutube, y);
            }
            catch { }

            foreach (var searchResult in y.items)
            {
                switch (searchResult.id.kind.ToLower())
                {
                    case "youtube#video":


                        getString = "https://www.googleapis.com/youtube/v3/videos/" + "?key=" + Settings.Default.apikey + "&part=snippet,contentDetails,statistics" + "&id=" + searchResult.id.videoId;
                        try
                        {
                            jsonYoutube = webClient.DownloadString(getString);
                            JsonConvert.PopulateObject(jsonYoutube, youtubeVideo);

                            title = WebUtility.HtmlDecode(youtubeVideo.items[0].snippet.title);
                            duration = YoutubeUseful.parseDuration(youtubeVideo.items[0].contentDetails.duration);

                            message = Privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + duration + "]\x02" + ": https://www.youtube.com/watch?v=" + searchResult.id.videoId);
                            Client.sendMessage(message);
                            return;//Only shows 1 link
                        }
                        catch { }

                        break;
                }
                
            }
            message = Privmsg(CHANNEL, "No results found");
            Client.sendMessage(message);
            stats.youtube();
            return;
        }

        public void giphySearch(string CHANNEL, string nick, string query)
        {
            if (ul.userIsMuted(nick)) return;
            if (Settings.Default.silence == true || Settings.Default.giphyEnabled == false) return;

            string message = Privmsg(CHANNEL, "No results found"); ;
            string request = "http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag=";
            string jsonResult;
            GiphyResult g = new GiphyResult();

            request += HttpUtility.UrlEncode(query);

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            try
            {
                jsonResult = webClient.DownloadString(request);
                JsonConvert.PopulateObject(jsonResult, g);

                if (g.data != null)
                    message = Privmsg(CHANNEL, query + ": " + g.data.url);
            }
            catch
            {
            }

            Client.sendMessage(message);
            stats.giphy();

            return;
        }

        public void vimeo(string CHANNEL, string nick, string line)
        {
            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.vimeoEnabled == true)
            {
                string title, duration;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int temp = 0;

                string message;
                string ID = Useful.getBetween(line, "vimeo.com/", "/");
                string URLString = "http://vimeo.com/api/v2/video/" + ID.Replace("\r", "").Replace("\n", "") + ".xml";

                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string readHtml = webClient.DownloadString(URLString);

                title = Useful.getBetween(readHtml, "<title>", "</title>");
                duration = Useful.getBetween(readHtml, "<duration>", "</duration>");

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
                    Client.sendMessage(message);

                }
                else
                {
                    message = Privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    Client.sendMessage(message);
                }

            }
        }
        public void lastKill(string CHANNEL, string nick, string arg)
        {
            if (ul.userIsMuted(nick) || String.IsNullOrEmpty(nick) || Settings.Default.silence || !Settings.Default.killEnabled || killsUsed.Count < 1) return;

            string message;
            int index = 0;

            if (!String.IsNullOrWhiteSpace(arg))
            {
                try
                {
                    index = Convert.ToInt32(arg) - 1;
                }
                catch
                {
                    message = Privmsg(CHANNEL, "Invalid Argument");
                    Client.sendMessage(message);
                    return;
                }
            }



            if (killsUsed == null || killsUsed.Count == 0)
                message = Privmsg(CHANNEL, "No kills since last reset");
            else if (killsUsed.Count < index+1 || index < 0)
                message = Privmsg(CHANNEL, "Out of Range");
            else if(index==0)
                message = Privmsg(CHANNEL, "[#" + killsUsed[0] + "] " + kill[killsUsed[0]]);
            else
                message = Privmsg(CHANNEL, "(" + (index+1) + " kills ago) " + "[#" + killsUsed[index] + "] " + kill[killsUsed[index]]);

            Client.sendMessage(message);
            
        }

        public void killUser(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string killString;
            int killID;
            string randomTarget;

            int MAX_KILLS = 500;

            if (ul.userIsMuted(nick) || String.IsNullOrEmpty(nick) || kill.Count < 1) return;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                string message;
                if (args.ToLower().Trim() == "la kill")
                {
                    message = Privmsg(CHANNEL, nick + " lost his way");
                }
                
                else if (args.ToLower() == "me baby".Trim())
                {
                    message = Privmsg(CHANNEL, "WASSA WASSA https://www.youtube.com/watch?v=dwkClIFBMEE");
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                        target = removeUserMode(userList[r.Next((userList.Count))]);
                    else
                        target = args.Trim();

                    if (kill.Count <= MAX_KILLS)
                    {
                        killsUsed.Clear();
                        killID = r.Next(kill.Count);
                        killsUsed.Insert(0, killID);
                    }
                        
                    else{
                        do killID = r.Next(kill.Count);
                        while (killsUsed.Contains(killID));
                    }


                    if (killsUsed.Count >= MAX_KILLS)
                    {
                        killsUsed.Remove(killsUsed[killsUsed.Count - 1]);
                    }

                    killsUsed.Insert(0, killID);

                    killString = kill[killID];

                    killString = killString.Replace("<TARGET>", target.ToUpper()).Replace("<USER>", nick.Trim().ToUpper());
                    killString = killString.Replace("<target>", target).Replace("<user>", nick.Trim());

                    if (killString.ToLower().Contains("<normal>"))
                    {
                        killString = killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                        message = Privmsg(CHANNEL, killString);
                    }
                    else {

                        message = Privmsg(CHANNEL, "\x01" + "ACTION " + killString + "\x01");
                    }
                }


                while (message.Contains("<random>"))
                {
                    do{
                        randomTarget = removeUserMode(userList[r.Next(userList.Count)].Trim());
                    } while (string.Compare(target, randomTarget, true) == 0 || userList.Count < 2);

                    message = regex.Replace(message, randomTarget, 1);
                }

                Client.sendMessage(message);
                stats.kill();
            }
        }

        public void factUser(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string factString, temp;
            int factID;
            string randomTarget;

            int MAX_FACTS = 300;

            if (ul.userIsMuted(nick) || String.IsNullOrEmpty(nick) || facts.Count <1) return;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Settings.Default.silence == false && Settings.Default.factsEnabled == true)
            {
                string message;
                
                if (String.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                    target = removeUserMode(userList[r.Next((userList.Count))]);
                else
                    target = args.Trim();

                if (facts.Count <= MAX_FACTS)
                {
                    factsUsed.Clear();
                    factID = r.Next(facts.Count);
                    factsUsed.Insert(0, factID);
                }

                else
                {
                    do factID = r.Next(facts.Count);
                    while (factsUsed.Contains(factID));
                }


                if (factsUsed.Count >= MAX_FACTS)
                {
                    factsUsed.Remove(factsUsed[factsUsed.Count - 1]);
                }

                factsUsed.Insert(0, factID);

                temp = facts[factID];


                factString = temp.Replace("<target>", target).Replace("<user>", nick.Trim());

                message = Privmsg(CHANNEL, factString);


                while (message.Contains("<random>"))
                {
                    do{
                        randomTarget = removeUserMode(userList[r.Next(userList.Count)].Trim());
                    } while (string.Compare(target, randomTarget, true) == 0 || userList.Count < 2);

                    message = regex.Replace(message, randomTarget, 1);
                }

                Client.sendMessage(message);
                stats.fact();
            }
        }

        public void randomKill(string CHANNEL, string nick, string args)
        {
            Random r = new Random();
            string target = "";
            string killString, temp;
            string randomTarget;


            if (ul.userIsMuted(nick) || String.IsNullOrEmpty(nick)) return;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Settings.Default.silence == false && Settings.Default.killEnabled == true)
            {
                string message;
               
                if (String.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                    target = removeUserMode(userList[r.Next((userList.Count))]);
                else
                    target = args.Trim();

                if (!killgen.readyToGenerate())
                {
                    message = Privmsg(CHANNEL, "Sorry, i can't think of a new kill right now.");
                }
                else
                {
                    temp = killgen.generateSentence();

                    if (temp.ToLower().Contains("<normal>"))
                    {
                        temp = temp.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                        killString = temp.Replace("<target>", target).Replace("<user>", nick.Trim());

                        message = Privmsg(CHANNEL, killString);
                    }
                    else
                    {
                        killString = temp.Replace("<target>", target).Replace("<user>", nick.Trim());

                        message = Privmsg(CHANNEL, "\x01" + "ACTION " + killString + "\x01");
                    }



                    while (message.Contains("<random>"))
                    {
                        do
                        {
                            randomTarget = removeUserMode(userList[r.Next(userList.Count)].Trim());
                        } while (string.Compare(target, randomTarget, true) == 0 || userList.Count < 2);

                        message = regex.Replace(message, randomTarget, 1);
                    }

                    message = message.Replace("  ", " ");
                }

                

                Client.sendMessage(message);
                stats.kill();
            }
        }

        private void parseQuestion(string CHANNEL, string user, string arg)
        {
            if (ul.userIsMuted(user)) return;
            if (Settings.Default.silence || !Settings.Default.questionEnabled) return;

            string subjectNPL = string.Empty;
            subjectNPL = qq.questionParser(arg, user);


            string message = "";
            Random r = new Random();

          
            string[] howMany = { "I dont know, maybe", "Probably", "More than", "Less than", "I think it was", "I don't know, so i'll give you a random number:","", "It's" };

            string[] howIs = { "fine", "not fine", "lost", "being retarded again", "not feeling well", "being annoying as always", "probably hungry", "good", "all right", "upset", "bored" };

            string[] because = { "was lost", "is stupid", "asked me to", "was asked to", "has an inferiority complex", "is a terrible person",
                                   "felt like so", "wanted to", "liked it", "already had plans to do it", "wanted it that way"  };

            string[] when = { "maybe next week", "a few days ago", "last year", "yesterday", "tomorrow", "in a few hours",
                                "nobody knows", "next year", "it was yesterday", "I'm not sure", "next week" };

            string[] why = { "I dont know, maybe", "I don't know", "Yeah", "Nope.", "Yes.", "No.", "Probably", "Everything makes me believe so", 
                               "Not sure, ask somebody else", "I don't know, im not wikipedia", "Sorry, but i don't know", "Because that was destined to be so" };

            string[] where = { "somewhere in a far away land" , "on the Youtube datacenter", "behind you", "in your house", "in Europe", "near Lygs", "that special place",
                               "in outer space","somewhere i belong", "On the shaddiest subreddit","on tumblr", "in space", "on your computer", 
                               "beneath your bed!", "where you didnt expect", "near your house"};

            string[] whoDid = { "Probably", "Maybe it was", "I'm sure it was", "I don't think it was", "I suspect", "Someone told me it was","I think it was" };
            string[] whoDo = { "Probably", "Maybe it is", "I'm sure it is", "I don't think it was", "I suspect", "Someone told me it is", "I think it is" };

            string[] what = { "Sorry, I have no idea","Can you ask somebody else? I really don't know","No idea, try Google", "I'm not good with questions",
                            "I'm under pressure, i can't answer you that","Stop bullying me!" };

            string[] whyY = { "Im not sure if", "Yeah,", "Yes,", "Correct!", "I think", "I believe that" , ""};

            string[] whyN = { "Nope,", "No,", "I think that", "I believe that", "Negative!", ""};

            if(arg[arg.Length - 1] == '?')
            {

                arg = arg.Replace("?", string.Empty).TrimStart(new char[] { ' ' });
                string[] split = arg.Split(new char[] { ' ' });

                if (split.Length >= 1)
                {
                    if (String.Compare(split[0], "how", true) == 0)
                    {
                        if (split.Length >= 2)
                        {
                            if (String.Compare(split[1], "many", true) == 0)
                            {
                                if (String.Compare(arg, "how many killstrings do you have", true) == 0 || String.Compare(arg, "how many kills do you have", true) == 0)
                                    message = Privmsg(CHANNEL, "I have " + kill.Count + " killstrings loaded in.");
                                else if (arg == "how many fucks do you give")
                                    message = Privmsg(CHANNEL, "I always give 0 fucks.");
                                else
                                    message = Privmsg(CHANNEL, (howMany[r.Next(howMany.Length)] + " " + r.Next(21)).Trim());
                            }

                            else if (split.Length >= 2 && String.Compare(split[1], "are", true) == 0)
                            {
                                if (String.Compare(arg, "how are you", true) == 0 || String.Compare(arg, "how are you doing", true) == 0)
                                    message = Privmsg(CHANNEL, "I'm fine, thanks for asking. And you?");
                                else
                                    message = Privmsg(CHANNEL, "I dont know yet, ask later");
                            }

                            else if (String.Compare(split[1], "is", true) == 0)
                            {
                                if (split.Length == 3)
                                    message = Privmsg(CHANNEL, split[2] + " is " + howIs[r.Next(howIs.Length)]);
                            }

                            else if (String.Compare(split[1], "did", true) == 0 && String.Compare(split[split.Length], "die", true) == 0)
                                killUser(CHANNEL, user, Useful.getBetween(arg, "did", "die"));

                            else if (String.Compare(split[1], "old", true) == 0)
                            {
                                if (split.Length >= 4)
                                {
                                    string replaced = questionsRegex(subjectNPL);

                                    if (String.Compare(split[2], "is", true) == 0)
                                    {
                                        message = Privmsg(CHANNEL, replaced + " is " + r.Next(41) + " years old");
                                    }
                                    else if (String.Compare(split[2], "are", true) == 0)
                                    {
                                        if (String.Compare(subjectNPL, "you", true) == 0)
                                            message = Privmsg(CHANNEL, "I was compiled on " + getCompilationDate.RetrieveLinkerTimestamp().ToString("R"));
                                        else
                                            message = Privmsg(CHANNEL, replaced + " are " + r.Next(41) + " years old");
                                    }

                                    else if (String.Compare(split[2], "am", true) == 0 || String.Compare(split[3], "i", true) == 0)
                                        message = Privmsg(CHANNEL, "You are " + r.Next(41) + " years old");
                                }
                            }
                        }
                        else
                            message = Privmsg(CHANNEL, user + ", no idea...");
                    }

                    else if (String.Compare(split[0], "how's", true) == 0)
                    {
                        string replaced = questionsRegex(subjectNPL);
                        message = Privmsg(CHANNEL, replaced + " is " + howIs[r.Next(howIs.Length)]);
                    }

                    else if (String.Compare(split[0], "why", true) == 0)
                    {
                        if (split.Length >= 2)
                            message = Privmsg(CHANNEL, "Because " + removeUserMode(userList[r.Next(userList.Count)]) + " " + because[r.Next(because.Length)]);
                    }

                    else if (String.Compare(split[0], "is", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            if (yes)
                                message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my") + " is " + replaced).Trim());
                            else
                                message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my") + " isn't " + replaced).Trim());
                        }
                    }
                    else if (String.Compare(split[0], "was", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            if (yes)
                                message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " was " + replaced).Trim());
                            else
                                message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " wasn't " + replaced).Trim());
                        }
                    }
                    else if (String.Compare(split[0], "when", true) == 0)
                    {
                        message = Privmsg(CHANNEL, when[r.Next(when.Length)]);
                    }

                    else if (String.Compare(split[0], "are", true) == 0)
                    {
                        if (String.Compare(arg, "are you real", true) == 0)
                            message = Privmsg(CHANNEL, "Yes, i am real");

                        else if (String.Compare(arg, "are you a real person", true) == 0 || String.Compare(arg, "are you a real human", true) == 0 || String.Compare(arg, "are you human", true) == 0)
                            message = Privmsg(CHANNEL, "No, i'm a bot");


                        else
                        {
                            if (r.Next(100) < 15)
                                message = Privmsg(CHANNEL, why[r.Next(why.Length)]);
                            else
                            {
                                bool yes = false;
                                if (r.Next(0, 2) == 1)
                                    yes = true;

                                string subject = subjectNPL;
                                string rest = "";

                                for (int i = 1; i < split.Length; i++)
                                {
                                    rest += split[i] + " ";
                                }
                                rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                                string replaced = questionsRegex(rest);


                                if (subject.Trim() == "you")
                                {

                                    if (yes)
                                        message = Privmsg(CHANNEL, ((whyY[r.Next(whyY.Length)]) + " " + "I'm " + replaced).Trim());
                                    else
                                        message = Privmsg(CHANNEL, ((whyN[r.Next(whyN.Length)]) + " " + "I'm not " + replaced).Trim());

                                }

                                else
                                {
                                    subject = questionsRegex(subject);

                                    if (yes)
                                        message = Privmsg(CHANNEL, ((whyY[r.Next(whyY.Length)]) + " " + subject + " are " + replaced).Trim());
                                    else
                                        message = Privmsg(CHANNEL, ((whyN[r.Next(whyN.Length)]) + " " + subject + " aren't " + replaced).Trim());
                                }
                            }
                        }


                    }

                    else if (String.Compare(split[0], "can", true) == 0)
                    {
                        if (String.Compare(arg, "can you give me a nick", true) == 0 || String.Compare(arg, "can you make me a nick", true) == 0 ||
                            String.Compare(arg, "can you generate a nick", true) == 0 || String.Compare(arg, "can you create a nick", true) == 0 ||
                            String.Compare(arg, "can you make me a new nick", true) == 0)
                        {
                            message = Privmsg(CHANNEL, "Yes, here it is: " + NickGen.GenerateNick(nickGenStrings, nickGenStrings.Count, false, false, false, false));
                            stats.nick();
                        }

                        else if (arg.ToLower().Contains("can you kill "))
                            killUser(CHANNEL, user, Useful.getBetween(arg.ToLower(), "can you kill ", ""));
                        else
                            message = Privmsg(CHANNEL, why[r.Next(why.Length)]);
                    }

                    else if (String.Compare(split[0], "would", true) == 0)
                    {
                        if (String.Compare(arg, "would you make me a nick", true) == 0 || String.Compare(arg, "would you generate a nick", true) == 0 ||
                            String.Compare(arg, "would you create a nick", true) == 0 || String.Compare(arg, "would you make me a new nick", true) == 0)
                            message = Privmsg(CHANNEL, "Yes, here it is: " + NickGen.GenerateNick(nickGenStrings, nickGenStrings.Count, false, false, false, false));

                        else
                            message = Privmsg(CHANNEL, why[r.Next(why.Length)]);
                    }

                    else if (String.Compare(split[0], "where", true) == 0)
                    {
                        message = Privmsg(CHANNEL, where[r.Next(where.Length)]);

                    }

                    else if (String.Compare(split[0], "who", true) == 0 || String.Compare(split[0], "who's", true) == 0)
                    {
                        if (String.Compare(arg, "who are you", true) == 0)
                            message = Privmsg(CHANNEL, "I'm a bot!");
                        else if (split[1] == "do")
                            message = Privmsg(CHANNEL, whoDid[r.Next(whoDo.Length)] + " " + removeUserMode(userList[r.Next(userList.Count)]));
                        else
                            message = Privmsg(CHANNEL, whoDid[r.Next(whoDid.Length)] + " " + removeUserMode(userList[r.Next(userList.Count)]));
                    }

                    else if (String.Compare(split[0], "what", true) == 0 || String.Compare(split[0], "what's", true) == 0)
                    {
                        if (String.Compare(arg, "what are you", true) == 0)
                            message = Privmsg(CHANNEL, "I'm a bot!");
                        else
                        {
                            message = Privmsg(CHANNEL, what[r.Next(what.Length)]);
                        }
                    }

                    else if (String.Compare(split[0], "if", true) == 0)
                    {
                        string answer = bot1session.Think(arg + "?");
                        message = Privmsg(CHANNEL, answer);

                    }
                    else if (String.Compare(split[0], "am", true) == 0 && String.Compare(split[1], "i", true) == 0)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        string rest = "";

                        for (int i = 1; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                        string replaced = questionsRegex(rest);

                        if (yes)
                            message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " you are " + replaced).Trim());
                        else
                            message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " you aren't " + replaced).Trim());
                    }

                    else if (String.Compare(split[0], "do", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            if (split[1] == "you")
                            {

                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I don't " + replaced)).Trim();
                            }
                            else if (split[1] == "i")
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you do " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you don't " + replaced)).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " do " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " doesn't " + replaced)).Trim();
                            }
                        }
                    }

                    else if (String.Compare(split[0], "should", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            if (split[1] == "you")
                            {

                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I should " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I shouldn't " + replaced)).Trim();
                            }
                            else if (split[1] == "i")
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you should " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you shouldn't " + replaced)).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " should " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " shouldn't " + replaced)).Trim();
                            }
                        }
                        else
                        {
                            if (yes)
                                message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)])).Trim();
                            else
                                message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)])).Trim();
                        }
                    }

                    else if (String.Compare(split[0], "did", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            if (split[1] == "you")
                            {

                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "I did " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "I didn't " + replaced)).Trim();
                            }
                            else if (split[1] == "i")
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + "you did " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + "you didn't " + replaced)).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " did " + replaced)).Trim();
                                else
                                    message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " didn't " + replaced)).Trim();
                            }
                        }


                    }

                    else if (String.Compare(split[0], "does", true) == 0)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNPL;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNPL, string.Empty);

                            string replaced = questionsRegex(rest);

                            subject = questionsRegex(subject);

                            if (yes)
                                message = Privmsg(CHANNEL, (whyY[r.Next(whyY.Length)] + " " + subject + " does " + replaced)).Trim();
                            else
                                message = Privmsg(CHANNEL, (whyN[r.Next(whyN.Length)] + " " + subject + " does not " + replaced)).Trim();

                        }
                    }
                }
                else
                {
                    string answer = bot1session.Think(arg + "?");
                    message = Privmsg(CHANNEL, answer);
                }
            }

            else
            {
                string answer = bot1session.Think(arg);
                message = Privmsg(CHANNEL, answer);
            }

            if (!String.IsNullOrWhiteSpace(message))
            {
                Client.sendMessage(message.Replace("  ", " "));
                stats.question();
            }
        }

        public void nickGen(string CHANNEL, string nick, string args)
        {
            if (String.IsNullOrEmpty(nick)) return;
            Random rnd = new Random();

            bool randomnumber = false;
            bool randomUpper = false;
            bool switchLetterNumb = false;
            bool Ique = false;
            bool targeted = false;

            string target = null;
            string message;

            if (ul.userIsMuted(nick)) return;

            if (nickGenStrings.Count < 2)
            {
                message = Privmsg(CHANNEL, "Nickname generator was not initialized properly");
                Client.sendMessage(message);
                return;
            }

            if (Settings.Default.silence == false && Settings.Default.nickEnabled == true)
            {
                foreach (string s in args.Split(' '))
                {
                    if (s.ToLower() == "random")
                    {
                        switchLetterNumb = rnd.Next(0, 100) <= 30;
                        randomnumber = rnd.Next(0, 100) <= 30;
                        randomUpper = rnd.Next(0, 100) <= 30;
                        Ique = rnd.Next(0, 100) <= 10;
                    }

                    else if (s.ToLower() == "for")
                    {
                        targeted = true;
                        target = Useful.getBetween(args, "for ", null);
                    }

                    if(s.ToLower() == "sl") switchLetterNumb = true;
                    else if(s.ToLower() == "rn") randomnumber = true;
                    else if (s.ToLower() == "ru") randomUpper = true;
                    else if (s.ToLower() == "iq") Ique = true;
                }

                string nick_ = NickGen.GenerateNick(nickGenStrings, nickGenStrings.Count, randomnumber, randomUpper, switchLetterNumb, Ique);

                if(targeted)
                    message = Privmsg(CHANNEL, nick + " generated a nick for "+ target +": " + nick_);
                else 
                    message = Privmsg(CHANNEL, nick + " generated the nick " + nick_);

                Client.sendMessage(message);
                stats.nick();
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
            RedditSharp.Things.Comment comment;

            if (ul.userIsMuted(nick)) return;

            foreach (string st in temp)
            {
                if (st.Contains("reddit.com") && st.Contains("/r/") && st.Contains("/comments/"))
                {
                    url = st;
                    url = url.Replace("http://", string.Empty).Replace("https://", string.Empty);
                }
                else return;
            }

            subreddit = Useful.getBetween(url, "/r/", "/");
            
            string[] linkParse = url.Replace("\r", string.Empty).Split('/');

            if (linkParse.Length >= 7 && !String.IsNullOrEmpty(linkParse[6]))    //With Comment
            {
                string postName = linkParse[4];
                string commentName = linkParse[6].Split(new char[] { '?' }, 2)[0];  //Split is for removing url args

                try{
                    //post = (RedditSharp.Things.Post) reddit.GetThingByFullname("t3_" + postName);
                    post = reddit.GetPost(new Uri("http://" + url));

                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit + "] " + "[" + "↑" + post.Upvotes + "] " + "\x02" + HttpUtility.HtmlDecode(post.Title) + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                    Client.sendMessage(message);
                }
                catch{
                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get post info" + "\x02");
                    Client.sendMessage(message);
                }

                try{
                    comment = reddit.GetComment(new Uri("https://"+url));

                    if (comment.Body.ToString().Length > 300)
                        message = Privmsg(CHANNEL, "\x02" + "Comment by " + comment.Author + " [↑" + comment.Upvotes + "] " + HttpUtility.HtmlDecode(comment.Body.Truncate(300).Replace("\r", " ").Replace("\n", " ") + "(...)" + "\x02"));
                    else
                        message = Privmsg(CHANNEL, "\x02" + "Comment by " + comment.Author + " [↑" + comment.Upvotes + "] " + HttpUtility.HtmlDecode(comment.Body.Replace("\r", " ").Replace("\n", " ") + "\x02"));
                    Client.sendMessage(message);
                }
                catch{
                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get comment info" + "\x02");
                    Client.sendMessage(message);
                }

            }
            else  //No comment link
            {      
                try {
                    //post = (RedditSharp.Things.Post)reddit.GetThingByFullname("t3_" + linkParse[4]);
                    post = reddit.GetPost(new Uri("http://" + url));

                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + HttpUtility.HtmlDecode(post.Title) + "\x02" + ", submitted by /u/" + post.Author + "\x02");
                    Client.sendMessage(message);

                    if (!post.IsSelfPost)
                        Client.sendMessage(Privmsg(CHANNEL, "\x033" + post.Url + "\x03"));
                }

                catch {
                    message = Privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Trim() + "] " + "Failed to get post info" + "\x02");
                    Client.sendMessage(message);
                }
            }    
        }

        void wiki(string CHANNEL, string nick, string args)
        {
            if (ul.userIsMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.wikiEnabled == true)
            {
                string message = Privmsg(CHANNEL, "Here's a Wiki for \"" + args + "\": " + "http://en.wikipedia.org/w/index.php?title=Special:Search&search=" + args.Replace(" ", "%20"));
                Client.sendMessage(message);
                stats.wiki();
            }
        }

        void greetUser(string nick)
        {
            if (ul.userIsMuted(nick) || !Settings.Default.greetingsEnabled || Settings.Default.silence) return;

            foreach (User u in ul.Users)
            {
                if (String.Compare(u.Nick, nick, true) == 0 && u.GreetingEnabled)
                {
                    string mensagem = Privmsg(Client.HOME_CHANNEL, u.Greeting);
                    Client.sendMessage(mensagem);
                    stats.greet();
                    break;
                }
            }
        }

        bool quitIRC(string nick)
        {
            if(ul.userIsOperator(nick)){
                
                timeoutTimer.Stop();
                waitingForPong = false;
                Client.Disconnect(Settings.Default.quitMessage);

                return true;
            }
            return false;
        }

        void printQuote(string CHANNEL, string args, string nick)
        {
            Random r = new Random();
            int i;
            string message = "";
            List<string> temp = new List<string>();
            List<string> temp2 = new List<string>();

            if (ul.userIsMuted(nick) || !Settings.Default.quotesEnabled) return;

            if (String.IsNullOrWhiteSpace(args) && quotes.Count>0) //pring random
            {
                i = r.Next(quotes.Count);
                message = Privmsg(CHANNEL, quotes[i]);
            }
            else if (args[0] == '#')
            {
                string split = args.Split(new char[] { ' ' }, 2)[0];
                int number = Convert.ToInt32(split.Replace("#",string.Empty));

                if (number <= quotes.Count)
                    message = Privmsg(CHANNEL, quotes[number - 1]);
                else
                    message = Privmsg(CHANNEL, "Quote "+ number +" not found");
            }
            else{
                string[] queries = args.Trim().ToLower().Split(new char[] { ' ' });

                if (queries.Length > 1)
                {
                    foreach (string s in quotes)
                    {
                        if (s.ToLower().Contains(queries[0]))
                            temp.Add(s);
                    }

                    foreach (string t in temp)
                    {
                        for (int d = 1; d < queries.Length; d++)
                        {
                            if (t.ToLower().Contains(queries[d].ToLower()))
                            {
                                temp2.Add(t);
                            }
                        }

                    }
                    if (temp2.Count > 0)
                        message = Privmsg(CHANNEL, temp2[r.Next(temp2.Count)]);
                    else message = Privmsg(CHANNEL, "No quotes found");
                }
                else
                {
                    foreach (string s in quotes)
                    {
                        if (s.ToLower().Contains(args.ToLower()))
                        {
                            temp.Add(s);
                        }
                    }
                    if (temp.Count > 0)
                        message = Privmsg(CHANNEL, temp[r.Next(temp.Count)]);
                    else message = Privmsg(CHANNEL, "No quotes found");
                }
                
            }

            Client.sendMessage(message);
            stats.quote();

        }

        void addQuote(string args, string nick)
        {
            if (ul.userIsMuted(nick) || !Settings.Default.quotesEnabled) return;

            string add;

            if (quotes == null)
                quotes = new List<string>();


            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0] , "add", true) == 0)
                add = Useful.getBetween(args, "add ", null);
            else
                add = args;

            if(!string.IsNullOrWhiteSpace(add))
                quotes.Add(add);
            
            saveQuotes();
        }

        void tell(string nick, string args)
        {
            if (ul.userIsMuted(nick) || !Settings.Default.tellEnabled) return;

            string[] split = args.Split(new char[] { ' ' }, 2);

            if (split.Length < 2) return;
            else
            {
                ul.addUserMessage(split[0], nick, split[1]);
                stats.tell();
                ul.saveData();
            }
        }

        private void choose(string CHANNEL, string user, string arg)
        {
            if (ul.userIsMuted(user) || !Settings.Default.chooseEnabled) return;

            Random r = new Random();
            string[] choices;
            string message;

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            int random = r.Next(choices.Length);

            message = Privmsg(CHANNEL, user+": "+ choices[random].Trim());

            Client.sendMessage(message);
            stats.choose();
        }

        private void shuffle(string CHANNEL, string user, string arg)
        {
            if (ul.userIsMuted(user) || !Settings.Default.shuffleEnabled) return;

            Random r = new Random();
            string message;
            string[] choices = arg.Split(new char[] { ' ' });
            List<string> sList = new List<string>();

            foreach(string s in choices){
                sList.Add(s);
            }

            message = "";

            while (sList.Count > 0)
            {
                int random = r.Next(sList.Count);
                message = message + " " + sList[random];
                sList.Remove(sList[random]);
            }



            Client.sendMessage(Privmsg(CHANNEL, user + ":" + message));
            stats.shuffle();
        }

        void printFunk(string CHANNEL, string args, string nick)
        {
            Random r = new Random();
            int i;
            string message = "";

            if (ul.userIsMuted(nick) || !Settings.Default.funkEnabled) return;

            if (String.IsNullOrWhiteSpace(args) && funk.Count > 0) //pring random
            {
                i = r.Next(funk.Count);
                message = Privmsg(CHANNEL, funk[i]);
            }

            Client.sendMessage(message);
            stats.funk();

        }

        void addFunk(string args, string nick)
        {
            if (ul.userIsMuted(nick) || !Settings.Default.funkEnabled) return;

            if (funk == null)
                funk = new List<string>();

            string [] splits;
            splits = args.Split(new char[] { ' ' }, 2);

            if (string.Compare(splits[0].ToLower(), "add") == 0)
                args = args.Replace("add ", string.Empty);

            if ((args.Contains("youtu.be") && (args.Contains("?v=") == false && args.Contains("&v=") == false))
                            || (args.Contains("youtube") && args.Contains("watch") && (args.Contains("?v=") || args.Contains("&v="))))
            {
                string id = YoutubeUseful.getYoutubeIdFromURL(args);

                string result = YoutubeUseful.getYoutubeInfoFromID(id);

                args = result + " : " + args;
            }




                funk.Add(args);

            saveFunk();
            stats.funk();
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
            Client.sendMessage(message);
        }
        public void ctcpVersion(string u)
        {
            string message = Notice(u, "\x01" + "VERSION " + botVersion + "\x01");
            Client.sendMessage(message);
        }

        public void ctcpPing(string u, string stamp)
        {
            string message = Notice(u, "\x01" + "PING " + stamp + "\x01");
            Client.sendMessage(message);
        }
        ////

       
        public void muteUser(string nick)
        {
            ul.muteUser(nick);
            ul.saveData();
        }
        public void unmuteUser(string nick)
        {
            ul.unmuteUser(nick);
            ul.saveData();
        }

        public void giveOps(string nick)
        {
            ul.opUser(nick);
            ul.saveData();

        }
        public void takeOps(string nick)
        {
            ul.deopUser(nick);
            ul.saveData();
        }

        public bool changeNick(string nick)
        {
            Client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();
            OnBotNickChanged(EventArgs.Empty);

            //do Nick change to server
            if (Client.isConnected)
            {
                string message = "NICK " + Client.NICK + "\n";
                Client.sendMessage(message);
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

        /// <summary>
        /// Writes a Message on the output window
        /// </summary>
        /// <param Name="Message">A sting with the Message to write</param>
        public void WriteMessage(string message) //Writes Message on the TextBox (bot console)
        {
            if (OutputBox.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message);
                    OutputBox.Invoke(invoker);
                }
                catch { }
            }
            else
            {
                string timeString = DateTime.Now.ToString("[HH:mm:ss]");

                if (Settings.Default.showTimeStamps)
                    this.OutputBox.AppendText(timeString + " " + message + "\n");

                else
                    this.OutputBox.AppendText(message + "\n");

                if (Settings.Default.autoScrollToBottom)
                {
                    OutputBox.SelectionStart = OutputBox.Text.Length;       //Set the current caret position at the end
                    OutputBox.ScrollToCaret();                          //Now scroll it automatically
                }
            }

            //also, should make a log

        }
        public void WriteMessage(string message, Color color) //Writes Message on the TextBox (bot console)
        {
            if (OutputBox.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message, color);
                    OutputBox.Invoke(invoker);
                }
                catch { }
            }
            else
            {
                string timeString = DateTime.Now.ToString("[HH:mm:ss]");

                if(Settings.Default.showTimeStamps){
                    this.OutputBox.AppendText(timeString + " ");
                    this.OutputBox.AppendText(message + "\n", color);
                }
                    
                else
                    this.OutputBox.AppendText(message + "\n", color);

                if (Settings.Default.autoScrollToBottom)
                {
                    OutputBox.SelectionStart = OutputBox.Text.Length;       //Set the current caret position at the end
                    OutputBox.ScrollToCaret();                          //Now scroll it automatically
                }
            }

            //also, should make a log

        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("mmssffff");
        }

        public void pingSever()
        {
            if (!WaitingForPong)
            {
                string message = "PING " + GetTimestamp(DateTime.Now);

                #if DEBUG
                    WriteMessage(message);
                #endif

                Client.sendMessage(message);

                WaitingForPong = true;

                timeoutTimer = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000);
                timeoutTimer.Elapsed += (sender, e) => checkIfTimeout(sender, e);
                timeoutTimer.Enabled = true;
            }
        }

        public void kickUser(string userToBeKicked)
        {
            string message = "KICK " + Client.HOME_CHANNEL + " " + userToBeKicked;
            Client.sendMessage(message);
        }

        void checkIfTimeout(object sender, EventArgs e)
        {
            if (WaitingForPong)
            {
                OnTimeout(EventArgs.Empty);
                WaitingForPong = false;
            }
        }


        void messageDelivery(string nick)
        {
            int count = ul.userMessageCount(nick);
            if (count == 0) return;

            string message;

            Client.sendMessage(Privmsg(nick, "Hello " + nick + ". You have " + count + " message(s)"));

            for(int i = 0 ; i < count ; i++)
            {

                UserMessage m = ul.getUserMessage(nick, i);
                TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(m.Timestamp);

                string timeDiff = "";

                if (diff.Days >= 1)
                    if (diff.Days == 1)
                        timeDiff += diff.Days + " day, ";
                    else
                        timeDiff += diff.Days + " days, ";

                if (diff.Hours >= 1)
                    if (diff.Hours == 1)
                        timeDiff += diff.Hours + " hour ago";
                    else
                        timeDiff += diff.Hours + " hours ago";
                else
                    if (diff.Minutes == 1)
                        timeDiff += diff.Minutes + " minute ago";
                    else
                        timeDiff += diff.Minutes + " minutes ago";


                message = Privmsg(nick, "From "+ m.Sender +", sent " + timeDiff +": " + m.Message );
                Client.sendMessage(message);
            }

            Client.sendMessage(Privmsg(nick, "Mark all messages as read using " + Client.SYMBOL + "acknowledge or " + Client.SYMBOL + "a"));
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
                if(timeoutTimer!=null) timeoutTimer.Close();
            }
            userList.Clear();
            Client.Disconnect(Settings.Default.quitMessage);
            OutputBox.Clear();
        }

        private static string questionsRegex(string rest)
        {
            var someVariable1 = "you";
            var someVariable2 = "me";
            var someVariable3 = "you are";
            var someVariable4 = "my";
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
                    {"my", someVariable5},
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

        internal void redditLogin(string userName, string password)
        {
            reddit.User = reddit.LogIn(userName, password, true);
        }
    }
}
