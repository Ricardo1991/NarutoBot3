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
        delegate void SetEventCallback(object sender, EventArgs e, string text);
        delegate void SetBoolCallback(bool status);
        delegate void ChangeDataSource();

        ConnectWindow Connect = new ConnectWindow();
        SettingsWindow settingsWindow = new SettingsWindow();
        ChangeBotNickWindow nickWindow = new ChangeBotNickWindow();
        EditRulesWindow rulesWindow = new EditRulesWindow();
        HelpTextWindow helpWindow = new HelpTextWindow();
        MangaReleaseCheckerWindow releaseChecker = new MangaReleaseCheckerWindow();
        AboutBox aboutbox = new AboutBox();

        MutedUsersWindow mutedWindow;
        BotOperatorWindow operatorsWindow;

        private Bot bot;
        private IRC_Client client;

        System.Timers.Timer mangaReleaseTimer;      //To check for manga releases
        System.Timers.Timer randomTextTimer;        //To check for random text
        System.Timers.Timer timeoutTimer;           //To check for connection lost

        string HOME_CHANNEL;
        string HOST;
        string NICK;
        int PORT;
        string REALNAME;

        string lastCommand;

        bool exitTheLoop = false;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.WorkerSupportsCancellation = true;

            lastCommand = "";

            var result = Connect.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (connect())          //If connected with success, then start the bot
                    backgroundWorker.RunWorkerAsync();
                else
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void loadSettings()
        {
            t30.Checked = false;
            t45.Checked = false;
            t60.Checked = false;
            switch (Settings.Default.randomTextInterval)
            {
                case 30:
                    t30.Checked = true;
                    break;
                case 45:
                    t45.Checked = true;
                    break;
                case 60:
                    t60.Checked = true;
                    break;
                default:
                    Settings.Default.randomTextInterval = 30;
                    t30.Checked = true;
                    break;
            }

            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
                Settings.Default.aniSearchEnabled = false;

            if (Settings.Default.apikey.Length < 5){
                Settings.Default.timeEnabled = false;
                Settings.Default.youtubeSearchEnabled = false;
            }
            
            if (Settings.Default.malPass.Length < 2 || Settings.Default.malUser.Length < 2)
                Settings.Default.aniSearchEnabled = false;

            if (String.IsNullOrEmpty(Settings.Default.redditUser) || String.IsNullOrEmpty(Settings.Default.redditPass))
                Settings.Default.redditEnabled = false;

            if (String.IsNullOrWhiteSpace(Settings.Default.twitterAccessToken) ||
                    String.IsNullOrWhiteSpace(Settings.Default.twitterAccessTokenSecret) ||
                    String.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKey) ||
                    String.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKeySecret))
                Settings.Default.twitterEnabled = false;

            mangaReleaseTimer = new System.Timers.Timer(Settings.Default.checkInterval);
            mangaReleaseTimer.Enabled = false;

            randomTextTimer = new System.Timers.Timer(Settings.Default.randomTextInterval * 60 * 1000);
            randomTextTimer.Enabled = Settings.Default.randomTextEnabled;
            randomTextTimer.Elapsed += (sender, e) => randomTextSender(sender, e);

            timeoutTimer = new System.Timers.Timer(Settings.Default.timeOutTimeInterval * 1000);
            timeoutTimer.Enabled = true;
            timeoutTimer.Elapsed += new ElapsedEventHandler(pingServer);

            Settings.Default.releaseEnabled = false;

            if (Settings.Default.silence == true){
                silencedToolStripMenuItem.Checked = true;
                toolStripStatusLabelSilence.Text = "Bot is Silenced";
            }
            else{
                silencedToolStripMenuItem.Checked = false;
                toolStripStatusLabelSilence.Text = "";
            }

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);
            REALNAME = Settings.Default.RealName;

            Settings.Default.Save();

        }
        public bool connect()   //This is where the bot connects to the server and logs in
        {
            ChangeConnectingLabel("Connecting...");

            loadSettings();
            client = new IRC_Client(HOME_CHANNEL, HOST, PORT, NICK, REALNAME);

            if ( client.Connect() ) {
                exitTheLoop = false;
                timeoutTimer.Enabled = true;

                return true;
            }

            else {
                timeoutTimer.Enabled = false;

                return false;
            } 
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //Main Loop
        {
            String buffer;
            String line;

            bot = new Bot(ref client, ref OutputBox);

            initializeBot();

            while (!exitTheLoop)
            { 
                buffer = "";
                
                try
                {
                    buffer = client.readLine();
                    byte[] bytes = Encoding.Default.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    bot.processMessage(line);
                }
                catch
                { }
            }
        }

        private void initializeBot()
        {
            bot.Connected += new EventHandler<EventArgs>(nowConnected);
            bot.ConnectedWithServer += new EventHandler<EventArgs>(nowConnectedWithServer);

            bot.Created += new EventHandler<EventArgs>(userListCreated);
            bot.Joined += (sender, e) => userJoined(bot.Who);
            bot.Left += (sender, e) => userLeft(bot.WhoLeft);
            bot.NickChanged += (sender, e) => userNickChange(bot.Who, bot.NewNick);
            bot.Kicked += (sender, e) => userKicked(bot.Who);
            bot.ModeChanged += (sender, e) => userModeChanged(bot.Who, bot.Mode);

            bot.Timeout += new EventHandler<EventArgs>(timeout);

            bot.BotNickChanged += (sender, e) => eventChangeTitle(sender, e);

            bot.BotSilenced += new EventHandler<EventArgs>(botSilence);
            bot.BotUnsilenced += new EventHandler<EventArgs>(botUnsilence);

            bot.Quit += new EventHandler<EventArgs>(letsQuit);

            bot.DuplicatedNick += new EventHandler<EventArgs>(duplicatedNick);

            bot.PongReceived += (sender, e) => updateLag(bot.TimeDifference);

            bot.TopicChange += (sender, e) => changeTopicTextBox(sender, e, bot.Topic);

            bot.LoadSettings();

            operatorsWindow = new BotOperatorWindow(ref bot.ul);
            mutedWindow = new MutedUsersWindow(ref bot.ul);

        }

        private void changeTopicTextBox(object sender, EventArgs e, string p)
        {
            if (tbTopic.InvokeRequired)
            {
                SetEventCallback d = new SetEventCallback(changeTopicTextBox);
                this.Invoke(d, new object[] { sender, e, p });
            }
            else
            {
                tbTopic.Text = p;
            }
        }

        private void disconnectClient()
        {
            ChangeConnectingLabel("Disconnecting...");
            client.Disconnect();
            
            Thread.Sleep(250);

            exitTheLoop = true;
            timeoutTimer.Enabled = false;
            UpdateDataSource();
            OutputClean();
            ChangeTitle("NarutoBot");
        }
        
        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeConnectingLabel("Disconnected");
        }

        public void isMangaOutEvent(object source, ElapsedEventArgs e)
        {
            String rawHTML;
            string url = Settings.Default.baseURL.TrimEnd('/') + "/" + Settings.Default.chapterNumber;
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            HttpWebRequest request;

            if (!Settings.Default.releaseEnabled) return;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                request.Timeout = 7 * 1000;   //7 seconds
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                rawHTML = readStream.ReadToEnd();
            }
            catch
            {
                return;
            }

            if (!rawHTML.Contains("is not released yet.")) //Not yet
            {
                string message;

                message = privmsg(client.HOME_CHANNEL, "*");
                client.messageSender(message);
                message = privmsg(client.HOME_CHANNEL, "\x02" + "\x030,4Chapter " + Settings.Default.chapterNumber.ToString() + " appears to be out! \x030,4" + url + " [I'm a bot, so i can be wrong!]" + "\x02");
                client.messageSender(message);
                message = privmsg(client.HOME_CHANNEL, "*");
                client.messageSender(message);

                Settings.Default.releaseEnabled = false;
                Settings.Default.Save();

                mangaReleaseTimer.Enabled = false;
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

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadKills();
        }

        public void ChangeConnectingLabel(String message)
        {
            try
            {
                l_Status.Text = message;
            }
            catch { }
        }
        public void ChangeSilenceLabel(String message)
        {
            if (statusStripBottom.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeSilenceLabel);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                toolStripStatusLabelSilence.Text = message;

            }
        }
        public void ChangeSilenceCheckBox(bool status)//toolStrip1
        {
            if (toolStripMenu.InvokeRequired)
            {
                SetBoolCallback d = new SetBoolCallback(ChangeSilenceCheckBox);
                this.Invoke(d, new object[] { status });
            }
            else
            {
                silencedToolStripMenuItem.Checked = status;
            }
        }

        public void ChangeTitle(String title)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeTitle);
                this.Invoke(d, new object[] { title });
            }
            else
            {
                this.Text = title;
            }

        }
        public void ChangeInput(String title)
        {
            if (InputBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeInput);
                this.Invoke(d, new object[] { title });
            }
            else
            {
                this.InputBox.Text = title;
            }
        }
        public void WriteMessage(String message) //Writes Message on the TextBox (bot console)
        {
            if (OutputBox.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message);
                    Invoke(invoker);
                }
                catch { }
            }
            else
            {
                this.OutputBox.AppendText(message + "\n");
            }

            //also, should make a log
        }

        public void WriteMessage(String message, Color color) //Writes Message on the TextBox (bot console)
        {
            if (OutputBox.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message, color);
                    Invoke(invoker);
                }
                catch { }
            }
            else
            {
                this.OutputBox.AppendText(message + "\n", color);
            }

            //also, should make a log
        }

        public void OutputClean()
        {
            if (OutputBox.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => OutputClean();
                    Invoke(invoker);
                }
                catch { }
            }
            else
            {
                OutputBox.Clear();
            }
        }

        public void UpdateDataSource()
        {
            if (InterfaceUserList.InvokeRequired)
            {
                ChangeDataSource d = new ChangeDataSource(UpdateDataSource);
                this.Invoke(d);
            }
            else
            {
                bot.userList.Sort();
                InterfaceUserList.DataSource = null;
                InterfaceUserList.DataSource = bot.userList;
            }
        }

        //UI Events

        private void connectMenuItem1_Click(object sender, EventArgs e) //Connect to...
        {
            var result = Connect.ShowDialog();
            DialogResult resultWarning;

            if (result == DialogResult.OK)
            {
                //Re-do Connect!
                if (client != null)
                {
                    if (client.isConnected)
                    {
                        resultWarning = MessageBox.Show("This bot is already connected.\nDo you want to end the current connection?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        if (resultWarning == System.Windows.Forms.DialogResult.OK)
                        {
                            disconnectClient();
                            Thread.Sleep(250);

                            ChangeConnectingLabel("Connecting...");

                            if (connect()) //If connected with success, then start the bot
                                backgroundWorker.RunWorkerAsync();
                            
                            else
                            {
                                MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                ChangeConnectingLabel("Disconnected");
                            }
                        }
                    }
                    else
                    {
                        ChangeConnectingLabel("Connecting...");

                        if (connect())//If connected with success, then start the bot
                        {
                            backgroundWorker.RunWorkerAsync();
                        }
                        else
                        {
                            MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ChangeConnectingLabel("Disconnected");
                        }
                    }
                }
                else
                {
                    ChangeConnectingLabel("Connecting...");

                    HOME_CHANNEL = Settings.Default.Channel;
                    HOST = Settings.Default.Server;
                    NICK = Settings.Default.Nick;
                    PORT = Convert.ToInt32(Settings.Default.Port);

                    if (String.IsNullOrWhiteSpace(HOME_CHANNEL)) HOME_CHANNEL = "#reddit-naruto";
                    if (String.IsNullOrWhiteSpace(HOST)) HOST = "irc.freenode.net";
                    if (String.IsNullOrWhiteSpace(NICK)) NICK = "NarutoBot";
                    if (PORT <= 0 || PORT > 65535) PORT = 6667;

                    if (connect()) //If connected with success, then start the bot
                    {
                        backgroundWorker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ChangeConnectingLabel("Disconnected");
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) //Quit Button
        {
            if (client.isConnected) 
                disconnectClient();

            this.Close();
        }

        private void silencedToolStripMenuItem_Click(object sender, EventArgs e)  //Toogle Silence
        {
            if (Settings.Default.silence == true)
            {
                silencedToolStripMenuItem.Checked = false;
                Settings.Default.silence = false;
                toolStripStatusLabelSilence.Text = "";
            }
            else
            {
                silencedToolStripMenuItem.Checked = true;
                Settings.Default.silence = true;
                toolStripStatusLabelSilence.Text = "Bot is Silenced";
            }
            Settings.Default.Save();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e) //Disconnect Button
        {
            if (client.isConnected)
                disconnectClient();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsWindow.ShowDialog();

            if (Settings.Default.redditEnabled) bot.redditLogin(Settings.Default.redditUser, Settings.Default.redditPass);
            if (Settings.Default.twitterEnabled) bot.TwitterLogin();
                
        }

        private void changeNickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nickWindow.ShowDialog();
            bot.changeNick(Settings.Default.Nick);
        }

        private void operatorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            operatorsWindow.ShowDialog();
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                InputBox.Text = lastCommand;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                lastCommand = InputBox.Text;
                e.Handled = true;
                e.SuppressKeyPress = true;

                string message = "";

                if (!client.isConnected) return;
                if (String.IsNullOrEmpty(InputBox.Text)) return;

                string[] parsed = InputBox.Text.Split(new char[] {' '}, 2); //parsed[0] is the command (first word), parsed[1] is the rest              

                if (parsed.Length >= 2 && !String.IsNullOrEmpty(parsed[1]))
                {

                    if (parsed[0].ToLower() == "/me")  //Action send
                            message = privmsg(HOME_CHANNEL, "\x01" + "ACTION " + parsed[1] + "\x01");

                    else if (parsed[0].ToLower() == "/whois" )  //Action send
                            message = "WHOIS " + parsed[1] + "\n";

                    else if (parsed[0].ToLower() == "/whowas" )  //Action send
                            message = "WHOWAS " + parsed[1] + "\n";

                    else if (parsed[0].ToLower() == "/Nick" )  //Action send
                            changeNick(parsed[1]);

                    else if (parsed[0].ToLower() == "/ns" || parsed[0].ToLower() == "/nickserv" )  //NickServ send
                            message = privmsg("NickServ", parsed[1]);

                    else if (parsed[0].ToLower() == "/cs" || parsed[0].ToLower() == "/chanserv")  //Chanserv send
                            message = privmsg("ChanServ", parsed[1]);

                    else if (parsed[0].ToLower() == "/query" || parsed[0].ToLower() == "/pm" || parsed[0].ToLower() == "/msg")  //Action send
                    {
                        parsed = InputBox.Text.Split(new char[] { ' ' }, 3);
                        if (parsed.Length >= 3)
                            message = privmsg(parsed[1], parsed[2]);
                        else
                            WriteMessage("Not enough arguments");
                    }
                    else if (parsed[0][0] != '/') //Normal send
                            message = privmsg(HOME_CHANNEL, InputBox.Text);
                        
                }
                else
                    if (parsed[0][0] == '/')
                        WriteMessage("Not enough arguments");

                    else //Normal send
                        message = privmsg(HOME_CHANNEL, InputBox.Text);
                    
                if(!String.IsNullOrWhiteSpace(message)) client.messageSender(message);
                ChangeInput("");
            }
        }

        private void rulesTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rulesWindow.ShowDialog();
        }


        private void helpTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            helpWindow.ShowDialog();
            bot.ReadHelp();
        }

        private void mutedUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mutedWindow.ShowDialog();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadRules();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadHelp();
        }

        private void nickGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadNickGen();
        }

        private void triviaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadTrivia();
        }

        private void botSilence(object sender, EventArgs e)
        {
            ChangeSilenceCheckBox(true);
            Settings.Default.silence = true;
            ChangeSilenceLabel("Bot is Silenced");
            Settings.Default.Save();
        }

        private void timeout(object sender, EventArgs e)
        {
            disconnectClient();

            ChangeConnectingLabel("Re-Connecting...");
            WriteMessage("* The connection timed out. Will try to reconnect.");

            if (connect()) //If connected with success, then start the bot
                backgroundWorker.RunWorkerAsync();

            else{
                MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ChangeConnectingLabel("Disconnected");
            }
        }

        private void botUnsilence(object sender, EventArgs e)
        {
            ChangeSilenceCheckBox(false);
            Settings.Default.silence = false;
            ChangeSilenceLabel("");

            Settings.Default.Save();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuUserList.Items.Clear();
            string nick = InterfaceUserList.SelectedItem.ToString().Replace("@", string.Empty).Replace("+", string.Empty);

            contextMenuUserList.Items.Add("Give " + nick + " Ops");
            contextMenuUserList.Items.Add("Take " + nick + " Ops");
            contextMenuUserList.Items.Add("Mute " + nick);
            contextMenuUserList.Items.Add("Unmute " + nick);
            contextMenuUserList.Items.Add("Poke " + nick);
            contextMenuUserList.Items.Add("Whois " + nick);
            contextMenuUserList.Items.Add("Kick " + nick + " (if operator)");
        }
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string[] split = e.ClickedItem.Text.Split(' ');
            switch (split[0])
            {
                case "Give":
                    bot.giveOps(split[1]);
                    break;
                case "Take":
                    bot.takeOps(split[1]);
                    break;
                case "Mute":
                    bot.muteUser(split[1]);
                    break;
                case "Unmute":
                    bot.unmuteUSer(split[1]);
                    break;
                case "Poke":
                    bot.pokeUser(split[1]);
                    break;
                case "Whois":
                    bot.whoisUser(split[1]);
                    break;
                case "Kick":
                    bot.kickUser(split[1]);
                    break;
            } 
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                InterfaceUserList.SelectedIndex = InterfaceUserList.IndexFromPoint(e.Location);
                if (InterfaceUserList.SelectedIndex != -1)
                {
                    contextMenuUserList.Show();
                }
            }
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            contextMenuUserList.Items.Clear();
        }

        public void releaseCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = releaseChecker.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (Settings.Default.releaseEnabled)
                {
                    string message = privmsg(HOME_CHANNEL, "I'm now checking " + Settings.Default.baseURL + Settings.Default.chapterNumber + " for the chapter every " + Settings.Default.checkInterval + " seconds.");
                    mangaReleaseTimer.Interval = Settings.Default.checkInterval * 1000;
                    mangaReleaseTimer.Elapsed += new ElapsedEventHandler(isMangaOutEvent);
                    mangaReleaseTimer.Enabled = true;
                    client.messageSender(message);
                }
                else
                {
                    mangaReleaseTimer.Enabled = false;
                }
            }
        }

        private void t30_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 30;
            t30.Checked = true;
            t45.Checked = false;
            t60.Checked = false;

            randomTextTimer.Interval = Settings.Default.randomTextInterval * 60 * 1000;
        }

        private void t45_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 45;
            t30.Checked = false;
            t45.Checked = true;
            t60.Checked = false;

            randomTextTimer.Interval = Settings.Default.randomTextInterval * 60 * 1000;
        }

        private void t60_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 60;
            t30.Checked = false;
            t45.Checked = false;
            t60.Checked = true;

            randomTextTimer.Interval = Settings.Default.randomTextInterval * 60 * 1000;
        }


        public string privmsg(string destinatary, string message)
        {
            string result;

            result = "PRIVMSG " + destinatary + " :" + message + "\r\n";

            if (client.NICK.Length > 15)
                WriteMessage(client.NICK.Truncate(16) + ":" + message);
            else if (NICK.Length >= 8)                       //Write the Message on the bot console
                WriteMessage(client.NICK + "\t: " + message);
            else
                WriteMessage(client.NICK + "\t\t: " + message);

            return result;
        }

        private void userJoined(string whoJoined)
        {
            WriteMessage("** " + whoJoined + " joined", Color.Green);
            UpdateDataSource();
        }

        private void userLeft(string whoLeft)
        {
            WriteMessage("** " + whoLeft + " parted", Color.Red);
            UpdateDataSource();
        }
        private void userNickChange(string whoJoined, string newNick)
        {
            WriteMessage("** " + whoJoined + " is now known as " + newNick, Color.Yellow);
            UpdateDataSource();
        }

        private void userModeChanged(string user, string mode)
        {
            switch (mode)
            {
                case ("+o"):
                    WriteMessage("** " + user + " was opped", Color.Blue);
                    break;
                case ("-o"):
                    WriteMessage("** " + user + " was deopped", Color.Blue);
                    break;
                case ("+v"):
                    WriteMessage("** " + user + " was voiced", Color.Blue);
                    break;
                case ("-v"):
                    WriteMessage("** " + user + " was devoiced", Color.Blue);
                    break;
                case ("+h"):
                    WriteMessage("** " + user + " was half opped", Color.Blue);
                    break;
                case ("-h"):
                    WriteMessage("** " + user + " was half deopped", Color.Blue);
                    break;
                case ("+q"):
                    WriteMessage("** " + user + " was given Owner permissions", Color.Blue);
                    break;
                case ("-q"):
                    WriteMessage("** " + user + " was removed as a Owner", Color.Blue);
                    break;
                case ("+a"):
                    WriteMessage("** " + user + " was given Admin permissions", Color.Blue);
                    break;
                case ("-a"):
                    WriteMessage("** " + user + " was removed as an Admin", Color.Blue);
                    break;
            }

            UpdateDataSource();
        }

        private void userKicked(string userkicked)
        {
            WriteMessage("** " + userkicked + " was kicked", Color.Red);
            UpdateDataSource();
        }

        private void nowConnected(object sender, EventArgs e)
        {
            ChangeConnectingLabel("Connected");
            client.Join();
            ChangeTitle(NICK + " @ " + HOME_CHANNEL + " - " + HOST + ":" + PORT);
        }
        private void nowConnectedWithServer(object sender, EventArgs e)
        {
            ChangeTitle(NICK + " @ " + HOME_CHANNEL + " - " + HOST + ":" + PORT + " (" + client.HOST_SERVER + ")");
        }

        private void userListCreated(object sender, EventArgs e)
        {
            UpdateDataSource();
        }
        public void randomTextSender(object source, ElapsedEventArgs e)
        {
            bot.randomTextSender(source, e);
        }

        public void eventChangeTitle(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(client.HOST_SERVER))
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT + " (" + client.HOST_SERVER + ")");
            else
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT);
        }

        public void letsQuit(object sender, EventArgs e)
        {
            disconnectClient();
        }

        public bool changeNick(string nick)
        {
            client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();

            if (!String.IsNullOrEmpty(client.HOST_SERVER))
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT + " (" + client.HOST_SERVER + ")");
            else
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT);

            //do Nick change to server
            if (client.isConnected)
            {
                client.messageSender("NICK " + client.NICK + "\n");
                return true;
            }

            return false;
        }

        public void duplicatedNick(object sender, EventArgs e)
        {
            Random r = new Random();

            disconnectClient();

            Settings.Default.Nick = client.NICK + r.Next(10);
            Settings.Default.Save();

            if (connect())  //If connected with success, then start the bot
                backgroundWorker.RunWorkerAsync();
        }

        private void pingServer(object sender, EventArgs e)
        {
            bot.pingSever();
        }

        private void updateLag(TimeSpan diff)
        {
            try
            {
                int seconds = diff.Seconds * 60 + diff.Seconds;
                toolstripLag.Text = seconds + "." + diff.Milliseconds.ToString("000") +"s";
            }
            catch { }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutbox.ShowDialog();
        }

    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            if (box == null) return;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

    static class getCompilationDate
    {
        static public DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
    }

}
