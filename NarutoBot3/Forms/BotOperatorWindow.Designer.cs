namespace NarutoBot3
{
    partial class BotOperatorWindow
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
            this.listOperators = new System.Windows.Forms.ListBox();
            this.t_operator = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bAdd = new System.Windows.Forms.Button();
            this.bRemove = new System.Windows.Forms.Button();
            this.bExit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listOperators
            // 
            this.listOperators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listOperators.FormattingEnabled = true;
            this.listOperators.Location = new System.Drawing.Point(12, 12);
            this.listOperators.Name = "listOperators";
            this.listOperators.Size = new System.Drawing.Size(138, 147);
            this.listOperators.TabIndex = 0;
            // 
            // t_operator
            // 
            this.t_operator.Location = new System.Drawing.Point(6, 19);
            this.t_operator.Name = "t_operator";
            this.t_operator.Size = new System.Drawing.Size(131, 20);
            this.t_operator.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bAdd);
            this.groupBox1.Controls.Add(this.t_operator);
            this.groupBox1.Location = new System.Drawing.Point(156, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 56);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Operator";
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(143, 16);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(75, 23);
            this.bAdd.TabIndex = 2;
            this.bAdd.Text = "Add";
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bRemove
            // 
            this.bRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bRemove.Location = new System.Drawing.Point(156, 136);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(107, 23);
            this.bRemove.TabIndex = 0;
            this.bRemove.Text = "Remove Selected";
            this.bRemove.UseVisualStyleBackColor = true;
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bExit
            // 
            this.bExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bExit.Location = new System.Drawing.Point(299, 136);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(75, 23);
            this.bExit.TabIndex = 4;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // BotOperatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 172);
            this.Controls.Add(this.bRemove);
            this.Controls.Add(this.bExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listOperators);
            this.MinimumSize = new System.Drawing.Size(408, 211);
            this.Name = "BotOperatorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Bot Operators";
            this.Shown += new System.EventHandler(this.operators_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listOperators;
        private System.Windows.Forms.TextBox t_operator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Button bRemove;
        private System.Windows.Forms.Button bExit;
    }
}