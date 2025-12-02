# AWEfinal – Online Electronics Store System  
_Final Project Report_

---

## Title Page

**Project Name:** AWEfinal – Online Electronics Store (3‑Tier System)  
**Course:** Software Engineering Final Project  
**Team Members:** _[Fill in student names and IDs]_  
**Supervisor:** _[Fill in supervisor name]_  
**Date:** _[Fill in submission date]_

---

## Executive Summary

AWEfinal is a three‑tier online electronics store system that enables customers to browse products, manage a shopping cart, place orders, and track their order status, while giving staff/admin users secure tools to manage products and monitor sales. The solution consists of:

- A **Data Access Layer (DAL)** using Entity Framework Core and SQL Server.
- A **Business Logic Layer (BLL)** implementing core rules (pricing, inventory, authentication).
- An **ASP.NET Core MVC web application** for customers and admin agents.
- A **WinForms desktop dashboard** for in‑store/office staff.

Key capabilities include product catalog browsing, cart and checkout,customer profile management, role‑based admin/staff features, and analytics over orders and revenue.  
Quality is supported by a dedicated xUnit test project (`AWEfinal.Tests`) that applies **Equivalence Partitioning (EP)** and **Boundary Value Analysis (BVA)** to core logic (stock validation, financial calculations, and authentication). All 17 unit tests currently pass, and a formal test plan plus traceability matrix are provided (`docs/TestPlan.md`, `docs/matrix.md`).

---

## Table of Contents

1. Introduction  
&nbsp;&nbsp;1.1 Purpose and Scope  
&nbsp;&nbsp;1.2 Product Overview  
&nbsp;&nbsp;1.3 Structure of the Document  
&nbsp;&nbsp;1.4 Terms, Acronyms, and Abbreviations  
2. Project Management Plan  
&nbsp;&nbsp;2.1 Project Organization  
&nbsp;&nbsp;2.2 Lifecycle Model Used  
&nbsp;&nbsp;2.3 Risk Analysis  
&nbsp;&nbsp;2.4 Hardware and Software Resource Requirements  
&nbsp;&nbsp;2.5 Deliverables and Schedule  
&nbsp;&nbsp;2.6 Monitoring, Reporting, and Controlling Mechanisms  
&nbsp;&nbsp;2.7 Professional Standards  
&nbsp;&nbsp;2.8 Configuration Management Evidence  
&nbsp;&nbsp;2.9 Impact on Individuals and Organizations  
3. Requirement Specifications  
&nbsp;&nbsp;3.1 Stakeholders  
&nbsp;&nbsp;3.2 Use Case Model  
&nbsp;&nbsp;3.3 Rationale for the Use Case Model  
&nbsp;&nbsp;3.4 Non‑Functional Requirements  
4. Architecture  
&nbsp;&nbsp;4.1 Architectural Style(s)  
&nbsp;&nbsp;4.2 Architectural Model  
&nbsp;&nbsp;4.3 Technology, Software, and Hardware  
&nbsp;&nbsp;4.4 Architectural Rationale  
5. Design  
&nbsp;&nbsp;5.1 GUI Design  
&nbsp;&nbsp;5.2 Static Model – Class Diagrams  
&nbsp;&nbsp;5.3 Dynamic Model – Sequence Diagrams  
&nbsp;&nbsp;5.4 Design Rationale  
&nbsp;&nbsp;5.5 Traceability from Requirements to Design  
6. Test Plan  
&nbsp;&nbsp;6.1 Requirements‑Based System Level Test Cases  
&nbsp;&nbsp;6.2 Traceability of Test Cases to Use Cases  
&nbsp;&nbsp;6.3 Techniques Used for Test Generation  
&nbsp;&nbsp;6.4 Assessment of Test Suite Quality  
7. Acknowledgment  
8. References  

---

## List of Figures

> _Note: Insert actual diagrams in your final document (e.g., exported from modeling tools) and update figure numbers accordingly._

