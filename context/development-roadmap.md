# AutoShine Development Roadmap & Learning Context

> **Purpose:** This is the operational roadmap for the AutoShine learning project.
>
> It exists so development can continue consistently even when conversation history, model memory, or the active AI model changes.
>
> This file complements the other context files. It does not replace `business-rules.md`, `architecture.md`, `database.md`, `api.md`, `dashboard.md`, `messaging.md`, `development-rules.md`, or `README.md`.

## 1. How to use this file

Before starting implementation work:

1. Read this file.
2. Read `context/business-rules.md` when the change involves business behavior.
3. Read `context/architecture.md` when the change involves project boundaries or dependencies.
4. Read `context/database.md` when the change involves entities, relationships, persistence, or historical data.
5. Read `context/api.md` when the change involves HTTP/API behavior.
6. Read `context/dashboard.md` for reporting/KPI work.
7. Read `context/messaging.md` for RabbitMQ/Outbox/Worker work.
8. Read `context/development-rules.md` for coding and learning guardrails.
9. Read the current progress section in this file and `context/progress.md`.
10. Continue from the next incomplete step. Do not repeat completed work unless a later cycle intentionally refactors it.

### Central principle

AutoShine is built progressively.

Do not create the final architecture at the beginning.

Each cycle should introduce a meaningful concept, make it work, explain why it exists, verify it, and only then move on.

The project is intentionally allowed to refactor earlier code in later cycles. That is part of the learning exercise.

---

## 2. Authority and conflict resolution

When two pieces of documentation appear to disagree, use this order:

1. `context/business-rules.md` — business behavior is authoritative.
2. `context/architecture.md` — architectural boundaries are authoritative.
3. `context/database.md` — entity/data rules are authoritative for persistence design.
4. `context/api.md` — API boundary and endpoint principles.
5. `context/messaging.md` — messaging/Outbox/Worker behavior.
6. `context/dashboard.md` — KPI/reporting definitions.
7. `context/development-rules.md` — implementation/learning guardrails.
8. This file — implementation sequence and progress.
9. `README.md` — high-level project overview.

If implementation genuinely changes an agreed design:

- do not silently change code only;
- identify the affected context file;
- update that context file;
- update this roadmap if the implementation sequence changes;
- record the reason.

---

## 3. Cycles vs steps

The original project design defines these cycles:

```text
Cycle 0 → design/documentation freeze
Cycle 1 → single-project CRUD foundation
Cycle 2 → service layer + business workflow
Cycle 3 → Web API + UI/API boundary
Cycle 4 → Telerik/Kendo UI
Cycle 5 → multi-project solution structure
Cycle 6 → DTO + FluentValidation + DMapper
Cycle 7 → dashboard + Dapper reporting
Cycle 8 → RabbitMQ + Worker + Outbox
Cycle 9 → testing, hardening, cleanup
```

The source `cycles.md` defines the purpose, deliverable, learning goal, and exit criteria for each cycle, but does not fully number every implementation step.

Therefore:

- **Cycle-level scope below is the project baseline.**
- **Step-level breakdown below is the operational decomposition used to execute that baseline.**
- Step-level details are explicit so another model can resume without guessing.
- An operational step is not automatically a business rule.

---

# 4. Current project state

## Current cycle

**Cycle 1 — Single-project CRUD foundation**

## Current step

**Step 5 — completed**

## Next step

**Step 6 — Implement CRUD operations for Bookings & Services**

## Current checkpoint

```text
Cycle 0  ✅
  Design/documentation freeze

Cycle 1
  Step 1 ✅
  Step 2 ✅
  Step 3 ✅
  Step 4 ✅
  Step 5 ✅
  Step 6 ⬅ NEXT
```

## Current application shape

