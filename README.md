# AWEfinal – Online Electronics Store (3‑Tier)

AWEfinal delivers a full e‑commerce stack for the SE Final Project brief: SQL Server + EF Core (DAL), business services (BLL), an ASP.NET Core MVC site for customers/admin, and a WinForms desktop dashboard for staff. A dedicated xUnit test project covers core rules (pricing, stock, auth) with EP/BVA.

## Solution Layout
```
AWEfinal.sln
├── AWEfinal.DAL/           # EF Core DbContext, entities, repositories
├── AWEfinal.BLL/           # Business services (products, orders, users)
├── AWEfinal.UI/            # ASP.NET Core MVC site (customer + admin portal)
├── AWEfinal.AdminWinForms/ # WinForms admin dashboard
└── AWEfinal.Tests/         # xUnit unit tests (EP/BVA on core logic)
```

## Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (or SQL Server instance)
- Node.js only if you modify ClientApp assets (not required to run MVC)

## Database Setup
1) Ensure SQL Server LocalDB is running.  
2) Update connection strings if your pipe name differs. Defaults:
```json
"ConnectionStrings": {
  "LocalDBConn": "Data Source=np:\\\\.\\pipe\\LOCALDB#B9917A2A\\tsql\\query;Initial Catalog=AWEfinal;Integrated Security=True;MultipleActiveResultSets=true",
  "LoginConn":  "Data Source=np:\\\\.\\pipe\\LOCALDB#B9917A2A\\tsql\\query;Initial Catalog=AWEfinal;Integrated Security=True;MultipleActiveResultSets=true"
}
```
3) Create DB and seed: run the scripts in `AWEfinal.UI/Database` (e.g., `SeedData.sql`) or apply migrations if you add them later.

## Running the Web App (MVC)
```bash
dotnet run --project AWEfinal.UI
```
Visit the shown HTTPS URL (typically https://localhost:5001).

## Admin WinForms Dashboard
Open the solution in Visual Studio, set `AWEfinal.AdminWinForms` as startup, then run. Uses the same DB and services.

## Default Admin Credentials
- Email: admin@electrostore.com
- Password: (set in DB; update to your own)
(Use the admin password-change flow; passwords are hashed.)

## Tests (EP/BVA)
Project: `AWEfinal.Tests` (xUnit, Moq, FluentAssertions). Run:
```bash
dotnet test
```
Docs: `docs/TestPlan.md` (plan) and `docs/matrix.md` (traceability matrix).

## Notes
- Keep connection strings in sync across `AWEfinal.UI` and `AWEfinal.AdminWinForms`.
- If you add requirements or rules, extend the tests and update the matrix.***
