using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4_Music_playeer_Framework
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void guna2TextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            guna2TextBox1.Text = "";
        }

        private void guna2TextBox2_MouseClick(object sender, MouseEventArgs e)
        {
            guna2TextBox2.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "admin" && guna2TextBox2.Text == "admin")
            {
                MessageBox.Show("Авторизция прошла успешно", "Успешно",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                //this.Close();
            }
        }
    }
}
