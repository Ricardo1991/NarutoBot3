using NarutoBot3.Properties;
using System;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class ConnectWindow : Form
    {
        private bool alreadyConencted = false;

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
            if (!ValidateFields())
                return;

            if (alreadyConencted)
            {
                DialogResult resultWarning = MessageBox.Show("This bot is already connected.\nDo you want to end the current connection?",
                    "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (resultWarning == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }
            }

            Save();
            this.DialogResult = DialogResult.OK;
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
                Save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_port_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Save();

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
                Save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_BotNick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void t_RealName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Save();

                //do connect after this
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void Save()
        {
            Settings.Default.Channel = t_Channel.Text;
            Settings.Default.Nick = t_BotNick.Text;
            Settings.Default.Server = t_Server.Text;
            Settings.Default.Port = t_port.Text;
            Settings.Default.RealName = t_RealName.Text;
            Settings.Default.silence = cb_silence.Checked;

            Settings.Default.Save();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.Channel) ||
               string.IsNullOrWhiteSpace(Settings.Default.Server) ||
               string.IsNullOrWhiteSpace(Settings.Default.Nick) ||
               Convert.ToInt32(Settings.Default.Port) <= 0 ||
               Convert.ToInt32(Settings.Default.Port) > 65535)
            {
                MessageBox.Show("Please fill all the fields with correct information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}