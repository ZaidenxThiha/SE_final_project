# Connection String Guide

## Current Connection String

The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LocalDBConn": "Server=(localdb)\\MSSQLLocalDB;Database=AWEfinal;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Alternative Connection Strings

If the above doesn't work, try these alternatives:

### Option 1: Standard LocalDB Connection

```
Server=(localdb)\MSSQLLocalDB;Database=AWEfinal;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Option 2: LocalDB with Integrated Security

```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AWEfinal;Integrated Security=True;MultipleActiveResultSets=true
```

### Option 3: Using Named Pipe (if you know the exact pipe)

```
Server=np:\\.\pipe\LOCALDB#3F5A8627\tsql\query;Database=AWEfinal;Trusted_Connection=True;
```

### Option 4: SQL Server Express (if installed)

```
Server=localhost\SQLEXPRESS;Database=AWEfinal;Trusted_Connection=True;MultipleActiveResultSets=true
```

## Troubleshooting

### 1. Check if LocalDB is Running

Open PowerShell or Command Prompt and run:

```powershell
sqllocaldb info MSSQLLocalDB
```

If it shows "Stopped", start it:

```powershell
sqllocaldb start MSSQLLocalDB
```

### 2. List Available LocalDB Instances

```powershell
sqllocaldb info
```

### 3. Create LocalDB Instance (if needed)

```powershell
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### 4. Verify Database Exists

Open SQL Server Management Studio (SSMS) or SQL Server Object Explorer in Visual Studio:

- Connect to: `(localdb)\MSSQLLocalDB`
- Check if `AWEfinal` database exists
- If not, create it or run the FullDatabaseScript.sql

### 5. Test Connection in Visual Studio

1. Open **Server Explorer** (View → Server Explorer)
2. Right-click **Data Connections** → **Add Connection**
3. Select **Microsoft SQL Server Database File**
4. Browse to your database file location
5. Or use **SQL Server** connection type:
   - Server name: `(localdb)\MSSQLLocalDB`
   - Database name: `AWEfinal`
   - Authentication: Windows Authentication

### 6. Check Connection String Format

The connection string format in JSON requires double backslashes:

- Correct: `(localdb)\\MSSQLLocalDB`
- Wrong: `(localdb)\MSSQLLocalDB`

## Common Errors and Solutions

### Error: "A network-related or instance-specific error occurred"

**Solution:** LocalDB might not be running. Start it using the commands above.

### Error: "Cannot open database 'AWEfinal'"

**Solution:** The database doesn't exist. Run the FullDatabaseScript.sql to create it.

### Error: "Login failed for user"

**Solution:** Use `Trusted_Connection=True` for Windows Authentication, or ensure your Windows account has access to LocalDB.

### Error: "The parameter is incorrect"

**Solution:** The connection string format is wrong. Use one of the standard formats above.

## Quick Fix

If you're still having issues, try this step-by-step:

1. **Start LocalDB:**

   ```powershell
   sqllocaldb start MSSQLLocalDB
   ```

2. **Verify it's running:**

   ```powershell
   sqllocaldb info MSSQLLocalDB
   ```

3. **Create/Verify Database:**

   - Open SSMS or SQL Server Object Explorer
   - Connect to `(localdb)\MSSQLLocalDB`
   - Run `FullDatabaseScript.sql` to create the database

4. **Update appsettings.json** with the connection string from Option 1

5. **Restart the application**

## Using the Original Pipe Connection

If you need to use the original pipe connection format, make sure:

1. The LocalDB instance is running
2. The pipe name matches exactly
3. The database name is correct

You can find the exact pipe name by:

1. Starting LocalDB: `sqllocaldb start MSSQLLocalDB`
2. Checking the instance info: `sqllocaldb info MSSQLLocalDB`
3. Looking for the pipe name in the output
