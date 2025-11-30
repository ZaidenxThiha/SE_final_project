CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Storage] nvarchar(50) NULL,
    [Colors] nvarchar(max) NOT NULL,
    [Sizes] nvarchar(max) NULL,
    [Rating] decimal(3,2) NOT NULL DEFAULT 0.0,
    [Images] nvarchar(max) NOT NULL,
    [Features] nvarchar(max) NOT NULL,
    [InStock] bit NOT NULL DEFAULT CAST(1 AS bit),
    [StockQuantity] int NOT NULL DEFAULT 0,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Phone] nvarchar(50) NULL,
    [AddressLine1] nvarchar(150) NULL,
    [AddressLine2] nvarchar(150) NULL,
    [City] nvarchar(100) NULL,
    [PostalCode] nvarchar(20) NULL,
    [Country] nvarchar(100) NULL,
    [Role] nvarchar(20) NOT NULL DEFAULT N'customer',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(50) NOT NULL,
    [UserId] int NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'pending',
    [PaymentMethod] nvarchar(50) NULL,
    [ShippingFullName] nvarchar(100) NOT NULL,
    [ShippingPhone] nvarchar(20) NOT NULL,
    [ShippingAddress] nvarchar(max) NOT NULL,
    [ShippingCity] nvarchar(50) NOT NULL,
    [ShippingPostalCode] nvarchar(20) NOT NULL,
    [ShippingCountry] nvarchar(50) NOT NULL,
    [InvoiceNumber] nvarchar(50) NULL,
    [TrackingNumber] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [InventoryAdjusted] bit NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [ProductName] nvarchar(200) NOT NULL,
    [Quantity] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO


CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
GO


CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
GO


CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
GO


CREATE INDEX [IX_Products_Category] ON [Products] ([Category]);
GO


CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO


