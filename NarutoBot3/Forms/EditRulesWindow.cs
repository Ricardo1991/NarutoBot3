﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class EditRulesWindow : Form
    {
        private static List<string> rls = new List<string>();

        private static void SaveRLS()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/rules.txt", false))
            {
                foreach (string rl in rls)
                {
                    newTask.WriteLine(rl);
                }
            }
        }

        private static void readRLS()
        {
            StreamReader sr = new StreamReader("TextFiles/rules.txt");
            try
            {
                while (sr.Peek() >= 0)
                {
                    rls.Add(sr.ReadLine());
                }
            }
            catch
            {
            }

            sr.Close();
        }

        public EditRulesWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rls.Clear();
            foreach (var myString in rulesBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                rls.Add(myString);
            SaveRLS();

            this.Close();
        }

        private void rules_Shown(object sender, EventArgs e)
        {
            rls.Clear();
            rulesBox.Text = "";
            readRLS();
            foreach (string rl in rls)
            {
                rulesBox.AppendText(rl + "\n");
            }
        }
    }
}