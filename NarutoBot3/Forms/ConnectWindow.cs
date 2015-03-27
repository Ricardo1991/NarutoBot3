using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class ConnectWindow : Form
    {
        public bool gotData { get; set; } 
        public ConnectWindow()
        {
            InitializeComponent();

            t_Server.Text = Settings.Default.Server;
            t_Channel.Text = Settings.Default.Channel;
            t_BotNick.Text = Settings.Default.Nick;
            t_port.Text = Settings.Default.Port;
            t_RealName.Text = Settings.Default.RealName;

            cb_silence.Checked = Settings.Default.silence;
        }

        private void b_Conect_Click(object sender, EventArgs e)
        {
            Settings.Default.Channel = t_Channel.Text;
            Settings.Default.Nick = t_BotNick.Text;
            Settings.Default.Server = t_Server.Text;
            Settings.Default.Port = t_port.Text;
            Settings.Default.RealName = t_RealName.Text;
            Settings.Default.silence = cb_silence.Checked;
            Settings.Default.Save();


            //do connect after this
            this.DialogResult=System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void t_Channel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Settings.Default.Channel = t_Channel.Text;
                Settings.Default.Nick = t_BotNick.Text;
                Settings.Default.Server = t_Server.Text;
                Settings.Default.Port = t_port.Text;
                Settings.Default.RealName = t_RealName.Text;
                Settings.Default.Save();


                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_port_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Settings.Default.Channel = t_Channel.Text;
                Settings.Default.Nick = t_BotNick.Text;
                Settings.Default.Server = t_Server.Text;
                Settings.Default.Port = t_port.Text;
                Settings.Default.RealName = t_RealName.Text;
                Settings.Default.Save();


                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }

        private void t_Server_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Settings.Default.Channel = t_Channel.Text;
                Settings.Default.Nick = t_BotNick.Text;
                Settings.Default.Server = t_Server.Text;
                Settings.Default.Port = t_port.Text;
                Settings.Default.RealName = t_RealName.Text;
                Settings.Default.Save();


                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_BotNick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Settings.Default.Channel = t_Channel.Text;
                Settings.Default.Nick = t_BotNick.Text;
                Settings.Default.Server = t_Server.Text;
                Settings.Default.Port = t_port.Text;
                Settings.Default.RealName = t_RealName.Text;
                Settings.Default.Save();


                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_RealName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Settings.Default.Channel = t_Channel.Text;
                Settings.Default.Nick = t_BotNick.Text;
                Settings.Default.Server = t_Server.Text;
                Settings.Default.Port = t_port.Text;
                Settings.Default.RealName = t_RealName.Text;
                Settings.Default.Save();


                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
    }
}
