﻿using NarutoBot3.Events;
using NarutoBot3.Messages;
using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MainWindow : Form
    {
        private delegate void SetTextCallback(string text);

        private delegate void SetEventCallback(object sender, TopicChangedEventArgs e);

        private delegate void SetBoolCallback(bool status);

        private delegate void ChangeDataSource();

        private delegate void ChangeTimeStamp(object sender, PongEventArgs e);

        public ThemeCollection themes = new ThemeCollection();

        private Bot bot;
        private IRC_Client client;

        private System.Timers.Timer mangaReleaseTimer;      //To check for manga releases
        private System.Timers.Timer randomTextTimer;        //To check for random text
        private System.Timers.Timer timeoutTimer;           //To check for connection lost

        private string HOME_CHANNEL;
        private string HOST;
        private string NICK;
        private int PORT;
        private string REALNAME;

        private List<string> lastCommand = new List<string>();
        private int lastCommandIndex = 0;

        private bool exitTheLoop = false;

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            //Events for BGWorker
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_MainBotCycle);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.WorkerSupportsCancellation = true;
            //

            //Themes
            applyTheme(Settings.Default.themeName);
            //

            //Show ConnectWindow Form and try to connect
            loadSettings();
            ConnectWindow connect = new ConnectWindow();

            if (connect.ShowDialog() == DialogResult.OK)
            {
                if (backgroundWorker.IsBusy)
                {
                    disconnectClient();
                    Thread.Sleep(250);
                    backgroundWorker.CancelAsync();
                }

                if (this.connect())          //If connected with success, then start the bot
                    backgroundWorker.RunWorkerAsync();
                else
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //
        }

        public bool connect()
        {
            ChangeConnectingLabel("Connecting...");

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);
            REALNAME = Settings.Default.RealName;

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
            if (!string.IsNullOrWhiteSpace(Settings.Default.autojoinCommand))
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

            if (string.IsNullOrEmpty(Settings.Default.redditUser) || string.IsNullOrEmpty(Settings.Default.redditPass))
                Settings.Default.redditEnabled = false;

            if (string.IsNullOrWhiteSpace(Settings.Default.twitterAccessToken) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterAccessTokenSecret) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKey) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKeySecret))
                Settings.Default.twitterEnabled = false;

            mangaReleaseTimer = new System.Timers.Timer(Settings.Default.checkInterval);
            mangaReleaseTimer.Enabled = false;

            randomTextTimer = new System.Timers.Timer(Settings.Default.randomTextInterval * 60 * 1000);
            randomTextTimer.Enabled = Settings.Default.randomTextEnabled;
            randomTextTimer.Elapsed += (sender, e) => randomTextSender(sender, e);

            if (Settings.Default.randomTextEnabled)
                randomTextTimer.Start();

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

            if (Settings.Default.enforceMirrorOff)
            {
                forceMirrorModeOffToolStripMenuItem.Checked = true;
            }
            else
            {
                forceMirrorModeOffToolStripMenuItem.Checked = false;
            }

            HOME_CHANNEL = Settings.Default.Channel;
            HOST = Settings.Default.Server;
            NICK = Settings.Default.Nick;
            PORT = Convert.ToInt32(Settings.Default.Port);
            REALNAME = Settings.Default.RealName;

            Settings.Default.Save();
        }

        private void applyTheme(string themeName)
        {
            ColorScheme currentColorScheme = themes.CurrentColorScheme;

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

        public void backgroundWorker_MainBotCycle(object sender, DoWorkEventArgs e) //Main Loop
        {
            bot = new Bot(ref client, ref OutputBox, themes.CurrentColorScheme);

            initializeBotEvents();

            while (!exitTheLoop)
            {
                string buffer = "";
                string line;

                try
                {
                    buffer = client.readMessage();

                    byte[] bytes = Encoding.UTF8.GetBytes(buffer);
                    line = Encoding.UTF8.GetString(bytes);

                    if (line.Length > 0) bot.processMessage(line);
                }
                catch
                { }
            }
        }

        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeConnectingLabel("Disconnected");
        }

        private void initializeBotEvents()
        {
            bot.Connected += new EventHandler<EventArgs>(nowConnected);
            bot.ConnectedWithServer += new EventHandler<EventArgs>(nowConnectedWithServer);

            bot.Created += new EventHandler<EventArgs>(userListCreated);

            bot.Joined += new EventHandler<UserJoinLeftMessageEventArgs>(userJoined);
            bot.Left += new EventHandler<UserJoinLeftMessageEventArgs>(userLeft);

            bot.NickChanged += new EventHandler<NickChangeEventArgs>(userNickChange);
            bot.Kicked += new EventHandler<UserKickedEventArgs>(userKicked);
            bot.ModeChanged += new EventHandler<ModeChangedEventArgs>(userModeChanged);

            bot.Timeout += new EventHandler<EventArgs>(timeout);

            bot.BotNickChanged += new EventHandler<EventArgs>(eventChangeTitle);

            bot.BotSilenced += new EventHandler<EventArgs>(botSilence);
            bot.BotUnsilenced += new EventHandler<EventArgs>(botUnsilence);

            bot.Quit += new EventHandler<EventArgs>(letsQuit);

            bot.DuplicatedNick += new EventHandler<EventArgs>(duplicatedNick);

            bot.PongReceived += new EventHandler<PongEventArgs>(updateLag);

            bot.TopicChange += new EventHandler<TopicChangedEventArgs>(changeTopicTextBox);

            bot.EnforceMirrorChanged += new EventHandler<EventArgs>(enforceChanged);
        }

        private void disconnectClient()
        {
            if (bot != null)
            {
                bot.ul.saveData();
                bot.tmc.save("textSample.xml");
            }

            CustomCommand.saveCustomCommands(bot.customCommands);

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

        public void WriteMessage(string message) //Writes Message on the TextBox (bot console)
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
                if (Settings.Default.showTimeStamps)
                {
                    string timeString = DateTime.Now.ToString("[hh:mm:ss]");
                    this.OutputBox.AppendText(timeString + " " + message + "\n");
                }
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

        public void WriteMessage(string message, Color color) //Writes Message on the TextBox (bot console)
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
                if (Settings.Default.showTimeStamps)
                {
                    string timeString = DateTime.Now.ToString("[hh:mm:ss]");
                    this.OutputBox.AppendText(timeString + " " + message + "\n", color);
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

        private void userJoined(object sender, UserJoinLeftMessageEventArgs e)
        {
            WriteMessage("** " + e.Who + " (" + e.Message + ") joined", themes.CurrentColorScheme.Join);
            UpdateDataSource();
        }

        private void userLeft(object sender, UserJoinLeftMessageEventArgs e)
        {
            WriteMessage("** " + e.Who + " parted (" + e.Message.Trim() + ")", themes.CurrentColorScheme.Leave);
            UpdateDataSource();
        }

        private void userNickChange(object sender, NickChangeEventArgs e)
        {
            WriteMessage("** " + e.OldNick + " is now known as " + e.NewNick, themes.CurrentColorScheme.Rename);
            UpdateDataSource();
        }

        private void userModeChanged(object sender, ModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case ("+o"):
                    WriteMessage("** " + e.User + " was opped", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("-o"):
                    WriteMessage("** " + e.User + " was deopped", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("+v"):
                    WriteMessage("** " + e.User + " was voiced", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("-v"):
                    WriteMessage("** " + e.User + " was devoiced", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("+h"):
                    WriteMessage("** " + e.User + " was half opped", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("-h"):
                    WriteMessage("** " + e.User + " was half deopped", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("+q"):
                    WriteMessage("** " + e.User + " was given Owner permissions", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("-q"):
                    WriteMessage("** " + e.User + " was removed as a Owner", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("+a"):
                    WriteMessage("** " + e.User + " was given Admin permissions", themes.CurrentColorScheme.StatusChanged);
                    break;

                case ("-a"):
                    WriteMessage("** " + e.User + " was removed as an Admin", themes.CurrentColorScheme.StatusChanged);
                    break;
            }

            UpdateDataSource();
        }

        private void userKicked(object sender, UserKickedEventArgs e)
        {
            WriteMessage("** " + e.KickedUser + " was kicked", themes.CurrentColorScheme.Leave);
            UpdateDataSource();
        }

        public bool changeNick(string nick)
        {
            client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();

            if (!string.IsNullOrEmpty(client.HOST_SERVER))
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT + " (" + client.HOST_SERVER + ")");
            else
                ChangeTitle(client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT);

            //do Nick change to server
            if (client.isConnected)
            {
                bot.sendMessage(new Nick(client.NICK));
                return true;
            }

            return false;
        }

        public void ChangeConnectingLabel(string message)
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

        public void ChangeTitle(string title)
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

        public void ChangeInput(string title)
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
                List<User> ul = bot.ul.getAllOnlineUsers();
                ul.Sort();
                InterfaceUserList.DataSource = ul;

                List<string> temp = new List<string>();
                List<User> lu = bot.ul.getAllOnlineUsers();

                foreach (User s in lu)
                {
                    temp.Add(s.Nick);
                }

                InputBox.Values = temp.ToArray();
            }
        }

        private void updateLag(object sender, PongEventArgs e)
        {
            if (statusStripBottom.InvokeRequired)
            {
                ChangeTimeStamp d = new ChangeTimeStamp(updateLag);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                try
                {
                    int seconds = e.TimeDifference.Seconds * 60 + e.TimeDifference.Seconds;
                    toolstripLag.Text = seconds + "." + e.TimeDifference.Milliseconds.ToString("000") + "s";
                }
                catch { }
            }
        }

        ////// Events

        private void changeTopicTextBox(object sender, TopicChangedEventArgs e)
        {
            if (tbTopic.InvokeRequired)
            {
                SetEventCallback d = new SetEventCallback(changeTopicTextBox);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                tbTopic.Text = e.Topic;
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
            ConnectWindow Connect = new ConnectWindow();

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

                    if (string.IsNullOrWhiteSpace(HOME_CHANNEL)) HOME_CHANNEL = "#reddit-naruto";
                    if (string.IsNullOrWhiteSpace(HOST)) HOST = "irc.freenode.net";
                    if (string.IsNullOrWhiteSpace(NICK)) NICK = "NarutoBot";
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
            if (bot != null)
            {
                bot.ul.saveData();
                bot.tmc.save("textSample.xml");
            }

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
            SettingsWindow settingsWindow = new SettingsWindow(ref themes);
            settingsWindow.ThemeChanged += new EventHandler<EventArgs>(refreshTheme);

            settingsWindow.ShowDialog();

            if (Settings.Default.twitterEnabled) bot.TwitterLogin();

            try
            {
                if (Settings.Default.redditUserEnabled) bot.redditLogin(Settings.Default.redditUser, Settings.Default.redditPass);
            }
            catch
            {
            }

            if (Settings.Default.randomTextEnabled)
                randomTextTimer.Start();
        }

        private void changeNickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBotNickWindow nickWindow = new ChangeBotNickWindow();
            nickWindow.ShowDialog();
            bot.changeNick(Settings.Default.Nick);
        }

        private void operatorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BotOperatorWindow operatorsWindow;

            if (bot == null)
            {
                UserList ul = Bot.getSavedUsers();
                operatorsWindow = new BotOperatorWindow(ref ul);
            }
            else
                operatorsWindow = new BotOperatorWindow(ref bot.ul);

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

                if (string.IsNullOrEmpty(InputBox.Text)) return;

                parseInputMessage(InputBox.Text);

                ChangeInput("");
            }
        }

        private void parseInputMessage(string inmessage)
        {
            string[] parsed = inmessage.Split(new char[] { ' ' }, 2); //parsed[0] is the command (first word), parsed[1] is the rest
            Message message = null;

            if (!client.isConnected) return;

            if (parsed.Length >= 2 && !string.IsNullOrEmpty(parsed[1]))
            {
                if (parsed[0][0] == '/')
                {
                    if (parsed[0].ToLower() == "/me")  //Action send
                        message = new Messages.Action(HOME_CHANNEL, parsed[1]);
                    else if (parsed[0].ToLower() == "/whois")  //Action send
                        message = new Whois(parsed[1]);
                    else if (parsed[0].ToLower() == "/whowas")  //Action send
                        message = new Whowas(parsed[1]);
                    else if (parsed[0].ToLower() == "/nick")  //Action send
                        changeNick(parsed[1]);
                    else if (parsed[0].ToLower() == "/ns" || parsed[0].ToLower() == "/nickserv")  //NickServ send
                        message = new Privmsg("NickServ", parsed[1]);
                    else if (parsed[0].ToLower() == "/cs" || parsed[0].ToLower() == "/chanserv")  //Chanserv send
                        message = new Privmsg("ChanServ", parsed[1]);
                    else if (parsed[0].ToLower() == "/query" || parsed[0].ToLower() == "/pm" || parsed[0].ToLower() == "/msg")  //Action send
                    {
                        parsed = InputBox.Text.Split(new char[] { ' ' }, 3);
                        if (parsed.Length >= 3)
                            message = new Privmsg(parsed[1], parsed[2]);
                        else
                            WriteMessage("Not enough arguments");
                    }
                    else if (parsed[0].ToLower() == "/identify")
                        message = new Privmsg("NickServ", "identify " + parsed[1]);
                }
                else //Normal send
                    message = new Privmsg(HOME_CHANNEL, InputBox.Text);
            }
            else
                if (parsed[0][0] == '/')
                WriteMessage("Not enough arguments");
            else //Normal send
                message = new Privmsg(HOME_CHANNEL, InputBox.Text);

            if (message != null && !string.IsNullOrWhiteSpace(message.body)) bot.sendMessage(message);
        }

        private void rulesTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRulesWindow rulesWindow = new EditRulesWindow();
            rulesWindow.ShowDialog();
        }

        private void helpTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpTextWindow helpWindow = new HelpTextWindow();
            helpWindow.ShowDialog();
            bot.ReadHelp();
        }

        private void mutedUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MutedUsersWindow mutedWindow;

            if (bot == null)
            {
                UserList ul = Bot.getSavedUsers();
                mutedWindow = new MutedUsersWindow(ref ul);
            }
            else
                mutedWindow = new MutedUsersWindow(ref bot.ul);

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
            else
            {
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

            if (bot != null && !bot.ul.userIsOperator(nick))
                contextMenuUserList.Items.Add("Give Bot Ops", null, new EventHandler(delegate (Object o, EventArgs a) { bot.giveOps(nick); }));
            else
                contextMenuUserList.Items.Add("Take Bot Ops", null, new EventHandler(delegate (Object o, EventArgs a) { bot.takeOps(nick); }));

            if (bot != null && !bot.ul.userIsMuted(nick))
                contextMenuUserList.Items.Add("Ignore", null, new EventHandler(delegate (Object o, EventArgs a) { bot.muteUser(nick); }));
            else
                contextMenuUserList.Items.Add("Stop Ignoring", null, new EventHandler(delegate (Object o, EventArgs a) { bot.unmuteUser(nick); }));

            contextMenuUserList.Items.Add(new ToolStripSeparator());

            contextMenuUserList.Items.Add("Poke", null, new EventHandler(delegate (Object o, EventArgs a) { bot.pokeUser(nick); }));
            contextMenuUserList.Items.Add("Whois", null, new EventHandler(delegate (Object o, EventArgs a) { bot.whoisUser(nick); }));

            if (bot.ul.getUserMode(NICK) == '@')
            {
                contextMenuUserList.Items.Add(new ToolStripSeparator());
                contextMenuUserList.Items.Add("Kick", null, new EventHandler(delegate (Object o, EventArgs a) { bot.kickUser(nick); }));
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
            if (!string.IsNullOrEmpty(client.HOST_SERVER))
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
            AboutBox aboutbox = new AboutBox();
            aboutbox.ShowDialog();
        }

        public void refreshTheme(object sender, EventArgs e)
        {
            SettingsWindow s = (SettingsWindow)sender;

            ColorScheme currentColorScheme = themes.CurrentColorScheme;
            this.OutputBox.BackColor = currentColorScheme.MainWindowBG;
            this.OutputBox.ForeColor = currentColorScheme.MainWindowText;

            this.tbTopic.BackColor = currentColorScheme.TopicBG;
            this.tbTopic.ForeColor = currentColorScheme.TopicText;

            this.InterfaceUserList.BackColor = currentColorScheme.UserListBG;
            this.InterfaceUserList.ForeColor = currentColorScheme.UserListText;

            this.InputBox.BackColor = currentColorScheme.InputBG;
            this.InputBox.ForeColor = currentColorScheme.InputText;

            OutputBox.Clear();

            if (bot != null)
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
            bot.ReadFacts();
        }

        private void forceMirrorModeOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.enforceMirrorOff == true)
            {
                forceMirrorModeOffToolStripMenuItem.Checked = false;
                Settings.Default.enforceMirrorOff = false;
            }
            else
            {
                forceMirrorModeOffToolStripMenuItem.Checked = true;
                Settings.Default.enforceMirrorOff = true;
            }
            Settings.Default.Save();
        }

        private void enforceChanged(object sender, EventArgs e)
        {
            forceMirrorModeOffToolStripMenuItem.Checked = Settings.Default.enforceMirrorOff;
        }

        private void factsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot.ReadFacts();
        }
    }
}