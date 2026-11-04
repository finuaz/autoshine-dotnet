# AutoShine — Cycle 2 Development Plan

**Status:** Architecture decisions locked; Cycle 2 not started  
**Framework target:** .NET 10 (`net10.0`)  
**Solution projects:** `AutoShine.Web` and `AutoShine.Api` only  
**Communication:** Hybrid (Web server-side HTTP + browser-to-API HTTP)

---

## 1. Locked architecture decisions

| Area | Locked decision | Implementation consequence |
|---|---|---|
| Solution structure | Exactly two application projects | `AutoShine.Web` and `AutoShine.Api`; no separate Services, Data, Models, Messaging, or Worker projects for now |
| Framework | .NET 10 | Both `.csproj` files target `net10.0` |
| Web/API communication | Hybrid | Razor MVC actions use `HttpClient`; Kendo/jQuery/browser interactions can call the API directly |
| Booking/customer relation | No `Booking.CustomerId` | A booking identifies its vehicle; the customer is reached through `Booking.Vehicle.Customer` |
| Service lifecycle | Deactivate, do not hard-delete referenced services | Inactive services are unavailable for new bookings but remain available to historical booking records |
| Customer lifecycle | Archive/deactivate, do not hard-delete | Customer and vehicle records remain for history; archived customers are not offered for new work |
| Database ownership (target state) | API owns persistence | `ApplicationDbContext`, EF Core, migrations, and database connection move into `AutoShine.Api` in Cycle 3 |
| Business logic (target state) | API owns business workflows | Place application services inside `AutoShine.Api/Services/`; no separate project |
| Persistence (target state) | API owns data access | Place context, entity configurations and migrations inside `AutoShine.Api/Data/`; no separate project |
| Web's database access (target state) | Prohibited | Web uses HTTP contracts only after the Cycle 3 migration is complete |

### Hybrid communication rules

1. **Server-side path:** Razor form → MVC controller in Web → typed `HttpClient` client → API endpoint → response → Razor view.
2. **Browser path:** Kendo/jQuery/fetch in the browser → API endpoint → JSON response → browser UI.
3. Both paths call the same API operations and must not implement separate copies of business rules.
4. Web must not reference the API project to call its C# classes. The boundary is HTTP/JSON, not a project reference.
5. Browser cross-origin requests require a narrowly configured CORS policy during local development. CORS does not apply to server-to-server `HttpClient` calls.
6. Do not allow every origin with `AllowAnyOrigin()` just to make development easier. Use the exact Web development origin.
7. Authentication and authorization are not part of Cycle 2 unless the mentor explicitly requires them now. Do not expose real customer or financial data through the temporary diagnostic endpoint.

### Domain rules to carry forward

**Booking and customer**
- `Booking` keeps `VehicleId` and its `Vehicle` navigation property.
- Do not add `CustomerId` to `Booking` while ownership transfer remains unsupported.
- Customer lookup follows `Booking.Vehicle.Customer`.

**Services**
- A service is deactivated rather than physically deleted.
- Inactive services cannot be chosen for new bookings.
- Historical booking items retain their service name/type/price snapshots.
- Keep the service relationship restrictive while `BookingItem.ServiceId` is non-nullable.

**Customers**
- Archive/deactivate customers rather than deleting them.
- Archiving must not delete vehicles, bookings, payments, or status history.
- Archiving preserves vehicles and all historical bookings, payments, and status history; it is not a physical delete.
- The exact field (`IsActive`, `ArchivedAt`, or both), reactivation behaviour, and how archived customers appear in selection/search screens are schema/workflow decisions for Cycle 3; do not add these as part of the API skeleton.

---

## 2. Why Cycle 2 is deliberately database-independent

Cycle 2 establishes the HTTP boundary and a working API host. It does **not** move EF Core or MySQL yet. That data migration belongs to Cycle 3.

There is a dependency decision that must be handled before Cycle 3:

- The currently documented Pomelo release `Pomelo.EntityFrameworkCore.MySql` 9.0.0 supports EF Core 9.0.x, not EF Core 10.
- The Oracle-maintained `MySql.EntityFrameworkCore` package has a 10.0.12 release that advertises support for EF Core 8, 9 and 10. Its licensing and behaviour must be reviewed before adoption.
- Do not combine EF Core 10 packages with Pomelo 9.0.0. That mixed graph caused the previous `MissingMethodException`.
- The API scaffold in Cycle 2 does not need a MySQL provider, EF Core, or a `DbContext`.
- The existing Web project may temporarily retain its internally consistent EF Core 9/Pomelo 9 package set while the API boundary is being built. Treat this as a short transition only, not the intended .NET 10 end state. Do not upgrade only one EF Core package in isolation.
- Before Cycle 3, run a small provider compatibility spike and obtain any necessary mentor/team approval. A provider decision must include EF Core version compatibility, migrations, MySQL server compatibility, licensing, and whether the team accepts that provider.

