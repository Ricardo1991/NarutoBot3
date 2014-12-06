using System;
using System.Windows.Forms;
using NarutoBot3.Properties;

namespace NarutoBot3
{
    public partial class RedditCredentials : Form
    {
        public RedditCredentials()
        {
            InitializeComponent();
            tb_User.Text = Settings.Default.redditUser;
            tb_Pass.Text = Settings.Default.redditPass;

            if (Settings.Default.redditEnabled)
                bt_Logout.Enabled = true;
            else bt_Logout.Enabled = false;
        }

        private void b_Login_Click(object sender, EventArgs e)
        {
            Settings.Default.redditUser=tb_User.Text;
            Settings.Default.redditPass=tb_Pass.Text;
            Settings.Default.redditEnabled = true;
            Settings.Default.Save();
            this.Close();
        }

        private void RedditCredentials_Shown(object sender, EventArgs e)
        {
            tb_User.Text = Settings.Default.redditUser;
            tb_Pass.Text = Settings.Default.redditPass;

            if (Settings.Default.redditEnabled)
                bt_Logout.Enabled = true;
            else bt_Logout.Enabled = false;
        }

        private void bt_Logout_Click(object sender, EventArgs e)
        {
            tb_User.Text = "";
            tb_Pass.Text = "";

            Settings.Default.redditUser = tb_User.Text;
            Settings.Default.redditPass = tb_Pass.Text;
            Settings.Default.redditEnabled = false;
            Settings.Default.Save();
        }
    }
}
