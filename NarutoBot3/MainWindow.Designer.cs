namespace NarutoBot3
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param Name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (ircBot != null) ircBot.Dispose();
                if (client != null) client.Dispose();

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStripBottom = new System.Windows.Forms.StatusStrip();
            this.l_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSilence = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstripLag = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNarutoBot = new System.Windows.Forms.ToolStripDropDownButton();
            this.connectMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.changeNickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.silencedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseCheckerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomTextIntervalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t30 = new System.Windows.Forms.ToolStripMenuItem();
            this.t45 = new System.Windows.Forms.ToolStripMenuItem();
            this.t60 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.redditCredentialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.twitterAPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.operatorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mutedUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonText = new System.Windows.Forms.ToolStripDropDownButton();
            this.rulesTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ColorkageURLsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.changeETAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.rulesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nickGeneratorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.triviasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.InterfaceUserList = new System.Windows.Forms.ListBox();
            this.contextMenuUserList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusStripBottom.SuspendLayout();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripBottom
            // 
            this.statusStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.l_Status,
            this.toolStripStatusLabelSilence,
            this.toolstripLag});
            this.statusStripBottom.Location = new System.Drawing.Point(0, 467);
            this.statusStripBottom.Name = "statusStripBottom";
            this.statusStripBottom.Size = new System.Drawing.Size(1015, 22);
            this.statusStripBottom.TabIndex = 0;
            this.statusStripBottom.Text = "statusStrip1";
            // 
            // l_Status
            // 
            this.l_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.l_Status.Name = "l_Status";
            this.l_Status.Size = new System.Drawing.Size(79, 17);
            this.l_Status.Text = "Disconnected";
            // 
            // toolStripStatusLabelSilence
            // 
            this.toolStripStatusLabelSilence.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelSilence.Name = "toolStripStatusLabelSilence";
            this.toolStripStatusLabelSilence.Size = new System.Drawing.Size(83, 17);
            this.toolStripStatusLabelSilence.Text = "Bot is Silenced";
            // 
            // toolstripLag
            // 
            this.toolstripLag.Name = "toolstripLag";
            this.toolstripLag.Size = new System.Drawing.Size(807, 17);
            this.toolstripLag.Spring = true;
            this.toolstripLag.Text = "0.000s";
            this.toolstripLag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNarutoBot,
            this.toolStripButtonSettings,
            this.toolStripButtonText});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1015, 25);
            this.toolStripMenu.TabIndex = 2;
            this.toolStripMenu.Text = "toolStripMenu";
            // 
            // toolStripButtonNarutoBot
            // 
            this.toolStripButtonNarutoBot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNarutoBot.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectMenuItem1,
            this.disconnectToolStripMenuItem,
            this.toolStripSeparator5,
            this.changeNickToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripButtonNarutoBot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNarutoBot.Image")));
            this.toolStripButtonNarutoBot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNarutoBot.Name = "toolStripButtonNarutoBot";
            this.toolStripButtonNarutoBot.ShowDropDownArrow = false;
            this.toolStripButtonNarutoBot.Size = new System.Drawing.Size(66, 22);
            this.toolStripButtonNarutoBot.Text = "&NarutoBot";
            // 
            // connectMenuItem1
            // 
            this.connectMenuItem1.Name = "connectMenuItem1";
            this.connectMenuItem1.Size = new System.Drawing.Size(142, 22);
            this.connectMenuItem1.Text = "&Connect...";
            this.connectMenuItem1.Click += new System.EventHandler(this.connectMenuItem1_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(139, 6);
            // 
            // changeNickToolStripMenuItem
            // 
            this.changeNickToolStripMenuItem.Name = "changeNickToolStripMenuItem";
            this.changeNickToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.changeNickToolStripMenuItem.Text = "Change &Nick";
            this.changeNickToolStripMenuItem.Click += new System.EventHandler(this.changeNickToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exitToolStripMenuItem.Text = "&Quit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.silencedToolStripMenuItem,
            this.toolStripSeparator2,
            this.commandsToolStripMenuItem,
            this.releaseCheckerToolStripMenuItem,
            this.randomTextIntervalToolStripMenuItem,
            this.toolStripSeparator6,
            this.redditCredentialsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.twitterAPIToolStripMenuItem,
            this.toolStripSeparator3,
            this.operatorsToolStripMenuItem,
            this.mutedUsersToolStripMenuItem});
            this.toolStripButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSettings.Image")));
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.ShowDropDownArrow = false;
            this.toolStripButtonSettings.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonSettings.Text = "&Settings";
            // 
            // silencedToolStripMenuItem
            // 
            this.silencedToolStripMenuItem.Checked = true;
            this.silencedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.silencedToolStripMenuItem.Name = "silencedToolStripMenuItem";
            this.silencedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.silencedToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.silencedToolStripMenuItem.Text = "&Silenced";
            this.silencedToolStripMenuItem.Click += new System.EventHandler(this.silencedToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.commandsToolStripMenuItem.Text = "Enable/Disable Commands";
            this.commandsToolStripMenuItem.Click += new System.EventHandler(this.commandsToolStripMenuItem_Click);
            // 
            // releaseCheckerToolStripMenuItem
            // 
            this.releaseCheckerToolStripMenuItem.Name = "releaseCheckerToolStripMenuItem";
            this.releaseCheckerToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.releaseCheckerToolStripMenuItem.Text = "Chapter Release Checker";
            this.releaseCheckerToolStripMenuItem.Click += new System.EventHandler(this.releaseCheckerToolStripMenuItem_Click);
            // 
            // randomTextIntervalToolStripMenuItem
            // 
            this.randomTextIntervalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.t30,
            this.t45,
            this.t60});
            this.randomTextIntervalToolStripMenuItem.Name = "randomTextIntervalToolStripMenuItem";
            this.randomTextIntervalToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.randomTextIntervalToolStripMenuItem.Text = "Random Text Interval";
            // 
            // t30
            // 
            this.t30.Name = "t30";
            this.t30.Size = new System.Drawing.Size(132, 22);
            this.t30.Text = "30 minutes";
            this.t30.Click += new System.EventHandler(this.t30_Click);
            // 
            // t45
            // 
            this.t45.Name = "t45";
            this.t45.Size = new System.Drawing.Size(132, 22);
            this.t45.Text = "45 minutes";
            this.t45.Click += new System.EventHandler(this.t45_Click);
            // 
            // t60
            // 
            this.t60.Name = "t60";
            this.t60.Size = new System.Drawing.Size(132, 22);
            this.t60.Text = "60 minutes";
            this.t60.Click += new System.EventHandler(this.t60_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(214, 6);
            // 
            // redditCredentialsToolStripMenuItem
            // 
            this.redditCredentialsToolStripMenuItem.Name = "redditCredentialsToolStripMenuItem";
            this.redditCredentialsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.redditCredentialsToolStripMenuItem.Text = "Reddit Credentials";
            this.redditCredentialsToolStripMenuItem.Click += new System.EventHandler(this.redditCredentialsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(217, 22);
            this.toolStripMenuItem1.Text = "Anime and Time API";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // twitterAPIToolStripMenuItem
            // 
            this.twitterAPIToolStripMenuItem.Name = "twitterAPIToolStripMenuItem";
            this.twitterAPIToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.twitterAPIToolStripMenuItem.Text = "Twitter API";
            this.twitterAPIToolStripMenuItem.Click += new System.EventHandler(this.twitterAPIToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(214, 6);
            // 
            // operatorsToolStripMenuItem
            // 
            this.operatorsToolStripMenuItem.Name = "operatorsToolStripMenuItem";
            this.operatorsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.operatorsToolStripMenuItem.Text = "Bot Operators";
            this.operatorsToolStripMenuItem.Click += new System.EventHandler(this.operatorsToolStripMenuItem_Click);
            // 
            // mutedUsersToolStripMenuItem
            // 
            this.mutedUsersToolStripMenuItem.Name = "mutedUsersToolStripMenuItem";
            this.mutedUsersToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.mutedUsersToolStripMenuItem.Text = "Ignored Users";
            this.mutedUsersToolStripMenuItem.Click += new System.EventHandler(this.mutedUsersToolStripMenuItem_Click);
            // 
            // toolStripButtonText
            // 
            this.toolStripButtonText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonText.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rulesTextToolStripMenuItem,
            this.helpTextToolStripMenuItem,
            this.toolStripSeparator4,
            this.ColorkageURLsButton,
            this.changeETAToolStripMenuItem,
            this.toolStripSeparator7,
            this.reloadToolStripMenuItem1});
            this.toolStripButtonText.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonText.Image")));
            this.toolStripButtonText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonText.Name = "toolStripButtonText";
            this.toolStripButtonText.ShowDropDownArrow = false;
            this.toolStripButtonText.Size = new System.Drawing.Size(33, 22);
            this.toolStripButtonText.Text = "&Text";
            // 
            // rulesTextToolStripMenuItem
            // 
            this.rulesTextToolStripMenuItem.Name = "rulesTextToolStripMenuItem";
            this.rulesTextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.rulesTextToolStripMenuItem.Text = "Rules Text";
            this.rulesTextToolStripMenuItem.Click += new System.EventHandler(this.rulesTextToolStripMenuItem_Click);
            // 
            // helpTextToolStripMenuItem
            // 
            this.helpTextToolStripMenuItem.Name = "helpTextToolStripMenuItem";
            this.helpTextToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.helpTextToolStripMenuItem.Text = "Help Text";
            this.helpTextToolStripMenuItem.Click += new System.EventHandler(this.helpTextToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(154, 6);
            // 
            // ColorkageURLsButton
            // 
            this.ColorkageURLsButton.Name = "ColorkageURLsButton";
            this.ColorkageURLsButton.Size = new System.Drawing.Size(157, 22);
            this.ColorkageURLsButton.Text = "Colorkage URLs";
            this.ColorkageURLsButton.Click += new System.EventHandler(this.assignmentsURLToolStripMenuItem1_Click);
            // 
            // changeETAToolStripMenuItem
            // 
            this.changeETAToolStripMenuItem.Name = "changeETAToolStripMenuItem";
            this.changeETAToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.changeETAToolStripMenuItem.Text = "Change ETA";
            this.changeETAToolStripMenuItem.Click += new System.EventHandler(this.changeETAToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(154, 6);
            // 
            // reloadToolStripMenuItem1
            // 
            this.reloadToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.toolStripSeparator8,
            this.rulesToolStripMenuItem1,
            this.helpToolStripMenuItem1,
            this.nickGeneratorToolStripMenuItem1,
            this.triviasToolStripMenuItem,
            this.killStringsToolStripMenuItem});
            this.reloadToolStripMenuItem1.Name = "reloadToolStripMenuItem1";
            this.reloadToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.reloadToolStripMenuItem1.Text = "Reload...";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.allToolStripMenuItem.Text = "All";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(150, 6);
            // 
            // rulesToolStripMenuItem1
            // 
            this.rulesToolStripMenuItem1.Name = "rulesToolStripMenuItem1";
            this.rulesToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.rulesToolStripMenuItem1.Text = "Rules";
            this.rulesToolStripMenuItem1.Click += new System.EventHandler(this.rulesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // nickGeneratorToolStripMenuItem1
            // 
            this.nickGeneratorToolStripMenuItem1.Name = "nickGeneratorToolStripMenuItem1";
            this.nickGeneratorToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.nickGeneratorToolStripMenuItem1.Text = "Nick Generator";
            this.nickGeneratorToolStripMenuItem1.Click += new System.EventHandler(this.nickGeneratorToolStripMenuItem_Click);
            // 
            // triviasToolStripMenuItem
            // 
            this.triviasToolStripMenuItem.Name = "triviasToolStripMenuItem";
            this.triviasToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.triviasToolStripMenuItem.Text = "Trivias";
            this.triviasToolStripMenuItem.Click += new System.EventHandler(this.triviaToolStripMenuItem_Click);
            // 
            // killStringsToolStripMenuItem
            // 
            this.killStringsToolStripMenuItem.Name = "killStringsToolStripMenuItem";
            this.killStringsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.killStringsToolStripMenuItem.Text = "Kill Strings";
            this.killStringsToolStripMenuItem.Click += new System.EventHandler(this.killToolStripMenuItem_Click);
            // 
            // InputBox
            // 
            this.InputBox.AcceptsReturn = true;
            this.InputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.InputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.InputBox.Location = new System.Drawing.Point(0, 447);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(1015, 20);
            this.InputBox.TabIndex = 3;
            this.InputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.OutputBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.InterfaceUserList);
            this.splitContainer1.Size = new System.Drawing.Size(1015, 422);
            this.splitContainer1.SplitterDistance = 866;
            this.splitContainer1.TabIndex = 4;
            // 
            // OutputBox
            // 
            this.OutputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.OutputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputBox.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.OutputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.OutputBox.Location = new System.Drawing.Point(0, 0);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ReadOnly = true;
            this.OutputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.OutputBox.Size = new System.Drawing.Size(866, 422);
            this.OutputBox.TabIndex = 2;
            this.OutputBox.Text = "";
            this.OutputBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.output2_LinkClicked);
            // 
            // InterfaceUserList
            // 
            this.InterfaceUserList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.InterfaceUserList.ContextMenuStrip = this.contextMenuUserList;
            this.InterfaceUserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InterfaceUserList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.InterfaceUserList.FormattingEnabled = true;
            this.InterfaceUserList.Location = new System.Drawing.Point(0, 0);
            this.InterfaceUserList.Name = "InterfaceUserList";
            this.InterfaceUserList.Size = new System.Drawing.Size(145, 422);
            this.InterfaceUserList.TabIndex = 0;
            this.InterfaceUserList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // contextMenuUserList
            // 
            this.contextMenuUserList.Name = "contextMenuStrip1";
            this.contextMenuUserList.Size = new System.Drawing.Size(61, 4);
            this.contextMenuUserList.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip1_Closed);
            this.contextMenuUserList.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.contextMenuUserList.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 489);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.statusStripBottom);
            this.Controls.Add(this.toolStripMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "NarutoBot3";
            this.statusStripBottom.ResumeLayout(false);
            this.statusStripBottom.PerformLayout();
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripBottom;
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripStatusLabel l_Status;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonNarutoBot;
        private System.Windows.Forms.ToolStripMenuItem connectMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonSettings;
        private System.Windows.Forms.ToolStripMenuItem operatorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem silencedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mutedUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeNickToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSilence;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonText;
        private System.Windows.Forms.ToolStripMenuItem rulesTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem ColorkageURLsButton;
        private System.Windows.Forms.ToolStripMenuItem changeETAToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox InterfaceUserList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem redditCredentialsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuUserList;
        private System.Windows.Forms.ToolStripMenuItem releaseCheckerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomTextIntervalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t30;
        private System.Windows.Forms.ToolStripMenuItem t45;
        private System.Windows.Forms.ToolStripMenuItem t60;
        private System.Windows.Forms.RichTextBox OutputBox;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem rulesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nickGeneratorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem triviasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killStringsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolstripLag;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem twitterAPIToolStripMenuItem;
    }
}