Useful current references (checked 4 November 2026):
- .NET support policy: https://dotnet.microsoft.com/en-us/platform/support/policy
- Pomelo provider compatibility: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
- Oracle MySQL EF provider package: https://www.nuget.org/packages/MySql.EntityFrameworkCore
- EF Core provider-selection guidance: https://learn.microsoft.com/en-us/ef/core/providers/
- Controller-based Web API tutorial: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0

.NET 10 is the LTS choice. On 4 November 2026, Microsoft's support policy lists .NET 10.0.14 as the latest patch and support through November 2028. .NET 9 support is scheduled to end on 10 November 2026.

---

## 3. Cycle 2 goal and non-goals

### Goal

Create a separately runnable, controller-based `AutoShine.Api` project and prove that `AutoShine.Web` can communicate with it through both server-side and browser-side HTTP.

### Cycle 2 deliverables

- A two-project solution: `AutoShine.Web` and `AutoShine.Api`.
- Both projects target `net10.0`.
- API has controller-based routing and an OpenAPI document.
- A safe diagnostic endpoint such as `GET /api/v1/health` returns a JSON response.
- Web can call that endpoint using a typed `HttpClient` from an MVC controller.
- A browser page can call the same endpoint directly using `fetch()` or jQuery AJAX under an exact-origin CORS policy.
- Both applications can be run simultaneously in development.
- There is no project reference between Web and API.
- Documentation describes the transitional state and the intended API-owned database boundary.

### Explicit non-goals

Do not implement these in Cycle 2:

- Moving `ApplicationDbContext`, entities, migrations, or MySQL connection strings into API (Cycle 3).
- Customer, Vehicle, Service, Booking, or Payment CRUD endpoints (Cycle 3 onward).
- Booking workflow/business services (Cycle 4).
- Kendo CRUD grids (Cycle 5).
- FluentValidation, DMapper, broad DTO mapping (Cycle 6).
- Dapper dashboard/reporting (Cycle 7).
- RabbitMQ, Outbox, Worker, notification processing (Cycle 8).
- Authentication/authorization, unless required by the mentor as a prerequisite.
- A third project or shared class library.

---

## 4. Expected repository structure after Cycle 2

```text
AutoShine/
├── AutoShine.sln
├── Directory.Packages.props
├── context/
│   ├── architecture.md
│   ├── api.md
│   ├── business-rules.md
│   ├── cycles.md
│   ├── database.md
│   └── development-rules.md
└── src/
    ├── AutoShine.Web/
    │   ├── Controllers/          # MVC controllers; views are returned here
    │   ├── Views/                # Razor pages
    │   ├── ViewModels/            # UI-only models
    │   ├── ApiClients/            # Typed HTTP clients for API
    │   ├── wwwroot/               # CSS, JS, Kendo assets
    │   └── Program.cs
    └── AutoShine.Api/
        ├── Controllers/           # API controllers derive from ControllerBase
        ├── Contracts/             # Only contracts needed by endpoints
        ├── Properties/
        │   └── launchSettings.json
        ├── Program.cs
        └── AutoShine.Api.csproj
```

Notes:
- `AutoShine.Web` may still contain its current `Data/` and `Models/` during Cycle 2 because the persistence move is a Cycle 3 task. Do not create a second copy in API yet.
- The intended final API folders (`Services/`, `Data/`, `Models/`, `Validators/`, `Mappings/`) are introduced only when their responsibilities are implemented. They are folders, not separate `.csproj` files.
- `Contracts/` should not become a shared project. Web may have small client response models of its own until a stronger contract strategy is justified.
- Keep `Directory.Packages.props` for package version management. Put `TargetFramework` in each project file unless the solution later deliberately adopts a `Directory.Build.props` convention.

---

## 5. Detailed Cycle 2 steps

## Step 2.0 — Preflight and documentation freeze

**Purpose:** Establish a safe starting point before creating a second project.

