using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MangaETAWindow : Form
    {
        public MangaETAWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.eta = textBox1.Text;
            Settings.Default.Save();
            this.Close();
        }

        private void eta_Shown(object sender, EventArgs e)
        {
            textBox1.Text = Settings.Default.eta;
        }
    }
}