- **Figure 4.1** – High‑Level 3‑Tier Architecture Diagram  
- **Figure 5.1** – Main Web UI Navigation Wireframe  
- **Figure 5.2** – WinForms Dashboard Layout  
- **Figure 5.3** – Core Domain Class Diagram (User, Product, Order, OrderItem)  
- **Figure 5.4** – Sequence Diagram: “Customer Checkout and Payment”  
- **Figure 5.5** – Sequence Diagram: “Admin Updates Order Status”

---

## List of Tables

- **Table 3.1** – Stakeholders and Interests  
- **Table 3.2** – Summary of Use Cases  
- **Table 3.3** – Non‑Functional Requirements  
- **Table 6.1** – Requirements‑to‑Test Cases Traceability (see also `docs/matrix.md`)

---

## 1. Introduction

### 1.1 Purpose and Scope

This report documents the development of **AWEfinal**, a three‑tier online electronics store system created as a Software Engineering final project. The document:

- Describes the project’s goals, stakeholders, and functional/non‑functional requirements.
- Explains the architecture and detailed design of the system.
- Presents the project management plan and process followed.
- Describes the test strategy and provides traceability from requirements to test cases.

The scope covers the customer web portal, the admin web portal, the WinForms staff dashboard, and the underlying BLL/DAL components, but excludes deployment automation and production‑grade monitoring.

### 1.2 Product Overview

AWEfinal supports the following high‑level capabilities:

- **Customer (Web)**  
  - Browse and filter electronics products.  
  - Add items to cart, adjust quantities, and checkout.   
  - View order history and individual order details.  
  - Manage profile information and change password.

- **Agent/Admin (Web)**  
  - Secure login with role‑based access.  
  - View product catalog and order list.  
  - For **admin**: create, edit, and delete products; manage inventory levels; update order status; view analytics and top‑selling products.  
  - For **agent/staff**: view‑only access to product list and safer admin screens (no destructive actions).

- **Staff (WinForms Desktop)**  
  - Login using admin or staff credentials.  
  - Dashboard overview: total revenue, total orders, products sold, average order value.  
  - Product management: add/edit/delete products, manage stock, upload images.  
  - Order management: search/filter orders, view details, update status, print delivery notes and receipts.  
  - Analytics: sales over time, top products, key KPIs.

### 1.3 Structure of the Document

The document is organized as follows:

- **Section 2** outlines project management aspects (organization, lifecycle, risks, resources).  
- **Section 3** specifies functional and non‑functional requirements with use cases.  
- **Section 4** describes the architecture and technologies used.  
- **Section 5** details GUI, static, and dynamic design models.  
- **Section 6** presents the test plan, test generation techniques (EP/BVA), and traceability.  
- **Sections 7–8** give acknowledgments and references.

### 1.4 Terms, Acronyms, and Abbreviations

- **AWEfinal** – Project name: AWE Electronics Final Project.  
- **DAL** – Data Access Layer.  
- **BLL** – Business Logic Layer.  
- **UI** – User Interface.  
- **MVC** – Model‑View‑Controller.  
- **EF Core** – Entity Framework Core.  
- **EP** – Equivalence Partitioning (test design technique).  
- **BVA** – Boundary Value Analysis (test design technique).  
- **KPI** – Key Performance Indicator.

---

## 2. Project Management Plan

### 2.1 Project Organization

The project team is logically organized into three roles (one or more people per role):

- **Full‑Stack Lead:** responsible for solution architecture, project setup, and cross‑layer integration (DAL/BLL/UI).  
- **Backend Engineer:** responsible for EF Core models, repositories, business services, and test project.  
- **UI/UX Engineer:** responsible for MVC views, styling, and WinForms layout and usability.

Responsibilities are shared for requirements clarification, test design, and documentation.

### 2.2 Lifecycle Model Used

The project followed an **iterative incremental lifecycle**:

1. **Iteration 1 – Core CRUD & Architecture:**  
   - Set up solution structure and EF Core context.  
   - Implement basic product listing and admin product CRUD.  
2. **Iteration 2 – Customer Journeys & Authentication:**  
   - Implement registration/login, cart, and checkout.  
   - Introduce role‑based access for admin vs regular users.  
3. **Iteration 3 – WinForms Dashboard & Analytics:**  
   - Design WinForms dashboard with product/order/analytics tabs.  
   - Implement printing, summaries, and filtering.  
