/*
    PAP.BookStore - Ampliacion de la base de datos.

    Fuente: Programacion_Avanzada_-_Caso_Practico_1.pdf, paginas 5 a 12.
    Requiere BookStoreDB y la tabla dbo.Books creadas en los pasos anteriores.
    Ejecutar el archivo completo desde una consulta conectada a LocalDB.

    ADAPTACIONES POR TEXTO RECORTADO EN EL PDF:
    - Se completan las referencias de las claves foraneas hacia las tablas
      indicadas en sus nombres, utilizando la columna Id como destino.
    - Se completa el filtro de sys.indexes para UX_Books_ISBN.
    - Para las columnas nuevas de Books se eligen estas definiciones:
        PublisherId INT NULL, CategoryId INT NULL, GenreId INT NULL,
        ISBN NVARCHAR(20) NULL, Pages INT NULL,
        Language NVARCHAR(50) NULL, CoverImageUrl NVARCHAR(500) NULL.
      Los tipos, longitudes y nulabilidad de esas columnas no se pueden leer
      completos en el documento. Estas son decisiones de esta implementacion.
    - Las claves foraneas usan NO ACTION, sin eliminacion en cascada.

    No elimina tablas ni datos. Omite los objetos que ya existen.
    No migra objetos existentes que tengan una estructura diferente.
    Solo inserta los roles iniciales que aparecen al final del enunciado.
*/

USE [BookStoreDB];
GO

-- 1. LOOKUP TABLES

IF OBJECT_ID(N'dbo.Authors', N'U') IS NULL
CREATE TABLE [dbo].[Authors]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Biography] NVARCHAR(MAX) NULL,
    [BirthDate] DATE NULL,
    [Country] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Authors] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

IF OBJECT_ID(N'dbo.Publishers', N'U') IS NULL
CREATE TABLE [dbo].[Publishers]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Country] NVARCHAR(100) NULL,
    [Website] NVARCHAR(255) NULL,
    [Email] NVARCHAR(150) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Publishers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
CREATE TABLE [dbo].[Categories]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Categories_Name] UNIQUE ([Name])
);
GO

IF OBJECT_ID(N'dbo.Genres', N'U') IS NULL
CREATE TABLE [dbo].[Genres]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Genres_Name] UNIQUE ([Name])
);
GO

-- 2. BOOK RELATIONSHIPS
-- Las definiciones de estas siete columnas se completaron para este proyecto.

IF COL_LENGTH(N'dbo.Books', N'PublisherId') IS NULL
    ALTER TABLE [dbo].[Books] ADD [PublisherId] INT NULL;

IF COL_LENGTH(N'dbo.Books', N'CategoryId') IS NULL
    ALTER TABLE [dbo].[Books] ADD [CategoryId] INT NULL;

IF COL_LENGTH(N'dbo.Books', N'GenreId') IS NULL
    ALTER TABLE [dbo].[Books] ADD [GenreId] INT NULL;

IF COL_LENGTH(N'dbo.Books', N'ISBN') IS NULL
    ALTER TABLE [dbo].[Books] ADD [ISBN] NVARCHAR(20) NULL;

IF COL_LENGTH(N'dbo.Books', N'Pages') IS NULL
    ALTER TABLE [dbo].[Books] ADD [Pages] INT NULL;

IF COL_LENGTH(N'dbo.Books', N'Language') IS NULL
    ALTER TABLE [dbo].[Books] ADD [Language] NVARCHAR(50) NULL;

IF COL_LENGTH(N'dbo.Books', N'CoverImageUrl') IS NULL
    ALTER TABLE [dbo].[Books] ADD [CoverImageUrl] NVARCHAR(500) NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE [name] = N'FK_Books_Publishers'
      AND [parent_object_id] = OBJECT_ID(N'dbo.Books')
)
    ALTER TABLE [dbo].[Books] ADD CONSTRAINT [FK_Books_Publishers]
        FOREIGN KEY ([PublisherId]) REFERENCES [dbo].[Publishers] ([Id]);

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE [name] = N'FK_Books_Categories'
      AND [parent_object_id] = OBJECT_ID(N'dbo.Books')
)
    ALTER TABLE [dbo].[Books] ADD CONSTRAINT [FK_Books_Categories]
        FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]);

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE [name] = N'FK_Books_Genres'
      AND [parent_object_id] = OBJECT_ID(N'dbo.Books')
)
    ALTER TABLE [dbo].[Books] ADD CONSTRAINT [FK_Books_Genres]
        FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genres] ([Id]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'UX_Books_ISBN'
      AND [object_id] = OBJECT_ID(N'dbo.Books')
)
    CREATE UNIQUE NONCLUSTERED INDEX [UX_Books_ISBN]
        ON [dbo].[Books] ([ISBN]) WHERE [ISBN] IS NOT NULL;
