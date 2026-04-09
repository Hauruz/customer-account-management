-- ============================================================
-- Customer Account Management — Database Schema
-- Run this script in SSMS or Azure Data Studio
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CustomerAccountManagement')
BEGIN
    CREATE DATABASE CustomerAccountManagement;
END
GO

USE CustomerAccountManagement;
GO

-- ── Clients ──────────────────────────────────────────────────

IF OBJECT_ID(N'[dbo].[Clients]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Clients] (
        [Id]        INT            IDENTITY(1,1)  NOT NULL,
        [Name]      NVARCHAR(200)                 NOT NULL,
        [Email]     NVARCHAR(320)                 NOT NULL,
        [CreatedAt] DATETIME2(7)                  NOT NULL
            CONSTRAINT [DF_Clients_CreatedAt] DEFAULT (GETUTCDATE()),

        CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Clients_Email_Unique'
      AND object_id = OBJECT_ID('[dbo].[Clients]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Clients_Email_Unique]
        ON [dbo].[Clients] ([Email] ASC);
END
GO

-- ── Invoices ─────────────────────────────────────────────────

IF OBJECT_ID(N'[dbo].[Invoices]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Invoices] (
        [Id]               INT             IDENTITY(1,1)  NOT NULL,
        [ClientId]         INT                            NOT NULL,
        [Amount]           DECIMAL(18,2)                  NOT NULL,
        [Currency]         NVARCHAR(10)                   NOT NULL,
        [OriginalFileName] NVARCHAR(500)                  NOT NULL,
        [StoredFileName]   NVARCHAR(500)                  NOT NULL,
        [CreatedAt]        DATETIME2(7)                   NOT NULL
            CONSTRAINT [DF_Invoices_CreatedAt] DEFAULT (GETUTCDATE()),

        CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC),

        CONSTRAINT [FK_Invoices_Clients_ClientId]
            FOREIGN KEY ([ClientId])
            REFERENCES [dbo].[Clients] ([Id])
            ON DELETE NO ACTION
            ON UPDATE NO ACTION
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Invoices_ClientId'
      AND object_id = OBJECT_ID('[dbo].[Invoices]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Invoices_ClientId]
        ON [dbo].[Invoices] ([ClientId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Invoices_Currency'
      AND object_id = OBJECT_ID('[dbo].[Invoices]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Invoices_Currency]
        ON [dbo].[Invoices] ([Currency] ASC);
END
GO

PRINT 'Schema created successfully.';
GO