4. **Iteration 4 – Hardening:**  
    -enforce password hashing.  
   - Add dedicated unit test project and documentation.

Each iteration ended with a buildable solution and a short internal review.

### 2.3 Risk Analysis

Key risks and mitigations:

- **R1 – Database corruption or schema mismatch:**  
  Mitigation: keep SQL scripts (`FullDatabaseScript.sql`, `SeedData.sql`) under version control; use a single LocalDB instance and shared connection string.

- **R2 – Security flaws (e.g., hardcoded admin password):**  
  Mitigation: hash passwords in BLL, remove hardcoded credentials, add tests for auth behaviors, and document SQL scripts to fix older data.

- **R3 – Role misuse (agents accidentally deleting products):**  
  Mitigation: introduce `staff`/`agent` roles with limited permissions; web UI hides destructive actions; server checks enforce roles.

- **R4 – Schedule overrun due to unfamiliar technologies (.NET 8):**  
  Mitigation: adopt incremental milestones; complete core CRUD before payment.
- **R5 – Insufficient test coverage:**  
  Mitigation: add a dedicated `AWEfinal.Tests` project with EP/BVA‑based tests on core services and maintain a traceability matrix.

### 2.4 Hardware and Software Resource Requirements

**Hardware:**  
- Development machines: standard laptops/PCs with ≥8 GB RAM.  
- Database: LocalDB hosted on the same machine.

**Software:**  
- .NET 8 SDK and Visual Studio 2022.  
- SQL Server LocalDB and (optionally) SSMS.  
- Git and GitHub for version control.  

New technologies learned by team members:

- Backend: Entity Framework Core 9, .NET 8 minimal 
- Frontend: ASP.NET Core MVC view composition, Bootstrap 5, modern card‑based UI patterns.  
- Desktop: advanced WinForms layout (TableLayoutPanel, DataGridView styling, printing APIs).  
- Quality: xUnit, Moq, FluentAssertions, EP/BVA test design.

### 2.5 Deliverables and Schedule

Main deliverables:

- Source code for all projects (`AWEfinal.DAL`, `AWEfinal.BLL`, `AWEfinal.UI`, `AWEfinal.AdminWinForms`, `AWEfinal.Tests`).  
- SQL scripts for database creation and seed data.  
- Test Plan and Test Traceability Matrix (`docs/TestPlan.md`, `docs/matrix.md`).  
- Final Report (`docs/report.md`).

Schedule followed course milestones (requirements, architecture, implementation, testing, final demo) over the semester.

### 2.6 Monitoring, Reporting, and Controlling Mechanisms

- Version control with Git and GitHub: feature branches and pull requests.  
- Informal weekly check‑ins to discuss progress and blockers.  
- Build and test via `dotnet build` and `dotnet test` before merges.  
- Manual smoke tests on core flows after each significant change.

### 2.7 Professional Standards

- Passwords stored as hashes (no plain text checks).  
- Separation of concerns: DAL/BLL/UI layers.  
- Use of industry documentation formats (this report, test plan).  
- Simple, readable coding style aligned with .NET conventions.  
- Respect for privacy: no real customer data; only demo seed data.

### 2.8 Configuration Management Evidence

- All source files, SQL scripts, and documentation are tracked in a Git repository.  
- The Visual Studio solution (`AWEfinal.sln`) references all projects including tests.  
- Database scripts (`FullDatabaseScript.sql`, `SeedData.sql`, troubleshooting scripts) are versioned under `AWEfinal.UI/Database`.

### 2.9 Impact on Individuals and Organizations

For **individuals**, AWEfinal demonstrates how a customer can safely shop online, review orders, and manage their data. It also shows staff how dashboards can simplify daily tasks, such as tracking orders and stock.  
For **organizations**, the system illustrates how role‑based access and analytics can reduce operational risk (e.g., limiting destructive actions for agents) and improve decision‑making (sales trends, top products).  
In a broader sense, the project emphasizes good security practices (hashed passwords, no hardcoded secrets) and test‑driven thinking, which are crucial for trustworthy systems in society.

---

## 3. Requirement Specifications