GO

IF OBJECT_ID(N'dbo.BookAuthors', N'U') IS NULL
CREATE TABLE [dbo].[BookAuthors]
(
    [BookId] INT NOT NULL,
    [AuthorId] INT NOT NULL,
    CONSTRAINT [PK_BookAuthors] PRIMARY KEY CLUSTERED ([BookId], [AuthorId]),
    CONSTRAINT [FK_BookAuthors_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [FK_BookAuthors_Authors] FOREIGN KEY ([AuthorId])
        REFERENCES [dbo].[Authors] ([Id])
);
GO

-- 3. CUSTOMERS

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
CREATE TABLE [dbo].[Customers]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(150) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(30) NULL,
    [BirthDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT (1),
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Customers_Email] UNIQUE ([Email])
);
GO

IF OBJECT_ID(N'dbo.Addresses', N'U') IS NULL
CREATE TABLE [dbo].[Addresses]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [CustomerId] INT NOT NULL,
    [AddressType] NVARCHAR(20) NOT NULL DEFAULT ('Shipping'),
    [Street] NVARCHAR(200) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(100) NULL,
    [PostalCode] NVARCHAR(20) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id])
);
GO

-- 4. ORDERS AND PAYMENTS

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
CREATE TABLE [dbo].[Orders]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [CustomerId] INT NOT NULL,
    [ShippingAddressId] INT NULL,
    [OrderDate] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [Status] NVARCHAR(30) NOT NULL DEFAULT ('Pending'),
    [Subtotal] DECIMAL(18, 2) NOT NULL,
    [TaxAmount] DECIMAL(18, 2) NOT NULL DEFAULT (0),
    [ShippingCost] DECIMAL(18, 2) NOT NULL DEFAULT (0),
    [Total] DECIMAL(18, 2) NOT NULL,
    [Notes] NVARCHAR(500) NULL,
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Orders_Addresses] FOREIGN KEY ([ShippingAddressId])
        REFERENCES [dbo].[Addresses] ([Id])
);
GO

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
CREATE TABLE [dbo].[OrderItems]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [OrderId] INT NOT NULL,
    [BookId] INT NOT NULL,
    [Quantity] INT NOT NULL CHECK ([Quantity] > 0),
    [UnitPrice] DECIMAL(18, 2) NOT NULL,
    [Discount] DECIMAL(18, 2) NOT NULL DEFAULT (0),
    CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderId])
        REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_OrderItems_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id])
);
GO

IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
CREATE TABLE [dbo].[Payments]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [OrderId] INT NOT NULL,
    [PaymentMethod] NVARCHAR(30) NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [PaymentDate] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [Status] NVARCHAR(30) NOT NULL DEFAULT ('Completed'),
    [TransactionRef] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payments_Orders] FOREIGN KEY ([OrderId])
        REFERENCES [dbo].[Orders] ([Id])
);
GO

-- 5. SHOPPING CART

IF OBJECT_ID(N'dbo.ShoppingCarts', N'U') IS NULL
CREATE TABLE [dbo].[ShoppingCarts]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [CustomerId] INT NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ShoppingCarts_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id])
);
GO

IF OBJECT_ID(N'dbo.CartItems', N'U') IS NULL
CREATE TABLE [dbo].[CartItems]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [CartId] INT NOT NULL,
    [BookId] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT (1) CHECK ([Quantity] > 0),
    [AddedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_CartItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CartItems_Carts] FOREIGN KEY ([CartId])
        REFERENCES [dbo].[ShoppingCarts] ([Id]),
    CONSTRAINT [FK_CartItems_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [UQ_CartItems_Cart_Book] UNIQUE ([CartId], [BookId])
);
GO

-- 6. REVIEWS

