# Campus One Digital Academy — Final Project Report & Code Documentation Guide
**Qualification:** Pearson BTEC Level 3 Diploma in Information Technology  
**Unit:** Final Project / Programming  
**System:** Campus One Digital Academy Student Management System  
**Technology:** C# .NET Windows Forms, Microsoft SQL Server, ADO.NET  

---

## 1. Requirements Traceability Matrix

| # | Specification / Brief Requirement | Implementation Location | Method / Handler |
|---|---|---|---|
| 1 | **Login Form with Controls**: PictureBox (Logo), GroupBox, 3 Labels, 2 TextBoxes, 3 Buttons | `Forms/LoginForm.Designer.cs` | `InitializeComponent()` |
| 2 | **Hardcoded Login Authentication**: Credentials `Admin` / `Campusone@123` | `Forms/LoginForm.cs` | `btnLogin_Click` |
| 3 | **Database-backed Login Extension**: Multi-user credentials from `Users` table | `Data/DbHelper.cs` | `ValidateLogin()` |
| 4 | **Login Failure Message**: Error dialog `"Invalid Login credentials, please check Username and Password and try again"` | `Forms/LoginForm.cs` | `btnLogin_Click` |
| 5 | **Login Clear Button**: Empties username & password, focuses username textbox | `Forms/LoginForm.cs` | `btnClear_Click` |
| 6 | **Login Exit Button**: Prompts `"Are you sure, Do you really want to Exit...?"` | `Forms/LoginForm.cs` | `btnExit_Click` |
| 7 | **Registration Form Controls**: 4 GroupBoxes, 13 Labels, 9 TextBoxes, 4 Buttons, 2 LinkLabels, 2 RadioButtons, 1 DateTimePicker, 1 ComboBox | `Forms/RegistrationForm.Designer.cs` | `InitializeComponent()` |
| 8 | **Database Table Design**: `Student` DB, `Registration` table with `regNo` as Primary Key | `Database/CreateDatabase.sql` | `CREATE TABLE Registration` |
| 9 | **Student Registration (Insert)**: Inserts record, displays `"Record Added Successfully"`, updates ComboBox | `Forms/RegistrationForm.cs` & `Data/DbHelper.cs` | `btnRegister_Click` & `InsertRegistration()` |
| 10 | **Student Record Update**: Updates record by `regNo`, displays `"Record Updated Successfully"` | `Forms/RegistrationForm.cs` & `Data/DbHelper.cs` | `btnUpdate_Click` & `UpdateRegistration()` |
| 11 | **Student Record Delete**: Prompts `"Are you sure, Do you really want to Delete this Record...?"`, deletes record, displays `"Record Deleted Successfully"` | `Forms/RegistrationForm.cs` & `Data/DbHelper.cs` | `btnDelete_Click` & `DeleteRegistration()` |
| 12 | **Registration Form Clear**: Resets all 9 textboxes, combobox, datepicker, and radio buttons | `Forms/RegistrationForm.cs` | `btnClear_Click` & `ResetFormFields()` |
| 13 | **Search & Auto-fill via ComboBox**: Selecting `regNo` auto-populates all student fields | `Forms/RegistrationForm.cs` & `Data/DbHelper.cs` | `cboRegNo_SelectedIndexChanged` & `GetRegistrationByRegNo()` |
| 14 | **Logout Link**: Prompts confirmation and redirects back to `LoginForm` | `Forms/RegistrationForm.cs` | `lnkLogout_LinkClicked` |
| 15 | **Exit Link**: Prompts confirmation and exits application | `Forms/RegistrationForm.cs` | `lnkExit_LinkClicked` |

---

## 2. Database Design & SQL Scripts

### 2.1 Table Schema: `Registration` (Primary Table)

```sql
CREATE TABLE Registration (
    regNo       INT IDENTITY(1,1) NOT NULL,
    firstName   VARCHAR(50)       NULL,
    lastName    VARCHAR(50)       NULL,
    dateOfBirth DATETIME          NULL,
    gender      VARCHAR(50)       NULL,
    address     VARCHAR(50)       NULL,
    email       VARCHAR(50)       NULL,
    mobilePhone INT               NULL,
    homePhone   INT               NULL,
    parentName  VARCHAR(50)       NULL,
    nic         VARCHAR(50)       NULL,
    contactNo   INT               NULL,
    CONSTRAINT PK_Registration PRIMARY KEY CLUSTERED (regNo ASC)
);
```

