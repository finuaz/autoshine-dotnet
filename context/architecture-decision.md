# AutoShine Architecture Decisions

Last updated: 2026-11-05

## 1. Project Structure

The target solution contains exactly two projects:

- AutoShine.Web
- AutoShine.Api

Services, Data, Models, Contracts, Validators and Mappings are logical folders inside these projects, not separate projects.

## 2. Target Framework

Both projects target .NET 10 (`net10.0`).

Package versions must remain compatible with their respective dependencies.

The MySQL EF Core provider for .NET 10 remains an explicit technical decision to verify before Cycle 3. Do not mix EF Core 10 with Pomelo 9.

## 3. Web and API Communication

The application uses a hybrid communication model:

- Server-side MVC controllers use HttpClient to call the API.
- Browser-side JavaScript uses AJAX/fetch to call the API directly.
- Both communication patterns use the same API and business rules.

There must be no direct project reference from Web to API.

## 4. Database Ownership

AutoShine.Api owns database access, EF Core, database migrations and application business logic.

AutoShine.Web must not access MySQL or ApplicationDbContext directly.

## 5. Domain Decisions

- Booking does not contain a redundant CustomerId. Customer is determined through Vehicle.
- Services are deactivated rather than hard-deleted.
- Historical booking items preserve service identity and price snapshots.
- Customers are archived/deactivated rather than physically deleted.
- Historical bookings, payments and status history must be preserved.

## 6. Architectural Discipline

- Avoid unnecessary project proliferation.
- Establish working HTTP boundaries before moving database ownership.
- Introduce DTOs and validation after the basic API integration works.
- Defer RabbitMQ, Dapper and background processing until their respective cycles.
- Revisit architectural decisions explicitly when requirements change.