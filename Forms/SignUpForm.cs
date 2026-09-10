using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CampusOneDigitalAcademy.Data;

namespace CampusOneDigitalAcademy.Forms
{
    public partial class SignUpForm : Form
    {
        private readonly WelcomeForm _parentWelcomeForm;

        public SignUpForm(WelcomeForm parentWelcomeForm = null)
        {
            InitializeComponent();
            _parentWelcomeForm = parentWelcomeForm;
        }

        private void SignUpForm_Load(object sender, EventArgs e)
        {
            LoadLogoImage();
            txtUsername.Focus();
        }

        private void LoadLogoImage()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string[] possiblePaths = new string[]
                {
                    Path.Combine(basePath, "Assets", "logo.png"),
                    Path.Combine(basePath, "Assets", "logo.jpg"),
                    Path.Combine(basePath, "..", "..", "Assets", "logo.png"),
                    Path.Combine(basePath, "..", "..", "Assets", "logo.jpg"),
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
                // Fallback gracefully if logo file is unavailable
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool showPassword = chkShowPassword.Checked;
            txtPassword.UseSystemPasswordChar = !showPassword;
            txtConfirmPassword.UseSystemPasswordChar = !showPassword;
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // 1. Validation - Empty Checks
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show(
                    "Please enter a Username to create your account.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtUsername.Focus();
                return;
            }

            if (username.Length < 3)
            {
                MessageBox.Show(
                    "Username must be at least 3 characters long.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Please enter a Password.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtPassword.Focus();
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show(
                    "Password must be at least 4 characters long for security.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show(
                    "Please confirm your Password.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtConfirmPassword.Focus();
                return;
            }

            // 2. Validation - Password Matching
            if (!string.Equals(password, confirmPassword))
            {
                MessageBox.Show(
                    "Password and Confirm Password do not match. Please verify and try again.",
                    "Password Mismatch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtConfirmPassword.Clear();
                txtConfirmPassword.Focus();
                return;
            }

            // 3. Database & In-Memory Registration
            string errorMessage;
            bool isRegistered = DbHelper.RegisterUser(username, password, "User", out errorMessage);

            if (isRegistered)
            {
                MessageBox.Show(
                    "Account created successfully!\nYou can now log in with your new credentials.",
                    "Sign Up Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Seamlessly open LoginForm with the new username pre-filled
                LoginForm loginForm = new LoginForm(_parentWelcomeForm, username);
                this.Hide();
                loginForm.Show();
            }
            else
            {
                MessageBox.Show(
                    errorMessage,
                    "Sign Up Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtUsername.Focus();
                txtUsername.SelectAll();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            chkShowPassword.Checked = false;
            txtUsername.Focus();
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

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm(_parentWelcomeForm);
            this.Hide();
            loginForm.Show();
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

        private void SignUpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
