-- Verify and Fix Admin User Script
-- This script checks the admin user and fixes the password if needed

USE AWEfinal;
GO

-- Check if admin user exists
PRINT 'Checking admin user...';
SELECT Id, Email, Name, Role, PasswordHash, LEN(PasswordHash) as HashLength FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

-- Verify the hash format
-- The correct hash for "admin123" using SHA256 + Base64 should be: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
-- This is 44 characters long (Base64 encoded SHA256 hash)

-- Delete existing admin if it exists (to recreate with correct password)
DELETE FROM [Users] WHERE Email = 'admin@electrostore.com';
GO

-- Insert admin user with plain text password (no hashing)
-- Password: admin123
INSERT INTO [Users] (Email, PasswordHash, Name, Role, CreatedAt)
VALUES ('admin@electrostore.com', 'admin123', 'Admin User', 'admin', GETUTCDATE());
GO

-- Verify the admin user was created correctly
PRINT '=============================================';
PRINT 'Admin User Verification:';
PRINT '=============================================';
SELECT 
    Id,
    Email,
    Name,
    Role,
    PasswordHash,
    LEN(PasswordHash) as PasswordLength,
    CASE 
        WHEN PasswordHash = 'admin123' THEN 'Correct Password'
        ELSE 'Incorrect Password'
    END as PasswordStatus
FROM [Users] 
WHERE Email = 'admin@electrostore.com';
GO

PRINT '=============================================';
PRINT 'Admin Login Credentials:';
PRINT '  Email: admin@electrostore.com';
PRINT '  Password: admin123';
PRINT '=============================================';
GO

