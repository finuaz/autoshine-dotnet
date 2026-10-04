# AutoShine Progress Context

> This file is a progress checkpoint for the AutoShine learning project.
> Update it after each meaningful implementation step so future sessions can resume without guessing.

## Current Position

**Current cycle:** Cycle 1 — Single-project CRUD foundation  
**Current step:** Step 3 — completed  
**Next:** Cycle 1 — Step 4.

## Project State

The project currently uses a **single ASP.NET Core MVC/Web project**:

```text
AutoShine/
├── context/
├── src/
│   └── AutoShine.Web/
│       ├── Controllers/
│       ├── Models/
│       ├── Views/
│       ├── Properties/
│       ├── wwwroot/
│       ├── appsettings.Development.json
│       └── ...
└── AutoShine.sln
```

The current Visual Studio Solution Explorer shows the standard MVC project structure with:

- `Controllers/HomeController.cs`
- `Models/ErrorViewModel.cs`
- `Views/Home/Index.cshtml`
- `Views/Home/Privacy.cshtml`
- `Views/Shared/*`
- `wwwroot/*`
- `appsettings.Development.json`
- project properties / launch settings

## Completed So Far

### Cycle 0 — Documentation / Design Freeze

Completed.

Business direction, main workflow, membership rules, payment separation, messaging purpose, multi-project direction, and cycle-based development approach were agreed before implementation.

### Cycle 1 — Single-project CRUD Foundation

#### Step 1
Completed.

The initial ASP.NET Core MVC project was created as the starting application.

#### Step 2
Completed.

The basic project dependencies for database/EF Core work were installed:

```powershell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

The `AutoShine.Web.csproj` project was also registered in the solution.

Equivalent command:

```powershell
dotnet sln AutoShine.sln add src/AutoShine.Web/AutoShine.Web.csproj
```

This is correct: the `.csproj` is the project file, while `.sln` is the solution container that references the project.

#### Step 3
Completed.

Project structural fixes applied:

- **Program.cs**: Removed duplicate `WebApplication.CreateBuilder()` and reorganized initialization order (DbContext setup before service registration).
- **ApplicationDbContext.cs**: Fixed namespace case mismatch (`Autoshine.Web.Data` → `AutoShine.Web.Data`).
- **.gitignore**: Enhanced with production-grade entries (publish/, .vscode/, ApplicationInsights.config, launchSettings.json.user, etc.).
- **appsettings.json**: Verified MySQL connection string structure ready for local development.

Project now compiles cleanly with no validation errors on `using AutoShine.Web.Data;`.

#### Step 4
Completed.

EF Core DbContext configuration and initial migration created:

- **appsettings.json**: Fixed malformed `ConnectionStrings` object, added `"DefaultConnection"` key name.
- **appsettings.Development.json**: Added connection string with database password (kept out of source control via `.gitignore`).
- **ApplicationDbContext**: Verified DbContext dependency injection is properly wired in `Program.cs`.
- **Initial Migration**: Successfully ran `dotnet ef migrations add InitialCreate` to generate the baseline migration schema.

The application is now ready for domain entity modeling and data access patterns.

#### Step 5
Completed.

Domain entity models and CRUD scaffolding implemented:

- **Domain Models**: Created `Customer`, `Vehicle`, `MembershipPlan`, `CustomerMembership`, `Service`, `Booking`, `BookingItem`, and `Payment` classes in `/Models`.
- **ApplicationDbContext**: Added DbSet properties for all domain entities.
- **CustomersController**: Implemented with dependency injection of `ApplicationDbContext` and `Index()` action returning customer list.
- **Customers/Index.cshtml**: Created view template with table display of customer data (Name, PhoneNumber, Email, CreatedAt).
- **Database Migration**: Applied `dotnet ef database update` to create schema in MySQL.

The application now has a working CRUD read operation for customers with a functional view layer.

## Important Boundary

Do **not** jump to the final architecture yet.

At this stage AutoShine is intentionally still a **single-project application**. Later cycles will introduce:

- service layer
- Web API
- Razor/jQuery + Telerik/Kendo usage
- multiple `.csproj` projects
- DTOs / FluentValidation / DMapper
- Dapper reporting
- RabbitMQ / Worker / Outbox
- automated tests

The purpose of Cycle 1 is to establish the basic application and CRUD foundation first.

## Business Rules Remain Locked

The implementation must follow `context/business-rules.md`.

Important rules include:

- Booking lifecycle is server-owned.
- Cancellation is allowed only from `Booked`.
- Service prices are snapshotted into booking items.
- Membership discounts are snapshotted onto bookings.
- Member: 5% discount, 20% deposit.
- VIP: 10% discount, 0% mandatory deposit.
- Non-member: 30% deposit.
- Payment is separate from booking workflow status.
- Payments cannot exceed the outstanding balance.
- A booking can become `Completed` only when fully paid.
- Completed bookings are not ordinary CRUD-editable.
- Booking status changes must be auditable.

## Resume Rule

When continuing work:

1. Check this file first.
2. Check `README.md` and `context/business-rules.md` before making architectural decisions.
3. Continue from the next incomplete step rather than rebuilding completed work.
4. Update this file whenever a cycle/step is completed or the project state changes.

## Current Checkpoint

```text
Cycle 0  ✅
Cycle 1
  Step 1 ✅
  Step 2 ✅
  Step 3 ✅
  Step 4 ✅
  Step 5 ✅
  Step 6 ⬅ next (Implement CRUD operations for Bookings & Services)
```

**Last known terminal action:**

```powershell
dotnet ef database update
```

**Last known structural action:**

- Domain models defined with proper relationships (Customer, Vehicle, MembershipPlan, Service, Booking, BookingItem, Payment)
- CustomersController implemented with Index view
- Database migrated and schema created in MySQL
- Customers list view functional and ready for CRUD expansion
