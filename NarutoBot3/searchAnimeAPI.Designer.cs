namespace NarutoBot3
{
    partial class searchAnimeAPI
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
            this.cx_group = new System.Windows.Forms.GroupBox();
            this.tb_cx = new System.Windows.Forms.TextBox();
            this.groupBoxAPI = new System.Windows.Forms.GroupBox();
            this.tb_API = new System.Windows.Forms.TextBox();
            this.bt_Save = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_Pass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_User = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cx_group.SuspendLayout();
            this.groupBoxAPI.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cx_group
            // 
            this.cx_group.Controls.Add(this.tb_cx);
            this.cx_group.Location = new System.Drawing.Point(13, 12);
            this.cx_group.Name = "cx_group";
            this.cx_group.Size = new System.Drawing.Size(481, 63);
            this.cx_group.TabIndex = 0;
            this.cx_group.TabStop = false;
            this.cx_group.Text = "cx - for !anime";
            // 
            // tb_cx
            // 
            this.tb_cx.Location = new System.Drawing.Point(7, 20);
            this.tb_cx.Name = "tb_cx";
            this.tb_cx.Size = new System.Drawing.Size(468, 20);
            this.tb_cx.TabIndex = 0;
            // 
            // groupBoxAPI
            // 
            this.groupBoxAPI.Controls.Add(this.tb_API);
            this.groupBoxAPI.Location = new System.Drawing.Point(13, 81);
            this.groupBoxAPI.Name = "groupBoxAPI";
            this.groupBoxAPI.Size = new System.Drawing.Size(481, 63);
            this.groupBoxAPI.TabIndex = 1;
            this.groupBoxAPI.TabStop = false;
            this.groupBoxAPI.Text = "API - for !anime and !time";
            // 
            // tb_API
            // 
            this.tb_API.Location = new System.Drawing.Point(7, 19);
            this.tb_API.Name = "tb_API";
            this.tb_API.Size = new System.Drawing.Size(468, 20);
            this.tb_API.TabIndex = 0;
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(419, 266);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(75, 23);
            this.bt_Save.TabIndex = 3;
            this.bt_Save.Text = "Save";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_Pass);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_User);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 109);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "myAnimeList Credentials - for !anime";
            // 
            // tb_Pass
            // 
            this.tb_Pass.Location = new System.Drawing.Point(78, 64);
            this.tb_Pass.Name = "tb_Pass";
            this.tb_Pass.Size = new System.Drawing.Size(122, 20);
            this.tb_Pass.TabIndex = 3;
            this.tb_Pass.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Password";
            // 
            // tb_User
            // 
            this.tb_User.Location = new System.Drawing.Point(78, 28);
            this.tb_User.Name = "tb_User";
            this.tb_User.Size = new System.Drawing.Size(122, 20);
            this.tb_User.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // searchAnimeAPI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 297);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.groupBoxAPI);
            this.Controls.Add(this.cx_group);
            this.Name = "searchAnimeAPI";
            this.ShowIcon = false;
            this.Text = "Google and MAL API Keys";
            this.Shown += new System.EventHandler(this.searchAnimeAPI_Shown);
            this.cx_group.ResumeLayout(false);
            this.cx_group.PerformLayout();
            this.groupBoxAPI.ResumeLayout(false);
            this.groupBoxAPI.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox cx_group;
        private System.Windows.Forms.TextBox tb_cx;
        private System.Windows.Forms.GroupBox groupBoxAPI;
        private System.Windows.Forms.TextBox tb_API;
        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_Pass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_User;
        private System.Windows.Forms.Label label1;
    }
}