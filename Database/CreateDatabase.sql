-- ==========================================================
-- Campus One Digital Academy - SQL Server Database Script
-- Database Name: Student
-- Created for: L3 Diploma in IT Final Project
-- ==========================================================

-- 1. Create the Database if it does not already exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'Student')
BEGIN
    CREATE DATABASE [Student];
    PRINT 'Database [Student] created successfully.';
END
ELSE
BEGIN
    PRINT 'Database [Student] already exists.';
END
GO

USE [Student];
GO

-- 2. Create the Registration Table (Table 1 from Assignment Brief)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Registration]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Registration] (
        [regNo]       INT IDENTITY(1,1) NOT NULL,
        [firstName]   VARCHAR(50)       NULL,
        [lastName]    VARCHAR(50)       NULL,
        [dateOfBirth] DATETIME          NULL,
        [gender]      VARCHAR(50)       NULL,
        [address]     VARCHAR(50)       NULL,
        [email]       VARCHAR(50)       NULL,
        [mobilePhone] INT               NULL,
        [homePhone]   INT               NULL,
        [parentName]  VARCHAR(50)       NULL,
        [nic]         VARCHAR(50)       NULL,
        [contactNo]   INT               NULL,
        CONSTRAINT [PK_Registration] PRIMARY KEY CLUSTERED ([regNo] ASC)
    );
    PRINT 'Table [dbo].[Registration] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Registration] already exists.';
END
GO

-- 3. Create the Users Extension Table (Optional Extension for Bonus Marks)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [userId]   INT IDENTITY(1,1) NOT NULL,
        [username] VARCHAR(50)       NOT NULL UNIQUE,
        [password] VARCHAR(100)      NOT NULL,
        [role]     VARCHAR(50)       NULL DEFAULT 'Admin',
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([userId] ASC)
    );
    PRINT 'Table [dbo].[Users] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Users] already exists.';
END
GO

-- 4. Seed Default Administrator Account
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [username] = 'Admin')
BEGIN
    INSERT INTO [dbo].[Users] ([username], [password], [role])
    VALUES ('Admin', 'Campusone@123', 'System Administrator');
    PRINT 'Default Admin user inserted.';
END
GO

-- 5. Seed Sample Registration Data for Testing
IF NOT EXISTS (SELECT 1 FROM [dbo].[Registration])
BEGIN
    INSERT INTO [dbo].[Registration] ([firstName], [lastName], [dateOfBirth], [gender], [address], [email], [mobilePhone], [homePhone], [parentName], [nic], [contactNo])
    VALUES 
    ('Kasun', 'Perera', '2004-05-14', 'Male', '124 Galle Road, Colombo 03', 'kasun.perera@campusone.lk', 771234567, 112345678, 'Sunil Perera', '197512304567', 712345678),
    ('Nimali', 'Fernando', '2005-09-22', 'Female', '45 Kandy Road, Kiribathgoda', 'nimali.f@gmail.com', 718765432, 119876543, 'Kamal Fernando', '197854601234', 778901234),
    ('Arun', 'Kumar', '2003-11-08', 'Male', '78 Temple Road, Jaffna', 'arun.k@yahoo.com', 763456789, 212223334, 'Rajendran Kumar', '197289001234', 701234567);
    PRINT 'Sample Registration records inserted successfully.';
END
GO

-- 6. Verification Queries
SELECT '--- Registered Students ---' AS [Title];
SELECT * FROM [dbo].[Registration];

SELECT '--- System Users ---' AS [Title];
SELECT * FROM [dbo].[Users];
GO
