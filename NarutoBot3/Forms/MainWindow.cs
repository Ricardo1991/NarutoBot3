using IrcClient.Messages;
using NarutoBot3.Events;
using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MainWindow : Form
    {
        private Bot bot;
        private ThemeCollection themes = new ThemeCollection();

        private int lastCommandIndex = 0;
        private List<string> lastCommand = new List<string>();

        private delegate void ChangeDataSource();
        private delegate void ChangeTimeStamp(object sender, PongEventArgs e);
        private delegate void SetBoolCallback(bool status);
        private delegate void SetEventCallback(object sender, TopicChangedEventArgs e);
        private delegate void SetTextCallback(string text);


        public MainWindow()
        {
            InitializeComponent();

            LoadSettings();

            string[] args = Environment.GetCommandLineArgs();
            foreach (string s in args)
            {
                if (s.ToLower().CompareTo("skip") == 0)
                {
                    TryToConnect();
                    return;
                }
            }

            //Show ConnectWindow Form and try to connect
            ConnectWindow connectWindow = new ConnectWindow(false);

            if (connectWindow.ShowDialog() == DialogResult.OK)
            {
                UpdateSilenceMarks();
                TryToConnect();
            }
        }

        /// <summary>
        /// Change status label
        /// </summary>
        /// <param name="message">Text to display on status label</param>
        public void ChangeConnectingLabel(string message)
        {
            try
            {
                l_Status.Text = message;
            }
            catch { }
        }

        /// <summary>
        /// Change text in inputbox
        /// </summary>
        /// <param name="text"></param>
        public void ChangeInput(string text)
        {
            if (InputBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeInput);
                Invoke(d, new object[] { text });
            }
            else
            {
                InputBox.Text = text;
                InputBox.SelectionStart = InputBox.Text.Length;    //Set the current caret position at the end
                InputBox.ScrollToCaret();                          //Now scroll it automatically
            }
        }

        public bool ChangeNick(string nick)
        {
            //TODO: actually check if the nick change was accepted
            if (!string.IsNullOrEmpty(bot.Client.HOST_SERVER))
                ChangeTitle(nick + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
            else
                ChangeTitle(nick + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);

            //do Nick change to server
            if (bot.Client.isConnected)
            {
                bot.ChangeNick(nick);
                return true;
            }

            return false;
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

        public void DuplicatedNick(object sender, EventArgs e)
        {
            Random r = new Random();

            if (!bot.Client.isConnected)
            {
                DisconnectClient();

                Settings.Default.Nick = bot.Client.NICK + r.Next(10);
                Settings.Default.Save();

                TryToConnect();
            }
        }

        public void EventChangeTitle(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(bot.Client.HOST_SERVER))
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
            else
                ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);
        }

        public void ExitApplication(object sender, EventArgs e)
        {
            ExitApplication();
        }

        /// <summary>
        /// Update the bot UI and settings based on the available API keys and other settings
        /// </summary>
        public void LoadSettings()
        {
            UpdateSilenceMarks();
            CheckTwitterApi();
            CheckGoogleApi();

            ApplyTheme(Settings.Default.themeName);

            if (Settings.Default.malPass.Length < 2 || Settings.Default.malUser.Length < 2)
                Settings.Default.aniSearchEnabled = false;

            if (string.IsNullOrEmpty(Settings.Default.redditUser) || string.IsNullOrEmpty(Settings.Default.redditPass))
                Settings.Default.redditEnabled = false;

            forceMirrorModeOffToolStripMenuItem.Checked = Settings.Default.enforceMirrorOff;


            Settings.Default.Save();
        }

        /// <summary>
        /// Clears the UI console (chat window)
        /// </summary>
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

        /// <summary>
        /// Reapplies the current theme
        /// </summary>
        public void RefreshTheme(object sender, EventArgs e)
        {
            RefreshTheme();
        }

        /// <summary>
        /// Reapplies the current theme
        /// </summary>
        public void RefreshTheme()
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
                bot.UpdateTheme(themes.CurrentColorScheme);
        }

        public void TryToConnect()
        {
            ChangeConnectingLabel("Connecting...");

            bot = new Bot(ref OutputBox);
            bot.UpdateTheme(themes.CurrentColorScheme);
            InitializeBotEvents();

            try
            {
                bot.Connect(); 
            }

            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ChangeConnectingLabel("Disconnected");
            }

        }
        public void UpdateDataSource(object source, EventArgs e)
        {
            UpdateDataSource();
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
                List<User> ul = bot.userlist.GetAllOnlineUsers();
                ul.Sort();
                InterfaceUserList.DataSource = ul;

                List<string> temp = new List<string>();

                foreach (User s in ul)
                {
                    temp.Add(s.Nick);
                }

                InputBox.Values = temp.ToArray();
            }
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutbox = new AboutBox();
            aboutbox.ShowDialog();
        }

        private void AllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
            {
                bot.StringLib.ReloadLibrary();
            }
        }

        /// <summary>
        /// Applies and refreshes the UI theme
        /// </summary>
        /// <param name="themeName">Name of the theme to apply</param>
        private void ApplyTheme(string themeName)
        {
            themes.SelectTheme(themes.GetThemeByName(themeName));
            RefreshTheme();
        }

        private void BotSilence(object sender, EventArgs e)
        {
            ChangeSilenceCheckBox(true);
            Settings.Default.silence = true;
            ChangeSilenceLabel(true);
            Settings.Default.Save();
        }

        private void BotUnsilence(object sender, EventArgs e)
        {
            ChangeSilenceCheckBox(false);
            Settings.Default.silence = false;
            ChangeSilenceLabel(false);

            Settings.Default.Save();
        }

        private void ChangeNickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBotNickWindow nickWindow = new ChangeBotNickWindow();
            nickWindow.ShowDialog();

            if (bot != null)
                bot.ChangeNick(Settings.Default.Nick);
        }

        private void ChangeTopicTextBox(object sender, TopicChangedEventArgs e)
        {
            if (tbTopic.InvokeRequired)
            {
                SetEventCallback d = new SetEventCallback(ChangeTopicTextBox);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                tbTopic.Text = e.Topic;
            }
        }

        /// <summary>
        /// Disables the google functionalities if the API keys are not provided
        /// </summary>
        private void CheckGoogleApi()
        {
            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
                Settings.Default.aniSearchEnabled = false;

            if (Settings.Default.apikey.Length < 5)
            {
                Settings.Default.timeEnabled = false;
                Settings.Default.youtubeSearchEnabled = false;
            }
        }

        /// <summary>
        /// Disables the twitter functionality is the API keys are not provided
        /// </summary>
        private void CheckTwitterApi()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.twitterAccessToken) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterAccessTokenSecret) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKey) ||
                    string.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKeySecret))
                Settings.Default.twitterEnabled = false;
        }

        private void ConnectMenuItem1_Click(object sender, EventArgs e) //Connect to...
        {
            ConnectWindow connectWindow = new ConnectWindow(bot!= null && bot.Client != null && bot.Client.isConnected);

            if (connectWindow.ShowDialog() == DialogResult.OK)
            {
                if (bot.Client != null && bot.Client.isConnected)
                        DisconnectClient();

                TryToConnect();
            }
        }

        private void ContextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            contextMenuUserList.Items.Clear();
        }

        /// <summary>
        /// Creates the context menu to be shown when user right clicks the userlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuUserList.Items.Clear();
            if (InterfaceUserList.SelectedIndex == -1) return;

            string nick = Useful.RemoveUserMode(InterfaceUserList.SelectedItem.ToString());

            contextMenuUserList.Items.Add(nick);
            contextMenuUserList.Items.Add(new ToolStripSeparator());

            if (bot != null && !bot.userlist.UserIsOperator(nick))
                contextMenuUserList.Items.Add("Give Bot Permissions", null, new EventHandler(delegate (object o, EventArgs a) { bot.GiveOps(nick); }));
            else
                contextMenuUserList.Items.Add("Remove Bot Permissions", null, new EventHandler(delegate (object o, EventArgs a) { bot.RemoveBotOperatorStatus(nick); }));

            if (bot != null && !bot.userlist.UserIsMuted(nick))
                contextMenuUserList.Items.Add("Ignore", null, new EventHandler(delegate (object o, EventArgs a) { bot.MuteUser(nick); }));
            else
                contextMenuUserList.Items.Add("Stop Ignoring", null, new EventHandler(delegate (object o, EventArgs a) { bot.UnmuteUser(nick); }));

            contextMenuUserList.Items.Add(new ToolStripSeparator());
            contextMenuUserList.Items.Add("Poke", null, new EventHandler(delegate (object o, EventArgs a) { bot.PokeUser(nick); }));
            contextMenuUserList.Items.Add("Whois", null, new EventHandler(delegate (object o, EventArgs a) { bot.WhoisUser(nick); }));

            //Add kick option if the bot has kick permissions on channel
            if (bot.userlist.GetUserMode(bot.Client.NICK) == '@')
            {
                contextMenuUserList.Items.Add(new ToolStripSeparator());
                contextMenuUserList.Items.Add("Kick", null, new EventHandler(delegate (object o, EventArgs a) { bot.KickUser(nick); }));
            }
        }

        private void DisconnectClient()
        {
            ChangeConnectingLabel("Disconnecting...");
            OutputClean();
            ChangeTitle("NarutoBot");

            if (bot != null)
            {
                bot.Disconnect(Settings.Default.quitMessage);
            }
                
            UpdateDataSource();

            Thread.Sleep(250);
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e) //Disconnect Button
        {
            if (bot.Client.isConnected)
            {
                DisconnectClient();
            }
        }

        private void DoAutoJoinCommand()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.autojoinCommand))
            {
                ParseInputMessage(Settings.Default.autojoinCommand);
            }
        }
        private void EnforceChanged(object sender, EventArgs e)
        {
            forceMirrorModeOffToolStripMenuItem.Checked = Settings.Default.enforceMirrorOff;
        }

        private void ExitApplication()
        {

            DisconnectClient();

            ChangeConnectingLabel("Disconnected");
            Application.Exit();
            
        }

        private void FactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("facts");
        }

        private void ForceMirrorModeOffToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void FunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("funk");
        }

        private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process browser = new Process();

            string url = "https://github.com/Ricardo1991/NarutoBot3";
            browser.StartInfo.UseShellExecute = true;
            browser.StartInfo.FileName = url;
            browser.Start();

            browser.Close();
        }

        private void HelpTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpTextWindow helpWindow = new HelpTextWindow();
            helpWindow.ShowDialog();
            if (bot != null)
            {
                bot.StringLib.ReloadLibrary("help");
            }
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("help");
        }

        private void InitializeBotEvents()
        {
            bot.Connected += new EventHandler<EventArgs>(NowConnected);
            bot.ConnectedWithServer += new EventHandler<EventArgs>(NowConnectedWithServer);
            bot.Created += new EventHandler<EventArgs>(UpdateDataSource);
            bot.ModeChanged += new EventHandler<ModeChangedEventArgs>(UserModeChanged);
            bot.Timeout += new EventHandler<EventArgs>(Timeout);
            bot.BotNickChanged += new EventHandler<EventArgs>(EventChangeTitle);
            bot.BotSilenced += new EventHandler<EventArgs>(BotSilence);
            bot.BotUnsilenced += new EventHandler<EventArgs>(BotUnsilence);
            bot.Quit += new EventHandler<EventArgs>(ExitApplication);
            bot.DuplicatedNick += new EventHandler<EventArgs>(DuplicatedNick);
            bot.PongReceived += new EventHandler<PongEventArgs>(UpdateLag);
            bot.TopicChange += new EventHandler<TopicChangedEventArgs>(ChangeTopicTextBox);
            bot.EnforceMirrorChanged += new EventHandler<EventArgs>(EnforceChanged);
            bot.UpdateUserListSource += new EventHandler<EventArgs>(UpdateDataSource);
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (lastCommand.Count > 0)
                {
                    ChangeInput(lastCommand[(lastCommand.Count - 1) - lastCommandIndex]);
                    
                    if (lastCommandIndex + 1 < lastCommand.Count)
                        lastCommandIndex++;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (lastCommand.Count > 0 && lastCommandIndex > 0)
                {
                    lastCommandIndex--;
                    if (lastCommandIndex > 0)
                        ChangeInput(lastCommand[(lastCommand.Count - 1) - lastCommandIndex]);
                    else
                        ChangeInput("");
                }
            }
            else lastCommandIndex = 0;

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                lastCommand.Add(InputBox.Text);

                if (string.IsNullOrEmpty(InputBox.Text)) return;

                ParseInputMessage(InputBox.Text);

                ChangeInput("");
               
            }
        }

        /// <summary>
        /// Catches MouseDown events on the user list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void KillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("kills");
        }

        private void MutedUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MutedUsersWindow mutedWindow;

            if (bot == null)
            {
                UserList ul = Bot.GetSavedUsers();
                mutedWindow = new MutedUsersWindow(ref ul);
            }
            else
                mutedWindow = new MutedUsersWindow(ref bot.userlist);

            mutedWindow.ShowDialog();
        }

        private void NickGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("nick");
        }

        private void NowConnected(object sender, EventArgs e)
        {
            ChangeConnectingLabel("Connected");
            bot.Client.Join();
            ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT);

            DoAutoJoinCommand();
        }

        private void NowConnectedWithServer(object sender, EventArgs e)
        {
            ChangeTitle(bot.Client.NICK + " @ " + bot.Client.HOME_CHANNEL + " - " + bot.Client.HOST + ":" + bot.Client.PORT + " (" + bot.Client.HOST_SERVER + ")");
        }

        private void OperatorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BotOperatorWindow operatorsWindow;

            if (bot == null)
            {
                UserList ul = Bot.GetSavedUsers();
                operatorsWindow = new BotOperatorWindow(ref ul);
            }
            else
                operatorsWindow = new BotOperatorWindow(ref bot.userlist);

            operatorsWindow.ShowDialog();
        }

        private void Output2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process browser = new Process();
            string url = e.LinkText;
            browser.StartInfo.UseShellExecute = true;
            browser.StartInfo.FileName = url;
            browser.Start();

            browser.Close();
        }

        private void ParseInputMessage(string inputMessage)
        {
            IrcMessage message = null;

            if (!bot.Client.isConnected) return;


            if (inputMessage.StartsWith("/"))
            {
                string[] parsed = inputMessage.Split(new char[] { ' ' }, 2); //parsed[0] is the command (first word), arg is the rest
                string command = parsed[0].Substring(1);
                string arg;


                try
                {
                    arg = parsed[1];
                }
                catch (IndexOutOfRangeException ex)
                {
                    arg = String.Empty;
                    Console.Out.WriteLine(ex.Message);
                }

                switch (command)
                {
                    case "me":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        message = new ActionMessage(bot.Client.HOME_CHANNEL, arg);
                        break;

                    case "whois":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        message = new Whois(arg);
                        break;

                    case "whowas":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        message = new Whowas(arg);
                        break;

                    case "nick":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        ChangeNick(arg);
                        break;

                    case "nickserv":
                    case "ns":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        message = new Privmsg("NickServ", arg);
                        break;

                    case "chanserv":
                    case "cs":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }
                        message = new Privmsg("ChanServ", arg);
                        break;

                    case "query":
                    case "pm":
                    case "msg":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }

                        string[] msgargs = arg.Split(new char[] { ' ' }, 2);
                        if (arg.Length >= 2)
                            message = new Privmsg(msgargs[0], msgargs[1]);
                        else
                            WriteMessage("Not enough arguments");
                        break;

                    case "identify":
                        if(String.IsNullOrWhiteSpace(arg))
                        {
                            WriteMessage("Not enough arguments");
                            break;
                        }

                        message = new Privmsg("NickServ", "identify " + arg);
                        break;
                    case "clear":
                    case "clean":
                        OutputClean();
                        break;
                }  
            }

            else //Normal send
                message = new Privmsg(bot.Client.HOME_CHANNEL, InputBox.Text);

            if (message != null && !string.IsNullOrWhiteSpace(message.body)) bot.SendMessage(message);
        }

        private void QuotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("quotes");
        }

        private void RulesTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRulesWindow rulesWindow = new EditRulesWindow();
            rulesWindow.ShowDialog();
            if (bot != null)
            {
                bot.StringLib.ReloadLibrary("rules");
            }
        }

        private void RulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("rules");
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(ref themes);
            settingsWindow.ThemeChanged += new EventHandler<EventArgs>(RefreshTheme);

            settingsWindow.ShowDialog();

            if (bot != null)
            {
                CheckTwitterApi();
                CheckGoogleApi();

                if (Settings.Default.twitterEnabled)
                    bot.TwitterLogOn();

                try
                {
                    if (Settings.Default.redditUserEnabled)
                        bot.RedditLogin(Settings.Default.redditUser, Settings.Default.redditPass);
                }
                catch
                {
                }
            }
        }

        private void SilencedToolStripMenuItem_Click(object sender, EventArgs e)  //Toogle Silence
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

        private void Timeout(object sender, EventArgs e)
        {
            DisconnectClient();

            ChangeConnectingLabel("Re-Connecting...");
            WriteMessage("* The connection timed out. Will try to reconnect.");

            TryToConnect();
        }

        private void TriviaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot != null)
                bot.StringLib.ReloadLibrary("trivia");
        }

        private void UpdateLag(object sender, PongEventArgs e)
        {
            if (statusStripBottom.InvokeRequired)
            {
                ChangeTimeStamp d = new ChangeTimeStamp(UpdateLag);
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

        /// <summary>
        /// Show's or hides the UI silenced status based on settings
        /// </summary>
        private void UpdateSilenceMarks()
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
        private void UserModeChanged(object sender, ModeChangedEventArgs e)
        {
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

            if (modeChanges.TryGetValue(e.Mode, out string message))
            {
                WriteMessage("** " + e.User + " " + message, themes.CurrentColorScheme.StatusChanged);
            }

            UpdateDataSource();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplication();
        }
    }
}