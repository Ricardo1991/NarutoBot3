namespace NarutoBot3
{
    partial class ConnectWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectWindow));
            this.l_BotNick = new System.Windows.Forms.Label();
            this.t_BotNick = new System.Windows.Forms.TextBox();
            this.l_Server = new System.Windows.Forms.Label();
            this.t_Server = new System.Windows.Forms.TextBox();
            this.l_Channel = new System.Windows.Forms.Label();
            this.t_Channel = new System.Windows.Forms.TextBox();
            this.b_Conect = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.t_port = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // l_BotNick
            // 
            this.l_BotNick.AutoSize = true;
            this.l_BotNick.Location = new System.Drawing.Point(6, 26);
            this.l_BotNick.Name = "l_BotNick";
            this.l_BotNick.Size = new System.Drawing.Size(48, 13);
            this.l_BotNick.TabIndex = 0;
            this.l_BotNick.Text = "Bot Nick";
            // 
            // t_BotNick
            // 
            this.t_BotNick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_BotNick.Location = new System.Drawing.Point(60, 23);
            this.t_BotNick.Name = "t_BotNick";
            this.t_BotNick.Size = new System.Drawing.Size(205, 20);
            this.t_BotNick.TabIndex = 1;
            this.t_BotNick.KeyDown += new System.Windows.Forms.KeyEventHandler(this.t_BotNick_KeyDown);
            // 
            // l_Server
            // 
            this.l_Server.AutoSize = true;
            this.l_Server.Location = new System.Drawing.Point(6, 52);
            this.l_Server.Name = "l_Server";
            this.l_Server.Size = new System.Drawing.Size(38, 13);
            this.l_Server.TabIndex = 0;
            this.l_Server.Text = "Server";
            // 
            // t_Server
            // 
            this.t_Server.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_Server.Location = new System.Drawing.Point(60, 50);
            this.t_Server.Name = "t_Server";
            this.t_Server.Size = new System.Drawing.Size(205, 20);
            this.t_Server.TabIndex = 2;
            this.t_Server.KeyDown += new System.Windows.Forms.KeyEventHandler(this.t_Server_KeyDown);
            // 
            // l_Channel
            // 
            this.l_Channel.AutoSize = true;
            this.l_Channel.Location = new System.Drawing.Point(6, 104);
            this.l_Channel.Name = "l_Channel";
            this.l_Channel.Size = new System.Drawing.Size(46, 13);
            this.l_Channel.TabIndex = 0;
            this.l_Channel.Text = "Channel";
            // 
            // t_Channel
            // 
            this.t_Channel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_Channel.Location = new System.Drawing.Point(60, 104);
            this.t_Channel.Name = "t_Channel";
            this.t_Channel.Size = new System.Drawing.Size(205, 20);
            this.t_Channel.TabIndex = 4;
            this.t_Channel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.t_Channel_KeyDown);
            // 
            // b_Conect
            // 
            this.b_Conect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.b_Conect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_Conect.Location = new System.Drawing.Point(12, 156);
            this.b_Conect.Name = "b_Conect";
            this.b_Conect.Size = new System.Drawing.Size(196, 30);
            this.b_Conect.TabIndex = 5;
            this.b_Conect.Text = "Save and Connect";
            this.b_Conect.UseVisualStyleBackColor = true;
            this.b_Conect.Click += new System.EventHandler(this.b_Conect_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(214, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port";
            // 
            // t_port
            // 
            this.t_port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_port.Location = new System.Drawing.Point(60, 77);
            this.t_port.Name = "t_port";
            this.t_port.Size = new System.Drawing.Size(205, 20);
            this.t_port.TabIndex = 3;
            this.t_port.KeyDown += new System.Windows.Forms.KeyEventHandler(this.t_port_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.l_BotNick);
            this.groupBox1.Controls.Add(this.t_BotNick);
            this.groupBox1.Controls.Add(this.l_Server);
            this.groupBox1.Controls.Add(this.t_port);
            this.groupBox1.Controls.Add(this.t_Server);
            this.groupBox1.Controls.Add(this.t_Channel);
            this.groupBox1.Controls.Add(this.l_Channel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 138);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Info";
            // 
            // ConnectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 195);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.b_Conect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConnectWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ConnectWindow";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label l_BotNick;
        private System.Windows.Forms.TextBox t_BotNick;
        private System.Windows.Forms.Label l_Server;
        private System.Windows.Forms.TextBox t_Server;
        private System.Windows.Forms.Label l_Channel;
        private System.Windows.Forms.TextBox t_Channel;
        private System.Windows.Forms.Button b_Conect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox t_port;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}