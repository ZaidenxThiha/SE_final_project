# AWEfinal – Online Electronics Store (3-Tier)

AWEfinal is the proof-of-concept implementation for the **SE_FinalProject_Requirements_Sem1_2526** brief. It delivers a complete 3-tier system (DAL ➜ BLL ➜ UI) backed by SQL Server, exposes the customer-facing ASP.NET Core MVC site, and now adds the WinForms desktop dashboard demanded for staff operations.

## Solution Layout

```
AWEfinal.sln
├── AWEfinal.DAL/          # Entity Framework Core DbContext + Models + Repositories
├── AWEfinal.BLL/          # Business services (products, orders, users)
├── AWEfinal.UI/           # ASP.NET Core MVC app (customer & admin web portal)
├── AWEfinal.AdminWinForms/# WinForms staff dashboard (secure login, CRUD, reports)
└── docs/                  # Requirements alignment + future documents
```

## Architecture at a Glance

- **Database (MSSQL / LocalDB):** single shared instance, accessed exclusively via `AWEfinal.DAL`.
- **Data Access Layer:** EF Core models (`Models/`) + repositories that encapsulate queries.
- **Business Logic Layer:** service classes performing validation, pricing, and workflow rules.
- **Presentation:**
  - `AWEfinal.UI` ASP.NET Core MVC site for customers (browse, cart, checkout, order history) and basic admin pages.
  - `AWEfinal.AdminWinForms` WinForms desktop for staff (login, product CRUD, order status tracking, revenue reports).

## Getting Started

### Prerequisites
- .NET 8.0 SDK and Visual Studio 2022 (for both MVC + WinForms).
- SQL Server LocalDB (ships with Visual Studio) or another SQL Server instance.
- Optional: SQL Server Management Studio for database inspection.

### 1. Database Setup
1. Launch SSMS or the Visual Studio SQL Server Object Explorer.
2. Connect to `(localdb)\MSSQLLocalDB`.
3. Create a database named `AWEfinal`.
4. Update the `LocalDBConn` string in `AWEfinal.UI/appsettings.json` and `AWEfinal.AdminWinForms/appsettings.json` if your LocalDB pipe name differs.

```
"ConnectionStrings": {
  "LocalDBConn": "Data Source=np:\\.\pipe\LOCALDB#B9917A2A\tsql\query;Initial Catalog=AWEfinal;Integrated Security=True;MultipleActiveResultSets=true"
}
```

### 2. Apply EF Core Migrations
```
# From the Package Manager Console (Default project: AWEfinal.UI)
Add-Migration InitialCreate -Project AWEfinal.DAL -StartupProject AWEfinal.UI
Update-Database -Project AWEfinal.DAL -StartupProject AWEfinal.UI
```

### 3. Seed Sample Data
Execute `AWEfinal.UI/Database/SeedData.sql` against `AWEfinal` to create the admin user and demo products.

## Running the Applications

### ASP.NET Core MVC Site
1. Set `AWEfinal.UI` as the startup project.
2. Run (F5). The site opens at `https://localhost:<port>`.

### WinForms Staff Dashboard
1. Requires Windows host (or Windows VM) because of WinForms.
2. Set `AWEfinal.AdminWinForms` as the startup project and run.
3. Sign in with the admin credentials (below) to access dashboard metrics, product management, and order workflows.
4. The WinForms app talks to the same database as the MVC site—ensure the DB is provisioned before launching.

## Default Credentials
- **Admin:** `admin@electrostore.com / admin123`
- **Customer:** create via the public site registration flow.

## Feature Set

### Customer (Web)
- Browse and filter products by category.
- View product detail pages with photos/specs.
- Add to cart, update quantities, checkout, and see order confirmations.
- Maintain order history and profile data.

### Staff/Admin (Web + WinForms)
- Secure login via shared user table (admin role only).
- Product CRUD with image uploads (web) and lightweight editor (WinForms).
- Order management: status transitions, tracking numbers, cancellation control.
- Dashboard metrics (total products/orders, revenue, average order value).
- Reporting tab with timeframe-based revenue/volume breakdowns.

## Documentation & Requirements Alignment
- `docs/RequirementsAlignment.md` maps every rubric item (requirements spec, project plan, architecture diagrams, UML, traceability, automated tests, final report/demo) to its current status.
- Add your Part 1/Part 2 deliverables (requirements document, UI/UX mockups, UML, ERD, traceability matrix) inside `docs/`.
- Capture screenshots and a short video demo showing both the MVC site and WinForms dashboard for the final submission.

## Testing
- Current verification is manual plus full solution builds (`dotnet build AWEfinal.sln`).
- The PDF requires EP/BVA unit tests targeting business logic—add a dedicated test project under `/tests` and link it to the BLL.

## Troubleshooting
- **LocalDB connection:** confirm the named pipe (see `sqllocaldb info`) and update both `appsettings.json` files accordingly.
- **Admin login fails:** rerun `SeedData.sql` to recreate the seeded admin user.
- **WinForms build on macOS/Linux:** use Windows with `EnableWindowsTargeting=true` (already set) or build inside a Windows VM.

## License
Educational use only (capstone project).
