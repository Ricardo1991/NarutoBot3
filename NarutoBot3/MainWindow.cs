using NarutoBot3.Properties;
using Newtonsoft.Json;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MainWindow : Form
    {
        delegate void SetTextCallback(string text);
        delegate void ChangeDataSoure();

        searchAnimeAPI animeAPI = new searchAnimeAPI();
        ConnectWindow Connect = new ConnectWindow();
        enabledCommands enableCommandsWindow = new enabledCommands();
        assignments assignmentsWindow = new assignments();
        claims claimsWindow = new claims();
        nick nickWindow = new nick();
        operators operatorsWindow = new operators();
        rules rulesWindow = new rules();
        help helpWindow = new help();
        eta etaWindow = new eta();
        muted mutedWindow = new muted();
        RedditCredentials redditcredentials = new RedditCredentials();
        RleaseChecker releaseChecker = new RleaseChecker();

        Bot botMessageHandler = new Bot();
        public IrcClient client;

        string botVersion = "NarutoBot3 by Ricardo1991, compiled on " + getCompilationDate.RetrieveLinkerTimestamp();

        System.Timers.Timer aTime;      //To check for manga releases
        System.Timers.Timer rTime;      //To check for random text

        string HOME_CHANNEL;
        string HOST;
        string NICK;
        int PORT;
        string SYMBOL = "!";

        String line;

        string lastCommand;

        int exitThisShit = 0;

        Reddit reddit;

        RedditSharp.Things.AuthenticatedUser user;

        List<string> ops = new List<string>();
        List<string> rls = new List<string>();
        List<string> hlp = new List<string>();
        List<string> ban = new List<string>();
        List<string> tri = new List<string>();
        List<Greeting> greet = new List<Greeting>();

        List<pastMessage> pastMessages = new List<pastMessage>();

        string eta = Settings.Default.eta;

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        int lineNumber;
        int triviaNumber;
        List<string> nickGenStrings;

        public void loadSettings()
        {
            switch (Settings.Default.randomTextInterval)
            {
                case 30:
                    t30.Checked = true;
                    t45.Checked = false;
                    t60.Checked = false;
                    break;
                case 45:
                    t30.Checked = false;
                    t45.Checked = true;
                    t60.Checked = false;
                    break;
                case 60:
                    t30.Checked = false;
                    t45.Checked = false;
                    t60.Checked = true;
                    break;
                default:
                    Settings.Default.randomTextInterval = 30;
                    t30.Checked = true;
                    t45.Checked = false;
                    t60.Checked = false;
                    break;
            }


            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
            {
                Settings.Default.aniSearchEnabled = false;
                Settings.Default.timeEnabled = false;
            }

            if (Settings.Default.redditUser == "" || Settings.Default.redditPass == "")
                Settings.Default.redditEnabled = false;

            if (Settings.Default.redditUserEnabled)
            {
                try { user = reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass); }
                catch { }
            }

            Settings.Default.Save();

            aTime = new System.Timers.Timer(Settings.Default.checkInterval);
            aTime.Enabled = false;

            rTime = new System.Timers.Timer(Settings.Default.randomTextInterval * 60 * 1000);
            rTime.Enabled = Settings.Default.randomTextEnabled;
            rTime.Elapsed += new ElapsedEventHandler(randomTextSender);

            Settings.Default.releaseEnabled = false;

            if (Settings.Default.silence == true)
            {
                silencedToolStripMenuItem.Checked = true;
                toolStripStatusLabelSilence.Text = "Bot is Silenced";
            }
            else
            {
                silencedToolStripMenuItem.Checked = false;
                toolStripStatusLabelSilence.Text = "";
            }

            readOPS();              //operators
            readBAN();              //banned users
            readHLP();              //help text
            readTRI();              //trivia strings
            readGREET();            //read greetings
            loadNickGenStrings();   //For the nick generator

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);
        }

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker1.WorkerSupportsCancellation = true;

            botMessageHandler.Connected += new ConnectedChangedEventHandler(nowConnected);

            botMessageHandler.Created += new UserListChangedEventHandler(userListCreated);
            botMessageHandler.Joined += (sender, e) => userJoined(sender, e, botMessageHandler.Who);
            botMessageHandler.Left += (sender, e) => userLeft(sender, e, botMessageHandler.Wholeft);
            botMessageHandler.NickChanged += (sender, e) => userNickChange(sender, e, botMessageHandler.Who, botMessageHandler.NewNick);
            botMessageHandler.Kicked += (sender, e) => userKicked(sender, e, botMessageHandler.Who);
            botMessageHandler.ModeChanged += (sender, e) => userModeChanged(sender, e, botMessageHandler.Who, botMessageHandler.Mode);
           
            loadSettings();

            lastCommand = "";

            var result = Connect.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (connect())//If connected with success, then start the bot
                {                 
                    backgroundWorker1.RunWorkerAsync();
                    //isConnected = true;
                }
                else
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool connect()//This is where the bot connects to the server and logs in
        {
            ChangeLabel("Connecting...");
            reddit = new Reddit();

            client = new IrcClient(HOME_CHANNEL, HOST, PORT, NICK);
            return client.Connect();
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //This is the main bot cycle... Pretty much everything happens here
        {
            //Main Loop
            String buffer;
            exitThisShit = 0;
            while (exitThisShit == 0)
            {
                buffer = "";
                bool gotIt = false;
                try
                {
                    buffer = client.messageReader();
                    byte[] bytes = Encoding.Default.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    gotIt = true;
                }
                catch
                { }

                //Got a message
                if (gotIt)
                {
                    string returnmessage;

                    returnmessage = botMessageHandler.BotMessage(line, /*out returnmessage,*/ ref client.userList);

                    if (returnmessage != "")
                        client.messageSender(returnmessage);


                    //string[] stoperror = line.Split(' ');

                    //if (line.Contains("PING :"))
                    //{
                    //    string[] pingcmd = line.Split(new char[] { ':' }, 1);
                    //    ping(pingcmd[0], writer);
                    //}
                    //else if (line.Contains("End of /MOTD command."))
                    //{
                    //    messageSender(writer, join_message);
                    //    ChangeLabel("Connected");
                    //    isConnected = true;

                    //    WriteMessage(line);
                    //    
                    //    ChangeLabel("Connected");
                    //}

                    //else if (line.ToLower().Contains(NICK.ToLower() + " = " + HOME_CHANNEL.ToLower() + " :"))
                    //{
                    //    string correctCaps = getBetween(line, NICK + " = ", " :");

                    //    string users = getBetween(line, NICK + " = " + correctCaps + " :", "\r\n");
                    //    string[] usersTemp = users.Split(' ');

                    //    foreach (string user in usersTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Sort();
                    //    WriteMessage(line);
                    //    ChangeDataSource("");
                    //}

                    //else if (line.ToLower().Contains(NICK.ToLower() + " @ " + HOME_CHANNEL.ToLower() + " :"))
                    //{
                    //    string correctCaps = getBetween(line, NICK + " @ ", " :");

                    //    string users = getBetween(line, NICK + " @ " + correctCaps + " :", "\r\n");
                    //    string[] usersTemp = users.Split(' ');

                    //    foreach (string user in usersTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Sort();
                    //    WriteMessage(line);
                    //    ChangeDataSource("");
                    //}

                    //else if (line.Contains("JOIN " + HOME_CHANNEL))
                    //{
                    //    //someone joined
                    //    string whojoined = getBetween(line, ":", "!");
                    //    string message;
                    //    List<string> userTemp = new List<string>();
                    //    if (whojoined != NICK)
                    //    {
                    //        foreach (string user in userList)
                    //        {
                    //            if (user.Replace("@", string.Empty).Replace("+", string.Empty) != whojoined.Replace("@", string.Empty).Replace("+", string.Empty))
                    //            {
                    //                userTemp.Add(user);
                    //            }
                    //        }

                    //        userList.Clear();

                    //        foreach (string user in userTemp)
                    //        {
                    //            userList.Add(user);
                    //        }

                    //        foreach (Greeting g in greet)
                    //        {
                    //            if (g.Nick == whojoined.Replace("@", string.Empty).Replace("+", string.Empty) && g.Enabled==true)
                    //            {
                    //                message = privmsg(HOME_CHANNEL, g.Greetingg);
                    //                messageSender(writer, message);
                                
                    //            }
                            
                    //        }


                    //        userList.Add(whojoined);
                    //        userList.Sort();
                    //        WriteMessage(line);
                    //        WriteMessage("** " + whojoined + " joined", Color.Green);
                    //        ChangeDataSource("");
                    //    }
                    //}
                    //else if (line.Contains("PART " + HOME_CHANNEL))
                    //{
                    //    //someone left
                    //    string wholeft = getBetween(line, ":", "!");
                    //    List<string> userTemp = new List<string>();

                    //    foreach (string user in userList)
                    //    {
                    //        if (user.Replace("@", string.Empty).Replace("+", string.Empty) != wholeft.Replace("@", string.Empty).Replace("+", string.Empty))
                    //        {
                    //            userTemp.Add(user);
                    //        }
                    //    }
                    //    userList.Clear();

                    //    foreach (string user in userTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Sort();
                    //    WriteMessage("** " + wholeft + " parted", Color.Red);
                    //    ChangeDataSource("");
                    //}
                    //else if (line.Contains("QUIT :"))
                    //{
                    //    //someone left

                    //    string wholeft = getBetween(line, ":", "!");
                    //    List<string> userTemp = new List<string>();

                    //    foreach (string user in userList)
                    //    {
                    //        if (user.Replace("@", string.Empty).Replace("+", string.Empty) != wholeft.Replace("@", string.Empty).Replace("+", string.Empty))
                    //        {
                    //            userTemp.Add(user);
                    //        }
                    //    }
                    //    userList.Clear();

                    //    foreach (string user in userTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Sort();
                    //    WriteMessage("** " + wholeft + " quit", Color.Red);
                    //    ChangeDataSource("");
                    //}

                    //else if (line.Contains("NICK :"))
                    //{
                    //    //someone left

                    //    string oldnick = getBetween(line, ":", "!");
                    //    string newnick = getBetween(line, "NICK :", "\r");
                    //    char Mode = getUserMode(oldnick);

                    //    if (Mode != '0')
                    //        newnick = Mode + newnick;

                    //    List<string> userTemp = new List<string>();

                    //    foreach (string user in userList)
                    //    {
                    //        if (user.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty)
                    //            !=
                    //            oldnick.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty))
                    //        {
                    //            userTemp.Add(user);
                    //        }
                    //    }
                    //    userList.Clear();

                    //    foreach (string user in userTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Add(newnick);
                    //    userList.Sort();
                    //    WriteMessage("** " + oldnick + " is now known as " + newnick, Color.Yellow);
                    //    ChangeDataSource("");
                    //}

                    //else if (line.Contains("MODE " + HOME_CHANNEL))
                    //{
                    //    //modechange
                    //    List<string> userTemp = new List<string>();

                    //    string modechange = getBetween(line, "MODE " + HOME_CHANNEL + " ", " ");
                    //    string affectedUser = getBetween(line, modechange, "\n").Replace("\r", string.Empty).Replace(" ", string.Empty);

                    //    foreach (string user in userList)
                    //    {
                    //        if (user.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty)
                    //            !=
                    //            affectedUser.Replace("@", string.Empty).Replace("+", string.Empty).Replace("%", string.Empty).Replace("~", string.Empty).Replace("&", string.Empty))
                    //        {
                    //            userTemp.Add(user);
                    //        }
                    //    }
                    //    userList.Clear();

                    //    foreach (string user in userTemp)
                    //    {
                    //        userList.Add(user);
                    //    }

                    //    switch (modechange)
                    //    {
                    //        case ("+o"):
                    //            userList.Add("@" + affectedUser);
                    //            WriteMessage("** " + affectedUser + " was opped", Color.Blue);
                    //            break;
                    //        case ("-o"):
                    //            userList.Add(affectedUser);
                    //            WriteMessage("** " + affectedUser + " was deopped", Color.Blue);
                    //            break;
                    //        case ("+v"):
                    //            userList.Add("+" + affectedUser);
                    //            WriteMessage("** " + affectedUser + " was voiced", Color.Blue);
                    //            break;
                    //        case ("-v"):
                    //            userList.Add(affectedUser);
                    //            WriteMessage("** " + affectedUser + " was devoiced", Color.Blue);
                    //            break;
                    //        case ("+h"):
                    //            userList.Add("%" + affectedUser);
                    //            WriteMessage("** " + affectedUser + " was half opped", Color.Blue);
                    //            break;
                    //        case ("-h"):
                    //            userList.Add(affectedUser);
                    //            WriteMessage("** " + affectedUser + " was half deopped", Color.Blue);
                    //            break;
                    //        case ("+q"):
                    //            userList.Add("~" + affectedUser);
                    //            WriteMessage("** " + affectedUser + " was given Owner permissions", Color.Blue);
                    //            break;
                    //        case ("-q"):
                    //            userList.Add(affectedUser);
                    //            WriteMessage("** " + affectedUser + " was removed as a Owner", Color.Blue);
                    //            break;
                    //        case ("+a"):
                    //            userList.Add("&" + affectedUser);
                    //            WriteMessage("** " + affectedUser + " was given Admin permissions", Color.Blue);
                    //            break;
                    //        case ("-a"):
                    //            userList.Add(affectedUser);
                    //            WriteMessage("** " + affectedUser + " was removed as an Admin", Color.Blue);
                    //            break;

                    //    }

                    //    userList.Sort();
                    //    ChangeDataSource("");
                    //}
                    //else if (line.Contains("KICK " + HOME_CHANNEL))
                    //{
                    //    //user kicked
                    //    List<string> userTemp = new List<string>();

                    //    string affectedUser = getBetween(line, "KICK " + HOME_CHANNEL, ":").Replace("\r", string.Empty).Replace(" ", string.Empty);

                    //    foreach (string user in userList)
                    //    {
                    //        if (user.Replace("@", string.Empty).Replace("+", string.Empty) != affectedUser.Replace("@", string.Empty).Replace("+", string.Empty))
                    //        {
                    //            userTemp.Add(user);
                    //        }
                    //    }
                    //    userList.Clear();

                    //    foreach (string user in userTemp)
                    //    {
                    //        userList.Add(user);
                    //    }
                    //    userList.Sort();
                    //    WriteMessage("** " + affectedUser + " was kicked", Color.Red);
                    //    ChangeDataSource();

                    //}

                    else if (line.Contains("PRIVMSG"))
                    {
                        if (line.Length < 30)
                        {
                            WriteMessage("line.Length < 30 : " + line);
                        }
                        //else if (stoperror.Length < 4)
                        //{
                        //    int count = 0;
                        //    foreach (string s in stoperror)
                        //    {
                        //        WriteMessage("stoperror.Length < 4 : [" + count + " of " + stoperror.Length + "] " + s);
                        //        count++;
                        //    }

                        //}
                        else
                        {
                            string[] complete = line.Split(new char[] { ':' }, 3);
                            string info = complete[1];                  //info about the message
                            string msg = complete[2];                   //Message received
                            string[] split1 = msg.Split(' ');
                            string cmd = split1[0].Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty); ;
                            //cmd is the first word of a message, usefull to detect commands

                            string whoSent = info.Split(' ')[2];        //who sent is the source of the message. (channel, or user is private message)

                            string[] asd = info.Split('!');
                            string user = asd[0];                       //user is who sent the message

                            string[] blah = msg.Split(' ');
                            string arg;                                 //arg is the first word after the cmd

                            if (blah.Length >= 2) arg = blah[1];
                            else arg = "";

                            if (msg.ToLower().Contains(NICK.ToLower()))
                            {
                                if (user.Length > 15)
                                    WriteMessage(user.Truncate(16) + ":" + msg, Color.LightGreen);
                                else if (user.Length >= 8)                       //Write the message on the bot console
                                    WriteMessage(user + "\t: " + msg, Color.LightGreen);
                                else
                                    WriteMessage(user + "\t\t: " + msg, Color.LightGreen);
                            }
                            else
                            {
                                if (user.Length > 15)
                                    WriteMessage(user.Truncate(16) + ":" + msg);
                                else if (user.Length >= 8)                       //Write the message on the bot console
                                    WriteMessage(user + "\t: " + msg);
                                else
                                    WriteMessage(user + "\t\t: " + msg);
                            }

                            //StartParsing

                            if ((String.Compare(cmd, "hello", true) == 0
                                || String.Compare(cmd, "hi", true) == 0
                                || String.Compare(cmd, "hey", true) == 0)
                                && arg.ToLower().Contains(NICK.ToLower()))
                            {
                                WriteMessage("***** Recieved a hello from " + user);
                                hello(whoSent, user);
                            }

                            else if (msg == "!anime best anime ever")
                            {
                                client.messageSender(privmsg(whoSent, "[#15] [8.88 / 10] : "+"\x02"+"Code Geass: Hangyaku no Lelouch " +"\x02"+"-> http://myanimelist.net/anime/1575/Code_Geass:_Hangyaku_no_Lelouch"));
                            }

                            else if (String.Compare(cmd, SYMBOL + "help", true) == 0)
                            {
                                WriteMessage("*****  Recieved a help request from " + user);
                                help(whoSent, user);
                            }

                            else if (String.Compare(cmd, SYMBOL + "rules", true) == 0)
                            {
                                WriteMessage("*****  Recieved a rules request from " + user);
                                rules(whoSent, user);
                            }

                            else if (String.Compare(cmd, SYMBOL + "eta", true) == 0)
                            {
                                WriteMessage("*****  Recieved a eta request from " + user);
                                mangaETA(whoSent, user);
                            }

                            else if (String.Compare(cmd, SYMBOL + "quit", true) == 0)
                            {
                                WriteMessage("*****  Recieved a quit request from " + user);
                                int qu=quitIRC(HOME_CHANNEL, user);
                                client.Disconnect();
                                Thread.Sleep(100);
                                if (qu == 1)
                                    exitThisShit = 1;
                            }
                            else if (String.Compare(cmd, SYMBOL + "oplist", true) == 0)
                            {
                                WriteMessage("*****  Recieved a oplist request from " + user);
                                opList(whoSent, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "roll", true) == 0)
                            {
                                WriteMessage("*****  Recieved a roll request from " + user);
                                roll(whoSent, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "say", true) == 0 && arg != "")
                            {
                                int hhj = 0;

                                while (msg[hhj].ToString() != " ")
                                    hhj++;

                                string msgSay = msg.Substring(hhj + 1);
                                WriteMessage("****  Recieved a say request from " + user);
                                say(HOME_CHANNEL, msgSay, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "greetme", true) == 0)
                            {

                                if (arg == "")
                                { 
                                    WriteMessage("****  Recieved a greet TOOGLE request from " + user);
                                    greetToogle(client, HOME_CHANNEL, user);
                                
                                }
                                else
                                {
                                    int hhj = 0;

                                    while (msg[hhj].ToString() != " ")
                                        hhj++;

                                    string msgSay = msg.Substring(hhj + 1);
                                    WriteMessage("****  Recieved a greet request from " + user);
                                    addGreet(HOME_CHANNEL, msgSay, user);
                                }
                               
                            }


                            else if (String.Compare(cmd, SYMBOL + "me", true) == 0 && arg != "")
                            {
                                int hhj = 0;

                                while (msg[hhj].ToString() != " ")
                                    hhj++;

                                string msgSay = msg.Substring(hhj + 1);
                                WriteMessage("****  Recieved a me request from " + user);
                                me(HOME_CHANNEL, msgSay, user);
                            }

                            else if (String.Compare(cmd, SYMBOL + "claims", true) == 0 || String.Compare(cmd, SYMBOL + "c", true) == 0)
                            {
                                WriteMessage("*****  Recieved a claims request from " + user);
                                claims(whoSent, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "assignments", true) == 0 || String.Compare(cmd, SYMBOL + "a", true) == 0)
                            {
                                WriteMessage("*****  Recieved a assignments request from " + user);
                                assignments(whoSent, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "silence", true) == 0)
                            {
                                WriteMessage("*****  Recieved a silence request from " + user);
                                silence(whoSent, user);
                            }
                            else if (String.Compare(cmd, SYMBOL + "rename", true) == 0 && arg != "")
                            {
                                WriteMessage("*****  Recieved a rename request from " + user);
                                if (isOperator(user))
                                    changeNick(arg);
                            }

                            else if (String.Compare(cmd, SYMBOL + "op", true) == 0 && arg != "")
                            {
                                Console.Out.WriteLine("\n*****  Recieved a op request from " + user);
                                int did = addBotOP(HOME_CHANNEL, user, arg);
                                if (did == 1) SaveOPS();
                            }
                            else if (String.Compare(cmd, SYMBOL + "deop", true) == 0 && arg != "")
                            {
                                Console.Out.WriteLine("\n*****  Recieved a deop request from " + user);
                                int did = removeBotOP(HOME_CHANNEL, user, arg);
                                if (did == 1) SaveOPS();
                            }
                            else if (String.Compare(cmd, SYMBOL + "toF", true) == 0 && arg != "")
                            {
                                Console.Out.WriteLine("\n*****  Recieved a temp. conversion to F request from " + user);
                                toFahrenheit(HOME_CHANNEL, user, arg);
                            }
                            else if (String.Compare(cmd, SYMBOL + "toC", true) == 0 && arg != "")
                            {
                                Console.Out.WriteLine("\n*****  Recieved a temp. conversion to C request from " + user);
                                toCelcius(HOME_CHANNEL, user, arg);
                            }
                            else if (String.Compare(cmd, SYMBOL + "time", true) == 0)
                            {
                                Console.Out.WriteLine("\n*****  Recieved a time request from " + user);
                                time( HOME_CHANNEL, user, arg);
                            }
                            else if (String.Compare(cmd, SYMBOL + "wiki", true) == 0)
                            {
                                int hhj = 0;

                                while (msg[hhj].ToString() != " ")
                                    hhj++;

                                string msgSay = msg.Substring(hhj + 1);
                                Console.Out.WriteLine("\n*****  Recieved a explain request from " + user);
                                explain(HOME_CHANNEL, user, msgSay);
                            }

                            else if (String.Compare(cmd, SYMBOL + "anime", true) == 0 && arg != "")
                            {
                                int hhj = 0;

                                while (msg[hhj].ToString() != " ")
                                    hhj++;

                                string msgSay = msg.Substring(hhj + 1);
                                Console.Out.WriteLine("\n*****  Recieved a animeSearch request from " + user);
                                animeSeach(HOME_CHANNEL, user, msgSay);
                            }
                            else if (String.Compare(cmd, SYMBOL + "poke", true) == 0)
                            {
                                Console.Out.WriteLine("\n*****  Recieved a time request from " + user);
                                poke(HOME_CHANNEL, user, arg);
                            }

                            else if (String.Compare(cmd, SYMBOL + "trivia", true) == 0)
                            {
                                Console.Out.WriteLine("\n*****  Recieved a trivia request from " + user);
                                trivia(HOME_CHANNEL, user, arg);
                            }
                            else if (String.Compare(cmd, SYMBOL + "nick", true) == 0)
                            {
                                Console.Out.WriteLine("\n*****  Recieved a nickname request from " + user);

                                if (arg != "")
                                {
                                    int hhj = 0;
                                    while (msg[hhj].ToString() != " ")
                                        hhj++;

                                    string msgSay = msg.Substring(hhj + 1);
                                    nickGen(HOME_CHANNEL, user, msgSay);
                                }
                                else
                                {
                                    nickGen(HOME_CHANNEL, user, "");
                                }
                            }
                            else if (msg.Contains("youtube") && msg.Contains("watch") && (msg.Contains("?v=") || msg.Contains("&v=")))
                            {
                                WriteMessage("*****  Detected an youtube video from  " + user);
                                youtube(whoSent, user, msg);
                            }

                            else if (msg.Contains("youtu.be") && (msg.Contains("?v=") == false && msg.Contains("&v=") == false))
                            {
                                WriteMessage("*****  Detected a short youtube video from  " + user);
                                youtubeS(whoSent, user, msg);
                            }

                            else if (msg.Contains("vimeo.com"))
                            {
                                WriteMessage("*****  Detected an vimeo video from  " + user);
                                vimeo(whoSent, user, msg);
                            }

                            else if (msg.Contains("reddit.com") && msg.Contains("/r/") && msg.Contains("/comments/"))
                            {
                                WriteMessage("*****  Detected a reddit link from  " + user);
                                redditLink(whoSent, user, msg);
                            }

                            else if (msg.Contains("twitter.com") && msg.Contains("/status/"))
                            {
                                WriteMessage("*****  Detected a twiiter link from  " + user);
                                twitter(whoSent, user, msg);
                            }

                            //CTCP
                            else if (msg.Contains("\x01"))
                            {
                                string command = msg.Substring(msg.IndexOf("\x01") + 1, msg.Length - 2);
                                if (command.Contains("VERSION"))
                                {
                                    Console.Out.WriteLine("\n*****  Recieved a ctcp version request from " + user);
                                    ctcpVersion(user);
                                }

                                if (command.Contains("TIME"))
                                {
                                    Console.Out.WriteLine("\n*****  Recieved a ctcp time request from " + user);
                                    ctcpTime(user);
                                }
                            }


                            else //No parsing, just a normal message
                            {
                                if (whoSent == HOME_CHANNEL && msg != null)//Add to past messages
                                {
                                    pastMessage p = new pastMessage(user, msg);
                                    pastMessages.Add(p);
                                }
                            }
                        }

                        
                    }
                    else
                    {
                        WriteMessage(line);
                    }
                }
            }

        }

        ///////////////////////////////////
        //       Commands Functions      //
        ///////////////////////////////////

        int addBotOP(string CHANNEL, string nick, string arg)
        {
            string message;

            arg = arg.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (giveOps(arg))
            {
                message = notice(nick, arg + " was added as a bot operator!");
                client.messageSender(message);
                return 1;
            }
            else
            {
                message = notice(nick, "Error: " + arg + " is already a bot operator!");
                client.messageSender(message);
                return 0;
            }
        }
        int removeBotOP( string CHANNEL, string nick, string arg)
        {
            string message;

            arg = arg.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (takeOps(arg))
            {
                message = notice(nick, arg + " was removed as a bot operator!");
                client.messageSender(message);
                return 1;
            }
            else
            {
                message = notice(nick, "Error: " + arg + " was not a bot operator!");
                client.messageSender(message);
                return 0;
            }
        }
        int opList( string CHANNEL, string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = notice(nick, "Bot operators:");
                client.messageSender(message);
                foreach (string p in ops)
                {
                    message = notice(nick, nick + " :->" + p);
                    client.messageSender(message);
                }
                return 1;
            }

            return 0;
        }
        void say( string CHANNEL, string args, string nick)
        {
            string message;

            if (isOperator(nick))
            {
                message = privmsg(CHANNEL, args);
                client.messageSender(message);
                return;
            }
        }

        void me( string CHANNEL, string args, string nick)
        {
            string message;
            if (isOperator(nick))
            {
                message = privmsg(CHANNEL,"\x01" + "ACTION " + args + "\x01");
                client.messageSender(message);
                return;

            }

        }
        public void silence(string CHANNEL, string nick)
        {
            string message;
            if (isOperator(nick))
            {
                if (Settings.Default.silence == true)
                {
                    ChangeChecked("false");
                    Settings.Default.silence = false;
                    ChangeLabel2("");
                    message = notice(nick,"The bot was unmuted");               
                }
                else
                {
                    ChangeChecked("true");
                    Settings.Default.silence = true;
                    ChangeLabel2("Bot is Silenced");
                    message = notice(nick, "The bot was muted");
                }

                Settings.Default.Save();
                client.messageSender(message);
                return;
            }
        }

        void hello(string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.hello_Enabled == true && Settings.Default.silence == false)
            {
                string message = privmsg(CHANNEL, "Hello " + nick + "!");
                client.messageSender(message);
            }
        }
        void help( string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.help_Enabled == true)
            {
                foreach (string h in hlp)
                {
                    message = notice(nick, h.Replace("\n", "").Replace("\r", ""));
                    client.messageSender(message);

                }
            }
        }
        void rules( string CHANNEL, string nick)
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
                        client.messageSender(message);

                    }
                    return;
                }

            }
            else if (Settings.Default.rules_Enabled == true)
            {
                foreach (string h in ops)
                {
                    message = privmsg(CHANNEL, h.Replace("\n", "").Replace("\r", ""));
                    client.messageSender(message);
                    return;
                }
            }
        }
        void mangaETA( string CHANNEL, string nick)
        {
            if (Settings.Default.eta_Enabled == true)
            {
                if (isMuted(nick)) return;

                string message;
                if (Settings.Default.silence == true)
                {
                    message = notice(nick, "The average time for the chapter release is " + eta + ".");
                    client.messageSender(message);
                }
                else
                {
                    message = privmsg(CHANNEL, "The average time for the chapter release is " + eta + ".");
                    client.messageSender(message);
                }
            }
        }

        void roll( string CHANNEL, string nick)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.roll_Enabled == true)
            {
                Random random = new Random();
                int number = random.Next(0, 100);

                nick = nick.Replace("\r", "");
                string message = privmsg(CHANNEL, nick + " rolled a " + number);
                client.messageSender(message);
            }
        }
        private void poke( string CHANNEL, string nicks, string args)
        {
            string message;
            int userNumber = 0;
            Random rnd = new Random();

            if (isMuted(nicks)) return;

            do
            {
                userNumber = rnd.Next((client.userList.Count - 1));
            }
            while (client.userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) == nicks);

            if (Settings.Default.silence == false && Settings.Default.pokeEnabled == true)
            {
                message = privmsg(CHANNEL, "\x01" + "ACTION " + "pokes " + client.userList[userNumber].Replace("@", string.Empty).Replace("+", string.Empty) + "\x01");
                client.messageSender(message);
            }

        }

        void assignments( string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.assign_Enabled == true)
            {
                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
                    client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.assign_Enabled == true)
            {
                message = privmsg(CHANNEL, Settings.Default.currentAssignmentURL.Replace("\n", "").Replace("\r", ""));
                client.messageSender(message);

            }
        }
        void claims( string CHANNEL, string nick)
        {
            string message;
            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.claims_Enabled == true)
            {

                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
                    client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.claims_Enabled == true)
            {

                message = privmsg(CHANNEL, Settings.Default.currentClaimsURL.Replace("\n", "").Replace("\r", ""));
                client.messageSender(message);

            }
        }

        void toFahrenheit( string CHANNEL, string nick, string args)
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
                client.messageSender(message);
                return;
            }

            if (isMuted(nick)) return;

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
                    client.messageSender(message);
                    return;

                }
            }
            else if (Settings.Default.conversionEnabled == true)
            {
                message = privmsg(CHANNEL, cc + " C is " + Math.Round(f, 2) + " F");
                client.messageSender(message);

            }
        }
        void toCelcius( string CHANNEL, string nick, string args)
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
                client.messageSender(message);
                return;
            }

            if (Settings.Default.silence == true && Settings.Default.conversionEnabled == true)
            {

                if (isOperator(nick))
                {
                    message = privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                    client.messageSender(message);
                    return;
                }

            }
            else if (Settings.Default.conversionEnabled == true)
            {

                message = privmsg(CHANNEL, f + " F is " + Math.Round(cc, 2) + " C");
                client.messageSender(message);

            }
        }

        public void trivia( string CHANNEL, string nick, string args)
        {
            if (isMuted(nick)) return;
            if (triviaNumber == 0) return;
            if (Settings.Default.silence == false && Settings.Default.triviaEnabled == true)
            {
                string message;
                Random rnd = new Random();
                int rt = rnd.Next(triviaNumber-1);

                message = privmsg(CHANNEL, tri[rt]);
                client.messageSender(message);

            }
        }

        void time( string CHANNEL, string nick, string args)
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

            string IST = "23.7833, 85.9667";//Bokaro, india
            string MSK = "55.74941,37.614441";  //Moscow
            string FET = "53.895311,27.563324"; //Minsk
            string EET = "44.4476304,26.0860545"; //bucharest
            string CET = "48.8588589,2.3470599"; //paris

            string WER = "38.7436266,-9.1602038"; //lisbon
            string GMT = "51.5232391,-0.1166146"; //london

            string BRT = "-23.5778896,-46.6096585"; //sao paulo
            string ART = "-34.6158526,-58.4332985"; //buenos aires
            string AST = "53.3215407,-60.3542792"; //Happy+Valley-Goose+Bay
            string VET = "10.4683917,-66.8903658"; //caracas
            string EST = "40.7056308,-73.9780035"; //NYC
            string CST = "39.091919,-94.5757195"; //kansas city
            string MST = "40.7609881,-111.8936263"; //salt lake city
            string PST = "34.0469605,-118.2621293"; //LA
            string AKDT = "61.1878492,-149.8158133"; //Anchorage

            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;

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

                if (!wantUTC){
                    requestURL = "https://maps.googleapis.com/maps/api/timezone/json?location=" + location + "&timestamp=" + timestamp + "&sensor=" + sensor.ToString().ToLower() + "&key=" + Settings.Default.apikey;
                    var webClient = new WebClient();
                    webClient.Encoding = Encoding.UTF8;
                    try
                    {
                        json = webClient.DownloadString(requestURL);
                        JsonConvert.PopulateObject(json, g);

                    }
                    catch { }

                    convertedTime = ConvertFromUnixTimestamp(timestamp + g.dstOffset + g.rawOffset);

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
                client.messageSender(message);
                
            }
        }
      
        public void youtube(string CHANNEL, string nick, string line)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.youtube_Enabled == true)
            {
                string[] bah;
                string ID;
                string message;

                if (line.Contains("?v="))
                {
                    ID = getBetween(line, "?v=", "&");
                }
                else
                {
                    ID = getBetween(line, "&v=", "&");
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
                    client.messageSender(message); ;
                }
                else
                {
                    message = privmsg(CHANNEL,"\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02" );
                    client.messageSender(message);
                }
            }
        }
        public void youtubeS( string CHANNEL, string nick, string line)//for short links
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.youtube_Enabled == true)
            {
                string[] bah;
                string ID;
                string message;

                ID = getBetween(line, "youtu.be/", "?t");

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

                temp = Convert.ToInt32(duration);

                title = title.Replace("&lt;", "<");
                title = title.Replace("&rt;", ">");
                title = title.Replace("&amp;", "&");
                title = title.Replace("&quot;", "\"");

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
                    client.messageSender(message);
                }
                else
                {
                    message = privmsg(CHANNEL, "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    client.messageSender(message);
                }
            }
        }

        public void twitter( string CHANNEL, string nick, string line)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.twitterEnabled == true)
            {
                string[] bah;
                string url = "";
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

                author = getBetween(readHtml, "<span>&rlm;</span><span class=\"username js-action-profile-name\"><s>@</s><b>", "</b></span>");
                tweet = getBetween(readHtml, "<p class=\"js-tweet-text tweet-text\">", "</p>");

                author = StripTagsRegex(author);
                tweet = StripTagsRegex(tweet);

                tweet = tweet.Replace("&#39;", "'");
                tweet = tweet.Replace("&lt;", "<");
                tweet = tweet.Replace("&rt;", ">");
                tweet = tweet.Replace("&amp;", "&");
                tweet = tweet.Replace("&quot;", "\"");
                tweet = tweet.Replace("&#10;", " ");
                tweet = tweet.Replace("&nbsp;", " ");

                string message = privmsg(CHANNEL, "Tweet by @" + author + " : " + tweet);
                client.messageSender(message);
            }
        }

        public void animeSeach( string CHANNEL, string nick, string line)
        {
            string message;
            if (isMuted(nick)) return;
            GoogleSeach g = new GoogleSeach();
            string json;
            bool user = false;

            if (line == "" || line == " ") return;

            if (line.Contains("-u") == true) user = true;

            string getString = "https://www.googleapis.com/customsearch/v1?key=" + Settings.Default.apikey + "&cx=" + Settings.Default.cxKey + "&q=" + line.Replace(" ", "%20").Replace(" -u", "%20");

            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            if (Settings.Default.silence == false && Settings.Default.aniSearchEnabled == true)
            {
                try
                {
                    json = webClient.DownloadString(getString);
                    JsonConvert.PopulateObject(json, g);
                }
                catch {  }

                if (g.items == null) message = privmsg(CHANNEL, "Could not find anything, try http://myanimelist.net/anime.php?q=" + line.Replace(" ", "%20"));
                else
                {
                    int i_max = 0; int i = 0; bool found = false;

                    if (g.items.Length < 4) 
                        i_max = g.items.Length-1;
                    else i_max = 4;
                    
                    while (i<=i_max && found == false)
                    {
                        if (!user)
                        {
                            if (g.items[i].link.Contains("http://myanimelist.net/anime/"))
                            {
                                found = true;
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
                            string readHtml = webClient.DownloadString(g.items[i].link);

                            string score = getBetween(readHtml, "Score:</span> ", "<sup><small>");
                            string rank = getBetween(readHtml, ">Ranked #", "</div>");
                            string title = getBetween(readHtml, ">Ranked #" + rank + "</div>", "</h1>");


                            message = privmsg(CHANNEL, "[#" + rank + "] " + "[" + score + " / 10] : " + "\x02" + title + "\x02" + " -> " + g.items[i].link);

                        }
                        else 
                        {
                            string readHtml = webClient.DownloadString(g.items[i].link);

                            string profile = getBetween(readHtml, "<title>", "'s Profile - MyAnimeList.net</title>");

                            string completed = getBetween(readHtml, ">Completed</span></td>", "<td><div style=");
                            completed = getBetween(completed, "<td align=\"center\">", "</td>");


                            message = privmsg(CHANNEL, "[" + profile + "] " + "Completed " + completed + " animes"   +" -> " + g.items[i].link);
                        }
                }
                client.messageSender(message);

            }
        }

        public void vimeo( string CHANNEL, string nick, string line)
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
                    client.messageSender(message);

                }
                else
                {
                    message = privmsg(CHANNEL, "\x02" + "Vimeo Video: " + title + " [" + minutes + ":" + seconds.ToString("00") + "]\x02");
                    client.messageSender(message);
                }

            }
        }

        public void nickGen( string CHANNEL, string nick, string args)
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
                client.messageSender(message);
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
                client.messageSender(message);
            }
        }

        public void redditLink( string CHANNEL, string nick, string line)
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

                if (linkParse.Length >= 9 && linkParse[8] != "")    //Com comentário
                {
                    string urlFix = url;
                    tempQ = urlFix.Split(new char[] { '?' }, 2);
                    urlFix = tempQ[0];
                    Uri urlURI = new Uri(urlFix.Replace("\r", string.Empty));
                    post = reddit.GetPost(urlURI);     //slow

                    string commentID = linkParse[8];
                    tempQ = commentID.Split(new char[] { '?' }, 2);
                    commentID = tempQ[0];

                    com = reddit.GetComment(sub.Name, "t1_" + commentID, "t3_" + linkParse[6]);

                    message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submited by /u/" + post.Author + "\x02");
                    client.messageSender(message);

                    if (com.Body.ToString().Length > 250)
                        message2 = privmsg(CHANNEL, "\x02" + "Comment: " + com.Body.ToString().Truncate(250).Replace("\r", " ").Replace("\n", " ") + "(...)" + "\x02");
                    else
                        message2 = privmsg(CHANNEL, "\x02" + "Comment: " + com.Body.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty) + "\x02");

                    client.messageSender(message2);

                }
                else
                {                                               //No comment link
                    Uri urlURI = new Uri(url.Replace("\r", string.Empty));
                    post = reddit.GetPost(urlURI);     //slow

                    if (post.IsSelfPost)
                    {
                        message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "] " + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submited by /u/" + post.Author + "\x02");
                        client.messageSender(message);
                    }
                    else
                    {
                        message = privmsg(CHANNEL, "\x02" + "[/r/" + post.Subreddit + "]" + "[" + "↑" + +post.Upvotes + "] " + "\x02" + post.Title + "\x02" + ", submited by /u/" + post.Author + "\x02" + " :" + " \x033" + post.Url + "\x03");
                        client.messageSender(message);

                    }
                       
                }

            }

            catch   //403 error
            {
                subreddit = getBetween(url, "/r/", "/");

                message = privmsg(CHANNEL, "\x02" + "[/r/" + subreddit.Replace(" ", string.Empty) + "] " + "this subreddit is private" + "\x02");
                client.messageSender(message);
                return;
            }
        }

        void explain( string CHANNEL, string nick, string args)
        {
            if (isMuted(nick)) return;

            if (Settings.Default.silence == false && Settings.Default.wikiEnabled == true)
            {
                string message = privmsg(CHANNEL, "Here's a wiki for \"" + args + "\": " + "http://en.wikipedia.org/w/index.php?title=Special:Search&search=" + args.Replace(" ", "%20"));
                client.messageSender(message);
            }
        }

        ///////////////////////////////////
        //     Commands Functions End    //
        ///////////////////////////////////

        //CTCP replies
        public void ctcpTime( string CHANNEL)
        {
            DateTime dateValue = new DateTime();
            dateValue = DateTime.Now;
            string week = dateValue.ToString("ddd", new CultureInfo("en-US"));
            string month = dateValue.ToString("MMM", new CultureInfo("en-US"));
            string day = DateTime.Now.ToString("dd");
            string hour = DateTime.Now.ToString("HH:mm:ss");


            string complete = week + " " + month + " " + day + " " + hour;

            string message = notice(CHANNEL, "\x01" + "TIME " + complete + "\x01");
            client.messageSender(message);
        }
        public void ctcpVersion( string CHANNEL)
        {
            string message = notice(CHANNEL, "\x01" + "VERSION " + botVersion + "\x01");
            client.messageSender(message);
        }
        ////

        private void disconnect()
        {
            ChangeLabel("Disconnecting...");

            client.Disconnect();
            Thread.Sleep(250);

            output2.Clear();
            ChangeTitle("NarutoBot");
            exitThisShit = 1;

        }
        public int quitIRC( string CHANNEL, string nick)
        {
            string message;

            foreach (string n in ops)
            {
                if (String.Compare(n, nick, true) == 0)
                {
                    message = "QUIT :Goodbye everyone!\n";
                    client.messageSender(message);

                    ChangeLabel("Disconnecting...");

                    return 1;
                }
            }

            return 0;
        }
        public void ping(string pingmsg, StreamWriter s)
        {
            pingmsg = pingmsg.Replace("PING :", "");

            string message = "PONG :" + pingmsg + "\r\n";
            s.WriteLine(message);
        }
        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //isConnected = false;
        }

        public void isMangaOutEvent(object source, ElapsedEventArgs e)
        {
            String readHtml;
            string url = Settings.Default.baseURL.TrimEnd('/') + "/" + Settings.Default.chapterNumber;
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            HttpWebRequest request;


            if (!Settings.Default.releaseEnabled) return;

            try
            {
                //readHtml = webClient.DownloadString(url);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                request.Timeout = 7 * 1000;   //7 seconds
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                readHtml = readStream.ReadToEnd();
            }
            catch       //Error
            {
                return;
            }

            if (!readHtml.Contains("is not released yet."))//Not yet
            {
                string message;

                message = privmsg(HOME_CHANNEL, "*");
                client.messageSender(message);
                message = privmsg(HOME_CHANNEL, "\x02" + "\x030,4Chapter " + Settings.Default.chapterNumber.ToString() + " appears to be out! \x030,4" + url + " [I'm a bot, so i can be wrong!]" + "\x02");
                client.messageSender(message);
                message = privmsg(HOME_CHANNEL, "*");
                client.messageSender(message);

                Settings.Default.releaseEnabled = false;
                Settings.Default.Save();

                aTime.Enabled = false;
            }

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

            message = privmsg(HOME_CHANNEL, "\x02" + randomWords + "\x02");
            client.messageSender(message);

            //pastMessages.Clear();

        }

        private void output2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process myProcess = new Process();
            string url = e.LinkText;
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = url;
            myProcess.Start();

            myProcess.Close();
        }

    }
}
