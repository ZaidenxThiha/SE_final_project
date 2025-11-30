-- =============================================
-- AWEfinal Database - Complete Setup Script (MySQL)
-- =============================================

-- Create database if it doesn't exist
CREATE DATABASE IF NOT EXISTS AWEfinal;
USE AWEfinal;

-- =============================================
-- Drop existing tables if they exist (in reverse order due to foreign keys)
-- =============================================

DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Users;

-- =============================================
-- Create Tables
-- =============================================

-- Users Table
CREATE TABLE Users (
    Id INT NOT NULL AUTO_INCREMENT,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Phone VARCHAR(50) NULL,
    AddressLine1 VARCHAR(150) NULL,
    AddressLine2 VARCHAR(150) NULL,
    City VARCHAR(100) NULL,
    PostalCode VARCHAR(20) NULL,
    Country VARCHAR(100) NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'customer',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('customer', 'admin'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Products Table
CREATE TABLE Products (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(200) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Category VARCHAR(50) NOT NULL,
    Description TEXT NOT NULL,
    Storage VARCHAR(50) NULL,
    Colors TEXT NOT NULL DEFAULT '[]',
    Sizes TEXT NULL,
    Rating DECIMAL(3,2) NOT NULL DEFAULT 0,
    Images TEXT NOT NULL DEFAULT '[]',
    Features TEXT NOT NULL DEFAULT '[]',
    InStock TINYINT(1) NOT NULL DEFAULT 1,
    StockQuantity INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT CK_Products_Price CHECK (Price > 0),
    CONSTRAINT CK_Products_Rating CHECK (Rating >= 0 AND Rating <= 5)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Orders Table
CREATE TABLE Orders (
    Id INT NOT NULL AUTO_INCREMENT,
    OrderNumber VARCHAR(50) NOT NULL,
    UserId INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'pending',
    PaymentMethod VARCHAR(50) NULL,
    ShippingFullName VARCHAR(100) NOT NULL,
    ShippingPhone VARCHAR(20) NOT NULL,
    ShippingAddress TEXT NOT NULL,
    ShippingCity VARCHAR(50) NOT NULL,
    ShippingPostalCode VARCHAR(20) NOT NULL,
    ShippingCountry VARCHAR(50) NOT NULL,
    InvoiceNumber VARCHAR(50) NULL,
    TrackingNumber VARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    InventoryAdjusted TINYINT(1) NOT NULL DEFAULT 0,
    CONSTRAINT PK_Orders PRIMARY KEY (Id),
    CONSTRAINT CK_Orders_Total CHECK (Total >= 0),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('pending', 'paid', 'packaging', 'shipped', 'delivered', 'cancelled'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- OrderItems Table
CREATE TABLE OrderItems (
    Id INT NOT NULL AUTO_INCREMENT,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName VARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (Id),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_Price CHECK (Price >= 0),
    CONSTRAINT CK_OrderItems_Subtotal CHECK (Subtotal >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Create Indexes
-- =============================================

-- Users Indexes
CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);

-- Products Indexes
CREATE INDEX IX_Products_Category ON Products (Category);
CREATE INDEX IX_Products_InStock ON Products (InStock);

-- Orders Indexes
CREATE UNIQUE INDEX IX_Orders_OrderNumber ON Orders (OrderNumber);
CREATE INDEX IX_Orders_UserId ON Orders (UserId);
CREATE INDEX IX_Orders_Status ON Orders (Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders (CreatedAt DESC);

-- OrderItems Indexes
CREATE INDEX IX_OrderItems_OrderId ON OrderItems (OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems (ProductId);

-- =============================================
-- Create Foreign Keys
-- =============================================

-- Orders Foreign Keys
ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Users_UserId
    FOREIGN KEY (UserId) REFERENCES Users (Id)
    ON DELETE CASCADE;

-- OrderItems Foreign Keys
ALTER TABLE OrderItems
ADD CONSTRAINT FK_OrderItems_Orders_OrderId
    FOREIGN KEY (OrderId) REFERENCES Orders (Id)
    ON DELETE CASCADE;

ALTER TABLE OrderItems
ADD CONSTRAINT FK_OrderItems_Products_ProductId
    FOREIGN KEY (ProductId) REFERENCES Products (Id)
    ON DELETE NO ACTION;

-- =============================================
-- Seed Data
-- =============================================

-- Insert/Update Admin User (password hash for "admin123")
-- Use ON DUPLICATE KEY UPDATE instead of T-SQL IF/ELSE
INSERT INTO Users (Email, PasswordHash, Name, Role, CreatedAt)
VALUES (
    'admin@electrostore.com',
    'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=',
    'Admin User',
    'admin',
    CURRENT_TIMESTAMP
)
ON DUPLICATE KEY UPDATE
    PasswordHash = VALUES(PasswordHash),
    Name = VALUES(Name),
    Role = VALUES(Role);

-- Insert Sample Products
INSERT INTO Products (Name, Price, Category, Description, Storage, Colors, Rating, Images, Features, InStock, CreatedAt)
VALUES 
    ('iPhone 17 Pro', 24999.00, 'Smartphones', 'Latest flagship with A18 Pro chip, titanium design, and advanced camera system', '256GB', '["Black", "White", "Blue", "Natural Titanium"]', 5.0, '[]', '["6.7-inch Super Retina XDR display", "A18 Pro chip with 6-core CPU", "Pro camera system with 48MP main camera", "All-day battery life", "5G capable"]', 1, CURRENT_TIMESTAMP),
    
    ('MacBook Pro 16"', 45999.00, 'Laptops', 'Powerful laptop with M3 Max chip, perfect for professionals', '1TB', '["Space Black", "Silver"]', 5.0, '[]', '["16-inch Liquid Retina XDR display", "M3 Max chip", "Up to 22 hours battery life", "1080p FaceTime HD camera", "Six-speaker sound system"]', 1, CURRENT_TIMESTAMP),
    
    ('iPad Pro 12.9"', 19999.00, 'Tablets', 'Ultimate iPad experience with M2 chip and stunning display', '512GB', '["Space Grey", "Silver"]', 5.0, '[]', '["12.9-inch Liquid Retina XDR display", "M2 chip", "12MP Wide camera", "Face ID", "All-day battery life"]', 1, CURRENT_TIMESTAMP),
    
    ('AirPods Max', 12999.00, 'Headphones', 'Premium over-ear headphones with active noise cancellation', NULL, '["Space Grey", "Silver", "Pink", "Green", "Sky Blue"]', 4.0, '[]', '["Active Noise Cancellation", "Transparency mode", "Spatial audio", "20 hours of listening time", "Premium build quality"]', 1, CURRENT_TIMESTAMP),
    
    ('Samsung QLED 65"', 35999.00, 'Smart TVs', 'Stunning 4K QLED TV with quantum dot technology', NULL, '["Titan Gray"]', 5.0, '[]', '["65-inch QLED 4K display", "Quantum HDR", "Object Tracking Sound", "Smart TV with Tizen OS", "Gaming Hub"]', 1, CURRENT_TIMESTAMP),
    
    ('Sony A7 IV', 48999.00, 'Cameras', 'Professional mirrorless camera with 33MP full-frame sensor', NULL, '["Black"]', 5.0, '[]', '["33MP full-frame sensor", "4K 60p video recording", "Real-time Eye AF", "5-axis image stabilization", "Dual card slots"]', 1, CURRENT_TIMESTAMP),
    
    ('Apple Watch Ultra 2', 18999.00, 'Smartwatches', 'The most rugged and capable Apple Watch for extreme adventures', NULL, '["Titanium"]', 5.0, '[]', '["49mm titanium case", "Always-On Retina display", "Precision dual-frequency GPS", "Up to 36 hours battery life", "Water resistant 100m"]', 1, CURRENT_TIMESTAMP),
    
    ('PlayStation 5', 12999.00, 'Gaming Consoles', 'Next-gen gaming console with ultra-high speed SSD', '1TB', '["White"]', 5.0, '[]', '["Ultra-high speed SSD", "Ray tracing graphics", "4K gaming", "Tempest 3D AudioTech", "DualSense wireless controller"]', 1, CURRENT_TIMESTAMP),
    
    ('Sonos Arc', 18999.00, 'Speakers', 'Premium smart soundbar with Dolby Atmos', NULL, '["Black", "White"]', 5.0, '[]', '["Dolby Atmos", "11 high-performance drivers", "Voice control with Alexa", "Trueplay tuning", "Easy setup"]', 1, CURRENT_TIMESTAMP),
    
    ('Dell UltraSharp 32"', 15999.00, 'Monitors', 'Professional 4K monitor with IPS Black technology', NULL, '["Black"]', 4.0, '[]', '["32-inch 4K UHD display", "IPS Black technology", "99% sRGB color coverage", "USB-C connectivity", "Height adjustable stand"]', 1, CURRENT_TIMESTAMP),
    
    ('Samsung Galaxy S24 Ultra', 28999.00, 'Smartphones', 'Flagship Android phone with S Pen and advanced AI features', '512GB', '["Titanium Black", "Titanium Gray", "Titanium Violet"]', 5.0, '[]', '["6.8-inch Dynamic AMOLED display", "Snapdragon 8 Gen 3", "200MP main camera", "Built-in S Pen", "5000mAh battery"]', 1, CURRENT_TIMESTAMP),
    
    ('Microsoft Surface Laptop 5', 25999.00, 'Laptops', 'Sleek and powerful laptop for productivity', '512GB', '["Platinum", "Black", "Sage"]', 4.0, '[]', '["13.5-inch PixelSense touchscreen", "12th Gen Intel Core i7", "Up to 18 hours battery life", "Windows 11", "Premium Alcantara keyboard"]', 1, CURRENT_TIMESTAMP),
    
    ('Google Pixel 8 Pro', 21999.00, 'Smartphones', 'AI-powered smartphone with best-in-class camera', '256GB', '["Obsidian", "Porcelain", "Bay"]', 5.0, '[]', '["6.7-inch LTPO OLED display", "Google Tensor G3 chip", "50MP main camera with AI", "24-hour battery life", "7 years of OS updates"]', 1, CURRENT_TIMESTAMP),
    
    ('Lenovo ThinkPad X1 Carbon', 35999.00, 'Laptops', 'Business laptop with legendary reliability', '512GB', '["Black"]', 5.0, '[]', '["14-inch 2.8K OLED display", "13th Gen Intel Core i7", "Up to 21 hours battery life", "Military-grade durability", "TrackPoint keyboard"]', 1, CURRENT_TIMESTAMP),
    
    ('Samsung Galaxy Tab S9', 16999.00, 'Tablets', 'Premium Android tablet with S Pen included', '256GB', '["Graphite", "Beige", "Lavender"]', 4.0, '[]', '["11-inch Dynamic AMOLED display", "Snapdragon 8 Gen 2", "S Pen included", "IP68 water resistant", "DeX mode for productivity"]', 1, CURRENT_TIMESTAMP),
    
    ('Bose QuietComfort Ultra', 9999.00, 'Headphones', 'Premium noise-cancelling headphones with spatial audio', NULL, '["Black", "White Smoke", "Sandstone"]', 5.0, '[]', '["World-class noise cancellation", "Immersive spatial audio", "24-hour battery life", "Premium comfort", "CustomTune technology"]', 1, CURRENT_TIMESTAMP),
    
    ('LG OLED C3 55"', 29999.00, 'Smart TVs', 'Self-lit OLED with perfect blacks and infinite contrast', NULL, '["Titan"]', 5.0, '[]', '["55-inch OLED 4K display", "α9 AI Processor Gen6", "Dolby Vision & Atmos", "120Hz refresh rate", "webOS 23 smart platform"]', 1, CURRENT_TIMESTAMP),
    
    ('Canon EOS R6 Mark II', 55999.00, 'Cameras', 'Versatile full-frame mirrorless for photos and video', NULL, '["Black"]', 5.0, '[]', '["24.2MP full-frame sensor", "6K RAW video recording", "Dual Pixel AF II", "In-body stabilization", "40 fps continuous shooting"]', 1, CURRENT_TIMESTAMP),
    
    ('Garmin Fenix 7X Solar', 22999.00, 'Smartwatches', 'Rugged GPS smartwatch with solar charging', NULL, '["Black", "Slate Gray"]', 5.0, '[]', '["1.4-inch always-on display", "Solar charging capability", "Multi-GNSS satellite", "Up to 28 days battery", "Advanced health tracking"]', 1, CURRENT_TIMESTAMP),
    
    ('Xbox Series X', 14999.00, 'Gaming Consoles', 'Most powerful Xbox with 4K gaming at 120fps', '1TB', '["Black"]', 5.0, '[]', '["Custom 1TB NVMe SSD", "12 teraflops GPU power", "4K gaming at 120fps", "Ray tracing support", "Game Pass compatible"]', 1, CURRENT_TIMESTAMP),
    
    ('JBL Charge 5', 4999.00, 'Speakers', 'Portable waterproof speaker with powerful bass', NULL, '["Black", "Blue", "Red", "Gray"]', 4.0, '[]', '["IP67 waterproof & dustproof", "20 hours playtime", "PartyBoost feature", "Powerbank function", "Deep bass radiators"]', 1, CURRENT_TIMESTAMP),
    
    ('ASUS ROG Swift PG27UQ', 28999.00, 'Monitors', '4K gaming monitor with HDR and 144Hz', NULL, '["Black"]', 5.0, '[]', '["27-inch 4K UHD display", "144Hz refresh rate", "G-SYNC Ultimate", "HDR1000 certification", "Quantum-dot technology"]', 1, CURRENT_TIMESTAMP),
    
    ('OnePlus 12', 19999.00, 'Smartphones', 'Flagship killer with Hasselblad camera system', '256GB', '["Flowy Emerald", "Silky Black"]', 4.0, '[]', '["6.82-inch LTPO AMOLED", "Snapdragon 8 Gen 3", "Hasselblad camera system", "100W SUPERVOOC charging", "5400mAh battery"]', 1, CURRENT_TIMESTAMP),
    
    ('HP Spectre x360 14', 33999.00, 'Laptops', '2-in-1 convertible laptop with stunning OLED display', '1TB', '["Nightfall Black", "Poseidon Blue"]', 4.0, '[]', '["13.5-inch 3K2K OLED touchscreen", "13th Gen Intel Core i7", "360-degree hinge", "Up to 16 hours battery", "Thunderbolt 4 ports"]', 1, CURRENT_TIMESTAMP);

-- =============================================
-- Verification Queries
-- =============================================

-- Check table counts
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Products', COUNT(*) FROM Products
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM OrderItems;

-- Display Admin User
SELECT Id, Email, Name, Role, CreatedAt FROM Users WHERE Role = 'admin';

-- Display Sample Products
SELECT Id, Name, Category, Price, InStock FROM Products ORDER BY Id LIMIT 5;

-- "PRINT" equivalent
SELECT '=============================================' AS msg
UNION ALL SELECT 'Database setup completed successfully!'
UNION ALL SELECT '============================================='
UNION ALL SELECT 'Admin User Created:'
UNION ALL SELECT '  Email: admin@electrostore.com'
UNION ALL SELECT '  Password: admin123'
UNION ALL SELECT '=============================================';
