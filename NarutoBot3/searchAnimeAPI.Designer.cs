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
            this.cx_group = new System.Windows.Forms.GroupBox();
            this.tb_cx = new System.Windows.Forms.TextBox();
            this.groupBoxAPI = new System.Windows.Forms.GroupBox();
            this.tb_API = new System.Windows.Forms.TextBox();
            this.bt_Save = new System.Windows.Forms.Button();
            this.cx_group.SuspendLayout();
            this.groupBoxAPI.SuspendLayout();
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
            this.groupBoxAPI.TabIndex = 0;
            this.groupBoxAPI.TabStop = false;
            this.groupBoxAPI.Text = "API - for !time and !anime";
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
            this.bt_Save.Location = new System.Drawing.Point(418, 155);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(75, 23);
            this.bt_Save.TabIndex = 1;
            this.bt_Save.Text = "Save";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // searchAnimeAPI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 190);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.groupBoxAPI);
            this.Controls.Add(this.cx_group);
            this.Name = "searchAnimeAPI";
            this.ShowIcon = false;
            this.Text = "Google API Keys";
            this.Shown += new System.EventHandler(this.searchAnimeAPI_Shown);
            this.cx_group.ResumeLayout(false);
            this.cx_group.PerformLayout();
            this.groupBoxAPI.ResumeLayout(false);
            this.groupBoxAPI.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox cx_group;
        private System.Windows.Forms.TextBox tb_cx;
        private System.Windows.Forms.GroupBox groupBoxAPI;
        private System.Windows.Forms.TextBox tb_API;
        private System.Windows.Forms.Button bt_Save;
    }
}