The application is intentionally still a single ASP.NET Core MVC/Web project:

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
│       └── ...
└── AutoShine.sln
```

The project is registered in the solution.

This is **not yet Cycle 5 multi-project architecture**.

## Known completed implementation

According to the current progress checkpoint:

- initial ASP.NET Core MVC project exists;
- EF Core, EF Core Design, and Pomelo MySQL packages were installed;
- `AutoShine.Web.csproj` was registered in `AutoShine.sln`;
- Program/DbContext/configuration housekeeping was completed;
- EF Core `ApplicationDbContext` is registered through dependency injection;
- an initial migration was created;
- domain models exist for:
  - Customer
  - Vehicle
  - MembershipPlan
  - CustomerMembership
  - Service
  - Booking
  - BookingItem
  - Payment
- `DbSet` properties were added;
- `CustomersController` exists and uses `ApplicationDbContext`;
- `Customers/Index.cshtml` exists and displays customer data;
- `dotnet ef database update` was run successfully;
- the MySQL schema exists;
- the customer read/list path is functional.

The current progress checkpoint identifies Step 6 as:

> **Implement CRUD operations for Bookings & Services**

---

# 5. Global cycle philosophy

Every cycle follows this loop:

```text
Understand
   ↓
Plan small change
   ↓
Implement
   ↓
Build
   ↓
Run / manually verify
   ↓
Explain what was learned
   ↓
Update progress/context
   ↓
Commit/tag cycle checkpoint
   ↓
Next step/cycle
```

A step is not complete merely because the code compiles.

A meaningful step should have:

- working code;
- build success;
- relevant behavior manually verified;
- important edge cases considered;
- code understood well enough to explain;
- progress/context updated.

---

# 6. Cycle 0 — Design / Documentation Freeze

## Purpose

Remove ambiguity before implementation.

## Deliverables

- README
- business rules
- architecture
- database model
- API principles
- dashboard definitions
- messaging design
- implementation cycles
- development rules

## Step 0.1 — Establish the problem/domain

Define AutoShine as a small car-cleaning/detailing management system.

Main business objects:

```text
Customer
Vehicle
MembershipPlan
CustomerMembership
Service
Booking
BookingItem
Payment
BookingStatusHistory
Notification
OutboxEvent
```

## Step 0.2 — Freeze core workflow

Main lifecycle:

```text
Booked
  ↓
Waiting
  ↓
Washing
  ↓
Cleaning
  ↓
Drying
  ↓
Review
  ↓
Payment
  ↓
Completed
```

Additional treatment paths:

```text
Drying → AdditionalTreatment → Review
Review → AdditionalTreatment → Review
```

Cancellation:

```text
Booked → Cancelled
```

Only `Booked` can be cancelled.

## Step 0.3 — Freeze membership/payment rules

```text
Non-member → 0% discount, 30% deposit
Member     → 5% discount, 20% deposit
VIP        → 10% discount, 0% mandatory deposit
```

Financial facts are snapshotted.

Payment is separate from booking workflow status.

## Step 0.4 — Freeze architecture direction

Target architecture:

```text
Web
 ↓
API
 ↓
Services
 ↓
Data
 ↓
MySQL

Services → Messaging → RabbitMQ → Worker
```

The eventual system is multi-project, but implementation is intentionally incremental.

## Step 0.5 — Freeze reporting/messaging direction

Dashboard uses read-oriented reporting queries.

Dapper is reserved for dashboard/reporting reads.

RabbitMQ is for asynchronous secondary work.

Outbox is introduced later to protect DB/message consistency.

### Exit criteria

Cycle 0 is complete when:

- business flow is understood;
- main entities are understood;
- project boundaries are understood;
- dashboard responsibilities are understood;
- RabbitMQ purpose is understood;
- build order is accepted.

**Status: COMPLETE**

---

# 7. Cycle 1 — Single-Project CRUD Foundation

## Purpose

Start deliberately close to Repo 1.

Learn the AutoShine domain while refreshing:

- MVC
- models
- controllers
- Razor
- EF Core
- MySQL
- CRUD
- basic relationships

## Explicitly not yet

- RabbitMQ
- Dapper
- multi-project architecture
- heavy Kendo UI
- complex DTO architecture
- service-layer architecture

Cycle 1 should feel intentionally simple.

## Step 1 — Create the starting MVC application

### Goal

Have a standard ASP.NET Core MVC application that can run.

### Work

- create `AutoShine.Web`;
- establish Controllers/Models/Views/wwwroot structure;
- add it to `AutoShine.sln`;
- confirm it runs.

### Learning

- project file;
- solution file;
- startup;
- MVC routing;
- controller/view relationship.

### Done when

The default application builds and runs.

**Status: COMPLETE**

---

## Step 2 — Install initial persistence dependencies

### Goal

Prepare for MySQL + EF Core.

### Work

```powershell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