### 2.2 Table Schema: `Users` (Extension Table for Bonus Marks)

```sql
CREATE TABLE Users (
    userId   INT IDENTITY(1,1) NOT NULL,
    username VARCHAR(50)       NOT NULL UNIQUE,
    password VARCHAR(100)      NOT NULL,
    role     VARCHAR(50)       NULL DEFAULT 'Admin',
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (userId ASC)
);
```

### 2.3 Executing SQL Script in SSMS (SQL Server Management Studio)
1. Launch **SQL Server Management Studio (SSMS)**.
2. Connect to your local SQL Server instance (e.g., `.` or `.\SQLEXPRESS` or `(localdb)\MSSQLLocalDB`).
3. Click **File → Open → File...** and select `Database/CreateDatabase.sql`.
4. Click **Execute** (or press `F5`).
5. Verify that `Student` database and tables `Registration` and `Users` appear in the Object Explorer.

---

## 3. Key Code Snippets for Assignment Word Document

### 3.1 Data Access Layer: `DbHelper.cs`

#### Database Connection & Parameterized Insert Query
```csharp
public static int InsertRegistration(StudentRegistration student)
{
    string query = @"
        INSERT INTO Registration (
            firstName, lastName, dateOfBirth, gender, address, 
            email, mobilePhone, homePhone, parentName, nic, contactNo
        )
        VALUES (
            @firstName, @lastName, @dateOfBirth, @gender, @address, 
            @email, @mobilePhone, @homePhone, @parentName, @nic, @contactNo
        );
        SELECT SCOPE_IDENTITY();";

    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
    {
        conn.Open();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.Add("@firstName", SqlDbType.VarChar, 50).Value = (object)student.FirstName ?? DBNull.Value;
            cmd.Parameters.Add("@lastName", SqlDbType.VarChar, 50).Value = (object)student.LastName ?? DBNull.Value;
            cmd.Parameters.Add("@dateOfBirth", SqlDbType.DateTime).Value = student.DateOfBirth;
            cmd.Parameters.Add("@gender", SqlDbType.VarChar, 50).Value = (object)student.Gender ?? DBNull.Value;
            cmd.Parameters.Add("@address", SqlDbType.VarChar, 50).Value = (object)student.Address ?? DBNull.Value;
            cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = (object)student.Email ?? DBNull.Value;
            cmd.Parameters.Add("@mobilePhone", SqlDbType.Int).Value = student.MobilePhone;
            cmd.Parameters.Add("@homePhone", SqlDbType.Int).Value = student.HomePhone;
            cmd.Parameters.Add("@parentName", SqlDbType.VarChar, 50).Value = (object)student.ParentName ?? DBNull.Value;
            cmd.Parameters.Add("@nic", SqlDbType.VarChar, 50).Value = (object)student.NIC ?? DBNull.Value;
            cmd.Parameters.Add("@contactNo", SqlDbType.Int).Value = student.ContactNo;

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
```

#### Parameterized Update Query
```csharp
public static bool UpdateRegistration(StudentRegistration student)
{
    string query = @"
        UPDATE Registration
        SET 
            firstName   = @firstName,
            lastName    = @lastName,
            dateOfBirth = @dateOfBirth,
            gender      = @gender,
            address     = @address,
            email       = @email,
            mobilePhone = @mobilePhone,
            homePhone   = @homePhone,
            parentName  = @parentName,
            nic         = @nic,
            contactNo   = @contactNo
        WHERE regNo = @regNo";

    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
    {
        conn.Open();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@regNo", student.RegNo);
            AddRegistrationParameters(cmd, student);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
```

#### Parameterized Delete Query
```csharp
public static bool DeleteRegistration(int regNo)
{
    string query = "DELETE FROM Registration WHERE regNo = @regNo";

    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
    {
        conn.Open();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@regNo", regNo);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
```

---

### 3.2 Login Form Logic: `LoginForm.cs`

```csharp
private void btnLogin_Click(object sender, EventArgs e)
{
    string username = txtUsername.Text.Trim();
    string password = txtPassword.Text;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        MessageBox.Show("Please enter both Username and Password to proceed.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtUsername.Focus();
        return;
    }

    string userRole;
    bool isAuthenticated = DbHelper.ValidateLogin(username, password, out userRole);

    if (isAuthenticated)
    {
        RegistrationForm regForm = new RegistrationForm(this, username, userRole);
        this.Hide();
        regForm.Show();
        txtPassword.Clear();
    }
    else
    {
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
```

