using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CampusOneDigitalAcademy.Forms
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void WelcomeForm_Load(object sender, EventArgs e)
        {
            LoadLogoImage();
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
                // Smooth fallback if asset is unavailable
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm(this);
            this.Hide();
            loginForm.Show();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SignUpForm signUpForm = new SignUpForm(this);
            this.Hide();
            signUpForm.Show();
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

        private void WelcomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