Register:

```text
AutoShine.Web.csproj
    ↓
AutoShine.sln
```

### Learning

- NuGet packages;
- project dependencies;
- `.csproj`;
- `.sln`.

### Done when

Packages restore successfully and the project builds.

**Status: COMPLETE**

---

## Step 3 — Establish configuration and DbContext infrastructure

### Goal

Connect application configuration to EF Core.

### Work

Create:

```text
Data/ApplicationDbContext.cs
```

Register:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(...);
```

Configure the MySQL connection string.

### Learning

- `DbContext`;
- `DbContextOptions<T>`;
- dependency injection;
- configuration;
- connection strings;
- `ServerVersion.AutoDetect`.

### Done when

`dotnet build` succeeds and the application starts with DbContext registered.

**Status: COMPLETE**

---

## Step 4 — Create migration/database baseline

### Goal

Prove EF Core can create the MySQL schema.

### Work

Typical commands:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Learning

- migration;
- schema evolution;
- migration history;
- model vs database schema.

### Done when

Database exists and migration is applied.

**Status: COMPLETE**

---

## Step 5 — Establish domain entities and first working read path

### Goal

Move from an empty EF Core shell to real AutoShine data.

### Domain models

```text
Customer
Vehicle
MembershipPlan
CustomerMembership
Service
Booking
BookingItem
Payment
```

### Work

- create model classes;
- create initial relationships;
- add `DbSet`s;
- migrate/update database;
- create `CustomersController`;
- inject `ApplicationDbContext`;
- implement customer listing;
- create customer Razor list view.

### Learning

Refresh:

```csharp
private readonly ApplicationDbContext _context;

public CustomersController(ApplicationDbContext context)
{
    _context = context;
}
```

Learn:

- dependency injection in controllers;
- `DbSet<T>`;
- EF Core queries;
- async query execution;
- controller action → model → Razor view.

### Done when

Customer data is retrieved from MySQL and rendered in Razor.

**Status: COMPLETE**

---

## Step 6 — Implement CRUD operations for Bookings & Services

**NEXT STEP**

### Goal

Expand beyond the first customer read screen.

### Service CRUD

At minimum:

- list services;
- create service;
- view/edit service;
- respect active/inactive state;
- preserve information needed for future history.

### Booking CRUD foundation

At minimum:

- list bookings;
- create booking;
- view booking;
- edit booking data where appropriate for Cycle 1;
- show related customer/vehicle/service information;
- manage booking items sufficiently for the initial CRUD foundation.

### Important boundary

Cycle 1 is intentionally simpler than the final business workflow.

Do **not** implement the complete status state machine in controllers.

Do not silently invent service-layer business logic before Cycle 2.

Do not use arbitrary deletion to destroy historical booking/payment information.

Where a final business rule conflicts with a generic CRUD action, prefer preserving historical integrity and mark the richer behavior for Cycle 2.

### Learning

- model binding;
- related entities;
- dropdown/selection values;
- EF Core relationships;
- CRUD controller patterns;
- Razor forms.

### Verification

```text
Service:
Create → Read → Update → verify persistence

