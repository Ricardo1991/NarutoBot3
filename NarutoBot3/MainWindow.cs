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
        delegate void ChangeDataSource();

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

        Bot bot;
        public IrcClient client;

        System.Timers.Timer aTime;      //To check for manga releases
        System.Timers.Timer rTime;      //To check for random text

        string HOME_CHANNEL;
        string HOST;
        string NICK;
        int PORT;

        String line;

        string lastCommand;

        string returnmessage = "";

        int exitThisShit = 0;

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

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

           

            Settings.Default.Save();

            aTime = new System.Timers.Timer(Settings.Default.checkInterval);
            aTime.Enabled = false;

            rTime = new System.Timers.Timer(Settings.Default.randomTextInterval * 60 * 1000);
            rTime.Enabled = Settings.Default.randomTextEnabled;
            rTime.Elapsed += (sender, e) => randomTextSender(sender, e);

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

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);

            //bot.LoadSettings();
        }

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker1.WorkerSupportsCancellation = true;

            

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

            loadSettings();
            client = new IrcClient(HOME_CHANNEL, HOST, PORT, NICK);
            return client.Connect();
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //This is the main bot cycle... Pretty much everything happens here
        {
            //Main Loop
            String buffer;
            exitThisShit = 0;

            bot = new Bot(ref client, ref output2);

            bot.Connected += new ConnectedChangedEventHandler(nowConnected);

            bot.Created += new UserListChangedEventHandler(userListCreated);
            bot.Joined += (senderr, ee) => userJoined(senderr, ee, bot.Who);
            bot.Left += (senderr, ee) => userLeft(senderr, ee, bot.Wholeft);
            bot.NickChanged += (senderr, ee) => userNickChange(senderr, ee, bot.Who, bot.NewNick);
            bot.Kicked += (senderr, ee) => userKicked(senderr, ee, bot.Who);
            bot.ModeChanged += (senderr, ee) => userModeChanged(senderr, ee, bot.Who, bot.Mode);

            bot.MentionReceived += (senderr, ee) => { WriteMessage(returnmessage, Color.LightGreen); };
            bot.MessageReceived += (senderr, ee) => { WriteMessage(returnmessage); };

            bot.BotNickChanged += (senderr, ee) => eventChangeTitle(senderr, ee, returnmessage);

            bot.BotSilenced += new BotSilenceChange(botSilence);
            bot.BotUnsilenced += new BotSilenceChange(botUnsilence);

            bot.Quit += new Quit(letsQuit);
            bot.LoadSettings();

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
                   
                    returnmessage = "";

                    bot.BotMessage(line, out returnmessage);

                    //if (line.Contains("PRIVMSG"))
                    //{
                    //    if (line.Length < 30)
                    //    {
                    //        WriteMessage("line.Length < 30 : " + line);
                    //    }

                    //}
                    //else
                    //{
                    //    WriteMessage(line);
                    //}
                }
            }

        }


        private void disconnect()
        {
            ChangeLabel("Disconnecting...");

            client.Disconnect();
            Thread.Sleep(250);

            UpdateDataSource();

            output2.Clear();
            ChangeTitle("NarutoBot");
            exitThisShit = 1;

        }
        public int quitIRC( string CHANNEL, string nick)
        {
            string message;

            foreach (string n in bot.ops)
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
