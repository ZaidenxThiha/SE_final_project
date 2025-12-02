# Test Plan – AWEfinal

## 1. Scope
- Unit tests for business logic in `AWEfinal.BLL` (OrderService, ProductService, UserService).
- Focus: pricing/financial calculations, inventory adjustments, validation rules, authentication/password change.
- Out of scope: UI rendering (MVC/WinForms), EF Core integration tests, external payments.

## 2. Strategy (EP/BVA)
- Techniques: Equivalence Partitioning (valid/invalid inputs) and Boundary Value Analysis (min/max/zero).
- Level: Unit. Data access is mocked (Moq) to isolate service logic.
- Tooling: xUnit + FluentAssertions + Moq. Command: `dotnet test`.

## 3. Environment
- .NET 8 SDK
- No real database (all repositories mocked)
- Run from solution root: `dotnet test`

## 4. Test Items (Use Cases → Tests)
- UC1 Order pricing and inventory: O1, O2, O3, O4
- UC2 Product validation and update: P1, P2, P3, P4
- UC3 Authentication and password change: U1, U2, U3, U4, U5
- UC4 Order creation requires items: O2

## 5. Entry / Exit
- Entry: Solution builds; packages restore; test project referenced in solution.
- Exit: All unit tests pass; no P0/P1 defects; requirements unchanged or tests updated accordingly.

## 6. Risks / Mitigations
- Rule drift without test updates → keep matrix and tests in sync with requirements changes.
- Missing edge cases (currencies, locales) → expand EP/BVA partitions when new scenarios appear.

## 7. Traceability
- See `docs/matrix.md` for Requirements ↔ Test Cases (with EP/BVA tags) and current status.
