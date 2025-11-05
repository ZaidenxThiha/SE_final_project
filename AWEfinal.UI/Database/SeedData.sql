-- Seed Data for AWEfinal Database
-- Run this script after creating the database

USE AWEfinal;
GO

-- Insert Admin User
-- Password: admin123 (stored as plain text for admin login - no hashing)
-- Note: Admin login uses plain text password comparison for simplicity
INSERT INTO Users (Email, PasswordHash, Name, Role, CreatedAt)
VALUES ('admin@electrostore.com', 'admin123', 'Admin User', 'admin', GETUTCDATE());
GO

-- Insert Sample Products
INSERT INTO Products (Name, Price, Category, Description, Storage, Colors, Rating, Images, Features, InStock, CreatedAt)
VALUES 
('iPhone 17 Pro', 24999.00, 'Smartphones', 'Latest flagship with A18 Pro chip, titanium design, and advanced camera system', '256GB', '["Black", "White", "Blue", "Natural Titanium"]', 5.0, '[]', '["6.7-inch Super Retina XDR display", "A18 Pro chip with 6-core CPU", "Pro camera system with 48MP main camera"]', 1, GETUTCDATE()),
('MacBook Pro 16"', 45999.00, 'Laptops', 'Powerful laptop with M3 Max chip, perfect for professionals', '1TB', '["Space Black", "Silver"]', 5.0, '[]', '["16-inch Liquid Retina XDR display", "M3 Max chip", "Up to 22 hours battery life"]', 1, GETUTCDATE()),
('iPad Pro 12.9"', 19999.00, 'Tablets', 'Ultimate iPad experience with M2 chip and stunning display', '512GB', '["Space Grey", "Silver"]', 5.0, '[]', '["12.9-inch Liquid Retina XDR display", "M2 chip", "12MP Wide camera"]', 1, GETUTCDATE()),
('AirPods Max', 12999.00, 'Headphones', 'Premium over-ear headphones with active noise cancellation', NULL, '["Space Grey", "Silver", "Pink", "Green", "Sky Blue"]', 4.0, '[]', '["Active Noise Cancellation", "Transparency mode", "Spatial audio"]', 1, GETUTCDATE()),
('Samsung QLED 65"', 35999.00, 'Smart TVs', 'Stunning 4K QLED TV with quantum dot technology', NULL, '["Titan Gray"]', 5.0, '[]', '["65-inch QLED 4K display", "Quantum HDR", "Object Tracking Sound"]', 1, GETUTCDATE());
GO

