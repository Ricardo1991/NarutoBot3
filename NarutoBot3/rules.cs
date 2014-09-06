using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class rules : Form
    {
        static List<string> rls = new List<string>();

        static private void SaveRLS()
        {
            using (StreamWriter newTask = new StreamWriter("rules.txt", false))
            {
                foreach (string rl in rls)
                {
                    newTask.WriteLine(rl);
                }
            }


        }
        static void readRLS()
        {
            StreamReader sr = new StreamReader("rules.txt");
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


        public rules()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rls.Clear();
            foreach (var myString in textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                rls.Add(myString);
            SaveRLS();

            this.Close();
        }

        private void rules_Shown(object sender, EventArgs e)
        {
            rls.Clear();
            textBox1.Text = "";
            readRLS();
            foreach (string rl in rls)
            {
                textBox1.AppendText(rl+"\n");
            }
            
        }
    }
}
