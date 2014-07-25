using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class muted : Form
    {
        static List<string> ban = new List<string>();
        static List<string> tmp = new List<string>();

        bool foundRepeated = false;

        public muted()
        {
            InitializeComponent();
            readBAN();
        }

        public void readBAN()
        {
            ban.Clear();
            StreamReader sr = new StreamReader("banned.txt");
            try
            {
                while (sr.Peek() >= 0)
                {
                    ban.Add(sr.ReadLine());
                }
            }
            catch
            {
            }

            sr.Close();

            listMuted.DataSource = null;
            listMuted.DataSource = ban;
        }
        private void SaveBAN()
        {
            using (StreamWriter newTask = new StreamWriter("banned.txt", false))
            {
                foreach (string rl in ban)
                {
                    newTask.WriteLine(rl);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string ba in ban)
            {
                if (ba == t_muted.Text)
                {
                    foundRepeated = true;
                }

            }
            if (!foundRepeated)
            {
                ban.Add(t_muted.Text);
                listMuted.DataSource = ban;
                SaveBAN();
                readBAN();
            }
            else
                MessageBox.Show("User is already Muted", "Duplicated", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tmp.Clear();
            foreach (string ba in ban)
            {
                if (ba != listMuted.SelectedItem.ToString())
                {
                    tmp.Add(ba);

                }
            }

            ban.Clear();
            foreach (string ba2 in tmp)
            {
                ban.Add(ba2);
            }

            SaveBAN();
            readBAN();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            t_muted.Text = "";
            this.Close();
        }

        private void muted_Shown(object sender, EventArgs e)
        {
            readBAN();
        }
    }
}
