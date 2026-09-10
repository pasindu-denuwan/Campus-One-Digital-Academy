using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CampusOneDigitalAcademy.Data;
using CampusOneDigitalAcademy.Models;

namespace CampusOneDigitalAcademy.Forms
{
    public partial class RegistrationForm : Form
    {
        private readonly LoginForm _parentLoginForm;
        private readonly string _currentUsername;
        private readonly string _currentUserRole;
        private bool _isPopulatingFromSearch = false;

        public RegistrationForm(LoginForm parentLoginForm = null, string username = "Admin", string role = "Administrator")
        {
            InitializeComponent();
            _parentLoginForm = parentLoginForm;
            _currentUsername = username;
            _currentUserRole = role;
        }

        #region Form Lifecycle

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            RefreshRegNoList(0);
            ResetFormFields();
        }

        private void RegistrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        #endregion

        #region Data Loading & Search

        /// <summary>
        /// Populates the RegNo ComboBox from the SQL Server Registration table.
        /// </summary>
        private void RefreshRegNoList(int selectedRegNo)
        {
            try
            {
                _isPopulatingFromSearch = true;
                cboRegNo.Items.Clear();

                List<int> regNos = DbHelper.GetAllRegNos();
                foreach (int regNo in regNos)
                {
                    cboRegNo.Items.Add(regNo.ToString());
                }

                if (selectedRegNo > 0 && cboRegNo.Items.Contains(selectedRegNo.ToString()))
                {
                    cboRegNo.SelectedItem = selectedRegNo.ToString();
                }
                else
                {
                    cboRegNo.SelectedIndex = -1;
                    cboRegNo.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading Registration Numbers: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _isPopulatingFromSearch = false;
            }
        }

        /// <summary>
        /// Triggered when the user selects a Reg No in the ComboBox. Auto-fills all form controls.
        /// </summary>
        private void cboRegNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isPopulatingFromSearch || cboRegNo.SelectedIndex == -1)
            {
                return;
            }

            string selectedItemText = cboRegNo.SelectedItem != null ? cboRegNo.SelectedItem.ToString() : string.Empty;
            int selectedRegNo;
            if (!int.TryParse(selectedItemText, out selectedRegNo))
            {
                return;
            }

