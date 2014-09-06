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
    public partial class help : Form
    {
        static List<string> hlp = new List<string>();

        static private void SaveHLP()
        {
            using (StreamWriter newTask = new StreamWriter("help.txt", false))
            {
                foreach (string hp in hlp)
                {
                    newTask.WriteLine(hp);
                }
            }


        }
        static void readHLP()
        {
            StreamReader sr = new StreamReader("help.txt");
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

        public help()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hlp.Clear();
            foreach (var myString in textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                hlp.Add(myString);
            SaveHLP();

            this.Close();
        }

        private void help_Shown(object sender, EventArgs e)
        {
            hlp.Clear();
            textBox1.Text = "";
            readHLP();
            foreach (string hp in hlp)
            {
                textBox1.AppendText(hp + "\n");
            }
        }
    }
}