Tasks:
1. Open a terminal at the repository root (the folder containing `AutoShine.sln`).
2. Run `git status` and commit the current Cycle 1 work if it is ready. If it is not ready, create a named backup branch before the refactor.
3. Back up the local MySQL database. Do not use destructive migration commands or recreate the database.
4. Confirm that `AutoShine.sln` currently lists the expected existing Web project.
5. Correct `Directory.Packages.props`: it should contain `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>` and `<PackageVersion ... />` entries, not the `TargetFramework` property.
6. Record the current `AutoShine.Web.csproj`, package list, connection setup, and current app launch URL in a Cycle 2 notes file.
7. Update `architecture.md` and `api.md` with the locked decisions above.

Commands:

```powershell
git status
dotnet sln AutoShine.sln list
dotnet --list-sdks
dotnet --version
dotnet list src/AutoShine.Web/AutoShine.Web.csproj package --include-transitive
```

**Exit check:** the repository is clean or the current work is safely committed/backed up; the solution and current package graph are known.

## Step 2.1 — Move the project target to .NET 10 safely

**Purpose:** Make the framework target explicit without repeating the previous EF Core/Pomelo mismatch.

Tasks:
1. Confirm a .NET 10 SDK is installed (`dotnet --list-sdks`).
2. Change `AutoShine.Web.csproj` to `<TargetFramework>net10.0</TargetFramework>`.
3. Do not put `TargetFramework` inside `Directory.Packages.props`.
4. Keep all existing EF Core packages and `dotnet-ef` aligned with the provider during the temporary transition. Do not set `Microsoft.EntityFrameworkCore` to 10 while still using Pomelo 9.
5. Build and run the existing Web app and check a page that reads the database.
6. Record the final framework and resolved packages.

Commands:

```powershell
dotnet clean src/AutoShine.Web/AutoShine.Web.csproj
dotnet restore src/AutoShine.Web/AutoShine.Web.csproj
dotnet list src/AutoShine.Web/AutoShine.Web.csproj package --include-transitive
dotnet build src/AutoShine.Web/AutoShine.Web.csproj
dotnet run --project src/AutoShine.Web/AutoShine.Web.csproj
```

**Exit check:** Web targets `net10.0`, builds, starts, and the existing database-backed pages still work. No mixed EF Core major versions.

## Step 2.2 — Create `AutoShine.Api`

**Purpose:** Add the second and only other application project.

Run these from the repository root:

```powershell
dotnet new webapi --use-controllers --framework net10.0 --name AutoShine.Api --output src/AutoShine.Api
dotnet sln AutoShine.sln add src/AutoShine.Api/AutoShine.Api.csproj
dotnet sln AutoShine.sln list
dotnet build AutoShine.sln
```

Important:
- Use the controller-based Web API template, not Minimal APIs, because the learning goal includes API controllers, attributes, routing and `ControllerBase`.
- Let the template create OpenAPI support, then inspect its generated `Program.cs` and `.csproj` before editing.
- Because the solution uses Central Package Management, if the template adds `Microsoft.AspNetCore.OpenApi`, add its chosen .NET 10 package version to `Directory.Packages.props` and remove any per-project `Version` attribute from the `PackageReference`. Keep the API package aligned with the .NET 10 servicing line.
- Do not add EF Core, Pomelo, `ApplicationDbContext`, or MySQL connection strings to API in this cycle.
- Do not add `ProjectReference` from Web to API or API to Web.
- Remove only the template's unused sample WeatherForecast code after confirming how the generated OpenAPI setup works.

**Exit check:** `dotnet sln list` shows exactly two projects and the solution builds.

## Step 2.3 — Understand and configure the API host

**Purpose:** Learn what makes an API controller different from an MVC controller.

API controller concepts:
- API controllers normally derive from `ControllerBase`, not `Controller`.
- `[ApiController]` enables API-specific behaviours such as automatic model validation responses.
- `[Route]`, `[HttpGet]`, `[HttpPost]`, and related attributes define HTTP routing.
- API actions return HTTP responses and JSON, not Razor views.
- Use explicit response semantics. For example, `Ok(...)` for 200 and `CreatedAtAction(...)` for 201 when a resource is created.

Tasks:
1. Inspect `Program.cs` and identify `AddControllers()`, `MapControllers()`, and the generated OpenAPI registration/mapping.
2. Keep the template's OpenAPI JSON endpoint working.
3. Use a versioned route prefix for the first AutoShine endpoint: `/api/v1/health`.
4. Do not add API versioning packages yet. The route prefix is a simple convention for this learning project.
5. Keep controller actions thin; real business operations will later be delegated to classes under `Services/` inside the API project.