### 3.1 Stakeholders for the System

| Stakeholder        | Role / Interest |
|--------------------|-----------------|
| Customer           | Browse products, place orders, manage profile and history. |
| Admin              | Manage catalog, stock, orders, staff/agent permissions, view analytics. |
| Staff (Desktop)    | Operate WinForms dashboard for day‑to‑day product/order management. |
| Agent (Web Admin)  | View products and orders with safer, non‑destructive tools. |
| Business Owner     | See revenue, trends, and ensure profitable operation. |
| Course Instructor  | Evaluate engineering process, quality, and documentation. |

### 3.2 Use Case Model

#### 3.2.1 Graphic Use Case Model

> _In the final document, include a UML use case diagram showing actors (Customer, Admin, Staff, Agent) and the use cases below._

#### 3.2.2 Textual Description for Each Use Case

Below are representative use cases (UC):

**UC1 – Browse Products (Customer)**  
- **Primary actor:** Customer  
- **Preconditions:** Customer can access the site.  
- **Main flow:**
  1. Customer navigates to the home or products page.  
  2. System displays list of products with key details.  
  3. Customer filters by category and/or searches.  
  4. Customer selects a product to view details.  
- **Postconditions:** None (view‑only).

**UC2 – Checkout and Place Order (Customer)**  
- **Preconditions:** Customer is logged in and has items in cart.  
- **Main flow:**
  1. Customer opens the cart page and reviews items.  
  2. Customer proceeds to checkout and enters shipping/payment info.  
  3. System validates input and uses BLL to create an order and order items.  
  4. System persists the order and shows confirmation.  
- **Postconditions:** A new order record with status `pending` is stored.

**UC3 – Manage Products (Admin)**  
- **Preconditions:** Admin is logged in.  
- **Main flow:**
  1. Admin navigates to Admin → Products.  
  2. System lists existing products.  
  3. Admin can create a new product, edit an existing one, or delete it.  
  4. System validates required fields and price; updates database via BLL.  
- **Postconditions:** Product catalog is updated; associated stock changes are reflected.

**UC4 – View Products (Agent)**  
- **Preconditions:** Agent is logged in with `agent` role.  
- **Main flow:**
  1. Agent navigates to Admin → Products.  
  2. System lists products but hides create/edit/delete actions.  
  3. Agent can inspect product details but cannot modify or delete.  
- **Postconditions:** None (view‑only).

**UC5 – Update Order Status (Admin/Staff)**  
- **Preconditions:** Admin (web) or Staff (WinForms) is logged in. An order exists.  
- **Main flow:**
  1. User opens order management screen and selects an order.  
  2. User changes status (e.g., `paid`, `packaging`, `shipped`, `delivered`, `cancelled`).  
  3. BLL validates transition and adjusts inventory as needed.  
  4. System saves updated status and adjustment flags.  
- **Postconditions:** Order status and product stock are updated atomically.

**UC6 – Manage Profile and Password (Customer/Admin/Staff/Agent)**  
- **Preconditions:** User is logged in.  
- **Main flow:**
  1. User navigates to profile or change password page.  
  2. User updates profile fields or submits current and new password.  
  3. System validates input and updates records using BLL.  
- **Postconditions:** Profile is updated; for password changes, a new hash is stored.

### 3.3 Rationale for Your Use Case Model

The use case model focuses on:

- Clear separation of **customer** flows (browse, cart, checkout, profile) from **staff/admin** flows (product and order management, analytics).  
- Distinct roles for **admin** and **agent/staff**, reflecting safer web agent functions vs full admin control.  
- Specific order and payment flows, which are critical to the core value of an e‑commerce system.  

This ensures that the main business goals (selling products, managing orders, tracking performance) are supported without exposing dangerous operations to inappropriate roles.

### 3.4 Non‑Functional Requirements

Key non‑functional requirements include:

- **Performance:**  
  - The web UI must respond in under 2 seconds for normal operations on a typical development machine.  
  - Order listing and product listing must handle hundreds of records without noticeable lag.

