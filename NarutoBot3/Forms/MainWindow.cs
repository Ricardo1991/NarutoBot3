using NarutoBot3.Properties;
using Newtonsoft.Json;
using AutoComplete;
using System;
using System.Collections.Generic;
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

        public ColorScheme currentColorScheme = new ColorScheme();
        List<ColorScheme> schemeColection = new List<ColorScheme>();

        ConnectWindow Connect = new ConnectWindow();
        ChangeBotNickWindow nickWindow = new ChangeBotNickWindow();
        EditRulesWindow rulesWindow = new EditRulesWindow();
        HelpTextWindow helpWindow = new HelpTextWindow();
        MangaReleaseCheckerWindow releaseChecker = new MangaReleaseCheckerWindow();
        AboutBox aboutbox = new AboutBox();

        SettingsWindow settingsWindow;
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

        List<String> lastCommand = new List<String>();
        int lastCommandIndex = 0;

        bool exitTheLoop = false;
        UserList uList;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            uList = new UserList();

            operatorsWindow = new BotOperatorWindow(ref uList);
            mutedWindow = new MutedUsersWindow(ref uList);
            settingsWindow = new SettingsWindow(ref currentColorScheme);

            //Events for BGWorker
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.WorkerSupportsCancellation = true;
            ///

            //Themes
            loadThemes();
            applyTheme(Settings.Default.themeName);
            settingsWindow.ThemeChanged += new EventHandler<EventArgs>(refreshTheme);
            ///

            //Show ConnectWindow Form and try to connect

            if (Connect.ShowDialog() == DialogResult.OK)
            {
                if (connect())          //If connected with success, then start the bot
                    backgroundWorker.RunWorkerAsync();
                else
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ///
        }

        public bool connect()   
        {
            ChangeConnectingLabel("Connecting...");

            loadSettings();

            client = new IRC_Client(HOME_CHANNEL, HOST, PORT, NICK, REALNAME);

            if (client.Connect())
            {
                exitTheLoop = false;
                timeoutTimer.Enabled = true;

                return true;
            }

            else
            {
                timeoutTimer.Enabled = false;

                return false;
            }
        }

        private void doAutojoinCommand()
        {
            if(!string.IsNullOrWhiteSpace(Settings.Default.autojoinCommand))
            {
                parseInputMessage(Settings.Default.autojoinCommand);
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

            if (Settings.Default.apikey.Length < 5)
            {
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

            if (Settings.Default.silence == true)
            {
                silencedToolStripMenuItem.Checked = true;
                toolStripStatusLabelSilence.Visible = true;
            }
            else
            {
                silencedToolStripMenuItem.Checked = false;
                toolStripStatusLabelSilence.Visible = false;
            }

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);
            REALNAME = Settings.Default.RealName;

            Settings.Default.Save();
        }



        private void applyTheme(string p)
        {
            foreach(ColorScheme c in schemeColection)
            {
                if (String.Compare(c.Name, p, true) == 0)
                {
                    currentColorScheme = c;

                    //Apply UI Colors
                    this.OutputBox.BackColor = currentColorScheme.MainWindowBG;
                    this.OutputBox.ForeColor = currentColorScheme.MainWindowText;

                    this.tbTopic.BackColor = currentColorScheme.TopicBG;
                    this.tbTopic.ForeColor = currentColorScheme.TopicText;

                    this.InterfaceUserList.BackColor = currentColorScheme.UserListBG;
                    this.InterfaceUserList.ForeColor = currentColorScheme.UserListText;

                    this.InputBox.BackColor = currentColorScheme.InputBG;
                    this.InputBox.ForeColor = currentColorScheme.InputText;
                    /////
                    return;
                }
            }
        }

        private bool schemeAlreadyExists(string name)
        {
            foreach (ColorScheme c in schemeColection)
            {
                if (String.Compare(c.Name, name, true) == 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        private void loadThemes()
        {
            schemeColection.Clear();
            schemeColection.Add(currentColorScheme);

            ColorScheme tmpScheme = new ColorScheme();

            string[] dirs = Directory.GetFiles(@"Theme", "*.json");

            foreach (string dir in dirs)
            {
                tmpScheme = new ColorScheme();

                TextReader stream = new StreamReader(dir);
                string json = stream.ReadToEnd();
                JsonConvert.PopulateObject(json, tmpScheme);

                stream.Close();
                if (!schemeAlreadyExists(tmpScheme.Name))
                    schemeColection.Add(tmpScheme);
                tmpScheme = null;

            }
        }

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //Main Loop
        {
            String buffer;
            String line;

            bot = new Bot(ref client, ref OutputBox, currentColorScheme);

            initializeBot();

            uList = bot.ul;

            while (!exitTheLoop)
            { 
                buffer = "";
                
                try
                {
                    buffer = client.readMessage();
                    byte[] bytes = Encoding.Default.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    bot.processMessage(line);
                }
                catch
                { }
            }
        }

        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeConnectingLabel("Disconnected");
        }

        private void initializeBot()
        {
            bot.Connected += new EventHandler<EventArgs>(nowConnected);
            bot.ConnectedWithServer += new EventHandler<EventArgs>(nowConnectedWithServer);

            bot.Created += new EventHandler<EventArgs>(userListCreated);
            bot.Joined += (sender, e) => userJoined(bot.Who, bot.JoinMessage);
            bot.Left += (sender, e) => userLeft(bot.WhoLeft, bot.QuitMessage);
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

            operatorsWindow = new BotOperatorWindow(ref bot.ul);
            mutedWindow = new MutedUsersWindow(ref bot.ul);
            settingsWindow = new SettingsWindow(ref currentColorScheme);

            settingsWindow.ThemeChanged += new EventHandler<EventArgs>(refreshTheme);

        }

        private void disconnectClient()
        {
            if(bot!=null) bot.userList.Clear();

            InterfaceUserList.DataSource = null;
            ChangeConnectingLabel("Disconnecting...");
            client.Disconnect(Settings.Default.quitMessage);

            Thread.Sleep(250);

            exitTheLoop = true;
            timeoutTimer.Enabled = false;
            UpdateDataSource();
            OutputClean();
            ChangeTitle("NarutoBot");
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
                string timeString = DateTime.Now.ToString("[hh:mm:ss]");

                if (Settings.Default.showTimeStamps)
                    this.OutputBox.AppendText(timeString + " " + message + "\n");

                else
                    this.OutputBox.AppendText(message + "\n");

                if (Settings.Default.autoScrollToBottom)
                {
                    OutputBox.SelectionStart = OutputBox.Text.Length;   //Set the current caret position at the end
                    OutputBox.ScrollToCaret();                          //Now scroll it automatically
                }
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
                string timeString = DateTime.Now.ToString("[HH:mm:ss]");

                if (Settings.Default.showTimeStamps)
                {
                    this.OutputBox.AppendText(timeString + " ");
                    this.OutputBox.AppendText(message + "\n", color);
                }

                else
                    this.OutputBox.AppendText(message + "\n", color);

                if (Settings.Default.autoScrollToBottom)
                {
                    OutputBox.SelectionStart = OutputBox.Text.Length;   //Set the current caret position at the end
                    OutputBox.ScrollToCaret();                          //Now scroll it automatically
                }

            }

            //also, should make a log
        }

        private void userJoined(string whoJoined, string joinMessage)
        {
            WriteMessage("** " + whoJoined + " (" + joinMessage + ") joined", currentColorScheme.Join);
            UpdateDataSource();
        }

        private void userLeft(string whoLeft, string quitMessage)
        {
            WriteMessage("** " + whoLeft + " parted ("+quitMessage.Trim()+")", currentColorScheme.Leave);
            UpdateDataSource();
        }
        private void userNickChange(string whoJoined, string newNick)
        {
            WriteMessage("** " + whoJoined + " is now known as " + newNick, currentColorScheme.Rename);
            UpdateDataSource();
        }

        private void userModeChanged(string user, string mode)
        {
            switch (mode)
            {
                case ("+o"):
                    WriteMessage("** " + user + " was opped", currentColorScheme.StatusChanged);
                    break;
                case ("-o"):
                    WriteMessage("** " + user + " was deopped", currentColorScheme.StatusChanged);
                    break;
                case ("+v"):
                    WriteMessage("** " + user + " was voiced", currentColorScheme.StatusChanged);
                    break;
                case ("-v"):
                    WriteMessage("** " + user + " was devoiced", currentColorScheme.StatusChanged);
                    break;
                case ("+h"):
                    WriteMessage("** " + user + " was half opped", currentColorScheme.StatusChanged);
                    break;
                case ("-h"):
                    WriteMessage("** " + user + " was half deopped", currentColorScheme.StatusChanged);
                    break;
                case ("+q"):
                    WriteMessage("** " + user + " was given Owner permissions", currentColorScheme.StatusChanged);
                    break;
                case ("-q"):
                    WriteMessage("** " + user + " was removed as a Owner", currentColorScheme.StatusChanged);
                    break;
                case ("+a"):
                    WriteMessage("** " + user + " was given Admin permissions", currentColorScheme.StatusChanged);
                    break;
                case ("-a"):
                    WriteMessage("** " + user + " was removed as an Admin", currentColorScheme.StatusChanged);
                    break;
            }

            UpdateDataSource();
        }

        private void userKicked(string userkicked)
        {
            WriteMessage("** " + userkicked + " was kicked", currentColorScheme.Leave);
            UpdateDataSource();
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
                client.sendMessage("NICK " + client.NICK + "\n");
                return true;
            }

            return false;
        }

        public void ChangeConnectingLabel(String message)
        {
            try
            {
                l_Status.Text = message;
            }
            catch { }
        }

        public void ChangeSilenceLabel(bool status)
        {
            if (statusStripBottom.InvokeRequired)
            {
                SetBoolCallback d = new SetBoolCallback(ChangeSilenceLabel);
                this.Invoke(d, new object[] { status });
            }
            else
            {
                toolStripStatusLabelSilence.Visible = status;

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

                List<string> temp = new List<string>();

                foreach (string s in bot.userList.ToArray())
                {
                    temp.Add(Bot.removeUserMode(s));
                }

                InputBox.Values = temp.ToArray();
            }
        }

        private void updateLag(TimeSpan diff)
        {
            try
            {
                int seconds = diff.Seconds * 60 + diff.Seconds;
                toolstripLag.Text = seconds + "." + diff.Milliseconds.ToString("000") + "s";
            }
            catch { }
        }

        ////// Events

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

                message = bot.Privmsg(client.HOME_CHANNEL, "*");
                client.sendMessage(message);
                message = bot.Privmsg(client.HOME_CHANNEL, "\x02" + "\x030,4Chapter " + Settings.Default.chapterNumber.ToString() + " appears to be out! \x030,4" + url + " [I'm a bot, so i can be wrong!]" + "\x02");
                client.sendMessage(message);
                message = bot.Privmsg(client.HOME_CHANNEL, "*");
                client.sendMessage(message);

                Settings.Default.releaseEnabled = false;
                Settings.Default.Save();

                mangaReleaseTimer.Enabled = false;
            }
        }

        private void output2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process browser = new Process();
            string url = e.LinkText;
            browser.StartInfo.UseShellExecute = true;
            browser.StartInfo.FileName = url;
            browser.Start();

            browser.Close();
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadKills();
        }

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
            if(bot!=null) bot.ul.saveData();

            if (client != null && client.isConnected) 
                disconnectClient();

            this.Close();
        }

        private void silencedToolStripMenuItem_Click(object sender, EventArgs e)  //Toogle Silence
        {
            if (Settings.Default.silence == true)
            {
                silencedToolStripMenuItem.Checked = false;
                Settings.Default.silence = false;
                toolStripStatusLabelSilence.Visible = false;
            }
            else
            {
                silencedToolStripMenuItem.Checked = true;
                Settings.Default.silence = true;
                toolStripStatusLabelSilence.Visible = true;
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

            if (Settings.Default.twitterEnabled) bot.TwitterLogin();

            try
            {
                if (Settings.Default.redditEnabled) bot.redditLogin(Settings.Default.redditUser, Settings.Default.redditPass);
                
            }
            catch {
                Settings.Default.redditEnabled = false;
                Settings.Default.Save();
            }

                
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
                if (lastCommand.Count > 0)
                {
                    InputBox.Text = lastCommand[(lastCommand.Count - 1) - lastCommandIndex];
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    InputBox.SelectionStart = InputBox.Text.Length;    //Set the current caret position at the end
                    InputBox.ScrollToCaret();                          //Now scroll it automatically

                    if (lastCommandIndex + 1 < lastCommand.Count)
                        lastCommandIndex++;
                }


            }
            else if (e.KeyCode == Keys.Down)
            {
                if (lastCommand.Count > 0 && lastCommandIndex > 0)
                {
                    lastCommandIndex--;
                    if (lastCommandIndex > 0)
                        InputBox.Text = lastCommand[(lastCommand.Count - 1) - lastCommandIndex];
                    else
                        InputBox.Text = "";

                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    InputBox.SelectionStart = InputBox.Text.Length;    //Set the current caret position at the end
                    InputBox.ScrollToCaret();                          //Now scroll it automatically

                }


            }
            else lastCommandIndex = 0;

            if (e.KeyCode == Keys.Enter)
            {
                lastCommand.Add(InputBox.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (String.IsNullOrEmpty(InputBox.Text)) return;

                parseInputMessage(InputBox.Text);

                ChangeInput("");
            }
        }

        private void parseInputMessage(string inmessage)
        {
            string[] parsed = inmessage.Split(new char[] { ' ' }, 2); //parsed[0] is the command (first word), parsed[1] is the rest    
            string message = "";

            if (!client.isConnected) return;
            
            if (parsed.Length >= 2 && !String.IsNullOrEmpty(parsed[1]))
            {
                if (parsed[0][0] == '/')
                {
                    if (parsed[0].ToLower() == "/me")  //Action send
                        message = bot.Privmsg(HOME_CHANNEL, "\x01" + "ACTION " + parsed[1] + "\x01");

                    else if (parsed[0].ToLower() == "/whois")  //Action send
                        message = "WHOIS " + parsed[1] + "\n";

                    else if (parsed[0].ToLower() == "/whowas")  //Action send
                        message = "WHOWAS " + parsed[1] + "\n";

                    else if (parsed[0].ToLower() == "/nick")  //Action send
                        changeNick(parsed[1]);

                    else if (parsed[0].ToLower() == "/ns" || parsed[0].ToLower() == "/nickserv")  //NickServ send
                        message = bot.Privmsg("NickServ", parsed[1]);

                    else if (parsed[0].ToLower() == "/cs" || parsed[0].ToLower() == "/chanserv")  //Chanserv send
                        message = bot.Privmsg("ChanServ", parsed[1]);

                    else if (parsed[0].ToLower() == "/query" || parsed[0].ToLower() == "/pm" || parsed[0].ToLower() == "/msg")  //Action send
                    {
                        parsed = InputBox.Text.Split(new char[] { ' ' }, 3);
                        if (parsed.Length >= 3)
                            message = bot.Privmsg(parsed[1], parsed[2]);
                        else
                            WriteMessage("Not enough arguments");
                    }
                    else if (parsed[0].ToLower() == "/identify")
                        message = bot.Privmsg("NickServ", "identify " + parsed[1]);
                }

                else //Normal send
                    message = bot.Privmsg(HOME_CHANNEL, InputBox.Text);
            }
            else
                if (parsed[0][0] == '/')
                    WriteMessage("Not enough arguments");

                else //Normal send
                    message = bot.Privmsg(HOME_CHANNEL, InputBox.Text);

            if (!String.IsNullOrWhiteSpace(message)) client.sendMessage(message);
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
            ChangeSilenceLabel(true);
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
            ChangeSilenceLabel(false);

            Settings.Default.Save();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (InterfaceUserList.SelectedIndex == -1) return;

            contextMenuUserList.Items.Clear();
            string nick = Bot.removeUserMode(InterfaceUserList.SelectedItem.ToString());

            contextMenuUserList.Items.Add(nick);

            contextMenuUserList.Items.Add(new ToolStripSeparator());

            if (!uList.userIsOperator(nick))
                contextMenuUserList.Items.Add("Give Bot Ops", null, new EventHandler(delegate(Object o, EventArgs a) { bot.giveOps(nick); }));
            else
                contextMenuUserList.Items.Add("Take Bot Ops", null, new EventHandler(delegate(Object o, EventArgs a) { bot.takeOps(nick); }));

            if (!uList.userIsMuted(nick))
                contextMenuUserList.Items.Add("Ignore", null, new EventHandler(delegate(Object o, EventArgs a) { bot.muteUser(nick); }));
            else
                contextMenuUserList.Items.Add("Stop Ignoring", null, new EventHandler(delegate(Object o, EventArgs a) { bot.unmuteUser(nick); }));


            contextMenuUserList.Items.Add(new ToolStripSeparator());

            contextMenuUserList.Items.Add("Poke", null, new EventHandler(delegate(Object o, EventArgs a) { bot.pokeUser(nick); }));
            contextMenuUserList.Items.Add("Whois", null, new EventHandler(delegate(Object o, EventArgs a) { bot.whoisUser(nick); }));

            if (Bot.getUserMode(NICK, bot.userList) == '@') { 
                contextMenuUserList.Items.Add(new ToolStripSeparator());
                contextMenuUserList.Items.Add("Kick", null, new EventHandler(delegate(Object o, EventArgs a) { bot.kickUser(nick); }));
            }
        }


        private void InterfaceUserList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                InterfaceUserList.SelectedIndex = InterfaceUserList.IndexFromPoint(e.Location);
                if (InterfaceUserList != null && InterfaceUserList.SelectedIndex != -1)
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
                    string message = bot.Privmsg(HOME_CHANNEL, "I'm now checking " + Settings.Default.baseURL + Settings.Default.chapterNumber + " for the chapter every " + Settings.Default.checkInterval + " seconds.");
                    mangaReleaseTimer.Interval = Settings.Default.checkInterval * 1000;
                    mangaReleaseTimer.Elapsed += new ElapsedEventHandler(isMangaOutEvent);
                    mangaReleaseTimer.Enabled = true;
                    client.sendMessage(message);
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

        private void nowConnected(object sender, EventArgs e)
        {
            ChangeConnectingLabel("Connected");
            client.Join();
            ChangeTitle(NICK + " @ " + HOME_CHANNEL + " - " + HOST + ":" + PORT);

            doAutojoinCommand();
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

        public void duplicatedNick(object sender, EventArgs e)
        {
            Random r = new Random();

            if (!client.isConnected)
            {
                disconnectClient();

                Settings.Default.Nick = client.NICK + r.Next(10);
                Settings.Default.Save();

                if (connect())  //If connected with success, then start the bot
                    backgroundWorker.RunWorkerAsync();
            }
            
        }

        private void pingServer(object sender, EventArgs e)
        {
            bot.pingSever();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutbox.ShowDialog();
        }

        public void refreshTheme(object sender, EventArgs e){

            currentColorScheme = settingsWindow.currentColorScheme;
            this.OutputBox.BackColor = currentColorScheme.MainWindowBG;
            this.OutputBox.ForeColor = currentColorScheme.MainWindowText;

            this.tbTopic.BackColor = currentColorScheme.TopicBG;
            this.tbTopic.ForeColor = currentColorScheme.TopicText;

            this.InterfaceUserList.BackColor = currentColorScheme.UserListBG;
            this.InterfaceUserList.ForeColor = currentColorScheme.UserListText;

            this.InputBox.BackColor = currentColorScheme.InputBG;
            this.InputBox.ForeColor = currentColorScheme.InputText;

            OutputBox.Clear();

            if(bot != null)
                bot.updateTheme(currentColorScheme);
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process browser = new Process();

            string url = "https://github.com/Ricardo1991/NarutoBot3";
            browser.StartInfo.UseShellExecute = true;
            browser.StartInfo.FileName = url;
            browser.Start();

            browser.Close();
        }

        private void quotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadQuotes();
        }

        private void funkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadFunk();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadFunk();
            bot.ReadQuotes();
            bot.ReadKills();
            bot.ReadTrivia();
            bot.ReadNickGen();
            bot.ReadHelp();
            bot.ReadRules();
        }

    }
}
