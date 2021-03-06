﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class MutedUsersWindow : Form
    {
        private List<string> mute = new List<string>();
        private UserList ul = new UserList();

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
                ul.SetUserMuteStatus(t_muted.Text, true);
                updateList();
            }
            else
                MessageBox.Show("User is already Muted", "Duplicated", MessageBoxButtons.OK);

            ul.SaveData();
        }

        private void bUnmute_Click(object sender, EventArgs e)
        {
            ul.SetUserMuteStatus(listMuted.SelectedItem.ToString(), false);
            updateList();
            ul.SaveData();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            t_muted.Text = "";
            this.Close();
            ul.SaveData();
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