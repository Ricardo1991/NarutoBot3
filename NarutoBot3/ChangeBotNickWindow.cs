using System;
using System.Windows.Forms;
using NarutoBot3.Properties;

namespace NarutoBot3
{
    public partial class ChangeBotNickWindow : Form
    {
        public ChangeBotNickWindow()
        {
            InitializeComponent();
            tb_Nick.Text = Settings.Default.Nick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Nick = tb_Nick.Text;
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
