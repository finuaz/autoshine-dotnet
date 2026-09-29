# AutoShine

A small enterprise-style car cleaning and detailing management system built as a learning project for C#/.NET architecture.

> **Important:** AutoShine is intentionally built in several implementation cycles. We will not build the entire architecture in one pass. Each cycle introduces a small architectural step, validates it, and only then moves to the next.

## Purpose

AutoShine is designed to move beyond the basic CRUD patterns learned in Repo 1 and practice a realistic .NET application structure:

- Razor/MVC UI
- jQuery
- Telerik/Kendo UI
- ASP.NET Core Web API
- application/service layer
- FluentValidation
- LINQ
- Entity Framework Core
- Dapper
- DMapper
- MySQL
- RabbitMQ
- background Worker
- multiple `.csproj` projects inside one `.sln`
- DTO boundaries
- audit/history
- automated tests

The goal is not to maximize technology count. Every technology must solve a concrete problem in the application.

## Business overview

AutoShine is an internal system for a car-cleaning/detailing company.

Customers can book a vehicle cleaning service through a front-facing booking flow. Staff manage arrivals, cleaning progress, additional treatments, review, payment, and completion.

### Main business objects

- Customer
- Vehicle
- MembershipPlan
- CustomerMembership
- Service
- Booking
- BookingItem
- Payment
- BookingStatusHistory
- Notification
- OutboxEvent

### Main booking lifecycle

```text
BOOKED
  ↓
WAITING
  ↓
WASHING
  ↓
CLEANING
  ↓
DRYING
  ↓
REVIEW
  ↓
PAYMENT
  ↓
COMPLETED
```

Optional treatment path:

```text
DRYING → ADDITIONAL_TREATMENT → REVIEW
```

Rework discovered during review:

```text
REVIEW → ADDITIONAL_TREATMENT → REVIEW
```

Cancellation is allowed only before the vehicle reaches `WAITING`.

## Membership

There are two paid membership tiers plus non-member status:

- **Member** — 5% discount and 20% booking deposit requirement.
- **VIP** — 10% discount, no mandatory booking deposit, and priority treatment among vehicles currently waiting.
- **Non-member** — no membership discount and 30% booking deposit requirement.

Membership is assigned and renewed manually by staff. Membership has an expiry date and is not automatically renewed.

Membership benefits are snapshotted onto the booking so later membership changes do not alter historical bookings.

## Payment

Payment is separate from booking workflow state.

Booking status answers:

> Where is the vehicle in the cleaning process?

Payment status answers:

> What is the financial state of the booking?

Payment states:

- Unpaid
- PartiallyPaid
- Paid
- Refunded

Rules:

- A booking can have multiple payments.
- Non-members must make a deposit when booking.
- Members have a reduced deposit requirement.
- VIP customers have no mandatory deposit.
- Payments cannot exceed the outstanding balance.
- A booking can reach `COMPLETED` only when fully paid.
- Cancellation refund handling is recorded by staff; the system does not automatically send money to a payment provider.
- Historical booking prices and discounts are immutable financial snapshots.

## Scheduling

Booking time is informational rather than a hard scheduling constraint.

The system will not implement a sophisticated scheduling engine or block bookings based on theoretical capacity.

The dashboard may calculate workload and bay utilization for operational visibility.

## Additional treatment

`Service` is the catalog entity for both normal cleaning services and additional treatments.

Each service has a type:

- `Cleaning`
- `AdditionalTreatment`

An additional treatment discovered during review requires customer approval when it creates an additional charge.

If approved, it is added to the booking and the vehicle enters `ADDITIONAL_TREATMENT`.

If rejected, the existing booking proceeds without that treatment; the recommendation and decision remain in the audit/history trail.

Originally booked services require no additional approval.

## Architecture

AutoShine is a **multi-project application**, not a microservices system.

The repository will contain several .NET projects because each project is a separate build/dependency boundary. They still form one overall business system and are developed together.

Target structure:

```text
AutoShine/
├── src/
│   ├── AutoShine.Web/
│   ├── AutoShine.Api/
│   ├── AutoShine.Services/
│   ├── AutoShine.Data/
│   ├── AutoShine.Models/
│   ├── AutoShine.Messaging/
│   └── AutoShine.Worker/
├── tests/
│   └── AutoShine.Tests/
├── context/
├── docs/
├── README.md
└── AutoShine.sln
```

The exact project creation/splitting happens during later implementation cycles; we do not create every project on day one.

### Runtime/data flow

```text
Browser
  ↓
AutoShine.Web (Razor + jQuery + Telerik/Kendo)
  ↓ HTTP/JSON
AutoShine.Api
  ↓
AutoShine.Services
  ├── EF Core → MySQL       (transactional/application operations)
  ├── Dapper  → MySQL       (dashboard/reporting reads)
  └── Messaging → RabbitMQ  (asynchronous events)
                         ↓
                 AutoShine.Worker
```

