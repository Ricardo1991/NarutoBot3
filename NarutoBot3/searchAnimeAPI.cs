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
    public partial class searchAnimeAPI : Form
    {
        public searchAnimeAPI()
        {
            InitializeComponent();
        }

        private void searchAnimeAPI_Shown(object sender, EventArgs e)
        {
            tb_API.Text = Settings.Default.apikey;
            tb_cx.Text = Settings.Default.cxKey;
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            Settings.Default.apikey = tb_API.Text;
            Settings.Default.cxKey = tb_cx.Text;
            Settings.Default.Save();
            this.Close();
        }
    }
}