- **Security:**  
  - Passwords stored as SHA256 hashes (Base64) only.  
  - No hardcoded admin passwords; no plaintext password comparisons in code.  
  - Role‑based access control for admin/staff/agent/customer actions.  

- **Usability:**  
  - Simple card‑based UI for customers with clear navigation.  
  - WinForms dashboard organized into tabs (Overview, Products, Orders, Analytics) for staff.  

- **Reliability:**  
  - Critical operations (order creation, status updates, stock updates) must be transactional at the BLL/DAL level.  

- **Maintainability:**  
  - Clear separation between DAL, BLL, and UI projects.  
  - Unit tests around core logic, following EP/BVA techniques.

---

## 4. Architecture

### 4.1 Architectural Style(s) Used

- **Layered architecture:** three main layers (DAL, BLL, UI).  
- **MVC pattern:** within `AWEfinal.UI` for web views and controllers.  
- **Repository pattern:** in the DAL for data access.  
- **Service abstraction:** in the BLL for business logic.

### 4.2 Architectural Model

At a high level (see Figure 4.1):

- **Presentation Layer (UI):**  
  - ASP.NET Core MVC controllers (customer and admin) and Razor views.  
  - WinForms desktop app consuming BLL services.  

- **Business Logic Layer (BLL):**  
  - Services such as `UserService`, `ProductService`, `OrderService` that encapsulate rules (validation, pricing, inventory adjustment).

- **Data Access Layer (DAL):**  
  - `AWEfinalDbContext` and EF Core entity classes for Users, Products, Orders, OrderItems.  
  - Repository interfaces and implementations for each aggregate (`IUserRepository`, `IProductRepository`, `IOrderRepository`).

### 4.3 Technology, Software, and Hardware Used

- **Backend:** .NET 8, ASP.NET Core MVC.  
- **Database:** SQL Server LocalDB.  
- **ORM:** Entity Framework Core 9.  
- **Desktop:** WinForms (.NET 8).  
- **Testing:** xUnit, Moq, FluentAssertions.

### 4.4 Rationale for Architectural Style and Model

- A **layered architecture** was chosen for clarity and separation of concerns, which is suitable for educational projects and aligns with the SE course emphasis.  
- Using **ASP.NET Core MVC** and **WinForms** demonstrates both web and desktop front‑ends consuming the same BLL.  
- EF Core provides a modern, strongly‑typed way to work with SQL Server while still allowing explicit SQL scripts for seeding and troubleshooting.  
- The architecture naturally supports expansion (e.g., adding a mobile client or more payment methods) by adding new presentation layers that reuse the BLL/DAL.

---

## 5. Design

### 5.1 GUI (Graphical User Interface) Design

- **Web UI:**  
  - Global layout `_Layout.cshtml` with navbar (Home, Products, Cart, Orders, Login/Register).  
  - Customer pages: Home, Product list/detail, Cart, Checkout, Order Confirmation, Order History, Profile.  
  - Admin pages (`_AdminLayout.cshtml`): dashboard with tabs for Products, Orders, Analytics, Account (change password).

- **WinForms UI:**  
  - `LoginForm`: card‑based admin/staff login form.  
  - `DashboardForm`: header (title, Change Password, Logout), navigation buttons, tab control:
    - Overview: KPIs in metric cards.  
    - Products: grid with search, add/edit/delete.  
    - Orders: grid, filters, details, print buttons.  
    - Analytics: charts and top‑products list.

### 5.2 Static Model – Class Diagrams

Core domain classes (textual view; diagram should be included separately):

- **User:** Id, Email, PasswordHash, Name, Role, CreatedAt, Phone, Address fields.  
- **Product:** Id, Name, Price, Category, Description, Storage, Rating, StockQuantity, InStock, Images.  
- **Order:** Id, OrderNumber, UserId, Total, Status, PaymentMethod, Shipping fields, InvoiceNumber, TrackingNumber, CreatedAt, UpdatedAt, InventoryAdjusted, `ICollection<OrderItem>`.  
- **OrderItem:** Id, OrderId, ProductId, ProductName, Quantity, Price, Subtotal.

Services in the BLL operate on these entities through repository interfaces.

### 5.3 Dynamic Model – Sequence Diagrams

Representative sequences:

