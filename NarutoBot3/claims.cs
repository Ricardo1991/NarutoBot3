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
    public partial class claims : Form
    {
        public claims()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default.currentClaimsURL;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.currentClaimsURL = textBox1.Text;
            this.Close();
        }
    }
}
