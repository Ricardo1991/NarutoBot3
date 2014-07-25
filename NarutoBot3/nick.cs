using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NarutoBot3.Properties;

namespace NarutoBot3
{
    public partial class nick : Form
    {
        public nick()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default.Nick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Nick = textBox1.Text;
            Settings.Default.Save();
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                button1_Click(sender, e);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
