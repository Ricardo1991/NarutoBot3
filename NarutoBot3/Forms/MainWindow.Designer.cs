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
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.silencedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceMirrorModeOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.operatorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mutedUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonText = new System.Windows.Forms.ToolStripDropDownButton();
            this.rulesTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.rulesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nickGeneratorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.triviasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.funkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.gitHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputBox = new AutoComplete.AutoCompleteTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tbTopic = new System.Windows.Forms.TextBox();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.InterfaceUserList = new System.Windows.Forms.ListBox();
            this.contextMenuUserList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            this.statusStripBottom.Location = new System.Drawing.Point(0, 469);
            this.statusStripBottom.Name = "statusStripBottom";
            this.statusStripBottom.Size = new System.Drawing.Size(1024, 22);
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
            this.toolStripStatusLabelSilence.Image = global::NarutoBot3.Properties.Resources.warning;
            this.toolStripStatusLabelSilence.Name = "toolStripStatusLabelSilence";
            this.toolStripStatusLabelSilence.Size = new System.Drawing.Size(99, 17);
            this.toolStripStatusLabelSilence.Text = "Bot is Silenced";
            // 
            // toolstripLag
            // 
            this.toolstripLag.Name = "toolstripLag";
            this.toolstripLag.Size = new System.Drawing.Size(831, 17);
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
            this.toolStripButtonText,
            this.toolStripHelp});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1024, 25);
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
            this.connectMenuItem1.Image = global::NarutoBot3.Properties.Resources.connect_black;
            this.connectMenuItem1.Name = "connectMenuItem1";
            this.connectMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.connectMenuItem1.Text = "&Connect...";
            this.connectMenuItem1.Click += new System.EventHandler(this.ConnectMenuItem1_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Image = global::NarutoBot3.Properties.Resources.disconnect_black;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.DisconnectToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(177, 6);
            // 
            // changeNickToolStripMenuItem
            // 
            this.changeNickToolStripMenuItem.Name = "changeNickToolStripMenuItem";
            this.changeNickToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.changeNickToolStripMenuItem.Text = "Change &Nick";
            this.changeNickToolStripMenuItem.Click += new System.EventHandler(this.ChangeNickToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::NarutoBot3.Properties.Resources.close_black;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "&Quit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitApplication);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commandsToolStripMenuItem,
            this.toolStripSeparator2,
            this.silencedToolStripMenuItem,
            this.forceMirrorModeOffToolStripMenuItem,
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
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.commandsToolStripMenuItem.Text = "Settings";
            this.commandsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(232, 6);
            // 
            // silencedToolStripMenuItem
            // 
            this.silencedToolStripMenuItem.Checked = true;
            this.silencedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.silencedToolStripMenuItem.Name = "silencedToolStripMenuItem";
            this.silencedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.silencedToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.silencedToolStripMenuItem.Text = "&Silenced";
            this.silencedToolStripMenuItem.Click += new System.EventHandler(this.SilencedToolStripMenuItem_Click);
            // 
            // forceMirrorModeOffToolStripMenuItem
            // 
            this.forceMirrorModeOffToolStripMenuItem.Name = "forceMirrorModeOffToolStripMenuItem";
            this.forceMirrorModeOffToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.forceMirrorModeOffToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.forceMirrorModeOffToolStripMenuItem.Text = "Force MirrorMode Off";
            this.forceMirrorModeOffToolStripMenuItem.Click += new System.EventHandler(this.ForceMirrorModeOffToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(232, 6);
            // 
            // operatorsToolStripMenuItem
            // 
            this.operatorsToolStripMenuItem.Name = "operatorsToolStripMenuItem";
            this.operatorsToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.operatorsToolStripMenuItem.Text = "Bot Operators";
            this.operatorsToolStripMenuItem.Click += new System.EventHandler(this.OperatorsToolStripMenuItem_Click);
            // 
            // mutedUsersToolStripMenuItem
            // 
            this.mutedUsersToolStripMenuItem.Name = "mutedUsersToolStripMenuItem";
            this.mutedUsersToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.mutedUsersToolStripMenuItem.Text = "Ignored Users";
            this.mutedUsersToolStripMenuItem.Click += new System.EventHandler(this.MutedUsersToolStripMenuItem_Click);
            // 
            // toolStripButtonText
            // 
            this.toolStripButtonText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonText.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rulesTextToolStripMenuItem,
            this.helpTextToolStripMenuItem,
            this.toolStripSeparator4,
            this.reloadToolStripMenuItem1});
            this.toolStripButtonText.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonText.Image")));
            this.toolStripButtonText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonText.Name = "toolStripButtonText";
            this.toolStripButtonText.ShowDropDownArrow = false;
            this.toolStripButtonText.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonText.Text = "&Text";
            // 
            // rulesTextToolStripMenuItem
            // 
            this.rulesTextToolStripMenuItem.Name = "rulesTextToolStripMenuItem";
            this.rulesTextToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.rulesTextToolStripMenuItem.Text = "Rules Text";
            this.rulesTextToolStripMenuItem.Click += new System.EventHandler(this.RulesTextToolStripMenuItem_Click);
            // 
            // helpTextToolStripMenuItem
            // 
            this.helpTextToolStripMenuItem.Name = "helpTextToolStripMenuItem";
            this.helpTextToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.helpTextToolStripMenuItem.Text = "Help Text";
            this.helpTextToolStripMenuItem.Click += new System.EventHandler(this.HelpTextToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(123, 6);
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
            this.killStringsToolStripMenuItem,
            this.quotesToolStripMenuItem,
            this.funkToolStripMenuItem,
            this.factsToolStripMenuItem});
            this.reloadToolStripMenuItem1.Name = "reloadToolStripMenuItem1";
            this.reloadToolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.reloadToolStripMenuItem1.Text = "Reload...";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.AllToolStripMenuItem_Click);
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
            this.rulesToolStripMenuItem1.Click += new System.EventHandler(this.RulesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // nickGeneratorToolStripMenuItem1
            // 
            this.nickGeneratorToolStripMenuItem1.Name = "nickGeneratorToolStripMenuItem1";
            this.nickGeneratorToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.nickGeneratorToolStripMenuItem1.Text = "Nick Generator";
            this.nickGeneratorToolStripMenuItem1.Click += new System.EventHandler(this.NickGeneratorToolStripMenuItem_Click);
            // 
            // triviasToolStripMenuItem
            // 
            this.triviasToolStripMenuItem.Name = "triviasToolStripMenuItem";
            this.triviasToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.triviasToolStripMenuItem.Text = "Trivias";
            this.triviasToolStripMenuItem.Click += new System.EventHandler(this.TriviaToolStripMenuItem_Click);
            // 
            // killStringsToolStripMenuItem
            // 
            this.killStringsToolStripMenuItem.Name = "killStringsToolStripMenuItem";
            this.killStringsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.killStringsToolStripMenuItem.Text = "Kill Strings";
            this.killStringsToolStripMenuItem.Click += new System.EventHandler(this.KillToolStripMenuItem_Click);
            // 
            // quotesToolStripMenuItem
            // 
            this.quotesToolStripMenuItem.Name = "quotesToolStripMenuItem";
            this.quotesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.quotesToolStripMenuItem.Text = "Quotes";
            this.quotesToolStripMenuItem.Click += new System.EventHandler(this.QuotesToolStripMenuItem_Click);
            // 
            // funkToolStripMenuItem
            // 
            this.funkToolStripMenuItem.Name = "funkToolStripMenuItem";
            this.funkToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.funkToolStripMenuItem.Text = "Funk";
            this.funkToolStripMenuItem.Click += new System.EventHandler(this.FunkToolStripMenuItem_Click);
            // 
            // factsToolStripMenuItem
            // 
            this.factsToolStripMenuItem.Name = "factsToolStripMenuItem";
            this.factsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.factsToolStripMenuItem.Text = "Facts";
            this.factsToolStripMenuItem.Click += new System.EventHandler(this.FactsToolStripMenuItem_Click);
            // 
            // toolStripHelp
            // 
            this.toolStripHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gitHubToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.toolStripHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripHelp.Image")));
            this.toolStripHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripHelp.Name = "toolStripHelp";
            this.toolStripHelp.ShowDropDownArrow = false;
            this.toolStripHelp.Size = new System.Drawing.Size(36, 22);
            this.toolStripHelp.Text = "&Help";
            // 
            // gitHubToolStripMenuItem
            // 
            this.gitHubToolStripMenuItem.Image = global::NarutoBot3.Properties.Resources.github_black;
            this.gitHubToolStripMenuItem.Name = "gitHubToolStripMenuItem";
            this.gitHubToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.gitHubToolStripMenuItem.Text = "GitHub";
            this.gitHubToolStripMenuItem.Click += new System.EventHandler(this.GitHubToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::NarutoBot3.Properties.Resources.about_black;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // InputBox
            // 
            this.InputBox.AcceptsReturn = true;
            this.InputBox.AcceptsTab = true;
            this.InputBox.AllowDrop = true;
            this.InputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.InputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.InputBox.Location = new System.Drawing.Point(0, 449);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(1024, 20);
            this.InputBox.TabIndex = 3;
            this.InputBox.Values = null;
            this.InputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Input_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tbTopic);
            this.splitContainer1.Panel1.Controls.Add(this.OutputBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.InterfaceUserList);
            this.splitContainer1.Size = new System.Drawing.Size(1024, 424);
            this.splitContainer1.SplitterDistance = 902;
            this.splitContainer1.TabIndex = 4;
            // 
            // tbTopic
            // 
            this.tbTopic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.tbTopic.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbTopic.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTopic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.tbTopic.Location = new System.Drawing.Point(0, 0);
            this.tbTopic.Name = "tbTopic";
            this.tbTopic.ReadOnly = true;
            this.tbTopic.Size = new System.Drawing.Size(902, 20);
            this.tbTopic.TabIndex = 3;
            // 
            // OutputBox
            // 
            this.OutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.OutputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.OutputBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.OutputBox.Location = new System.Drawing.Point(0, 21);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ReadOnly = true;
            this.OutputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.OutputBox.Size = new System.Drawing.Size(902, 403);
            this.OutputBox.TabIndex = 2;
            this.OutputBox.Text = "";
            this.OutputBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Output2_LinkClicked);
            // 
            // InterfaceUserList
            // 
            this.InterfaceUserList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.InterfaceUserList.ContextMenuStrip = this.contextMenuUserList;
            this.InterfaceUserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InterfaceUserList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InterfaceUserList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(199)))), ((int)(((byte)(199)))));
            this.InterfaceUserList.FormattingEnabled = true;
            this.InterfaceUserList.Location = new System.Drawing.Point(0, 0);
            this.InterfaceUserList.Name = "InterfaceUserList";
            this.InterfaceUserList.Size = new System.Drawing.Size(118, 424);
            this.InterfaceUserList.TabIndex = 0;
            this.InterfaceUserList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.InterfaceUserList_MouseDown);
            // 
            // contextMenuUserList
            // 
            this.contextMenuUserList.Name = "contextMenuStrip1";
            this.contextMenuUserList.Size = new System.Drawing.Size(61, 4);
            this.contextMenuUserList.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.ContextMenuStrip1_Closed);
            this.contextMenuUserList.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 491);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.statusStripBottom);
            this.Controls.Add(this.toolStripMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "NarutoBot3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.statusStripBottom.ResumeLayout(false);
            this.statusStripBottom.PerformLayout();
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem mutedUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeNickToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSilence;
        private AutoComplete.AutoCompleteTextBox InputBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonText;
        private System.Windows.Forms.ToolStripMenuItem rulesTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox InterfaceUserList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ContextMenuStrip contextMenuUserList;
        private System.Windows.Forms.RichTextBox OutputBox;
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
        private System.Windows.Forms.ToolStripDropDownButton toolStripHelp;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TextBox tbTopic;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem gitHubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quotesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem funkToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem forceMirrorModeOffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem factsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}

