using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NarutoBot3
{
    public partial class BotOperatorWindow : Form
    {
        private List<string> ops = new List<string>();
        private UserList ul = new UserList();

        public BotOperatorWindow(ref UserList ul)
        {
            InitializeComponent();

            this.ul = ul;
            updateList();
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            bool foundRepeated = false;

            foreach (string op in ops)
            {
                if (op == t_operator.Text)
                    foundRepeated = true;
            }

            if (!foundRepeated)
            {
                ul.SetUserOperatorStatus(t_operator.Text, true);
                updateList();
            }
            else
                MessageBox.Show("User is already an operator", "Duplicated", MessageBoxButtons.OK);

            ul.SaveData();
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            ul.SetUserOperatorStatus(listOperators.SelectedItem.ToString(), false);
            updateList();
            ul.SaveData();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            t_operator.Text = "";
            this.Close();
            ul.SaveData();
        }

        private void operators_Shown(object sender, EventArgs e)
        {
            updateList();
        }

        private void updateList()
        {
            ops = new List<string>();

            foreach (User u in ul.Users)
            {
                if (u.IsOperator)
                    ops.Add(u.Nick);
            }

            listOperators.DataSource = ops;
        }
    }
}