using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CampusOneDigitalAcademy.Data;

namespace CampusOneDigitalAcademy.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
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
                // Fallback: Continue smoothly even if image asset is unavailable
            }
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
            txtUsername.Focus();
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
    }
}
