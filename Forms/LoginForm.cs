using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CampusOneDigitalAcademy.Data;

namespace CampusOneDigitalAcademy.Forms
{
    public partial class LoginForm : Form
    {
        private readonly WelcomeForm _parentWelcomeForm;
        private readonly string _initialUsername;

        public LoginForm(WelcomeForm parentWelcomeForm = null, string prefilledUsername = null)
        {
            InitializeComponent();
            _parentWelcomeForm = parentWelcomeForm;
            _initialUsername = prefilledUsername;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadLogoImage();

            if (!string.IsNullOrEmpty(_initialUsername))
            {
                txtUsername.Text = _initialUsername;
                txtPassword.Focus();
            }
            else
            {
                txtUsername.Focus();
            }
        }

        private void LoadLogoImage()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string[] possiblePaths = new string[]
                {
                    Path.Combine(basePath, "Assets", "logo.jpeg"),
                    Path.Combine(basePath, "Assets", "logo.png"),
                    Path.Combine(basePath, "Assets", "logo.jpg"),
                    Path.Combine(basePath, "..", "..", "Assets", "logo.jpeg"),
                    Path.Combine(basePath, "..", "..", "Assets", "logo.png"),
                    Path.Combine(basePath, "..", "..", "Assets", "logo.jpg"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.jpeg"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.jpg")
                };

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        using (var img = Image.FromFile(path))
                        {
                            picLogo.Image = new Bitmap(img);
                        }
                        break;
                    }
                }
            }
            catch
            {
                // Fallback: Continue smoothly even if image asset is unavailable
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Please enter both Username and Password to proceed.",
                    "Input Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtUsername.Focus();
                return;
            }

            string userRole;
            bool isAuthenticated = DbHelper.ValidateLogin(username, password, out userRole);

            if (isAuthenticated)
            {
                // Successful login
                RegistrationForm regForm = new RegistrationForm(this, username, userRole);
                this.Hide();
                regForm.Show();
                txtPassword.Clear();
            }
            else
            {
                // Failed login per assignment brief specification
                MessageBox.Show(
                    "Invalid Login credentials, please check Username and Password and try again",
                    "Invalid Login Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                btnClear_Click(sender, e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            chkShowPassword.Checked = false;
            txtUsername.Focus();
        }

        private void lnkSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUpForm signUpForm = new SignUpForm(_parentWelcomeForm);
            this.Hide();
            signUpForm.Show();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (_parentWelcomeForm != null && !_parentWelcomeForm.IsDisposed)
            {
                _parentWelcomeForm.Show();
            }
            else
            {
                WelcomeForm welcome = new WelcomeForm();
                welcome.Show();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                "Are you sure, Do you really want to Exit...?",
                "Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