IF OBJECT_ID(N'dbo.Reviews', N'U') IS NULL
CREATE TABLE [dbo].[Reviews]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [BookId] INT NOT NULL,
    [CustomerId] INT NOT NULL,
    [Rating] TINYINT NOT NULL CHECK ([Rating] BETWEEN 1 AND 5),
    [Comment] NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reviews_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [FK_Reviews_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [UQ_Reviews_Book_Customer] UNIQUE ([BookId], [CustomerId])
);
GO

-- 7. INVENTORY AND SUPPLIERS

IF OBJECT_ID(N'dbo.Inventory', N'U') IS NULL
CREATE TABLE [dbo].[Inventory]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [BookId] INT NOT NULL,
    [QuantityInStock] INT NOT NULL DEFAULT (0) CHECK ([QuantityInStock] >= 0),
    [ReorderLevel] INT NOT NULL DEFAULT (5),
    [LastRestockedAt] DATETIME2(7) NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Inventory_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [UQ_Inventory_Book] UNIQUE ([BookId])
);
GO

IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NULL
CREATE TABLE [dbo].[Suppliers]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [ContactName] NVARCHAR(150) NULL,
    [Email] NVARCHAR(150) NULL,
    [Phone] NVARCHAR(30) NULL,
    [Address] NVARCHAR(300) NULL,
    [IsActive] BIT NOT NULL DEFAULT (1),
    CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

IF OBJECT_ID(N'dbo.PurchaseOrders', N'U') IS NULL
CREATE TABLE [dbo].[PurchaseOrders]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [SupplierId] INT NOT NULL,
    [OrderDate] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [ExpectedDate] DATE NULL,
    [Status] NVARCHAR(30) NOT NULL DEFAULT ('Pending'),
    [TotalCost] DECIMAL(18, 2) NOT NULL DEFAULT (0),
    CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PurchaseOrders_Suppliers] FOREIGN KEY ([SupplierId])
        REFERENCES [dbo].[Suppliers] ([Id])
);
GO

IF OBJECT_ID(N'dbo.PurchaseOrderItems', N'U') IS NULL
CREATE TABLE [dbo].[PurchaseOrderItems]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [PurchaseOrderId] INT NOT NULL,
    [BookId] INT NOT NULL,
    [Quantity] INT NOT NULL CHECK ([Quantity] > 0),
    [UnitCost] DECIMAL(18, 2) NOT NULL,
    CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_POItems_PurchaseOrders] FOREIGN KEY ([PurchaseOrderId])
        REFERENCES [dbo].[PurchaseOrders] ([Id]),
    CONSTRAINT [FK_POItems_Books] FOREIGN KEY ([BookId])
        REFERENCES [dbo].[Books] ([Id])
);
GO

-- 8. STAFF / ADMIN USERS

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
CREATE TABLE [dbo].[Roles]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([Name])
);
GO

IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
CREATE TABLE [dbo].[Employees]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [RoleId] INT NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(150) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT (1),
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employees_Roles] FOREIGN KEY ([RoleId])
        REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [UQ_Employees_Email] UNIQUE ([Email])
);
GO

-- Roles iniciales del enunciado: agregar solamente los que falten.
INSERT INTO [dbo].[Roles] ([Name])
SELECT v.[Name]
FROM (VALUES (N'Admin'), (N'Manager'), (N'Cashier')) AS v([Name])
WHERE NOT EXISTS
(
    SELECT 1 FROM [dbo].[Roles] AS r WHERE r.[Name] = v.[Name]
);
GO

-- Verificacion: en una base nueva deben aparecer 20 tablas y 21 claves foraneas.
SELECT SCHEMA_NAME([schema_id]) AS [Esquema], [name] AS [Tabla]
FROM sys.tables
WHERE [is_ms_shipped] = 0 AND [schema_id] = SCHEMA_ID(N'dbo')
ORDER BY [name];

SELECT COUNT(*) AS [TotalTablas]
FROM sys.tables
WHERE [is_ms_shipped] = 0 AND [schema_id] = SCHEMA_ID(N'dbo');

SELECT COUNT(*) AS [TotalClavesForaneas]
FROM sys.foreign_keys
WHERE [schema_id] = SCHEMA_ID(N'dbo');

SELECT [Id], [Name] FROM [dbo].[Roles] ORDER BY [Id];
GO
