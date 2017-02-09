using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class ConnectWindow : Form
    {
        bool alreadyConencted = false;

        public ConnectWindow(bool alreadyConnected)
        {
            InitializeComponent();
            this.alreadyConencted = alreadyConnected;

            t_Server.Text = Settings.Default.Server;
            t_Channel.Text = Settings.Default.Channel;
            t_BotNick.Text = Settings.Default.Nick;
            t_port.Text = Settings.Default.Port;
            t_RealName.Text = Settings.Default.RealName;

            cb_silence.Checked = Settings.Default.silence;
        }

        private void b_Conect_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;

            if (alreadyConencted)
            {
                DialogResult resultWarning = MessageBox.Show("This bot is already connected.\nDo you want to end the current connection?", 
                    "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (resultWarning == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }
                else
                    save();
            }
            else
                save();

            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void t_Channel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_port_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();

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
                save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_BotNick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_RealName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }


        private void save()
        {
            Settings.Default.Channel = t_Channel.Text;
            Settings.Default.Nick = t_BotNick.Text;
            Settings.Default.Server = t_Server.Text;
            Settings.Default.Port = t_port.Text;
            Settings.Default.RealName = t_RealName.Text;
            Settings.Default.silence = cb_silence.Checked;
            Settings.Default.Save();

        }
    }
}