using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class EnabledCommandsWindow : Form
    {
        public EnabledCommandsWindow()
        {
            InitializeComponent();
            cb_Kill.Checked = Settings.Default.killEnabled;
            cb_Anime.Checked = Settings.Default.aniSearchEnabled;
            cb_randomText.Checked = Settings.Default.randomTextEnabled;
            cb_Wiki.Checked = Settings.Default.wikiEnabled;
            cb_Poke.Checked = Settings.Default.pokeEnabled;
            cb_vimeo.Checked = Settings.Default.vimeoEnabled;
            cb_assigns.Checked = Settings.Default.assign_Enabled;
            cb_claims.Checked = Settings.Default.claims_Enabled;
            cb_eta.Checked = Settings.Default.eta_Enabled;
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
        }


        private void button1_Click(object sender, EventArgs e)
        {
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
            Settings.Default.eta_Enabled = cb_eta.Checked;
            Settings.Default.help_Enabled = cb_help.Checked;
            Settings.Default.rules_Enabled = cb_rules.Checked;
            Settings.Default.roll_Enabled = cb_roll.Checked;
            Settings.Default.hello_Enabled = cb_hello.Checked;
            Settings.Default.assign_Enabled = cb_assigns.Checked;
            Settings.Default.claims_Enabled = cb_claims.Checked;
            Settings.Default.youtube_Enabled = cb_youtube.Checked;
            Settings.Default.conversionEnabled = cb_Convert.Checked;
            Settings.Default.nickEnabled = cb_nicks.Checked;
            Settings.Default.aniSearchEnabled = cb_Anime.Checked;

            Settings.Default.Save();
            this.Close();
        }

    }
}