---

### 3.3 Student Registration Logic: `RegistrationForm.cs`

#### Search & Auto-Fill from Dropdown
```csharp
private void cboRegNo_SelectedIndexChanged(object sender, EventArgs e)
{
    if (_isPopulatingFromSearch || cboRegNo.SelectedIndex == -1) return;

    string selectedItemText = cboRegNo.SelectedItem != null ? cboRegNo.SelectedItem.ToString() : string.Empty;
    int selectedRegNo;
    if (!int.TryParse(selectedItemText, out selectedRegNo)) return;

    try
    {
        StudentRegistration student = DbHelper.GetRegistrationByRegNo(selectedRegNo);
        if (student != null)
        {
            PopulateFields(student);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error retrieving student record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

#### Register Student Button Handler
```csharp
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

    if (!ValidateInputFields()) return;

    try
    {
        StudentRegistration student = BuildStudentFromForm(0);
        int newRegNo = DbHelper.InsertRegistration(student);

        if (newRegNo > 0)
        {
            MessageBox.Show("Record Added Successfully", "Register Student", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshRegNoList(newRegNo);
            ResetFormFields();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Database Error while inserting record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

#### Delete Student Button Handler with Confirmation
```csharp
private void btnDelete_Click(object sender, EventArgs e)
{
    string selectedItemText = cboRegNo.SelectedItem != null ? cboRegNo.SelectedItem.ToString() : string.Empty;
    int selectedRegNo;
    if (cboRegNo.SelectedIndex == -1 || !int.TryParse(selectedItemText, out selectedRegNo))
    {
        MessageBox.Show("Please select a Registration Number from the dropdown to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Record Deleted Successfully", "Delete Student", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshRegNoList(0);
                ResetFormFields();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database Error while deleting record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

---

## 4. Test Cases & Evidence Table

| Test Case ID | Test Scenario | Test Input | Expected Result | Actual Result | Status |
|---|---|---|---|---|---|
| **TC-01** | Valid Login Authentication | Username: `Admin`<br>Password: `Campusone@123` | Login successful; `LoginForm` hides; `RegistrationForm` opens | As Expected | **PASS** |
| **TC-02** | Invalid Username / Password | Username: `User1`<br>Password: `wrongpass` | Error MessageBox displays; textboxes cleared; focus to username | As Expected | **PASS** |
| **TC-03** | Clear Button on Login Form | Text in both fields | Textboxes emptied; focus restored to `txtUsername` | As Expected | **PASS** |
| **TC-04** | Exit Button on Login Form | Click Exit button | Confirmation dialog displayed; application closes on 'Yes' | As Expected | **PASS** |
| **TC-05** | Insert New Student Record | Full student details filled | Record inserted into SQL DB; Success message; RegNo combo refreshed | As Expected | **PASS** |
| **TC-06** | Search Student via RegNo ComboBox | Select RegNo `1` | All corresponding student fields auto-populated accurately | As Expected | **PASS** |
| **TC-07** | Update Existing Student | Modify student Address & Phone | Record updated in SQL DB; Success message displayed | As Expected | **PASS** |
| **TC-08** | Delete Student with Confirmation | Select RegNo and click Delete; choose 'Yes' | Confirmation dialog shown; record deleted from DB; Success message; fields reset | As Expected | **PASS** |
| **TC-09** | Form Input Validation | Leave First Name or Address blank | Warning dialog displayed indicating required field | As Expected | **PASS** |
| **TC-10** | Logout Link Action | Click 'Logout' link; confirm 'Yes' | `RegistrationForm` hides; `LoginForm` is redisplayed | As Expected | **PASS** |

---

## 5. Report Formatting Checklist

When creating your submission Word document:
- **Font:** Times New Roman, 12 pt for body text.
- **Line Spacing:** 1.15 lines.
- **Paragraph Alignment:** Justified.
- **Headings:** Heading 1 (16 pt Bold), Heading 2 (14 pt Bold), Heading 3 (12 pt Bold).
- **Screenshots:** Insert clear, cropped screenshots with captions (e.g., *Figure 1: Login Form with Academy Logo*, *Figure 2: Student Registration Form with Auto-Filled Record*).
- **Assignment Brief:** Place the original assignment brief at the very beginning of the document.
