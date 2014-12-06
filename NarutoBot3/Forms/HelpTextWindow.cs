using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class HelpTextWindow : Form
    {
        static List<string> hlp = new List<string>();

        static private void SaveHLP()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/help.txt", false))
            {
                foreach (string hp in hlp)
                {
                    newTask.WriteLine(hp);
                }
            }


        }
        static void readHLP()
        {
            hlp.Clear();
            StreamReader sr = new StreamReader("TextFiles/help.txt");
            try
            {
                while (sr.Peek() >= 0)
                {
                    hlp.Add(sr.ReadLine());
                }
            }
            catch
            {
            }

            sr.Close();
        }

        public HelpTextWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hlp.Clear();
            foreach (var myString in helpBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                hlp.Add(myString);
            SaveHLP();

            this.Close();
        }

        private void help_Shown(object sender, EventArgs e)
        {
            helpBox.Text = "";
            readHLP();
            foreach (string hp in hlp)
            {
                helpBox.AppendText(hp + "\n");
            }
        }
    }
}
