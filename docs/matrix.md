# Traceability Matrix

| Requirement ID | Requirement Summary | Design / Documentation References | Implementation References | Verification Status |
| --- | --- | --- | --- | --- |
| RQ-UI-001 | Customers must browse and filter electronics catalog by category. | High-level architecture in `README.md` (Presentation > Customer Web); planned UI mockups (`docs/` to-be-added). | Controllers + Razor views inside `AWEfinal.UI` (e.g., `Controllers/ProductController.cs`, `Views/Product`). | Manual web walkthrough; add automated UI tests (TBD). |
| RQ-UI-002 | Customers must manage a shopping cart and checkout. | Solution overview + flow discussion in `README.md` (Feature Set > Customer). | Cart + checkout controllers/services in `AWEfinal.UI` and shared services from `AWEfinal.BLL`. | Manual MVC testing; captured in final demo video. |
| RQ-AUTH-003 | Staff require secure login before performing management tasks. | Security section in requirements deck; admin-channel callout in `README.md` (Feature Set > Staff/Admin). | Authentication services in `AWEfinal.BLL`; login forms in both `AWEfinal.UI` admin pages and `AWEfinal.AdminWinForms/LoginForm.cs`. | Manual credential validation; future unit tests for auth logic (TBD). |
| RQ-ADM-004 | Staff can create/read/update/delete products and manage orders. | Staff dashboard scope documented in `README.md`; ERD in `docs/` (to be added). | `AWEfinal.AdminWinForms` (Dashboard/ProductEditor forms) plus shared services/repositories in `AWEfinal.BLL`/`AWEfinal.DAL`. | Manual WinForms demo; automated BLL tests pending. |
| RQ-REP-005 | System provides revenue metrics and reports for admin users. | Reporting requirement outlined under Feature Set > Staff/Admin. | Reporting services in `AWEfinal.BLL`; dashboard widgets in WinForms and MVC admin views. | Manual validation using seeded data snapshots; add regression tests later (TBD). |
| RQ-INFRA-006 | Platform must run on .NET 8 with SQL Server backend. | Architecture section of `README.md` and solution layout. | Project targets (`*.csproj`), EF Core DAL, SQL scripts (`sqllatest.sql`). | Verified by `dotnet build AWEfinal.sln` (see build logs) and DB migration scripts. |
| RQ-DOC-007 | Documentation bundle (requirements alignment, traceability, testing plan). | `docs/RequirementsAlignment.md` plus this matrix. | N/A (documentation deliverable). | Completed; review during submission checklist. |

> **Note:** Update this matrix whenever new requirements, design artifacts, implementation modules, or verification evidence are added so the submission stays audit-ready.

## Functional Requirements → Design Components Matrix

| FR ID | Functional Requirement | Classes / Components | Database Tables |
| --- | --- | --- | --- |
| FR-CUST-001 | Customers browse and filter catalog items by category and featured groupings. | `AWEfinal.UI/Controllers/HomeController.cs`, `AWEfinal.UI/Controllers/ProductController.cs`, Razor views under `Views/Product`, `AWEfinal.BLL/Services/ProductService.cs`, `AWEfinal.DAL/Repositories/ProductRepository.cs`, `AWEfinal.DAL/AWEfinalDbContext.cs`. | `Products` |
| FR-CUST-002 | Customers view detailed product information with specs, images, and availability. | `ProductController.Details`, shared view models, `ProductService`, `ProductRepository`, WinForms `ProductEditorForm` for data entry consistency. | `Products` |
| FR-CUST-003 | Customers manage cart contents and complete checkout, producing orders. | `AWEfinal.UI/Controllers/CartController.cs`, `OrderController.cs`, `ProductService`, `OrderService` (`AWEfinal.BLL/Services/OrderService.cs`), repositories for orders/products, session-backed cart helpers. | `Products`, `Orders`, `OrderItems`, `Users` (foreign keys). |
| FR-CUST-004 | Customers maintain order history and profile data. | `OrderController` history endpoints, `AuthController` profile/login flows, `AWEfinal.BLL/Services/UserService.cs`, `OrderService`, `UserService` repositories. | `Users`, `Orders`, `OrderItems`. |
| FR-STAFF-005 | Staff authenticate securely before accessing admin tooling. | `AWEfinal.UI/Controllers/AuthController.cs`, WinForms `LoginForm.cs`, `UserService`, `UserRepository`, shared session middleware. | `Users` |
| FR-STAFF-006 | Staff create/read/update/delete products via web admin and WinForms editor. | MVC `AdminController` (Products/Create/Edit/Delete actions), WinForms `ProductEditorForm.cs`, `DashboardForm.cs` product tab, `ProductService`, `ProductRepository`. | `Products` |
| FR-STAFF-007 | Staff manage orders (status transitions, tracking, cancellations). | `AdminController` order endpoints, WinForms `DashboardForm` order grid, `OrderService`, `OrderRepository`, supporting view models. | `Orders`, `OrderItems`, `Users` (association to purchaser). |
| FR-STAFF-008 | Staff view dashboards/reports (totals, revenue, timeframe filters). | `AdminController.Index` dashboard, WinForms `DashboardForm` reporting tab, `OrderService` aggregations, `ProductService` inventory counts, logging in `DashboardForm.cs`. | `Orders`, `OrderItems`, `Products`. |

Keep this matrix synchronized with the evolving feature backlog so every functional requirement maintains an explicit link to the classes and tables that satisfy it.