- **Admin/Staff Updates Order Status:**  
  Admin/Staff → Admin controller or DashboardForm → BLL (`OrderService.UpdateOrderStatusAsync`) → DAL (`OrderRepository.UpdateAsync`) → DAL (`ProductRepository.UpdateAsync` for stock).

These sequences ensure inventory is only updated at appropriate status transitions and that all updates flow through a single business service.

### 5.4 Rationale for Your Detailed Design Model

- Encapsulating rules like pricing and inventory in **services** rather than controllers/forms makes logic reusable between web and WinForms.  
- Passing `Order` and `OrderItem` models to BLL keeps database concerns within DAL and EF Core.  
- Using separate DTOs for API/ClientApp (where needed) avoids over‑exposing entity details.

### 5.5 Traceability from Requirements to Detailed Design Model

- Requirements and use cases in Section 3 map to specific controllers, views, and BLL services.  
  - E.g., UC2 (Checkout) → `CartController.Checkout`, `OrderService.CreateOrderAsync`, `OrderRepository.CreateAsync`.  
  - UC3 (Manage Products) → `AdminController.Products/CreateProduct/EditProduct/DeleteProduct`, `ProductService`, `ProductRepository`.  
- This mapping is captured informally in this report and formally in test traceability (`docs/matrix.md`), where each requirement is linked to one or more tests.

---

## 6. Test Plan

### 6.1 Requirements/Specifications‑Based System‑Level Test Cases

System‑level tests (manual) include:

- Login flows for customer, admin, staff, agent.  
- Full purchase flow: add to cart → checkout → order confirmation → print receipt 
- Product management: create/edit/delete and verify impact on customer views.  
- Order management: status transitions and stock updates.  
- Profile & password changes, including invalid password handling.

Detailed unit‑level cases are implemented in `AWEfinal.Tests` against BLL services.

### 6.2 Traceability of Test Cases to Use Cases

- The traceability matrix in `docs/matrix.md` maps requirements (derived from use cases) to implemented test cases (O1–O4, P1–P4, U1–U5).  
- For example, UC2 and UC5 map to R1/R4 and are covered by order service tests verifying totals and status/stock logic.

### 6.3 Techniques Used for Test Generation

- **Equivalence Partitioning (EP):**  
  - Valid vs invalid email/password combinations.  
  - Valid vs invalid product properties (e.g., name required, price > 0).  
  - Valid vs invalid order statuses.

- **Boundary Value Analysis (BVA):**  
  - Price at exactly 0 vs greater than 0.  
  - Stock quantity boundaries (0 units, 1 unit, more than 1).  
  - Minimum password length scenarios.

Unit tests in `AWEfinal.Tests` explicitly exercise these partitions and boundaries.

### 6.4 Assessment of the Goodness of Your Test Suite

The test suite quality was assessed by:

- **Coverage of core rules:** all critical BLL methods (pricing, inventory adjustment, validation, auth, password change) have at least one EP and one BVA test.  
- **Requirement coverage:** each core requirement R1–R4 in `docs/matrix.md` is mapped to at least one passing test.  
- **Result stability:** all 17 tests pass consistently (`dotnet test`), indicating no flakiness at the unit level.

Future enhancements could include automated code coverage metrics and additional system/integration tests.

---

## 7. Acknowledgment

The team would like to thank the course instructor and teaching assistants for guidance on software engineering processes, and the peers who provided feedback during demos and code reviews.

---

## 8. References

[1] Microsoft, “ASP.NET Core Documentation,” _Microsoft Learn_. [Online]. Available: https://learn.microsoft.com/aspnet/core  
[2] Microsoft, “Entity Framework Core Documentation,” _Microsoft Learn_. [Online]. Available: https://learn.microsoft.com/ef/core  
[3] Microsoft, “Windows Forms Overview,” _Microsoft Learn_. [Online]. Available: https://learn.microsoft.com/dotnet/desktop/winforms  
[4] xUnit.net, “xUnit.net Documentation,” _xUnit.net_. [Online]. Available: https://xunit.net  
[5] I. Sommerville, _Software Engineering_, 10th ed. Pearson, 2015.