Create `Controllers/HealthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace AutoShine.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "AutoShine.Api",
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }
}
```

Expected route: `GET /api/v1/health`.

Expected response shape (timestamp varies):

```json
{
  "service": "AutoShine.Api",
  "status": "ok",
  "timestampUtc": "..."
}
```

This is a diagnostic endpoint only. It does not prove MySQL is reachable; do not label it a database health check.

**Exit check:** API starts, OpenAPI JSON is reachable, and `GET /api/v1/health` returns HTTP 200 with JSON.

## Step 2.4 — Run both applications side by side

**Purpose:** Understand that Web and API are independent processes.

Tasks:
1. Inspect each project's `Properties/launchSettings.json`.
2. Give Web and API distinct HTTP/HTTPS ports; do not assume a particular port number.
3. Start API in one terminal:

```powershell
dotnet run --project src/AutoShine.Api/AutoShine.Api.csproj
```

4. Start Web in a second terminal:

```powershell
dotnet run --project src/AutoShine.Web/AutoShine.Web.csproj
```

5. Confirm both launch independently and neither depends on a C# reference to the other.
6. Record the API base URL and Web origin from the actual launch output.

**Exit check:** both processes run simultaneously on separate addresses.

## Step 2.5 — Server-side integration using a typed HttpClient

**Purpose:** Make an MVC controller in Web call an API endpoint without directly accessing API classes.

Tasks:
1. Add an API base URL to Web configuration. Use the actual API HTTPS launch URL; do not guess the port.
2. Create a small response model in Web, e.g. `ApiClients/HealthResponse.cs`.
3. Create a typed client, e.g. `IAutoShineApiClient` and `AutoShineApiClient`.
4. Register the typed client in Web with `AddHttpClient` and a configured `BaseAddress`.
5. Inject the client into a simple MVC controller action.
6. Call `GET api/v1/health` and display the returned status in a diagnostic Razor page.
7. Handle API unavailability gracefully. The Web page should show a clear service-unavailable message, not a raw exception or stack trace.
8. Pass `CancellationToken` through the HTTP call where practical.

Illustrative registration (adapt names/configuration to the generated project):

```csharp
builder.Services.AddHttpClient<IAutoShineApiClient, AutoShineApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Api:BaseUrl is missing.");

    client.BaseAddress = new Uri(baseUrl);
});
```

Do not hardcode a developer-specific API URL inside the client class. Keep it in environment-specific configuration.

**Exit check:** an MVC page displays the response returned by API through server-side `HttpClient`.

## Step 2.6 — Browser-side integration and CORS

**Purpose:** Prove that browser-based Kendo/jQuery code can call the API directly.

Tasks:
1. Add an API CORS policy for the exact Web development origin (scheme, host and port).
2. Apply the policy in the correct middleware position, before endpoint execution.
3. Do not use `AllowAnyOrigin()` as a shortcut.
4. Do not enable credentials unless a future authentication design explicitly requires them.
5. Add a small diagnostic button/page in Web whose JavaScript calls `GET /api/v1/health` using `fetch()` or jQuery AJAX.
6. Display the returned `status` in the page.
7. Test an allowed origin and confirm an unapproved origin is not granted access.
8. Record that CORS is a browser enforcement mechanism; it does not secure the API against non-browser clients.

**Exit check:** browser JavaScript can call the API from the permitted Web origin and receive JSON; an unapproved origin is not allowed by the CORS policy.

## Step 2.7 — Establish API response and error conventions

**Purpose:** Avoid inventing HTTP behaviour independently in every future controller.

For the future resource APIs, use these conventions:

| Situation | HTTP status | Meaning |
|---|---:|---|
| Successful read/update | 200 | Request succeeded and a response is returned |
| Resource created | 201 | Resource created; ideally return its location and representation |
| Request validation failed | 400 | Input is invalid |
| Resource does not exist | 404 | Requested resource is not found |
| Business conflict | 409 | Request conflicts with current domain state, e.g. duplicate active plate |
| Unexpected server failure | 500 | Unexpected error; don't expose stack traces to clients |

Tasks:
1. Record these conventions in `context/api.md`.
2. Inspect how `[ApiController]` handles invalid model state.
3. Keep server exceptions and stack traces out of API responses.
4. Do not build a large custom exception framework in Cycle 2. A simple consistent baseline is enough.
5. Authentication, authorization and detailed error middleware can be planned when real data endpoints are introduced.

**Exit check:** the API conventions are documented and the diagnostic endpoint behaves consistently.

## Step 2.8 — Documentation and Cycle 2 exit review