Booking:
Create → Read → Update → verify persistence
```

### Done when

Basic Service and Booking CRUD foundation works and is manually verified.

**Status: NEXT**

---

## Step 7 — Complete remaining CRUD foundation

### Goal

Finish the simple CRUD scope promised by Cycle 1.

### Work

Finish/verify:

- Customer CRUD;
- Vehicle CRUD;
- Service CRUD;
- Booking foundation cleanup.

### Boundary

Still one MVC project.

Still no service layer.

Still no API.

Still no Kendo-heavy UI.

### Done when

The intended Cycle 1 CRUD paths work manually.

---

## Step 8 — Relationship and integrity verification

### Goal

Verify the CRUD screens remain coherent with relationships.

### Relationships

```text
Customer → multiple Vehicles
Vehicle → Customer
Booking → Vehicle
Booking → Customer
Booking → BookingItems
BookingItem → Service
Booking → Payments
```

### Basic integrity

- plate number uniqueness;
- required customer/vehicle references;
- sensible service values;
- valid booking relationships.

Do not implement sophisticated workflow orchestration here.

---

## Step 9 — Cycle 1 cleanup and exit verification

### Checklist

- solution builds;
- app runs;
- database exists;
- migrations are clean;
- Customer CRUD works;
- Vehicle CRUD works;
- Service CRUD works;
- Booking foundation works;
- Razor screens work;
- persistence is verified;
- no later-cycle technology was introduced prematurely.

### Learning review

Be able to explain:

- what `DbContext` does;
- what DI is doing;
- how a controller gets data;
- how EF Core translates LINQ;
- how model binding works;
- how Razor receives a model;
- why everything is still in one project.

### Cycle exit

> CRUD works and is manually verified.

---

# 8. Cycle 2 — Service Layer + Booking Workflow

## Purpose

Move from CRUD to real business behavior.

Target:

```text
Controller
    ↓
Service
    ↓
EF Core
```

## Step 2.1 — Introduce application/service layer

- create service boundary;
- move workflow logic out of controllers;
- inject services into controllers;
- understand controller vs service responsibility.

## Step 2.2 — Introduce LINQ intentionally

Use LINQ for filtering, projection, relationships, and calculations.

Distinguish:

```text
LINQ to Objects
LINQ → EF Core → SQL
Dapper SQL
```

## Step 2.3 — Implement booking creation workflow

Calculate:

```text
service subtotal
→ membership discount
→ total
→ required deposit
```

## Step 2.4 — Implement financial snapshots

At booking/item creation, preserve:

- service name;
- service type;
- unit price;
- discount;
- membership benefit;
- total/deposit.

Never recompute historical totals from mutable master data.

## Step 2.5 — Implement membership rules

```text
Non-member = 30% deposit
Member     = 20% deposit + 5% discount
VIP        = 0% mandatory deposit + 10% discount
```

VIP priority only applies among waiting vehicles.

## Step 2.6 — Implement booking state machine

```text
Booked → Waiting
Waiting → Washing
Washing → Cleaning
Cleaning → Drying
Drying → Review
Review → Payment
Payment → Completed
```

Additional:

```text
Drying → AdditionalTreatment → Review
Review → AdditionalTreatment → Review
```

Cancellation:

```text
Booked → Cancelled
```

Invalid transitions are rejected.

## Step 2.7 — Implement status history

Every status change records:

- booking;
- previous status;
- new status;
- timestamp;
- actor when available;
- note/reason when applicable.

## Step 2.8 — Implement payment behavior

Support:

- multiple payments;
- partial payment;
- outstanding balance;
- no overpayment;
- fully paid state;
- completion only when paid.

## Step 2.9 — Implement cancellation and additional-treatment behavior

Cancellation:

- only from `Booked`;
- reason required;
- refund handling manual.

Additional treatment:

- customer approval when newly chargeable;
- approval makes it billable;
- rejection remains auditable.

## Step 2.10 — Verify service-layer business rules

Verify:

- valid transitions;
- invalid transitions;
- payment limits;
- membership calculations;
- completion/payment relationship;
- cancellation restriction;
- snapshots;
- repeated review/rework path.

### Cycle exit

A booking can correctly move through the workflow and invalid transitions are rejected server-side.

---

# 9. Cycle 3 — Web API + UI/API Boundary

## Purpose

Introduce HTTP/JSON while preserving service/business separation.

Target:

```text
UI
 ↓ HTTP/JSON
