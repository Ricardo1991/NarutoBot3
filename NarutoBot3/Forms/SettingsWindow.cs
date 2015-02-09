using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.youtubeSearchEnabled = cb_YTSearch.Checked;
            Settings.Default.questionEnabled = cb_Questions.Checked;
            Settings.Default.killEnabled = cb_Kill.Checked;
            Settings.Default.twitterEnabled = cb_twitter.Checked;
            Settings.Default.randomTextEnabled = cb_randomText.Checked;
            Settings.Default.wikiEnabled = cb_Wiki.Checked;
            Settings.Default.pokeEnabled = cb_Poke.Checked;
            Settings.Default.vimeoEnabled = cb_vimeo.Checked;
            Settings.Default.triviaEnabled = cb_Trivia.Checked;
            Settings.Default.redditEnabled = cb_Reddit.Checked;
            Settings.Default.timeEnabled = cb_Time.Checked;
            Settings.Default.help_Enabled = cb_help.Checked;
            Settings.Default.rules_Enabled = cb_rules.Checked;
            Settings.Default.roll_Enabled = cb_roll.Checked;
            Settings.Default.hello_Enabled = cb_hello.Checked;
            Settings.Default.youtube_Enabled = cb_youtube.Checked;
            Settings.Default.conversionEnabled = cb_Convert.Checked;
            Settings.Default.nickEnabled = cb_nicks.Checked;
            Settings.Default.aniSearchEnabled = cb_Anime.Checked;
            Settings.Default.greetingsEnabled = cb_greetings.Checked;


            Settings.Default.twitterAccessToken = tb_AccessToken.Text;
            Settings.Default.twitterAccessTokenSecret = tb_AccessTokenSecret.Text;
            Settings.Default.twitterConsumerKey = tb_ConsumerKey.Text;
            Settings.Default.twitterConsumerKeySecret = tb_ConsumerKeySecret.Text;

            Settings.Default.apikey = tb_API.Text;
            Settings.Default.cxKey = tb_cx.Text;
            Settings.Default.malPass = tb_MALpass.Text;
            Settings.Default.malUser = tb_MALuser.Text;

            Settings.Default.redditUser = tb_User.Text;
            Settings.Default.redditPass = tb_Pass.Text;

            if (String.IsNullOrWhiteSpace(Settings.Default.twitterAccessToken) ||
                String.IsNullOrWhiteSpace(Settings.Default.twitterAccessTokenSecret) ||
                String.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKey) ||
                String.IsNullOrWhiteSpace(Settings.Default.twitterConsumerKeySecret)
                )
                Settings.Default.twitterEnabled = false;

            if (Settings.Default.cxKey.Length < 5 || Settings.Default.apikey.Length < 5)
            {
                Settings.Default.aniSearchEnabled = false;
                Settings.Default.timeEnabled = false;
                Settings.Default.Save();
            }

            Settings.Default.Save();
            this.Close();
        }

        private void EnabledCommandsWindow_Shown(object sender, EventArgs e)
        {
            cb_Kill.Checked = Settings.Default.killEnabled;
            cb_Anime.Checked = Settings.Default.aniSearchEnabled;
            cb_randomText.Checked = Settings.Default.randomTextEnabled;
            cb_Wiki.Checked = Settings.Default.wikiEnabled;
            cb_Poke.Checked = Settings.Default.pokeEnabled;
            cb_vimeo.Checked = Settings.Default.vimeoEnabled;
            cb_hello.Checked = Settings.Default.hello_Enabled;
            cb_roll.Checked = Settings.Default.roll_Enabled;
            cb_rules.Checked = Settings.Default.rules_Enabled;
            cb_youtube.Checked = Settings.Default.youtube_Enabled;
            cb_help.Checked = Settings.Default.help_Enabled;
            cb_Convert.Checked = Settings.Default.conversionEnabled;
            cb_nicks.Checked = Settings.Default.nickEnabled;
            cb_Reddit.Checked = Settings.Default.redditEnabled;
            cb_Time.Checked = Settings.Default.timeEnabled;
            cb_Trivia.Checked = Settings.Default.triviaEnabled;
            cb_twitter.Checked = Settings.Default.twitterEnabled;
            cb_Questions.Checked = Settings.Default.questionEnabled;
            cb_YTSearch.Checked = Settings.Default.youtubeSearchEnabled;
            cb_greetings.Checked = Settings.Default.greetingsEnabled;


            cb_TwitterEnabled.Checked = Settings.Default.twitterEnabled;
            tb_AccessToken.Text = Settings.Default.twitterAccessToken;
            tb_AccessTokenSecret.Text = Settings.Default.twitterAccessTokenSecret;
            tb_ConsumerKey.Text = Settings.Default.twitterConsumerKey;
            tb_ConsumerKeySecret.Text = Settings.Default.twitterConsumerKeySecret;

            tb_API.Text = Settings.Default.apikey;
            tb_cx.Text = Settings.Default.cxKey;
            tb_MALuser.Text = Settings.Default.malUser;
            tb_MALpass.Text = Settings.Default.malPass;

            tb_User.Text = Settings.Default.redditUser;
            tb_Pass.Text = Settings.Default.redditPass;
            bt_Logout.Enabled = Settings.Default.redditEnabled;

        }

        private void b_Login_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Settings.Default.redditUser) && !String.IsNullOrEmpty(Settings.Default.redditPass))
            {
                Settings.Default.redditUser = tb_User.Text;
                Settings.Default.redditPass = tb_Pass.Text;
                Settings.Default.redditEnabled = true;

            }
            else
            {
                Settings.Default.redditEnabled = false;
                MessageBox.Show("Provide valid login information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Settings.Default.Save();
        }

        private void bt_Logout_Click(object sender, EventArgs e)
        {
            Settings.Default.redditUser = tb_User.Text = "";
            Settings.Default.redditPass = tb_Pass.Text = "";
            Settings.Default.redditEnabled = false;

            Settings.Default.Save();
        }
    }
}