            try
            {
                StudentRegistration student = DbHelper.GetRegistrationByRegNo(selectedRegNo);
                if (student != null)
                {
                    PopulateFields(student);
                }
                else
                {
                    MessageBox.Show(
                        "No student found with Registration Number " + selectedRegNo,
                        "Record Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error retrieving student record: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PopulateFields(StudentRegistration student)
        {
            txtFirstName.Text = student.FirstName != null ? student.FirstName : string.Empty;
            txtLastName.Text = student.LastName != null ? student.LastName : string.Empty;
            
            if (student.DateOfBirth >= dtpDOB.MinDate && student.DateOfBirth <= dtpDOB.MaxDate)
            {
                dtpDOB.Value = student.DateOfBirth;
            }

            if (string.Equals(student.Gender, "Female", StringComparison.OrdinalIgnoreCase))
            {
                rdoFemale.Checked = true;
            }
            else
            {
                rdoMale.Checked = true;
            }

            txtAddress.Text = student.Address != null ? student.Address : string.Empty;
            txtEmail.Text = student.Email != null ? student.Email : string.Empty;
            txtMobilePhone.Text = student.MobilePhone != 0 ? student.MobilePhone.ToString() : string.Empty;
            txtHomePhone.Text = student.HomePhone != 0 ? student.HomePhone.ToString() : string.Empty;
            txtParentName.Text = student.ParentName != null ? student.ParentName : string.Empty;
            txtNIC.Text = student.NIC != null ? student.NIC : string.Empty;
            txtContactNo.Text = student.ContactNo != 0 ? student.ContactNo.ToString() : string.Empty;
        }

        #endregion

        #region CRUD Actions

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // If an existing student is currently selected in the RegNo dropdown, prevent accidental duplicate registration
            if (cboRegNo.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(cboRegNo.Text))
            {
                MessageBox.Show(
                    "An existing student record (Reg No: " + cboRegNo.Text + ") is currently selected.\n\n" +
                    "• To save modifications to this student, click 'Update'.\n" +
                    "• To register a NEW student, click 'Clear' first and enter fresh details.",
                    "Cannot Register Existing Record",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!ValidateInputFields())
            {
                return;
            }

            try
            {
                StudentRegistration student = BuildStudentFromForm(0);
                int newRegNo = DbHelper.InsertRegistration(student);

                if (newRegNo > 0)
                {
                    MessageBox.Show(
                        "Record Added Successfully",
                        "Register Student",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    RefreshRegNoList(newRegNo);
                    ResetFormFields();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to insert record. Please verify database connection.",
                        "Insertion Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database Error while inserting record: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Updates an existing student record in the database.
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string selectedItemText = cboRegNo.SelectedItem != null ? cboRegNo.SelectedItem.ToString() : string.Empty;
            int selectedRegNo;
            if (cboRegNo.SelectedIndex == -1 || !int.TryParse(selectedItemText, out selectedRegNo))
            {
                MessageBox.Show(
                    "Please select a valid Registration Number from the dropdown to update.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                cboRegNo.Focus();
                return;
            }

            if (!ValidateInputFields())
            {
                return;
            }

            try
            {
                StudentRegistration student = BuildStudentFromForm(selectedRegNo);
                bool isUpdated = DbHelper.UpdateRegistration(student);

                if (isUpdated)
                {
                    MessageBox.Show(
                        "Record Updated Successfully",
                        "Update Student",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    RefreshRegNoList(selectedRegNo);
                }
                else
                {
                    MessageBox.Show(
                        "Update failed. Record may no longer exist.",
                        "Update Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database Error while updating record: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Deletes a student registration record with confirmation dialog.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string selectedItemText = cboRegNo.SelectedItem != null ? cboRegNo.SelectedItem.ToString() : string.Empty;
            int selectedRegNo;
            if (cboRegNo.SelectedIndex == -1 || !int.TryParse(selectedItemText, out selectedRegNo))
            {
                MessageBox.Show(
                    "Please select a Registration Number from the dropdown to delete.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                cboRegNo.Focus();
                return;
            }

            DialogResult confirmResult = MessageBox.Show(
                "Are you sure, Do you really want to Delete this Record...?",
                "Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    bool isDeleted = DbHelper.DeleteRegistration(selectedRegNo);
                    if (isDeleted)
                    {
                        MessageBox.Show(
                            "Record Deleted Successfully",
                            "Delete Student",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        RefreshRegNoList(0);
                        ResetFormFields();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Delete failed. Record may not exist.",
                            "Delete Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Database Error while deleting record: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Clears all input controls on the form.
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetFormFields();
        }

        #endregion

        #region Navigation & Window Controls

        private void lnkLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                "Are you sure you want to Log out?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.Yes)
            {
                this.Hide();
                if (_parentLoginForm != null && !_parentLoginForm.IsDisposed)
                {
                    _parentLoginForm.Show();
                }
                else
                {
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                }
            }
        }

        private void lnkExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        #endregion

        #region Helper Methods & Validation

        private void ResetFormFields()
        {
            _isPopulatingFromSearch = true;
            cboRegNo.SelectedIndex = -1;
            cboRegNo.Text = string.Empty;
            _isPopulatingFromSearch = false;

            txtFirstName.Clear();
            txtLastName.Clear();
            dtpDOB.Value = DateTime.Now.AddYears(-18);
            rdoMale.Checked = true;
            rdoFemale.Checked = false;
            txtAddress.Clear();
            txtEmail.Clear();
            txtMobilePhone.Clear();
            txtHomePhone.Clear();
            txtParentName.Clear();
            txtNIC.Clear();
            txtContactNo.Clear();

            txtFirstName.Focus();
        }

        private bool ValidateInputFields()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Please enter the First Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter the Last Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter the Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid Email Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtMobilePhone.Text))
            {
                int mobile;
                if (!int.TryParse(txtMobilePhone.Text.Trim(), out mobile))
                {
                    MessageBox.Show("Mobile Phone must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMobilePhone.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtHomePhone.Text))
            {
                int home;
                if (!int.TryParse(txtHomePhone.Text.Trim(), out home))
                {
                    MessageBox.Show("Home Phone must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHomePhone.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtContactNo.Text))
            {
                int contact;
                if (!int.TryParse(txtContactNo.Text.Trim(), out contact))
                {
                    MessageBox.Show("Parent Contact Number must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return false;
                }
            }

            return true;
        }

        private StudentRegistration BuildStudentFromForm(int regNo)
        {
            int mobile = 0;
            int.TryParse(txtMobilePhone.Text.Trim(), out mobile);

            int home = 0;
            int.TryParse(txtHomePhone.Text.Trim(), out home);

            int contact = 0;
            int.TryParse(txtContactNo.Text.Trim(), out contact);

            return new StudentRegistration
            {
                RegNo = regNo,
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DateOfBirth = dtpDOB.Value,
                Gender = rdoFemale.Checked ? "Female" : "Male",
                Address = txtAddress.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                MobilePhone = mobile,
                HomePhone = home,
                ParentName = txtParentName.Text.Trim(),
                NIC = txtNIC.Text.Trim(),
                ContactNo = contact
            };
        }

        #endregion
    }
}
