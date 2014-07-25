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
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.l_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSilence = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.connectMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.changeNickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.silencedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.redditCredentialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseCheckerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.operatorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mutedUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nickGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triviaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomTextIntervalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t30 = new System.Windows.Forms.ToolStripMenuItem();
            this.t45 = new System.Windows.Forms.ToolStripMenuItem();
            this.t60 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.rulesTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.assignmentsURLToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.claimsURLToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.changeETAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.input = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.output2 = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.l_Status,
            this.toolStripStatusLabelSilence});
            this.statusStrip1.Location = new System.Drawing.Point(0, 467);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1015, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
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
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1015, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectMenuItem1,
            this.disconnectToolStripMenuItem,
            this.toolStripSeparator5,
            this.changeNickToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.ShowDropDownArrow = false;
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(66, 22);
            this.toolStripDropDownButton1.Text = "&NarutoBot";
            // 
            // connectMenuItem1
            // 
            this.connectMenuItem1.Name = "connectMenuItem1";
            this.connectMenuItem1.Size = new System.Drawing.Size(142, 22);
            this.connectMenuItem1.Text = "Connect...";
            this.connectMenuItem1.Click += new System.EventHandler(this.connectMenuItem1_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
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
            this.changeNickToolStripMenuItem.Text = "Change Nick";
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
            this.exitToolStripMenuItem.Text = "Quit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.silencedToolStripMenuItem,
            this.commandsToolStripMenuItem,
            this.toolStripSeparator6,
            this.redditCredentialsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.releaseCheckerToolStripMenuItem,
            this.toolStripSeparator3,
            this.operatorsToolStripMenuItem,
            this.mutedUsersToolStripMenuItem,
            this.toolStripSeparator2,
            this.reloadToolStripMenuItem,
            this.randomTextIntervalToolStripMenuItem});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.ShowDropDownArrow = false;
            this.toolStripButton1.Size = new System.Drawing.Size(53, 22);
            this.toolStripButton1.Text = "&Settings";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // silencedToolStripMenuItem
            // 
            this.silencedToolStripMenuItem.Checked = true;
            this.silencedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.silencedToolStripMenuItem.Name = "silencedToolStripMenuItem";
            this.silencedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.silencedToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.silencedToolStripMenuItem.Text = "Silenced";
            this.silencedToolStripMenuItem.Click += new System.EventHandler(this.silencedToolStripMenuItem_Click);
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.commandsToolStripMenuItem.Text = "Enable/Disable Commands";
            this.commandsToolStripMenuItem.Click += new System.EventHandler(this.commandsToolStripMenuItem_Click);
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
            this.toolStripMenuItem1.Text = "Search Anime API";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // releaseCheckerToolStripMenuItem
            // 
            this.releaseCheckerToolStripMenuItem.Name = "releaseCheckerToolStripMenuItem";
            this.releaseCheckerToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.releaseCheckerToolStripMenuItem.Text = "Release Checker";
            this.releaseCheckerToolStripMenuItem.Click += new System.EventHandler(this.releaseCheckerToolStripMenuItem_Click);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rulesToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.nickGeneratorToolStripMenuItem,
            this.triviaToolStripMenuItem});
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.reloadToolStripMenuItem.Text = "Reload...";
            // 
            // rulesToolStripMenuItem
            // 
            this.rulesToolStripMenuItem.Name = "rulesToolStripMenuItem";
            this.rulesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.rulesToolStripMenuItem.Text = "Rules";
            this.rulesToolStripMenuItem.Click += new System.EventHandler(this.rulesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // nickGeneratorToolStripMenuItem
            // 
            this.nickGeneratorToolStripMenuItem.Name = "nickGeneratorToolStripMenuItem";
            this.nickGeneratorToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.nickGeneratorToolStripMenuItem.Text = "Nick Generator";
            this.nickGeneratorToolStripMenuItem.Click += new System.EventHandler(this.nickGeneratorToolStripMenuItem_Click);
            // 
            // triviaToolStripMenuItem
            // 
            this.triviaToolStripMenuItem.Name = "triviaToolStripMenuItem";
            this.triviaToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.triviaToolStripMenuItem.Text = "Trivia";
            this.triviaToolStripMenuItem.Click += new System.EventHandler(this.triviaToolStripMenuItem_Click);
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
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rulesTextToolStripMenuItem,
            this.helpTextToolStripMenuItem,
            this.toolStripSeparator4,
            this.assignmentsURLToolStripMenuItem1,
            this.claimsURLToolStripMenuItem1,
            this.changeETAToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.ShowDropDownArrow = false;
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(33, 22);
            this.toolStripDropDownButton2.Text = "&Text";
            // 
            // rulesTextToolStripMenuItem
            // 
            this.rulesTextToolStripMenuItem.Name = "rulesTextToolStripMenuItem";
            this.rulesTextToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.rulesTextToolStripMenuItem.Text = "Rules Text";
            this.rulesTextToolStripMenuItem.Click += new System.EventHandler(this.rulesTextToolStripMenuItem_Click);
            // 
            // helpTextToolStripMenuItem
            // 
            this.helpTextToolStripMenuItem.Name = "helpTextToolStripMenuItem";
            this.helpTextToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.helpTextToolStripMenuItem.Text = "Help Text";
            this.helpTextToolStripMenuItem.Click += new System.EventHandler(this.helpTextToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(163, 6);
            // 
            // assignmentsURLToolStripMenuItem1
            // 
            this.assignmentsURLToolStripMenuItem1.Name = "assignmentsURLToolStripMenuItem1";
            this.assignmentsURLToolStripMenuItem1.Size = new System.Drawing.Size(166, 22);
            this.assignmentsURLToolStripMenuItem1.Text = "Assignments URL";
            this.assignmentsURLToolStripMenuItem1.Click += new System.EventHandler(this.assignmentsURLToolStripMenuItem1_Click);
            // 
            // claimsURLToolStripMenuItem1
            // 
            this.claimsURLToolStripMenuItem1.Name = "claimsURLToolStripMenuItem1";
            this.claimsURLToolStripMenuItem1.Size = new System.Drawing.Size(166, 22);
            this.claimsURLToolStripMenuItem1.Text = "Claims URL";
            this.claimsURLToolStripMenuItem1.Click += new System.EventHandler(this.claimsURLToolStripMenuItem1_Click);
            // 
            // changeETAToolStripMenuItem
            // 
            this.changeETAToolStripMenuItem.Name = "changeETAToolStripMenuItem";
            this.changeETAToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.changeETAToolStripMenuItem.Text = "Change ETA";
            this.changeETAToolStripMenuItem.Click += new System.EventHandler(this.changeETAToolStripMenuItem_Click);
            // 
            // input
            // 
            this.input.AcceptsReturn = true;
            this.input.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.input.Location = new System.Drawing.Point(0, 447);
            this.input.Name = "input";
            this.input.Size = new System.Drawing.Size(1015, 20);
            this.input.TabIndex = 3;
            this.input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.output2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1015, 422);
            this.splitContainer1.SplitterDistance = 866;
            this.splitContainer1.TabIndex = 4;
            // 
            // output2
            // 
            this.output2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.output2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.output2.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.output2.ForeColor = System.Drawing.Color.Silver;
            this.output2.Location = new System.Drawing.Point(0, 0);
            this.output2.Name = "output2";
            this.output2.ReadOnly = true;
            this.output2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.output2.Size = new System.Drawing.Size(866, 422);
            this.output2.TabIndex = 2;
            this.output2.Text = "";
            this.output2.WordWrap = false;
            this.output2.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.output2_LinkClicked);
            // 
            // listBox1
            // 
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(145, 422);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip1_Closed);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 489);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.input);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "NarutoBot";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripStatusLabel l_Status;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem connectMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem operatorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem silencedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mutedUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeNickToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSilence;
        private System.Windows.Forms.TextBox input;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem rulesTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem assignmentsURLToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem claimsURLToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem changeETAToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nickGeneratorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triviaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redditCredentialsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem releaseCheckerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomTextIntervalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t30;
        private System.Windows.Forms.ToolStripMenuItem t45;
        private System.Windows.Forms.ToolStripMenuItem t60;
        private System.Windows.Forms.RichTextBox output2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}

