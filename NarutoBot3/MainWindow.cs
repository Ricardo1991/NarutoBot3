using NarutoBot3.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MainWindow : Form
    {
        delegate void SetTextCallback(string text);
        delegate void SetBoolCallback(bool status);
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

        Bot ircBot;
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

        bool exitTheLoop = false;

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


            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5 )
            {
                Settings.Default.aniSearchEnabled = false;
                Settings.Default.timeEnabled = false;
            }

            if(Settings.Default.malPass.Length < 2 || Settings.Default.malUser.Length < 2)
                Settings.Default.aniSearchEnabled = false;


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
                if (connect())//If connected with sfuccess, then start the bot
                {
                    exitTheLoop = false;
                    backgroundWorker1.RunWorkerAsync();
                    //isConnected = true;
                }
                else
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool connect()//This is where the bot connects to the server and logs in
        {
            ChangeConnectingLabel("Connecting...");

            loadSettings();
            client = new IrcClient(HOME_CHANNEL, HOST, PORT, NICK);


            if (client.Connect())
            {
                exitTheLoop = false;
                return true;
            }
            else return false;
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //This is the main bot cycle... Pretty much everything happens here
        {
            //Main Loop
            String buffer;

            ircBot = new Bot(ref client, ref OutputBox);

            ircBot.Connected += new ConnectedChangedEventHandler(nowConnected);
            ircBot.ConnectedWithServer += new ConnectedChangedEventHandler(nowConnectedWithServer);

            ircBot.Created += new UserListChangedEventHandler(userListCreated);
            ircBot.Joined += (senderr, ee) => userJoined(senderr, ee, ircBot.Who);
            ircBot.Left += (senderr, ee) => userLeft(senderr, ee, ircBot.Wholeft);
            ircBot.NickChanged += (senderr, ee) => userNickChange(senderr, ee, ircBot.Who, ircBot.NewNick);
            ircBot.Kicked += (senderr, ee) => userKicked(senderr, ee, ircBot.Who);
            ircBot.ModeChanged += (senderr, ee) => userModeChanged(senderr, ee, ircBot.Who, ircBot.Mode);

            ircBot.MentionReceived += (senderr, ee) => { WriteMessage(returnmessage, Color.LightGreen); };
            ircBot.MessageReceived += (senderr, ee) => { WriteMessage(returnmessage); };

            ircBot.BotNickChanged += (senderr, ee) => eventChangeTitle(senderr, ee, returnmessage);

            ircBot.BotSilenced += new BotSilenceChange(botSilence);
            ircBot.BotUnsilenced += new BotSilenceChange(botUnsilence);

            ircBot.Quit += new Quit(letsQuit);

            ircBot.DuplicatedNick += new NickAlreadyInUse(duplicatedNick);

            ircBot.LoadSettings();

            while (!exitTheLoop)
            {
                buffer = "";
                try
                {
                    buffer = client.messageReader();
                    byte[] bytes = Encoding.Default.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    ircBot.BotMessage(line, out returnmessage);
                }
                catch
                { }
            }
            //disconnect();
        }


        private void disconnect()
        {
            ChangeConnectingLabel("Disconnecting...");

            client.Disconnect();
            
            Thread.Sleep(250);

            UpdateDataSource();

            OutputClean();
            
            ChangeTitle("NarutoBot");

            exitTheLoop = true;
            
        }

        
        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeConnectingLabel("Disconnected");
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
