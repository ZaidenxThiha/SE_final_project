-- Fix Admin Password Script
-- This script updates the admin password to plain text (no hashing)
-- Password: admin123 (stored as plain text)

USE AWEfinal;
GO

-- Verify current admin user
SELECT Id, Email, Name, Role, PasswordHash FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

-- Update admin password to plain text
UPDATE [Users] 
SET PasswordHash = 'admin123'
WHERE Email = 'admin@electrostore.com';
GO

-- Verify the update
SELECT Id, Email, Name, Role, PasswordHash FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

PRINT 'Admin password updated successfully!';
PRINT 'Email: admin@electrostore.com';
PRINT 'Password: admin123 (plain text)';
GO

