using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MutedUsersWindow : Form
    {
        static List<string> mute = new List<string>();
        static List<string> tmp = new List<string>();

        bool foundRepeated = false;

        public MutedUsersWindow()
        {
            InitializeComponent();
            readMute();
        }

        public void readMute()
        {
            mute.Clear();
            StreamReader sr = new StreamReader("TextFiles/banned.txt");
            try
            {
                while (sr.Peek() >= 0)
                {
                    mute.Add(sr.ReadLine());
                }
            }
            catch
            {
            }

            sr.Close();

            listMuted.DataSource = null;
            listMuted.DataSource = mute;
        }
        static private void saveMute()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/banned.txt", false))
            {
                foreach (string rl in mute)
                {
                    newTask.WriteLine(rl);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string ba in mute)
            {
                if (ba == t_muted.Text)
                {
                    foundRepeated = true;
                }

            }
            if (!foundRepeated)
            {
                mute.Add(t_muted.Text);
                listMuted.DataSource = mute;
                saveMute();
                readMute();
            }
            else
                MessageBox.Show("User is already Muted", "Duplicated", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tmp.Clear();
            foreach (string ba in mute)
            {
                if (ba != listMuted.SelectedItem.ToString())
                {
                    tmp.Add(ba);

                }
            }

            mute.Clear();
            foreach (string ba2 in tmp)
            {
                mute.Add(ba2);
            }

            saveMute();
            readMute();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            t_muted.Text = "";
            this.Close();
        }

        private void muted_Shown(object sender, EventArgs e)
        {
            readMute();
        }
    }
}
