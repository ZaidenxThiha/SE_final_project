# AWEfinal - 3-Tier Architecture E-Commerce Store

This is a complete 3-tier architecture web application built with ASP.NET Core, Entity Framework Core, and SQL Server LocalDB.

## Project Structure

```
AWEfinal/
├── AWEfinal.DAL/          # Data Access Layer
│   ├── Models/           # Entity Models
│   ├── Repositories/      # Repository Pattern Implementation
│   └── AWEfinalDbContext.cs
├── AWEfinal.BLL/          # Business Logic Layer
│   └── Services/         # Business Services
├── AWEfinal.UI/          # Presentation Layer (ASP.NET Core MVC)
│   ├── Controllers/      # MVC Controllers
│   ├── Views/           # Razor Views
│   └── wwwroot/         # Static Files
└── AWEfinal.sln         # Solution File
```

## Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- SQL Server LocalDB (included with Visual Studio)
- SQL Server Management Studio (SSMS) - Optional

## Setup Instructions

### 1. Database Setup

1. Open SQL Server Management Studio or SQL Server Object Explorer in Visual Studio
2. Connect to LocalDB instance: `(localdb)\MSSQLLocalDB`
3. Create a new database named `AWEfinal`
4. The connection string in `appsettings.json` is already configured:
   ```
   Server=np:\\.\pipe\LOCALDB#3F5A8627\tsql\query;Database=AWEfinal;Trusted_Connection=True;
   ```

### 2. Run Entity Framework Migrations

1. Open Package Manager Console in Visual Studio
2. Select `AWEfinal.UI` as the default project
3. Run the following commands:

```powershell
# Add Migration
Add-Migration InitialCreate -Project AWEfinal.DAL -StartupProject AWEfinal.UI

# Update Database
Update-Database -Project AWEfinal.DAL -StartupProject AWEfinal.UI
```

### 3. Seed Initial Data

1. Run the SQL script in `AWEfinal.UI/Database/SeedData.sql` against your AWEfinal database
2. This will create:
   - Admin user (email: admin@electrostore.com, password: admin123)
   - Sample products

### 4. Run the Application

1. Set `AWEfinal.UI` as the startup project
2. Press F5 or click Run
3. The application will open in your browser

## Default Credentials

- **Admin Account:**
  - Email: admin@electrostore.com
  - Password: admin123

## Features

### Customer Features

- Browse products by category
- View product details
- Add products to cart
- Checkout and place orders
- View order history
- User registration and login

### Admin Features

- Product management (Create, Read, Update, Delete)
- Order management
- Order status updates
- Tracking number assignment

## Architecture

### Data Access Layer (DAL)

- Entity Framework Core DbContext
- Repository Pattern
- Entity Models with Data Annotations

### Business Logic Layer (BLL)

- Service Layer Pattern
- Business Rules and Validation
- Password Hashing (SHA256)

### Presentation Layer (UI)

- ASP.NET Core MVC
- Session-based Authentication
- Razor Views
- Controllers for each feature area

## Technologies Used

- **Backend:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server LocalDB
- **Frontend:** Bootstrap 5, Razor Views
- **Architecture:** 3-Tier (DAL, BLL, UI)

## Project Configuration

### Connection String

The connection string is configured in `AWEfinal.UI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LocalDBConn": "Server=np:\\.\\pipe\\LOCALDB#3F5A8627\\tsql\\query;Database=AWEfinal;Trusted_Connection=True;"
  }
}
```

### Dependency Injection

All services and repositories are registered in `Program.cs` using dependency injection.

## Notes

- Session is used for authentication (stored in memory)
- Password hashing uses SHA256
- Order numbers and invoice numbers are auto-generated
- All prices are stored as decimal(18,2)

## Troubleshooting

### Database Connection Issues

- Ensure LocalDB is installed and running
- Check that the database `AWEfinal` exists
- Verify the connection string in `appsettings.json`

### Migration Issues

- Ensure all projects build successfully
- Check that the DAL project is referenced correctly
- Verify Entity Framework Core packages are installed

## License

This project is created for educational purposes.
