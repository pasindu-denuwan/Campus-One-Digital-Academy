# Campus One Digital Academy — Student Management System

A desktop software application built with **C# Windows Forms** and **Microsoft SQL Server**, developed per the **Pearson BTEC Level 3 Diploma in IT Final Project** specification.

---

## 🏛️ Project Features

- **Authentication System (`LoginForm`):**
  - Institutional branding with Academy logo
  - Hardcoded credential authentication (`Admin` / `Campusone@123`) compliant with assignment requirements
  - Database-backed authentication extension via `Users` table
  - Error dialogs, form clearing, and exit confirmation dialog
- **Student Registration & Management (`RegistrationForm`):**
  - 4 GroupBoxes (**Student Registration**, **Basic Details**, **Contact Details**, **Parent Details**)
  - 13 Labels, 9 TextBoxes, 1 DateTimePicker (DOB), 2 RadioButtons (Gender: Male/Female), 1 Search ComboBox (Reg No)
  - Full CRUD operations: **Register**, **Update**, **Delete** (with confirmation), and **Clear**
  - Real-time search and auto-population when selecting any `Reg No`
  - **Logout** (returns to Login) and **Exit** links with confirmation
- **Architecture & Security:**
  - Strict separation of concerns (`Forms` UI, `DbHelper` Data Access Layer, `Models` Data Models)
  - 100% Parameterized SQL queries (`SqlParameter`) preventing SQL Injection attacks
  - Configuration-driven connection strings in `App.config`

---

## 📂 Project Structure

```
Campus One Digital Academy/
├── CampusOneDigitalAcademy.sln         # Visual Studio / MSBuild Solution
├── CampusOneDigitalAcademy.csproj      # C# Project File (.NET Framework 4.8 / 4.0)
├── App.config                          # SQL Server Connection Strings
├── Program.cs                          # Application Entry Point
├── build_and_run.bat                   # 1-Click Build & Launch Batch Script
├── Models/
│   └── StudentRegistration.cs          # Student Entity Data Model
├── Data/
│   └── DbHelper.cs                     # ADO.NET Parameterized SQL Operations
├── Forms/
│   ├── LoginForm.cs                    # Login Event Handlers & Auth
│   ├── LoginForm.Designer.cs           # Handcrafted UI Controls for Login
│   ├── RegistrationForm.cs             # Registration CRUD & Search Handlers
│   └── RegistrationForm.Designer.cs    # Handcrafted UI Controls for Registration
├── Database/
│   └── CreateDatabase.sql              # Database (Student), Tables (Registration, Users) & Seed Data
├── Assets/
│   ├── logo.png                        # High-Resolution Academy Logo (PNG)
│   └── logo.jpg                        # High-Resolution Academy Logo (JPG)
└── Docs/
    ├── Final_Project_Report_Guide.md   # Complete report guide with code snippets & test tables
    └── screenshots/                    # Screenshot repository for assignment submission
```

---

## 🚀 Setup & Running Instructions

### 1. Database Setup (SQL Server / SSMS)
1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your SQL Server instance (e.g. `.`, `.\SQLEXPRESS`, or `(localdb)\MSSQLLocalDB`).
3. Open and execute `Database/CreateDatabase.sql`.
4. This will create the `Student` database, `Registration` table, `Users` table, and seed initial records.

### 2. Configure Connection String (if needed)
In `App.config`, update the `StudentDbConnection` string if your SQL Server instance uses a specific name:
```xml
<connectionStrings>
    <add name="StudentDbConnection" 
         connectionString="Server=.;Database=Student;Integrated Security=True;TrustServerCertificate=True;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 3. Build & Run
- **Option A (Double-click):** Run `build_and_run.bat`.
- **Option B (Command Line):**
  ```powershell
  & "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" CampusOneDigitalAcademy.csproj /t:Build /p:Configuration=Release
  .\bin\Release\CampusOneDigitalAcademy.exe
  ```

---

## 🔑 Default Credentials

- **Username:** `Admin`
- **Password:** `Campusone@123`

---

## 📄 Documentation for Assignment Submission
See `Docs/Final_Project_Report_Guide.md` for ready-to-copy code snippets, traceability matrix, test cases, and Word report formatting guidelines.
