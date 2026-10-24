/*
    PAP.BookStore - Verificacion de los datos de demostracion.
    Consulta de solo lectura: no inserta, actualiza ni elimina registros.
    Ejecutar despues de completar la generacion desde Admin.

    Resultado 1: las veinte tablas deben contener registros.
    Resultado 2: todas las cantidades deben ser cero para el flujo generado.
    Resultado 1: 20 tablas revisadas, 20 con datos y 0 vacias.
    Resultado 2: cantidad de registros por tabla.
    Resultado 3: todas las inconsistencias deben ser cero para el flujo generado.
    Los pagos de este flujo cubren el total de un pedido; no se generan abonos.
*/

USE [BookStoreDB];
GO

DECLARE @Conteos TABLE
(
    [Tabla] NVARCHAR(128) NOT NULL,
    [Registros] BIGINT NOT NULL
);

INSERT INTO @Conteos ([Tabla], [Registros])
SELECT N'dbo.Addresses' AS [Tabla], COUNT_BIG(*) AS [Registros] FROM [dbo].[Addresses]
UNION ALL SELECT N'dbo.Authors', COUNT_BIG(*) FROM [dbo].[Authors]
UNION ALL SELECT N'dbo.BookAuthors', COUNT_BIG(*) FROM [dbo].[BookAuthors]
UNION ALL SELECT N'dbo.Books', COUNT_BIG(*) FROM [dbo].[Books]
UNION ALL SELECT N'dbo.CartItems', COUNT_BIG(*) FROM [dbo].[CartItems]
UNION ALL SELECT N'dbo.Categories', COUNT_BIG(*) FROM [dbo].[Categories]
UNION ALL SELECT N'dbo.Customers', COUNT_BIG(*) FROM [dbo].[Customers]
UNION ALL SELECT N'dbo.Employees', COUNT_BIG(*) FROM [dbo].[Employees]
UNION ALL SELECT N'dbo.Genres', COUNT_BIG(*) FROM [dbo].[Genres]
UNION ALL SELECT N'dbo.Inventory', COUNT_BIG(*) FROM [dbo].[Inventory]
UNION ALL SELECT N'dbo.OrderItems', COUNT_BIG(*) FROM [dbo].[OrderItems]
UNION ALL SELECT N'dbo.Orders', COUNT_BIG(*) FROM [dbo].[Orders]
UNION ALL SELECT N'dbo.Payments', COUNT_BIG(*) FROM [dbo].[Payments]
UNION ALL SELECT N'dbo.Publishers', COUNT_BIG(*) FROM [dbo].[Publishers]
UNION ALL SELECT N'dbo.PurchaseOrderItems', COUNT_BIG(*) FROM [dbo].[PurchaseOrderItems]
UNION ALL SELECT N'dbo.PurchaseOrders', COUNT_BIG(*) FROM [dbo].[PurchaseOrders]
UNION ALL SELECT N'dbo.Reviews', COUNT_BIG(*) FROM [dbo].[Reviews]
UNION ALL SELECT N'dbo.Roles', COUNT_BIG(*) FROM [dbo].[Roles]
UNION ALL SELECT N'dbo.ShoppingCarts', COUNT_BIG(*) FROM [dbo].[ShoppingCarts]
UNION ALL SELECT N'dbo.Suppliers', COUNT_BIG(*) FROM [dbo].[Suppliers]
UNION ALL SELECT N'dbo.Suppliers', COUNT_BIG(*) FROM [dbo].[Suppliers];

SELECT COUNT(*) AS [TablasRevisadas],
       SUM(CASE WHEN [Registros] > 0 THEN 1 ELSE 0 END) AS [TablasConDatos],
       SUM(CASE WHEN [Registros] = 0 THEN 1 ELSE 0 END) AS [TablasVacias]
FROM @Conteos;

SELECT [Tabla], [Registros]
FROM @Conteos
ORDER BY [Tabla];
GO

SELECT N'Pedidos con direccion de otro cliente' AS [Comprobacion], COUNT_BIG(*) AS [Inconsistencias]
FROM [dbo].[Orders] AS o
INNER JOIN [dbo].[Addresses] AS a ON a.[Id] = o.[ShippingAddressId]
WHERE o.[CustomerId] <> a.[CustomerId]

UNION ALL
SELECT N'Pedidos con total diferente de subtotal + impuestos + envio', COUNT_BIG(*)
FROM [dbo].[Orders]
WHERE [Total] <> [Subtotal] + [TaxAmount] + [ShippingCost]

UNION ALL
SELECT N'Pedidos con subtotal diferente de sus detalles', COUNT_BIG(*)
FROM [dbo].[Orders] AS o
OUTER APPLY
(
    SELECT SUM(i.[Quantity] * i.[UnitPrice] - i.[Discount]) AS [Subtotal]
    FROM [dbo].[OrderItems] AS i
    WHERE i.[OrderId] = o.[Id]
) AS calculated
WHERE o.[Subtotal] <> ISNULL(calculated.[Subtotal], 0)

UNION ALL
SELECT N'Compras con costo total diferente de sus detalles', COUNT_BIG(*)
FROM [dbo].[PurchaseOrders] AS o
OUTER APPLY
(
    SELECT SUM(i.[Quantity] * i.[UnitCost]) AS [TotalCost]
    FROM [dbo].[PurchaseOrderItems] AS i
    WHERE i.[PurchaseOrderId] = o.[Id]
) AS calculated
WHERE o.[TotalCost] <> ISNULL(calculated.[TotalCost], 0)

UNION ALL
SELECT N'Pagos generados con importe no positivo o diferente del total del pedido', COUNT_BIG(*)
FROM [dbo].[Payments] AS p
INNER JOIN [dbo].[Orders] AS o ON o.[Id] = p.[OrderId]
WHERE p.[Amount] <= 0 OR p.[Amount] <> o.[Total];
GO