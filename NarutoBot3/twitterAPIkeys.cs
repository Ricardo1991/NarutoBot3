using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class twitterAPIkeys : Form
    {
        public twitterAPIkeys()
        {
            InitializeComponent();
        }

        private void twitterAPIkeys_Load(object sender, EventArgs e)
        {
            cb_TwitterEnabled.Checked = Settings.Default.twitterEnabled;
            tb_AccessToken.Text = Settings.Default.twitterAccessToken;
            tb_AccessTokenSecret.Text = Settings.Default.twitterAccessTokenSecret;
            tb_ConsumerKey.Text = Settings.Default.twitterConsumerKey;
            tb_ConsumerKeySecret.Text = Settings.Default.twitterConsumerKeySecret;
        }

        private void twitterAPIkeys_Shown(object sender, EventArgs e)
        {
            cb_TwitterEnabled.Checked = Settings.Default.twitterEnabled;
            tb_AccessToken.Text = Settings.Default.twitterAccessToken;
            tb_AccessTokenSecret.Text = Settings.Default.twitterAccessTokenSecret;
            tb_ConsumerKey.Text = Settings.Default.twitterConsumerKey;
            tb_ConsumerKeySecret.Text = Settings.Default.twitterConsumerKeySecret;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            Settings.Default.twitterEnabled = cb_TwitterEnabled.Checked;

            Settings.Default.twitterAccessToken = tb_AccessToken.Text;
            Settings.Default.twitterAccessTokenSecret = tb_AccessTokenSecret.Text;
            Settings.Default.twitterConsumerKey = tb_ConsumerKey.Text;
            Settings.Default.twitterConsumerKeySecret = tb_ConsumerKeySecret.Text;
            
            Settings.Default.Save();
            this.Close();
        }


    }
}
