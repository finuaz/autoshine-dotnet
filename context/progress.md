# AutoShine Progress Context

> This file is a progress checkpoint for the AutoShine learning project.
> Update it after each meaningful implementation step so future sessions can resume without guessing.

## Current Position

**Current cycle:** Cycle 2 — API Foundation  
**Current step:** Step 2.4 — Run both applications  
**Current focus:** Remove the template `WeatherForecast` endpoint, then run Web and API together.  
**Next:** Step 2.5 — Typed `HttpClient` integration.

## Project State

The solution targets **.NET 10** and contains the Razor Pages Web project and the API project:

```text
AutoShine/
├── context/
├── src/
│   └── AutoShine.Web/      # Razor Pages application
│   └── AutoShine.Api/      # API application
└── AutoShine.slnx
```

Cycle 1 CRUD work is historical; the current work is establishing the Cycle 2 Web/API boundary.

## Completed So Far

Steps 1–5 below are historical implementation notes; the current status is summarized in the checkpoint at the end of this file.

### Cycle 0 — Documentation / Design Freeze

Completed.

Business direction, main workflow, membership rules, payment separation, messaging purpose, multi-project direction, and cycle-based development approach were agreed before implementation.

### Cycle 1 — CRUD Foundation

#### Step 1
Completed.

The initial ASP.NET Core Razor Pages project was created as the starting application.

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
dotnet sln AutoShine.slnx add src/AutoShine.Web/AutoShine.Web.csproj
```

This is correct: the `.csproj` is the project file, while `.slnx` is the solution container that references the project.

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
- Initial customer list functionality was created.
- **Database Migration**: Applied `dotnet ef database update` to create schema in MySQL.

The application had a working customer read operation at this historical checkpoint.

#### Steps 1–7
Completed (user-reported). Customer, Vehicle, Service, and Booking foundations have been implemented; verification remains part of the final review.

- CRUD foundation is reported complete for Customer, Vehicle, Service, and Booking.
- Six relationships have been explicitly configured with restrictive deletion rules.
- Final integrity review is in progress: inspect migrations, constraints, and application behavior.

## Architecture Direction

The next step after Cycle 1 is a retrospective and roadmap redesign, reconsidering the architecture with only Web and API projects.

The project is currently a Razor Pages Web application. Future architecture decisions are deferred to the Cycle 1 retrospective, which will reconsider:

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
Cycle 2 — API Foundation
  Step 2.4 — Run both applications
	API health endpoint ✅
	OpenAPI generation ✅
	Separate ports verified ✅
	Remove template WeatherForecast endpoint ⬅ PENDING
	Run Web and API together ⬅ PENDING
  Next — Step 2.5: Typed HttpClient integration
```

## Latest Progress Update

- Passed the API health endpoint and OpenAPI generation checks.
- Verified that Web and API are configured on separate ports.
- Pending: remove the template `WeatherForecast` endpoint and run Web and API simultaneously.
- Next: integrate a typed `HttpClient` in Step 2.5.
- Progress context only was updated; no application code or configuration was changed.
