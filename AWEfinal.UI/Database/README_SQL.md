# Database Setup Instructions

## Quick Start

1. **Open SQL Server Management Studio (SSMS)** or **SQL Server Object Explorer** in Visual Studio
2. **Connect to LocalDB:**
   - Server name: `(localdb)\MSSQLLocalDB`
   - Or use the pipe connection: `np:\\.\pipe\LOCALDB#B9917A2A\tsql\query`
3. **Open the SQL script:** `FullDatabaseScript.sql`
4. **Execute the entire script** (F5 or Execute button)

## What the Script Does

The `FullDatabaseScript.sql` script:

1. **Creates the database** `AWEfinal` if it doesn't exist
2. **Drops existing tables** (if they exist) in the correct order
3. **Creates all tables:**
   - Users
   - Products
   - Orders
   - OrderItems
4. **Creates indexes** for performance optimization
5. **Creates foreign key constraints** for data integrity
6. **Inserts seed data:**
   - Admin user (admin@electrostore.com / set your own; hash stored)
   - 24 sample products across all categories

## Alternative: Using Entity Framework Migrations

If you prefer to use EF Core migrations instead of the SQL script:

```powershell
# In Package Manager Console (Visual Studio)
# Set Default Project: AWEfinal.UI

Add-Migration InitialCreate -Project AWEfinal.DAL -StartupProject AWEfinal.UI
Update-Database -Project AWEfinal.DAL -StartupProject AWEfinal.UI

# Then run SeedData.sql for initial data
```

## Connection String

The connection string in `appsettings.json` is configured for LocalDB:

```json
{
  "ConnectionStrings": {
    "LocalDBConn": "Server=np:\\.\\pipe\\LOCALDB#B9917A2A\\tsql\\query;Database=AWEfinal;Trusted_Connection=True;"
  }
}
```

## Default Admin Credentials

- **Email:** admin@electrostore.com
- **Password:** set your own (hash stored; update scripts accordingly)

## Verification

After running the script, verify the setup:

```sql
-- Check all tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'

-- Check record counts
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'OrderItems', COUNT(*) FROM OrderItems

-- Verify admin user
SELECT * FROM Users WHERE Role = 'admin'
```

## Troubleshooting

### Database Already Exists

If you get an error about the database existing, you can either:

1. Drop the existing database first: `DROP DATABASE AWEfinal;`
2. Or modify the script to use `USE AWEfinal;` without creating it

### LocalDB Not Running

If LocalDB is not running, start it:

```powershell
sqllocaldb start MSSQLLocalDB
```

### Connection Issues

Try these connection strings:

- `(localdb)\MSSQLLocalDB`
- `np:\\.\pipe\LOCALDB#B9917A2A\tsql\query`
- `Server=(localdb)\\MSSQLLocalDB;Database=AWEfinal;Trusted_Connection=True;`
