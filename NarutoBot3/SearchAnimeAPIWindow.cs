using System;
using System.Windows.Forms;
using NarutoBot3.Properties;

namespace NarutoBot3
{
    public partial class SearchAnimeAPIWindow : Form
    {
        public SearchAnimeAPIWindow()
        {
            InitializeComponent();
        }

        private void searchAnimeAPI_Shown(object sender, EventArgs e)
        {
            tb_API.Text = Settings.Default.apikey;
            tb_cx.Text = Settings.Default.cxKey;
            tb_User.Text = Settings.Default.malUser;
            tb_Pass.Text = Settings.Default.malPass;
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            Settings.Default.apikey = tb_API.Text;
            Settings.Default.cxKey = tb_cx.Text;
            Settings.Default.malPass = tb_Pass.Text;
            Settings.Default.malUser = tb_User.Text;
            Settings.Default.Save();
            this.Close();
        }
    }
}
