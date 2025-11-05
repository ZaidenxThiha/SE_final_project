# Login Troubleshooting Guide

## Issue: Cannot login with admin credentials

### Default Admin Credentials
- **Email:** admin@electrostore.com
- **Password:** admin123

## Step 1: Verify Database User Exists

Run this SQL query to check if the admin user exists:

```sql
USE AWEfinal;
SELECT Id, Email, Name, Role, PasswordHash, LEN(PasswordHash) as HashLength 
FROM [Users] 
WHERE Email = 'admin@electrostore.com';
```

**Expected Result:**
- Should return 1 row
- Email: admin@electrostore.com
- Role: admin
- PasswordHash: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
- HashLength: 44 (Base64 encoded SHA256)

## Step 2: Fix Password Hash

If the user exists but login fails, run the fix script:

**Option A: Update existing user**
```sql
USE AWEfinal;
UPDATE [Users] 
SET PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg='
WHERE Email = 'admin@electrostore.com';
```

**Option B: Delete and recreate (if user doesn't exist)**
```sql
USE AWEfinal;
-- Delete if exists
DELETE FROM [Users] WHERE Email = 'admin@electrostore.com';

-- Insert with correct hash
INSERT INTO [Users] (Email, PasswordHash, Name, Role, CreatedAt)
VALUES ('admin@electrostore.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Admin User', 'admin', GETUTCDATE());
```

## Step 3: Run the Complete Fix Script

Run the `VerifyAndFixAdmin.sql` script which will:
1. Check if admin user exists
2. Delete existing admin (if incorrect)
3. Create admin with correct password hash
4. Verify the creation

## Step 4: Verify Password Hash

The password hash is generated using:
- Algorithm: SHA256
- Encoding: Base64
- Password: "admin123"
- Hash: `jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=`

### How to Generate Hash (C#)
```csharp
using System.Security.Cryptography;
using System.Text;

string password = "admin123";
using (var sha256 = SHA256.Create())
{
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    string hash = Convert.ToBase64String(hashedBytes);
    // Result: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
}
```

## Common Issues

### Issue 1: User Doesn't Exist
**Solution:** Run the `VerifyAndFixAdmin.sql` script to create the admin user.

### Issue 2: Wrong Password Hash
**Solution:** Update the password hash using the SQL update statement above.

### Issue 3: Case Sensitivity
**Solution:** The UserService converts email to lowercase, so ensure the email in the database is lowercase.

### Issue 4: Session Not Working
**Solution:** Check that session is configured in Program.cs and cookies are enabled in your browser.

## Testing After Fix

1. Clear browser cache/cookies
2. Navigate to the login page
3. Enter:
   - Email: `admin@electrostore.com`
   - Password: `admin123`
4. Click Login
5. Should redirect to Admin Dashboard

## Debugging

If login still fails:
1. Check application logs for error messages
2. Verify database connection string is correct
3. Check that the Users table exists and has data
4. Verify the password hash in database matches: `jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=`

## Quick Fix Command

Run this single SQL command to fix the admin password:

```sql
USE AWEfinal;
UPDATE [Users] 
SET PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg='
WHERE Email = 'admin@electrostore.com';
```

