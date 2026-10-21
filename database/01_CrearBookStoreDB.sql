/*
    PAP.BookStore - Base de datos y tabla Books inicial.

    Ejecutar este archivo antes de 02_AmpliarBookStoreDB.sql.
    Omite la base y la tabla si ya existen; conserva sus datos.
*/

USE [master];
GO

IF DB_ID(N'BookStoreDB') IS NULL
BEGIN
    CREATE DATABASE [BookStoreDB];
END;
GO

USE [BookStoreDB];
GO

IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Books]
    (
        [Id] INT IDENTITY(1, 1) NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [PublicationDate] DATE NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL
            CONSTRAINT [DF_Books_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL
            CONSTRAINT [DF_Books_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Books] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Books_Price] CHECK ([Price] >= 0)
    );
END;
GO
