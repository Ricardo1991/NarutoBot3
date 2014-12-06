namespace NarutoBot3
{
    partial class TwitterAPIKeysWindow
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
            this.consumerBox = new System.Windows.Forms.GroupBox();
            this.tb_ConsumerKeySecret = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_ConsumerKey = new System.Windows.Forms.TextBox();
            this.b_ConsumerKey = new System.Windows.Forms.Label();
            this.AccessTokenBox = new System.Windows.Forms.GroupBox();
            this.tb_AccessTokenSecret = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_AccessToken = new System.Windows.Forms.TextBox();
            this.cb_TwitterEnabled = new System.Windows.Forms.CheckBox();
            this.bSave = new System.Windows.Forms.Button();
            this.consumerBox.SuspendLayout();
            this.AccessTokenBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // consumerBox
            // 
            this.consumerBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consumerBox.Controls.Add(this.tb_ConsumerKeySecret);
            this.consumerBox.Controls.Add(this.label1);
            this.consumerBox.Controls.Add(this.tb_ConsumerKey);
            this.consumerBox.Controls.Add(this.b_ConsumerKey);
            this.consumerBox.Location = new System.Drawing.Point(13, 13);
            this.consumerBox.Name = "consumerBox";
            this.consumerBox.Size = new System.Drawing.Size(456, 77);
            this.consumerBox.TabIndex = 0;
            this.consumerBox.TabStop = false;
            this.consumerBox.Text = "Consumer Keys";
            // 
            // tb_ConsumerKeySecret
            // 
            this.tb_ConsumerKeySecret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_ConsumerKeySecret.Location = new System.Drawing.Point(129, 47);
            this.tb_ConsumerKeySecret.Name = "tb_ConsumerKeySecret";
            this.tb_ConsumerKeySecret.Size = new System.Drawing.Size(321, 20);
            this.tb_ConsumerKeySecret.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Consumer Key Secret";
            // 
            // tb_ConsumerKey
            // 
            this.tb_ConsumerKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_ConsumerKey.Location = new System.Drawing.Point(129, 21);
            this.tb_ConsumerKey.Name = "tb_ConsumerKey";
            this.tb_ConsumerKey.Size = new System.Drawing.Size(321, 20);
            this.tb_ConsumerKey.TabIndex = 1;
            // 
            // b_ConsumerKey
            // 
            this.b_ConsumerKey.AutoSize = true;
            this.b_ConsumerKey.Location = new System.Drawing.Point(7, 24);
            this.b_ConsumerKey.Name = "b_ConsumerKey";
            this.b_ConsumerKey.Size = new System.Drawing.Size(75, 13);
            this.b_ConsumerKey.TabIndex = 0;
            this.b_ConsumerKey.Text = "Consumer Key";
            // 
            // AccessTokenBox
            // 
            this.AccessTokenBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccessTokenBox.Controls.Add(this.tb_AccessTokenSecret);
            this.AccessTokenBox.Controls.Add(this.label3);
            this.AccessTokenBox.Controls.Add(this.label2);
            this.AccessTokenBox.Controls.Add(this.tb_AccessToken);
            this.AccessTokenBox.Location = new System.Drawing.Point(13, 97);
            this.AccessTokenBox.Name = "AccessTokenBox";
            this.AccessTokenBox.Size = new System.Drawing.Size(456, 82);
            this.AccessTokenBox.TabIndex = 1;
            this.AccessTokenBox.TabStop = false;
            this.AccessTokenBox.Text = "Access Tokens";
            // 
            // tb_AccessTokenSecret
            // 
            this.tb_AccessTokenSecret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_AccessTokenSecret.Location = new System.Drawing.Point(129, 46);
            this.tb_AccessTokenSecret.Name = "tb_AccessTokenSecret";
            this.tb_AccessTokenSecret.Size = new System.Drawing.Size(321, 20);
            this.tb_AccessTokenSecret.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Access Token Secret";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Access Token";
            // 
            // tb_AccessToken
            // 
            this.tb_AccessToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_AccessToken.Location = new System.Drawing.Point(129, 20);
            this.tb_AccessToken.Name = "tb_AccessToken";
            this.tb_AccessToken.Size = new System.Drawing.Size(321, 20);
            this.tb_AccessToken.TabIndex = 1;
            // 
            // cb_TwitterEnabled
            // 
            this.cb_TwitterEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_TwitterEnabled.AutoSize = true;
            this.cb_TwitterEnabled.Location = new System.Drawing.Point(14, 191);
            this.cb_TwitterEnabled.Name = "cb_TwitterEnabled";
            this.cb_TwitterEnabled.Size = new System.Drawing.Size(100, 17);
            this.cb_TwitterEnabled.TabIndex = 2;
            this.cb_TwitterEnabled.Text = "Twitter Enabled";
            this.cb_TwitterEnabled.UseVisualStyleBackColor = true;
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bSave.Location = new System.Drawing.Point(394, 185);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 3;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // TwitterAPIKeysWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 220);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.cb_TwitterEnabled);
            this.Controls.Add(this.AccessTokenBox);
            this.Controls.Add(this.consumerBox);
            this.MinimumSize = new System.Drawing.Size(497, 259);
            this.Name = "TwitterAPIKeysWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Twitter API Keys";
            this.Load += new System.EventHandler(this.twitterAPIkeys_Load);
            this.Shown += new System.EventHandler(this.twitterAPIkeys_Shown);
            this.consumerBox.ResumeLayout(false);
            this.consumerBox.PerformLayout();
            this.AccessTokenBox.ResumeLayout(false);
            this.AccessTokenBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox consumerBox;
        private System.Windows.Forms.TextBox tb_ConsumerKeySecret;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_ConsumerKey;
        private System.Windows.Forms.Label b_ConsumerKey;
        private System.Windows.Forms.GroupBox AccessTokenBox;
        private System.Windows.Forms.TextBox tb_AccessTokenSecret;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_AccessToken;
        private System.Windows.Forms.CheckBox cb_TwitterEnabled;
        private System.Windows.Forms.Button bSave;
    }
}