using IrcClient.Messages;
using NarutoBot3.Events;
using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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

        private List<string> lastCommand = new List<string>();
        private int lastCommandIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            loadSettings();

            bot = new Bot(ref OutputBox);
            bot.updateTheme(themes.CurrentColorScheme);
            initializeBotEvents();

            //Show ConnectWindow Form and try to connect
            ConnectWindow connect = new ConnectWindow(false);

            if (connect.ShowDialog() == DialogResult.OK)
            {
                setSilenceMarks();

                if (bot.Client != null && bot.Client.isConnected)
                {
                    bot.Client.changeHomeChannel(Settings.Default.Channel);
                    bot.Client.changeHostPort(Settings.Default.Server, Settings.Default.Port);
                    bot.Client.changeNickRealName(Settings.Default.Nick, Settings.Default.RealName);

                    disconnectClient();
                    Thread.Sleep(250);
                }

                if (!this.connect())
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //
        }

        public bool connect()
        {
            ChangeConnectingLabel("Connecting...");

            return bot.connect();
        }

        private void doAutoJoinCommand()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.autojoinCommand))
            {
                parseInputMessage(Settings.Default.autojoinCommand);
            }
        }

        public void loadSettings()
        {
            setSilenceMarks();
            checkTwitterApi();
            checkGoogleApi();

            //Themes
            applyTheme(Settings.Default.themeName);

            if (Settings.Default.malPass.Length < 2 || Settings.Default.malUser.Length < 2)
                Settings.Default.aniSearchEnabled = false;

            if (string.IsNullOrEmpty(Settings.Default.redditUser) || string.IsNullOrEmpty(Settings.Default.redditPass))
                Settings.Default.redditEnabled = false;

            Settings.Default.releaseEnabled = false;

            if (Settings.Default.enforceMirrorOff)
            {
                forceMirrorModeOffToolStripMenuItem.Checked = true;
            }
            else
            {
                forceMirrorModeOffToolStripMenuItem.Checked = false;
            }

            Settings.Default.Save();
        }

        private void checkGoogleApi()
        {
            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
                Settings.Default.aniSearchEnabled = false;

            if (Settings.Default.apikey.Length < 5)
            {
                Settings.Default.timeEnabled = false;
                Settings.Default.youtubeSearchEnabled = false;
            }
        }

        private void checkTwitterApi()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.twitterAccessToken) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterAccessTokenSecret) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKey) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKeySecret))
                Settings.Default.twitterEnabled = false;
        }

        private void setSilenceMarks()
        {
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
        }

        private void applyTheme(string themeName)
        {
            themes.selectTheme(themes.getThemeByName(themeName));
            refreshTheme();
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

            bot.pingServerTimer.Elapsed += new ElapsedEventHandler(pingServer);
        }

        private void disconnectClient()
        {
            if (bot != null)
            {
                CustomCommand.saveCustomCommands(bot.customCommands);
                bot.userlist.saveData();
            }

            InterfaceUserList.DataSource = null;
            ChangeConnectingLabel("Disconnecting...");
            bot.Client.Disconnect(Settings.Default.quitMessage);

            Thread.Sleep(250);

            bot.pingServerTimer.Enabled = false;
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
                    string timeString = DateTime.Now.ToString("[HH:mm:ss]");
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

            //TODO: should make a log
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
                    string timeString = DateTime.Now.ToString("[HH:mm:ss]");
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

            //TODO: should make a log
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
            string message;
            Dictionary<string, string> modeChanges = new Dictionary<string, string>
            {
                { "+o", "was opped" },
                { "-o", "was deopped" },
                { "+v", "was voiced" },
                { "-v", "was devoiced" },
                { "+h", "was half opped" },
                { "-h", "was half deopped" },
                { "+q", "was given Owner permissions" },
                { "-q", "was removed as a Owner" },
                { "+a", "was given Admin permissions" },
                { "-a", "had their Admin permissions removed" },
            };

            if (modeChanges.TryGetValue(e.Mode, out message))
            {
                WriteMessage("** " + e.User + " " + message, themes.CurrentColorScheme.StatusChanged);
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
            bot.Client.NICK = Settings.Default.Nick = nick;
            Settings.Default.Save();

            if (!string.IsNullOrEmpty(bot.Client.HOST_SERVER))
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
            else
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);

            //do Nick change to server
            if (bot.Client.isConnected)
            {
                bot.sendMessage(new Nick(bot.Client.NICK));
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
                List<User> ul = bot.userlist.getAllOnlineUsers();
                ul.Sort();
                InterfaceUserList.DataSource = ul;

                List<string> temp = new List<string>();
                List<User> lu = bot.userlist.getAllOnlineUsers();

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

        private void connectMenuItem1_Click(object sender, EventArgs e) //Connect to...
        {
            ConnectWindow Connect = new ConnectWindow(bot.Client != null && bot.Client.isConnected);

            if (Connect.ShowDialog() == DialogResult.OK)
            {
                //Re-do Connect!
                if (bot.Client != null)
                {
                    if (bot.Client.isConnected)
                    {
                        disconnectClient();
                        Thread.Sleep(250);

                        if (!connect()) //If connected with success, then start the bot
                        {
                            MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ChangeConnectingLabel("Disconnected");
                        }
                    }
                    else
                    {
                        if (!connect())//If connected with success, then start the bot
                        {
                            MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ChangeConnectingLabel("Disconnected");
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Settings.Default.Channel)) Settings.Default.Channel = "#reddit-naruto";
                    if (string.IsNullOrWhiteSpace(Settings.Default.Server)) Settings.Default.Server = "irc.freenode.net";
                    if (string.IsNullOrWhiteSpace(Settings.Default.Nick)) Settings.Default.Nick = "NarutoBot";
                    if (Convert.ToInt32(Settings.Default.Port) <= 0 || Convert.ToInt32(Settings.Default.Port) > 65535) Settings.Default.Port = 6667.ToString();

                    if (connect()) //If connected with success, then start the bot
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
                bot.userlist.saveData();
            }

            if (bot.Client != null && bot.Client.isConnected)
            {
                disconnectClient();
            }

            Close();
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
            if (bot.Client.isConnected)
            {
                disconnectClient();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(ref themes);
            settingsWindow.ThemeChanged += new EventHandler<EventArgs>(refreshTheme);

            settingsWindow.ShowDialog();

            if (bot != null)
            {
                checkTwitterApi();
                checkGoogleApi();

                if (Settings.Default.twitterEnabled)
                    bot.TwitterLogin();

                try
                {
                    if (Settings.Default.redditUserEnabled)
                        bot.redditLogin(Settings.Default.redditUser, Settings.Default.redditPass);
                }
                catch
                {
                }
            }
        }

        private void changeNickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBotNickWindow nickWindow = new ChangeBotNickWindow();
            nickWindow.ShowDialog();

            if (bot != null)
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
                operatorsWindow = new BotOperatorWindow(ref bot.userlist);

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
            IrcMessage message = null;

            if (!bot.Client.isConnected) return;

            if (parsed.Length >= 2 && !string.IsNullOrEmpty(parsed[1]))
            {
                if (parsed[0][0] == '/')
                {
                    if (parsed[0].ToLower() == "/me")  //Action send
                        message = new IrcClient.Messages.Action(bot.Client.HOME_CHANNEL, parsed[1]);
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
                    message = new Privmsg(bot.Client.HOME_CHANNEL, InputBox.Text);
            }
            else
                if (parsed[0][0] == '/')
                WriteMessage("Not enough arguments");
            else //Normal send
                message = new Privmsg(bot.Client.HOME_CHANNEL, InputBox.Text);

            if (message != null && !string.IsNullOrWhiteSpace(message.body)) bot.sendMessage(message);
        }

        private void rulesTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRulesWindow rulesWindow = new EditRulesWindow();
            rulesWindow.ShowDialog();
            if (bot != null)
            {
                bot.ReadRules();
            }
        }

        private void helpTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpTextWindow helpWindow = new HelpTextWindow();
            helpWindow.ShowDialog();
            if (bot != null)
            {
                bot.ReadHelp();
            }
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
                mutedWindow = new MutedUsersWindow(ref bot.userlist);

            mutedWindow.ShowDialog();
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadKills();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadRules();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadHelp();
        }

        private void nickGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadNickGen();
        }

        private void triviaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadTrivia();
        }

        private void quotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadQuotes();
        }

        private void funkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadFunk();
        }

        private void factsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.ReadFacts();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
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

            if (bot != null && !bot.userlist.userIsOperator(nick))
                contextMenuUserList.Items.Add("Give Bot Ops", null, new EventHandler(delegate (object o, EventArgs a) { bot.giveOps(nick); }));
            else
                contextMenuUserList.Items.Add("Take Bot Ops", null, new EventHandler(delegate (object o, EventArgs a) { bot.takeOps(nick); }));

            if (bot != null && !bot.userlist.userIsMuted(nick))
                contextMenuUserList.Items.Add("Ignore", null, new EventHandler(delegate (object o, EventArgs a) { bot.muteUser(nick); }));
            else
                contextMenuUserList.Items.Add("Stop Ignoring", null, new EventHandler(delegate (object o, EventArgs a) { bot.unmuteUser(nick); }));

            contextMenuUserList.Items.Add(new ToolStripSeparator());

            contextMenuUserList.Items.Add("Poke", null, new EventHandler(delegate (object o, EventArgs a) { bot.pokeUser(nick); }));
            contextMenuUserList.Items.Add("Whois", null, new EventHandler(delegate (object o, EventArgs a) { bot.whoisUser(nick); }));

            if (bot.userlist.getUserMode(bot.Client.NICK) == '@')
            {
                contextMenuUserList.Items.Add(new ToolStripSeparator());
                contextMenuUserList.Items.Add("Kick", null, new EventHandler(delegate (object o, EventArgs a) { bot.kickUser(nick); }));
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

        private void nowConnected(object sender, EventArgs e)
        {
            ChangeConnectingLabel("Connected");
            bot.Client.Join();
            ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);

            doAutoJoinCommand();
        }

        private void nowConnectedWithServer(object sender, EventArgs e)
        {
            ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
        }

        private void userListCreated(object sender, EventArgs e)
        {
            UpdateDataSource();
        }

        public void eventChangeTitle(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(bot.Client.HOST_SERVER))
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
            else
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);
        }

        public void letsQuit(object sender, EventArgs e)
        {
            disconnectClient();
        }

        public void duplicatedNick(object sender, EventArgs e)
        {
            Random r = new Random();

            if (!bot.Client.isConnected)
            {
                disconnectClient();

                Settings.Default.Nick = bot.Client.NICK + r.Next(10);
                Settings.Default.Save();

                connect();
            }
        }

        private void pingServer(object sender, EventArgs e)
        {
            if (bot != null)
                bot.pingSever();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutbox = new AboutBox();
            aboutbox.ShowDialog();
        }

        public void refreshTheme(object sender, EventArgs e)
        {
            refreshTheme();
        }

        public void refreshTheme()
        {
            this.OutputBox.BackColor = themes.CurrentColorScheme.MainWindowBG;
            this.OutputBox.ForeColor = themes.CurrentColorScheme.MainWindowText;

            this.tbTopic.BackColor = themes.CurrentColorScheme.TopicBG;
            this.tbTopic.ForeColor = themes.CurrentColorScheme.TopicText;

            this.InterfaceUserList.BackColor = themes.CurrentColorScheme.UserListBG;
            this.InterfaceUserList.ForeColor = themes.CurrentColorScheme.UserListText;

            this.InputBox.BackColor = themes.CurrentColorScheme.InputBG;
            this.InputBox.ForeColor = themes.CurrentColorScheme.InputText;

            OutputBox.Clear();

            if (bot != null)
                bot.updateTheme(themes.CurrentColorScheme);
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
    }
}