API controller
 ↓
Service
 ↓
EF Core
```

## Step 3.1 — Introduce API controllers

HTTP concerns only.

## Step 3.2 — Expose one complete CRUD resource

For example Customer:

```text
GET
GET by id
POST
PUT
DELETE
```

## Step 3.3 — Add Vehicle/Service API paths

Follow API context.

## Step 3.4 — Add booking workflow actions

Examples:

```text
arrive
start-washing
start-cleaning
start-drying
add-treatment
review
approve-treatment
reject-treatment
request-payment
complete
cancel
```

## Step 3.5 — Establish error semantics

Distinguish:

- validation failure;
- not found;
- invalid business transition;
- conflict;
- unexpected server/database failure.

## Step 3.6 — Move at least one browser flow through API

Demonstrate:

```text
Browser
 ↓
HTTP
 ↓
API
 ↓
Service
 ↓
EF Core
```

### Cycle exit

At least one complete CRUD path is driven through the API.

---

# 10. Cycle 4 — Telerik/Kendo UI

## Purpose

Introduce the data-heavy UI layer deliberately.

Start with Bookings.

## Step 4.1 — Telerik/Kendo setup

- configure the actual Telerik package/feed when reached;
- verify licensing/build setup;
- do not commit credentials.

## Step 4.2 — Booking Grid

Learn:

- grid;
- columns;
- data source;
- remote data.

## Step 4.3 — AJAX/API integration

Connect Kendo data sources to API endpoints.

## Step 4.4 — Sorting/filtering/paging

Implement practical server/API-backed interactions.

## Step 4.5 — Editing and lookup controls

Introduce customer/vehicle/service lookups.

## Step 4.6 — Use Kendo only where valuable

Do not widgetize everything.

### Cycle exit

Booking/customer/service grids can load and perform agreed operations through the API.

---

# 11. Cycle 5 — Multi-Project Solution

## Purpose

Refactor the working application into meaningful build/dependency boundaries.

Target:

```text
AutoShine.Web
AutoShine.Api
AutoShine.Services
AutoShine.Data
AutoShine.Models
AutoShine.Messaging
```

The `Worker` is planned for Cycle 8 in this detailed roadmap unless a concrete prerequisite requires its earlier creation.

## Step 5.1 — Extract Models

Move genuinely shared types.

## Step 5.2 — Extract Data

Move:

- `ApplicationDbContext`;
- EF configuration;
- database-specific concerns.

## Step 5.3 — Extract Services

Move application workflows/business logic.

## Step 5.4 — Extract API

Move HTTP/API controllers and HTTP-specific handling.

## Step 5.5 — Establish ProjectReferences

Target direction:

```text
Web → API boundary
API → Services
Services → Models + Data abstractions + Messaging abstractions
Data → Models
```

## Step 5.6 — Verify boundaries

Check:

- Web does not directly query MySQL;
- API does not contain core business logic;
- Services do not know Razor/Kendo;
- Data owns database details.

## Step 5.7 — Verify build boundaries

Use compiler failures deliberately to understand dependency enforcement.

### Cycle exit

Application still runs and the intended project boundaries are enforced.

---

# 12. Cycle 6 — DTO + FluentValidation + DMapper

## Purpose

Separate persistence models from API contracts.

## Step 6.1 — Request DTOs

Explicit input types where useful.

## Step 6.2 — Response DTOs

Explicit output contracts.

## Step 6.3 — FluentValidation

Use for input/request validation.

Example:

```text
Quantity > 0
Name required
```

Database/workflow state rules remain service logic.

## Step 6.4 — DMapper

Use meaningful mappings:

```text
Entity → Response DTO
Request DTO → application/entity model
```

## Step 6.5 — Protect persistence details

Avoid leaking unnecessary database structure through API responses.

## Step 6.6 — Refactor representative flows

At minimum, Customer and Booking should demonstrate the pattern.

### Cycle exit

Customers/Bookings use DTOs, FluentValidation, and DMapper coherently.

---

# 13. Cycle 7 — Dashboard + Dapper

## Purpose

Teach reporting-oriented SQL and build the business dashboard.

Default periods:

```text
Today
This Week
This Month
Custom Date Range
```

## Step 7.1 — Implement dashboard query contracts

Use `dashboard.md` as the KPI definition source.

## Step 7.2 — Introduce Dapper

Use Dapper for:

- KPI summaries;
- status counts;
- revenue;
- outstanding payments;
- service popularity;
- revenue by service;
- customer/membership reporting.

## Step 7.3 — Implement financial metrics

Respect:

```text
Expected revenue
Collected payments
Outstanding payments
Monthly revenue
```

Reflected refund semantics correctly.

## Step 7.4 — Implement workload metrics

- jobs;
- estimated hours;
- bay utilization;
- average duration.

Bay utilization remains an estimate, not a scheduling constraint.

## Step 7.5 — Query/service abstraction

Keep reporting orchestration out of controllers/UI.

## Step 7.6 — Dashboard API

Possible endpoints:

```text
GET /api/dashboard/overview
GET /api/dashboard/operations
GET /api/dashboard/revenue
GET /api/dashboard/services
GET /api/dashboard/customers
GET /api/dashboard/memberships
GET /api/dashboard/outstanding-payments
```

## Step 7.7 — Dashboard UI

Use simple Razor/HTML for KPI cards and Kendo where interaction provides value.

## Step 7.8 — Reconcile

Compare known expected numbers against actual dashboard output.

### Cycle exit

Dashboard KPIs reconcile with known database scenarios.

---

# 14. Cycle 8 — RabbitMQ + Worker + Outbox

## Purpose

Teach asynchronous processing and DB/message consistency.

Initial events:

```text
PaymentReceived
BookingCompleted
```

## Step 8.1 — Event contracts

Define the event messages.

## Step 8.2 — OutboxEvent

Support:

- event type;
- aggregate type;
- aggregate id;
- payload;
- created time;
- published time;
- retry count;
- error.

## Step 8.3 — Transactional outbox write

Example:

```text
DB transaction
 ├── business state
 └── OutboxEvent
       ↓ commit