### Messaging is not the main transaction path

The booking/payment database transaction must succeed before an application event is considered publishable.

RabbitMQ is used for asynchronous side effects such as simulated customer notifications and other background processing.

Because database commit and message publishing are different systems, AutoShine will learn the Outbox Pattern later in the project.

## Technology responsibilities

| Technology | Responsibility |
|---|---|
| ASP.NET Core MVC | Web application / Razor pages |
| Razor | Server-rendered UI |
| jQuery | Browser-side interaction |
| Telerik/Kendo UI | Data-heavy widgets such as grids/forms |
| ASP.NET Core Web API | HTTP/API boundary |
| Service layer | Application workflow and business logic |
| FluentValidation | Request/input validation |
| LINQ | Collection, in-memory, and EF query composition |
| EF Core | Main transactional/entity data access |
| Dapper | Reporting/read-heavy SQL queries |
| DMapper | Entity ↔ DTO mapping |
| MySQL | Persistent data store |
| RabbitMQ | Asynchronous messaging |
| Worker | RabbitMQ consumers/background processing |
| xUnit | Automated tests |

## Dashboard

The dashboard is a business dashboard, not merely a row count page.

### Operations

- today's bookings
- current counts by booking status
- jobs currently waiting/in progress
- completed and cancelled bookings

### Financial

- expected revenue
- collected payments
- outstanding payments
- monthly revenue

### Workload

- jobs today
- estimated work hours
- bay utilization
- average service duration

### Service performance

- most requested services
- revenue by service
- average booking value

### Customer/membership

- new customers
- returning customers
- active members
- VIP customers
- member/VIP booking and revenue summaries

### Cancellation

- cancelled bookings
- cancellation rate

Dashboard queries are intentionally read-heavy and will use Dapper for the main reporting/query path.

## API principles

The API will expose resource CRUD plus explicit workflow actions where appropriate.

Examples:

```text
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
DELETE /api/customers/{id}

GET    /api/vehicles
GET    /api/services
GET    /api/bookings
GET    /api/bookings/{id}
POST   /api/bookings
PUT    /api/bookings/{id}

POST   /api/bookings/{id}/arrive
POST   /api/bookings/{id}/start-washing
POST   /api/bookings/{id}/start-cleaning
POST   /api/bookings/{id}/start-drying
POST   /api/bookings/{id}/add-treatment
POST   /api/bookings/{id}/review
POST   /api/bookings/{id}/approve-treatment
POST   /api/bookings/{id}/reject-treatment
POST   /api/bookings/{id}/request-payment
POST   /api/bookings/{id}/complete
POST   /api/bookings/{id}/cancel
```

Endpoint naming can be refined during API design; these are the current business intents, not an immutable final URL list.

## Documentation/context

The `context/` directory is a guardrail for future work. It is useful even without AI-assisted IDE integration because it records the intended boundaries and decisions.

- `business-rules.md` — definitive business behavior
- `architecture.md` — project boundaries and dependency rules
- `database.md` — entity model and data rules
- `api.md` — API boundary and contract principles
- `dashboard.md` — KPI definitions and reporting expectations
- `messaging.md` — RabbitMQ/event/Outbox design
- `cycles.md` — implementation sequence and learning goals
- `development-rules.md` — project-level coding/learning guardrails

## Build in cycles

AutoShine is explicitly developed incrementally.

```text
Cycle 0  → documentation + design freeze
Cycle 1  → single-project CRUD foundation
Cycle 2  → service layer + business workflow
Cycle 3  → Web API + UI/API boundary
Cycle 4  → Telerik/Kendo UI
Cycle 5  → multi-project solution structure
Cycle 6  → DTO + FluentValidation + DMapper
Cycle 7  → dashboard + Dapper reporting
Cycle 8  → RabbitMQ + Worker + Outbox
Cycle 9  → testing, hardening, cleanup
```

Each cycle should end with a working, testable system and a review of the relevant context files before moving forward.

## Development rule

**Do not skip directly to the final architecture.**

The educational value of Repo 2 comes from evolving the system and seeing why each layer exists.

## Current status

- [x] Business direction agreed
- [x] Main workflow agreed
- [x] Membership concept agreed
- [x] Payment separation agreed
- [x] RabbitMQ purpose agreed
- [x] Multi-project vs microservice distinction agreed
- [x] Documentation/context-first rule agreed
- [x] Build-in-cycles rule agreed
- [ ] Project implementation started

Implementation begins only after this documentation pack is reviewed and accepted as the baseline.
