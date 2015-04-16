using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MutedUsersWindow : Form
    {
        List<string> mute = new List<string>();
        UserList ul = new UserList();

        public MutedUsersWindow(ref UserList ul)
        {
            InitializeComponent();

            this.ul = ul;
            updateList();
        }

        private void bMute_Click(object sender, EventArgs e)
        {
            bool foundRepeated = false;

            foreach (string ba in mute)
            {
                if (ba == t_muted.Text)
                    foundRepeated = true;
            }

            if (!foundRepeated)
            {
                ul.opUser(t_muted.Text);
                updateList();
            }

            else
                MessageBox.Show("User is already Muted", "Duplicated", MessageBoxButtons.OK);

            ul.saveData();
        }

        private void bUnmute_Click(object sender, EventArgs e)
        {
            ul.unmuteUser(listMuted.SelectedItem.ToString());
            updateList();
            ul.saveData();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            t_muted.Text = "";
            this.Close();
            ul.saveData();
        }

        private void muted_Shown(object sender, EventArgs e)
        {
            updateList();
        }

        private void updateList()
        {
            mute = new List<string>();

            foreach (User u in ul.Users)
            {
                if (u.IsMuted)
                    mute.Add(u.Nick);
            }

            listMuted.DataSource = mute;
        }
    }
}
