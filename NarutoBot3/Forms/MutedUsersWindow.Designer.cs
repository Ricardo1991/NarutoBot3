namespace NarutoBot3
{
    partial class MutedUsersWindow
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
            this.listMuted = new System.Windows.Forms.ListBox();
            this.t_muted = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bMute = new System.Windows.Forms.Button();
            this.bUnmute = new System.Windows.Forms.Button();
            this.bExit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listMuted
            // 
            this.listMuted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listMuted.FormattingEnabled = true;
            this.listMuted.Location = new System.Drawing.Point(12, 12);
            this.listMuted.Name = "listMuted";
            this.listMuted.Size = new System.Drawing.Size(138, 147);
            this.listMuted.TabIndex = 0;
            // 
            // t_muted
            // 
            this.t_muted.Location = new System.Drawing.Point(6, 19);
            this.t_muted.Name = "t_muted";
            this.t_muted.Size = new System.Drawing.Size(131, 20);
            this.t_muted.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bMute);
            this.groupBox1.Controls.Add(this.t_muted);
            this.groupBox1.Location = new System.Drawing.Point(156, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 56);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ignore User";
            // 
            // bMute
            // 
            this.bMute.Location = new System.Drawing.Point(143, 16);
            this.bMute.Name = "bMute";
            this.bMute.Size = new System.Drawing.Size(75, 23);
            this.bMute.TabIndex = 2;
            this.bMute.Text = "Ignore";
            this.bMute.UseVisualStyleBackColor = true;
            this.bMute.Click += new System.EventHandler(this.bMute_Click);
            // 
            // bUnmute
            // 
            this.bUnmute.Location = new System.Drawing.Point(156, 136);
            this.bUnmute.Name = "bUnmute";
            this.bUnmute.Size = new System.Drawing.Size(106, 23);
            this.bUnmute.TabIndex = 0;
            this.bUnmute.Text = "Remove Selected";
            this.bUnmute.UseVisualStyleBackColor = true;
            this.bUnmute.Click += new System.EventHandler(this.bUnmute_Click);
            // 
            // bExit
            // 
            this.bExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bExit.Location = new System.Drawing.Point(299, 136);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(75, 23);
            this.bExit.TabIndex = 4;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // MutedUsersWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 172);
            this.Controls.Add(this.bUnmute);
            this.Controls.Add(this.bExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listMuted);
            this.MinimumSize = new System.Drawing.Size(408, 211);
            this.Name = "MutedUsersWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Ignored Users";
            this.Shown += new System.EventHandler(this.muted_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listMuted;
        private System.Windows.Forms.TextBox t_muted;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bMute;
        private System.Windows.Forms.Button bUnmute;
        private System.Windows.Forms.Button bExit;
    }
}