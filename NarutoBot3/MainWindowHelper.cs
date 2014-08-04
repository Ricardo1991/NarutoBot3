using NarutoBot3.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MainWindow
    {
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
            if (statusStrip1.InvokeRequired)
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

            if (toolStrip1.InvokeRequired)
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
            if (output2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeInput);
                this.Invoke(d, new object[] { title });
            }
            else
            {
                this.input.Text = title;
            }
        }
        public void WriteMessage(String message) //Writes message on the TextBox (bot console)
        {
            if (output2.InvokeRequired)
            {
                try
                {
                    MethodInvoker invoker = () => WriteMessage(message);
                    Invoke(invoker);
                    //SetTextCallback d = new SetTextCallback(WriteMessage);
                    //this.Invoke(d, new object[] { message });
                }
                catch { }
            }
            else
            {
                this.output2.AppendText(message + "\n");
            }

            //also, should make a log

        }
        public void WriteMessage(String message, Color color) //Writes message on the TextBox (bot console)
        {
            if (output2.InvokeRequired)
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
                this.output2.AppendText(message + "\n", color);
            }

            //also, should make a log

        }
        public void OutputClear(string bah)
        {
            if (output2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(OutputClear);
                this.Invoke(d, new object[] { " " });
            }
            else
            {
                var lines = this.output2.Lines;
                var newLines = lines.Skip(1);

                this.output2.Lines = newLines.ToArray();


                //this.output.Clear();
            }

        }
        public void UpdateDataSource()
        {
            if (listBox1.InvokeRequired)
            {
                ChangeDataSource d = new ChangeDataSource(UpdateDataSource);
                this.Invoke(d);

            }
            else
            {
                listBox1.DataSource = null;
                listBox1.DataSource = client.userList;

            }
        }


        private void contextParse(string text)
        {
            string[] split = text.Split(' ');
            switch (split[0])
            {
                case "Give":
                    ircBot.giveOps(split[1]);
                    ircBot.SaveOPS();
                    break;
                case "Take":
                    ircBot.takeOps(split[1]);
                    ircBot.SaveOPS();
                    break;
                case "Mute":
                    ircBot.muteUser(split[1]);
                    ircBot.SaveBAN();
                    break;
                case "Unmute":
                    ircBot.unmuteUSer(split[1]);
                    ircBot.SaveBAN();
                    break;
                case "Poke":
                    ircBot.pokeUser(split[1]);
                    ircBot.SaveBAN();
                    break;
                case "Whois":
                    ircBot.whoisUser(split[1]);
                    ircBot.SaveBAN();
                    break;
            }
            ircBot.SaveOPS();

        }


        //UI Events

        private void connectMenuItem1_Click(object sender, EventArgs e)//Connect to...
        {
            var result = Connect.ShowDialog();

            if (result == DialogResult.OK)
            {
                //Re-do Connect!
                if (client.isConnected)
                {
                    DialogResult resultWarning;
                    resultWarning = MessageBox.Show("This bot is already connected./nDo you want to end the current connection?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        disconnect();

                        ChangeConnectingLabel("Connecting...");

                        HOME_CHANNEL = Settings.Default.Channel;
                        HOST = Settings.Default.Server;
                        NICK = Settings.Default.Nick;
                        PORT = Convert.ToInt32(Settings.Default.Port);

                        connect();
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else return;
                }
                ChangeConnectingLabel("Connecting...");

                HOME_CHANNEL = Settings.Default.Channel;
                HOST = Settings.Default.Server;
                NICK = Settings.Default.Nick;
                PORT = Convert.ToInt32(Settings.Default.Port);

                connect();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) //Quit Button
        {
            if (client.isConnected) disconnect();
            
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
                disconnect();
            ChangeConnectingLabel("Disconnected");
            //isConnected = false;
        }

        private void commandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableCommandsWindow.ShowDialog();
        }

        private void changeNickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nickWindow.ShowDialog();
            NICK = Settings.Default.Nick;
            ChangeTitle(NICK + " @ " + HOME_CHANNEL + " - " + HOST + ":" + PORT);
            //do nick change to server
            if (client.isConnected)
            {
                string message = "NICK " + NICK + "\n";
                client.messageSender(message);

            }
        }

        private void operatorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            operatorsWindow.ShowDialog();
            ircBot.readOPS();
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                input.Text = lastCommand;
            }

            if (e.KeyCode == Keys.Enter)
            {
                lastCommand = input.Text;
                e.Handled = true;
                e.SuppressKeyPress = true;

                string message="";

                if (!client.isConnected) return;
                if (input.Text == "") return;

                char[] param = new char[1];
                param[0] = ' ';
                string[] parsed = input.Text.Split(param, 2); //parsed[0] is the command, parsed[1] is the rest              

                if (parsed.Length >= 2)
                {
                    parsed[0] = parsed[0].ToLower();
                    if (parsed[0] == "/me" && parsed[1] != "")  //Action send
                    {
                        message = privmsg(HOME_CHANNEL, "\x01" + "ACTION " + parsed[1] + "\x01");
                    }
                    else
                        if (parsed[0] == "/whois" && parsed[1] != "")  //Action send
                        {
                            message = "WHOIS " + parsed[1] + "\n";
                        }
                        else
                            if (parsed[0] == "/whowas" && parsed[1] != "")  //Action send
                            {
                                message = "WHOWAS " + parsed[1] + "\n";
                            }
                            else
                                if (parsed[0] == "/nick" && parsed[1] != "")  //Action send
                                {
                                    changeNick(parsed[1]);
                                }
                                else
                                    if (parsed[0] == "/query" || parsed[0] == "/pm" && parsed[1] != "" || parsed[0] == "/msg" && parsed[1] != "")  //Action send
                                    {

                                        string[] parsed2 = input.Text.Split(param, 3);
                                        if (parsed2.Length >= 3)
                                            message = privmsg(parsed2[1], parsed2[2]);
                                    }
                                    else
                                        if (parsed[0] == "/ns" || parsed[0] == "/nickserv" && parsed[1] != "")  //NickServ send
                                        {
                                            message = privmsg("NickServ", parsed[1]);
                                        }
                                        else
                                            if (parsed[0] == "/cs" || parsed[0] == "/chanserv" && parsed[1] != "")  //Chanserv send
                                            {
                                                message = privmsg("ChanServ", parsed[1]);
                                            }
                                            else
                                            {
                                                //Normal send
                                                message = privmsg(HOME_CHANNEL, input.Text);

                                            }
                }
                else
                    if (parsed[0] == "/me" || parsed[0] == "/whois" || parsed[0] == "/whowas" || 
                        parsed[0] == "/query" || parsed[0] == "/pm" || parsed[0] == "/msg" ||
                         parsed[0] == "/ns" || parsed[0] == "/nickserv" || parsed[0] == "/cs" || parsed[0] == "/chanserv")  //Action send
                    {
                        WriteMessage("Not enough arguments");
                    }
                    else
                    {
                        //Normal send
                        message = privmsg(HOME_CHANNEL, input.Text);
                    }


                client.messageSender(message);
                message = "";
                ChangeInput("");

            }
        }

        private void rulesTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rulesWindow.ShowDialog();
        }

        private void assignmentsURLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            assignmentsWindow.Show();
        }

        private void claimsURLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            claimsWindow.Show();
        }

        private void changeETAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            etaWindow.ShowDialog();
        }

        private void helpTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            helpWindow.ShowDialog();
            ircBot.readHLP();
        }

        private void mutedUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mutedWindow.ShowDialog();
            ircBot.readBAN();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ircBot.readRLS();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ircBot.readHLP();
        }

        private void nickGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ircBot.loadNickGenStrings();
        }

        private void triviaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ircBot.readTRI();
        }

        private void redditCredentialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = redditcredentials.ShowDialog();

            if (result == DialogResult.OK)
            {
                Settings.Default.redditEnabled = true;
                ircBot.user = ircBot.reddit.LogIn(Settings.Default.redditUser, Settings.Default.redditPass);
                Settings.Default.Save();
            }


        }

        private void botSilence(object sender, EventArgs e)
        {
            ChangeSilenceCheckBox(true);
            Settings.Default.silence = true;
            ChangeSilenceLabel("Bot is Silenced");
            Settings.Default.Save();
        
        
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
            contextMenuStrip1.Items.Clear();
            string nick = listBox1.SelectedItem.ToString().Replace("@", string.Empty).Replace("+", string.Empty);

            contextMenuStrip1.Items.Add("Give " + nick + " Ops");
            contextMenuStrip1.Items.Add("Take " + nick + " Ops");
            contextMenuStrip1.Items.Add("Mute " + nick);
            contextMenuStrip1.Items.Add("Unmute " + nick);
            contextMenuStrip1.Items.Add("Poke " + nick);
            contextMenuStrip1.Items.Add("Whois " + nick);
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                listBox1.SelectedIndex = listBox1.IndexFromPoint(e.Location);
                if (listBox1.SelectedIndex != -1)
                {
                    contextMenuStrip1.Show();

                }
            }
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            contextMenuStrip1.Items.Clear();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextParse(e.ClickedItem.Text);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }
        public void releaseCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = releaseChecker.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (Settings.Default.releaseEnabled)
                {
                    string message = privmsg(HOME_CHANNEL, "I'm now checking " + Settings.Default.baseURL + " for the chapter " + Settings.Default.chapterNumber + " every " + Settings.Default.checkInterval + " seconds.");
                    aTime.Interval = Settings.Default.checkInterval * 1000;
                    aTime.Elapsed += new ElapsedEventHandler(isMangaOutEvent);
                    aTime.Enabled = true;
                    client.messageSender(message);
                }
                else
                {
                    aTime.Enabled = false;
                }

            }

        }

        private void t30_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 30;
            t30.Checked = true;
            t45.Checked = false;
            t60.Checked = false;

            rTime.Interval = Settings.Default.randomTextInterval * 60 * 1000;

        }

        private void t45_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 45;
            t30.Checked = false;
            t45.Checked = true;
            t60.Checked = false;

            rTime.Interval = Settings.Default.randomTextInterval * 60 * 1000;
        }

        private void t60_Click(object sender, EventArgs e)
        {
            Settings.Default.randomTextInterval = 60;
            t30.Checked = false;
            t45.Checked = false;
            t60.Checked = true;

            rTime.Interval = Settings.Default.randomTextInterval * 60 * 1000;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            animeAPI.ShowDialog();
            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
            {
                Settings.Default.aniSearchEnabled = false;
                Settings.Default.timeEnabled = false;
                Settings.Default.Save();
            }
        }

       

        public string privmsg(string destinatary, string message)
        {
            string result;

            result = "PRIVMSG " + destinatary + " :" + message + "\r\n";

            if (NICK.Length > 15)
                WriteMessage(NICK.Truncate(16) + ":" + message);
            else if (NICK.Length >= 8)                       //Write the message on the bot console
                WriteMessage(NICK + "\t: " + message);
            else
                WriteMessage(NICK + "\t\t: " + message);

            return result;
        }

        public string notice(string destinatary, string message)
        {
            string result;

            result = "NOTICE " + destinatary + " :" + message + "\r\n";

            if (NICK.Length > 15)
                WriteMessage(NICK.Truncate(16) + ":" + message);
            else if (NICK.Length >= 8)                       //Write the message on the bot console
                WriteMessage(NICK + "\t: " + message);
            else
                WriteMessage(NICK + "\t\t: " + message);

            return result;
        }

        private void userJoined(object sender, EventArgs e, string whoJoined)
        {
            foreach (Greeting g in ircBot.greet)
            {
                if (g.Nick == whoJoined.Replace("@", string.Empty).Replace("+", string.Empty) && g.Enabled == true)
                {
                    string message = privmsg(HOME_CHANNEL, g.Greetingg);
                    client.messageSender(message);
                }
            }

            WriteMessage("** " + whoJoined + " joined", Color.Green);
            UpdateDataSource();
        }

        private void userLeft(object sender, EventArgs e, string whoLeft)
        {
            WriteMessage("** " + whoLeft + " parted", Color.Red);
            UpdateDataSource();
        }
        private void userNickChange(object sender, EventArgs e, string whoJoined, string newNick)
        {
            WriteMessage("** " + whoJoined + " is now known as " + newNick, Color.Yellow);
            UpdateDataSource();
        }

        private void userModeChanged(object sender, EventArgs e, string user, string mode)
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

        private void userKicked(object sender, EventArgs e, string userkicked)
        {
            WriteMessage("** " + userkicked + " was kicked", Color.Red);
            UpdateDataSource();
        }

        private void nowConnected(object sender, EventArgs e)
        {
            //isConnected = true;
            ChangeConnectingLabel("Connected");
            client.Join();
            ChangeTitle(NICK + " @ " + HOME_CHANNEL + " - " + HOST + ":" + PORT);
        }

        private void userListCreated(object sender, EventArgs e)
        {
            UpdateDataSource();
        }
        public void randomTextSender(object source, ElapsedEventArgs e)
        {
            ircBot.randomTextSender(source, e);
        }

        public void eventChangeTitle(object sender, EventArgs e, string returnmessage)
        {
            ChangeTitle(returnmessage);
        
        }

        public void letsQuit(object sender, EventArgs e)
        {
            exitThisShit = 1;
            
        }

        public bool changeNick(string nick)
        {
            client.NICK = Settings.Default.Nick = nick;
            ChangeTitle( client.NICK + " @ " + client.HOME_CHANNEL + " - " + client.HOST + ":" + client.PORT);
            

            //do nick change to server
            if (client.isConnected)
            {
                string message = "NICK " + client.NICK + "\n";
                client.messageSender(message);
                return true;
            }
            else return false;
        }

    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }


}