```

## Step 8.4 — Publisher

Read pending outbox events and publish.

## Step 8.5 — Worker

Consume RabbitMQ messages and create simulated Notifications.

## Step 8.6 — Messaging vocabulary

Explicitly learn:

- producer;
- consumer;
- queue;
- exchange;
- routing key;
- message;
- acknowledgement;
- retry;
- failure/dead-letter concept.

## Step 8.7 — Failure scenario

```text
DB commit succeeds
RabbitMQ publish fails
```

Expected:

```text
OutboxEvent remains pending
Retry later
```

## Step 8.8 — End-to-end verification

```text
Business transaction
→ Outbox
→ Publisher
→ RabbitMQ
→ Worker
→ Notification
```

### Cycle exit

Successful transactions produce, publish, and consume events; simulated broker failure leaves work retryable.

---

# 15. Cycle 9 — Testing + Hardening

## Purpose

Turn the system into a maintainable learning repository.

## Step 9.1 — Business-rule tests

Prioritize:

- workflow transitions;
- cancellation;
- additional treatment;
- membership discount;
- deposits;
- payment limits;
- completion.

## Step 9.2 — API tests

Verify:

- validation;
- not found;
- conflict;
- invalid transition;
- success behavior.

## Step 9.3 — Edge cases

Examples:

- inactive service;
- expired membership;
- multiple payments;
- zero outstanding amount;
- attempted overpayment;
- duplicate plate;
- duplicate scheduled time;
- repeated review/rework.

## Step 9.4 — Dashboard reconciliation tests

Verify KPI results against known fixtures.

## Step 9.5 — Messaging tests

Verify:

- outbox creation;
- successful publication;
- failed publication/retry;
- Worker consumption;
- notification creation.

## Step 9.6 — Cleanup/refactoring

Remove dead code, duplication, unnecessary abstractions, and accidental dependencies.

## Step 9.7 — Documentation hardening

A new developer/model should be able to determine:

```text
What is this?
Why is it structured this way?
Where is each business rule?
Where is persistence?
Where is the API?
Where are reports?
Where is messaging?
Which cycle introduced this?
```

### Cycle exit

Critical business rules have automated coverage and the repository is understandable without the original chat.

---

# 16. Cross-cycle learning map

| Cycle | Main question |
|---|---|
| 0 | What are we building and why? |
| 1 | How do basic ASP.NET MVC + EF CRUD patterns work? |
| 2 | Where should real business logic live? |
| 3 | How does a UI communicate through HTTP/API? |
| 4 | How do Razor/jQuery/Kendo consume remote data? |
| 5 | What do `.sln` and `.csproj` boundaries actually enforce? |
| 6 | Why are entities different from API contracts? |
| 7 | Why use Dapper for reporting instead of forcing everything through EF? |
| 8 | Why use asynchronous messaging and an Outbox? |
| 9 | How do we prove the system remains correct? |

---

# 17. Technology introduction schedule

Technology should appear when it has a concrete reason.

```text
ASP.NET Core MVC       → Cycle 1
Razor                  → Cycle 1
EF Core                → Cycle 1
MySQL                  → Cycle 1
LINQ                   → Cycle 2
Service layer          → Cycle 2
Web API                → Cycle 3
jQuery                 → Cycle 3/4 as needed
Telerik/Kendo          → Cycle 4
Multi-project .csproj  → Cycle 5
DTOs                   → Cycle 6
FluentValidation       → Cycle 6
DMapper                → Cycle 6
Dapper                 → Cycle 7
RabbitMQ               → Cycle 8
Worker                 → Cycle 8
Outbox                 → Cycle 8
xUnit/tests            → Cycle 9
```

Do not install every dependency just because it appears in the final architecture.

---

# 18. Architectural boundaries that must survive every cycle

## Web

Owns:

- Razor;
- jQuery;
- Kendo;
- browser UI;
- API communication.

Must not:

- query MySQL directly;
- own the core booking state machine.

## API

Owns:

- HTTP;
- routing;
- request/response handling;
- HTTP error translation.

Must not contain the bulk of business logic.

## Services

Own:

- workflows;
- business rules;
- state transitions;
- booking pricing/deposit;
- orchestration.

Must not depend on Razor/Kendo.

## Data

Owns:

- DbContext;
- EF persistence;
- Dapper query implementation;
- database-specific details.

## Models

Own only genuinely shared types.

Do not turn it into a dumping ground.

## Messaging

Own:

- event contracts;
- messaging abstractions;
- RabbitMQ infrastructure.

## Worker

Own:

- background consumers;
- asynchronous secondary work.

---

# 19. Non-negotiable business rules

## Booking workflow

```text
Booked
→ Waiting
→ Washing
→ Cleaning
→ Drying
→ Review
→ Payment
→ Completed
```

Additional:

```text
Drying → AdditionalTreatment → Review
Review → AdditionalTreatment → Review
```

Cancellation:

```text
Booked → Cancelled
```

Only `Booked` may be cancelled.

## Membership

```text
Non-member: 0% discount, 30% deposit
Member:     5% discount, 20% deposit
VIP:       10% discount, 0% mandatory deposit
```

## Payment

- multiple payments allowed;
- payment cannot exceed outstanding balance;
- completion requires fully paid;
- payment status is independent from booking status;
- refund handling is recorded manually.

## Historical integrity

- booking item price is a snapshot;
- membership benefit is a snapshot;
- completed booking is not ordinary CRUD-editable;
- status changes are auditable;
- historical booking/payment information must not be silently rewritten.

## Scheduling

- `ScheduledAt` is informational;
- no sophisticated capacity scheduler;
- simple duplicate-time guard may exist;
- dashboard utilization is analytical.

---

# 20. Progress update protocol

After each meaningful step, update:

```text
Current cycle
Current step
Completed steps
Next step
Last verified build/run state
Important files/classes created or modified
Manual verification performed
Known issues
```

Recommended format:

```text
## Progress Update

