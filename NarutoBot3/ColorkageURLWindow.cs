using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class ColorkageURLs : Form
    {
        public ColorkageURLs()
        {
            InitializeComponent();
            assignmentsBox.Text = Settings.Default.currentAssignmentURL;
            claimsBox.Text = Settings.Default.currentClaimsURL;

        }

        private void assignments_Load(object sender, EventArgs e)
        {
            assignmentsBox.Text = Settings.Default.currentAssignmentURL;
            claimsBox.Text = Settings.Default.currentClaimsURL;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.currentAssignmentURL = assignmentsBox.Text;
            Settings.Default.currentClaimsURL = claimsBox.Text;
            Settings.Default.Save();
            this.Close();
        }
    }
}