Update:
- `context/architecture.md`: two projects, Hybrid HTTP boundary, API persistence ownership as the target state, and temporary Cycle 2 transitional state.
- `context/api.md`: route prefix, controller responsibilities, HTTP status conventions, CORS expectations, and server/browser call patterns.
- `context/cycles.md`: mark Cycle 2 complete only after exit criteria pass; make Cycle 3 the API-owned data migration.
- `context/business-rules.md`: record booking/customer and archive/deactivation decisions.
- `context/development-rules.md`: Web calls API; no direct database access after Cycle 3.
- `README.md`: two-project structure, how to run each app, and current transitional database ownership.

Run final checks:

```powershell
dotnet sln AutoShine.sln list
dotnet build AutoShine.sln
dotnet list src/AutoShine.Web/AutoShine.Web.csproj reference
dotnet list src/AutoShine.Api/AutoShine.Api.csproj reference
```

Expected: exactly two projects, no Web↔API project reference, clean solution build.

Commit the cycle once verified, for example:

```powershell
git add .
git commit -m "Build two-project Web and API foundation"
```

---

## 6. Cycle 2 progress checklist

- [ ] 2.0 Preflight and documentation freeze
- [ ] 2.1 Move Web to `net10.0` and verify existing app
- [ ] 2.2 Create `AutoShine.Api` and add it to the solution
- [ ] 2.3 Configure controller-based API and OpenAPI
- [ ] 2.4 Run Web and API simultaneously
- [ ] 2.5 Server-side Web → API call through typed `HttpClient`
- [ ] 2.6 Browser → API call through fetch/jQuery and exact-origin CORS
- [ ] 2.7 Document API response/error conventions
- [ ] 2.8 Update context docs, verify solution, and commit
- [ ] Provider compatibility decision recorded as a gate before Cycle 3

## 7. Cycle 2 exit criteria

Cycle 2 is complete only when all of the following are true:

1. The solution contains exactly `AutoShine.Web` and `AutoShine.Api` as application projects.
2. Both projects target `net10.0` and build successfully.
3. API is controller-based and exposes an OpenAPI document.
4. `GET /api/v1/health` returns 200 with JSON.
5. Both applications can run at the same time.
6. Web calls API server-side through a typed `HttpClient` and displays the result.
7. Browser JavaScript calls API directly from the allowed Web origin.
8. CORS is restricted to the configured origin; no wildcard shortcut.
9. There is no direct Web-to-API project reference.
10. There is no second `DbContext` or duplicated entity model in API yet.
11. Architecture/API/business-rule documentation and the README reflect the actual code.
12. The MySQL/EF Core 10 provider has been investigated and a decision is recorded before Cycle 3 moves persistence.

---

## 8. Cycle 3 preview (not part of Cycle 2 implementation)

Cycle 3 will move database ownership from Web to API and implement the first real vertical slice:

```text
Browser / Razor UI
    ↓
AutoShine.Web
    ↓ HTTP / JSON
AutoShine.Api controller
    ↓
AutoShine.Api service
    ↓
EF Core / MySQL
```

Proposed first resource: **Customers**.

The slice should prove:
- API owns `ApplicationDbContext`, entities, EF mappings, migrations and the MySQL connection string.
- Web no longer references `ApplicationDbContext` or connects to MySQL.
- API exposes Customer list/details/create/update/archive endpoints with DTOs.
- Web can use the API through the agreed hybrid pattern.
- Archived customers remain in history and are excluded from new-work selection.

Before this cycle starts, resolve the EF Core 10/MySQL provider choice and validate it against a backup/development database. Do not apply a provider-switch migration to the only copy of the working database.

---

## 9. References checked on 4 November 2026

1. Microsoft .NET support policy — .NET 10 LTS, latest patch 10.0.14 as of 25 October 2026; .NET 9 support end date 10 November 2026: https://dotnet.microsoft.com/en-us/platform/support/policy
2. Pomelo EF Core MySQL provider — published compatibility table lists Pomelo 9.0.0 for EF Core 9.0.x: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
3. Oracle MySQL EF Core provider package — `MySql.EntityFrameworkCore` 10.0.12 and package description lists EF Core 8, 9 and 10 support: https://www.nuget.org/packages/MySql.EntityFrameworkCore
4. Microsoft EF Core database provider guidance — provider compatibility and licensing should be evaluated: https://learn.microsoft.com/en-us/ef/core/providers/
5. Microsoft controller-based Web API tutorial for ASP.NET Core 10: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0
