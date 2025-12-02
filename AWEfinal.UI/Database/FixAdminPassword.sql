-- Fix Admin Password Script
-- This script updates the admin password to hashed value (SHA256 Base64)
-- Example: "admin123" => JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=

USE AWEfinal;
GO

-- Verify current admin user
SELECT Id, Email, Name, Role, PasswordHash FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

-- Update admin password to hashed value
UPDATE [Users] 
SET PasswordHash = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk='
WHERE Email = 'admin@electrostore.com';
GO

-- Verify the update
SELECT Id, Email, Name, Role, PasswordHash FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

PRINT 'Admin password updated successfully!';
PRINT 'Email: admin@electrostore.com';
PRINT 'Password: admin123 (hashed)';
GO