Cycle: 1
Step: 6
Status: COMPLETE

Completed:
- ...

Verified:
- dotnet build
- ...

Manual verification:
- ...

Files changed:
- ...

Next:
- Cycle 1 Step 7
```

Never mark a step complete solely because code was typed.

---

# 21. Resume protocol for a new AI/model/session

## First

Read:

```text
context/development-roadmap.md
context/progress.md
```

## Then, depending on the task

```text
Business rules     → business-rules.md
Architecture       → architecture.md
Database           → database.md
API                → api.md
Dashboard          → dashboard.md
Messaging          → messaging.md
Coding guardrails  → development-rules.md
```

## Then

Inspect the actual current code.

The progress file is a checkpoint, not proof that every code file still exactly matches its description.

## Then identify

```text
Current cycle
Current step
Last completed verification
Next smallest action
```

## Never

- restart from scratch;
- introduce later-cycle technologies early without a concrete reason;
- silently change business rules;
- silently change architecture;
- mark a step complete without verification;
- assume a project boundary exists merely because its name appears in documentation.

---

# 22. Package-management documentation note

The current README references `context/package-management.md`.

That file is **not currently part of the known context pack**.

Therefore:

- do not claim it exists;
- do not depend on it for implementation decisions;
- verify the actual repository/package configuration before following any package-management statement.

The README also describes Central Package Management through `Directory.Packages.props`; treat that as a design statement to verify against the actual repository before relying on it.

---

# 23. Definition of done for the entire project

AutoShine is complete when:

- Cycle 0 through Cycle 9 are completed;
- core business rules are implemented and tested;
- MVC/Razor CRUD works;
- service-layer workflows work;
- API boundary works;
- Kendo is meaningfully used;
- multi-project architecture is enforced;
- DTO/validation/mapping is coherent;
- dashboard KPIs reconcile;
- Dapper is used appropriately for reporting;
- RabbitMQ/Worker/Outbox works;
- critical automated tests exist;
- documentation matches final architecture;
- a new developer/model can understand the repository without the original conversation.

---

# 24. Exact resume point

```text
AutoShine
│
├── Cycle 0 ✅
│   └── Design/documentation freeze
│
├── Cycle 1
│   ├── Step 1 ✅
│   ├── Step 2 ✅
│   ├── Step 3 ✅
│   ├── Step 4 ✅
│   ├── Step 5 ✅
│   └── Step 6 ⬅ NEXT
│
└── Cycles 2–9
    planned, not started
```

## Immediate next task

**Cycle 1 — Step 6**

> Implement CRUD operations for **Bookings & Services** in the existing single-project MVC application.

Before implementation:

1. inspect current `Booking`, `BookingItem`, `Service`, and related models;
2. inspect `ApplicationDbContext`;
3. inspect the existing `CustomersController` pattern;
4. do not introduce the Cycle 2 service layer;
5. do not introduce API/Kendo/multi-project architecture yet.

The next step must build on the code that actually exists.

---

# 25. Change log

## v1.0 — Initial operational roadmap

Created as the long-lived development/learning checkpoint.

Based on the existing:

- README;
- `cycles.md`;
- `business-rules.md`;
- `architecture.md`;
- `api.md`;
- `database.md`;
- `dashboard.md`;
- `messaging.md`;
- `development-rules.md`;
- current `progress.md`.

The cycle-level roadmap comes from the existing project design.

The step-level breakdown is an explicit operational decomposition intended to make the project resumable and consistent across models and sessions.