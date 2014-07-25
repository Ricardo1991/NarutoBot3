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
    public partial class assignments : Form
    {
        public assignments()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default.currentAssignmentURL;

        }

        private void assignments_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.currentAssignmentURL = textBox1.Text;
            this.Close();
        }
    }
}
