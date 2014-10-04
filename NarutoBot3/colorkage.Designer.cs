namespace NarutoBot3
{
    partial class ColorkageURLs
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
            this.assignmentsBox = new System.Windows.Forms.TextBox();
            this.bSave = new System.Windows.Forms.Button();
            this.claimsBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // assignmentsBox
            // 
            this.assignmentsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assignmentsBox.Location = new System.Drawing.Point(13, 36);
            this.assignmentsBox.Name = "assignmentsBox";
            this.assignmentsBox.Size = new System.Drawing.Size(512, 20);
            this.assignmentsBox.TabIndex = 0;
            // 
            // bSave
            // 
            this.bSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bSave.Location = new System.Drawing.Point(450, 141);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 1;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // claimsBox
            // 
            this.claimsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.claimsBox.Location = new System.Drawing.Point(13, 103);
            this.claimsBox.Name = "claimsBox";
            this.claimsBox.Size = new System.Drawing.Size(513, 20);
            this.claimsBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Assignments URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Claims URL";
            // 
            // ColorkageURLs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 176);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.claimsBox);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.assignmentsBox);
            this.Name = "ColorkageURLs";
            this.ShowIcon = false;
            this.Text = "Change Assignments and Claims";
            this.Load += new System.EventHandler(this.assignments_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox assignmentsBox;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.TextBox claimsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}