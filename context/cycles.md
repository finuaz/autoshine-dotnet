# AutoShine Implementation Cycles

AutoShine must be built in several cycles. Do not implement the final architecture in one pass.

## Cycle 0 — Design/documentation freeze

### Deliverable

- README.md
- context files
- agreed business rules
- entity/relationship specification
- architecture baseline

### Goal

Remove ambiguity before code exists.

### Exit criteria

- Business flow is understood.
- Main entities are understood.
- Project boundaries are understood.
- Dashboard responsibilities are understood.
- RabbitMQ purpose is understood.
- Build order is accepted.

---

## Cycle 1 — Single-project CRUD foundation

### Deliverable

A small working AutoShine application with simple CRUD, intentionally starting close to Repo 1.

Suggested scope:

- Customer CRUD
- Vehicle CRUD
- Service CRUD
- MySQL
- EF Core
- basic Razor UI

### Learning goal

Refresh Repo 1 patterns while using the new domain.

### Do not add yet

- RabbitMQ
- Dapper
- multi-project split
- Kendo-heavy UI
- complex DTO architecture

### Exit criteria

CRUD works and is manually verified.

---

## Cycle 2 — Service layer + booking workflow

### Deliverable

Introduce:

```text
Controller → Service → EF Core
```

Implement booking creation and status transitions.

Add:

- LINQ
- business validation
- booking calculations
- membership price/deposit rules
- booking status history

### Learning goal

Understand why application logic should not be concentrated in controllers.

### Exit criteria

A booking can move correctly through the workflow, and invalid transitions are rejected server-side.

---

## Cycle 3 — Web API + UI/API boundary

### Deliverable

Introduce an ASP.NET Core Web API.

Move the browser-facing data operations toward:

```text
UI → HTTP → API → Service → EF Core
```

### Learning goal

Understand controllers vs API controllers, HTTP verbs, JSON, request/response contracts, and separation of concerns.

### Exit criteria

At least one complete CRUD path is driven through the API.

---

## Cycle 4 — Telerik/Kendo UI

### Deliverable

Introduce Telerik/Kendo components, starting with the most data-heavy screen: Bookings.

Focus:

- Grid
- AJAX/data source
- sorting
- filtering
- paging
- editing
- lookup/dropdown controls

### Learning goal

Understand how Razor/jQuery-based UI talks to an API and how Kendo consumes remote data.

### Exit criteria

The booking/customer/service grid can load and perform agreed operations through the API.

---

## Cycle 5 — Multi-project solution

### Deliverable

Refactor the working application into multiple `.csproj` projects inside one `.sln`.

Target structure:

```text
Web
Api
Services
Data
Models
Messaging
Worker
```

The Worker may be created here or in Cycle 8 depending on implementation readiness.

### Learning goal

Understand:

- `.csproj`
- `.sln`
- ProjectReference
- dependency direction
- library vs executable project
- build boundaries

### Exit criteria

The application still runs while the compiler enforces the intended project boundaries.

---

## Cycle 6 — DTO + FluentValidation + DMapper

### Deliverable

Introduce explicit request/response models and mapping.

Example:

```text
Entity → DMapper → Response DTO
Request DTO → validation → service/application model
```

### Learning goal

Understand why an API contract should not simply expose database entities.

### Exit criteria

At least Customers/Bookings use DTOs with FluentValidation and DMapper in a coherent way.

---

## Cycle 7 — Dashboard + Dapper

### Deliverable

Build the staff dashboard and reporting queries.

Introduce:

- dashboard service/query abstraction
- Dapper
- SQL aggregation
- date-range filtering
- KPI calculations
- Kendo grids/charts where useful

### Learning goal

Understand the difference between transactional EF Core work and reporting-oriented SQL reads.

### Exit criteria

Dashboard KPIs reconcile with database data for known test scenarios.

---

## Cycle 8 — RabbitMQ + Worker + Outbox

### Deliverable

Introduce:

- RabbitMQ
- event contracts
- OutboxEvent
- background publisher
- Worker consumers
- Notification records

Initial events:

- PaymentReceived
- BookingCompleted

### Learning goal

Understand asynchronous messaging, producer/consumer roles, retries, and the DB/message consistency problem.

### Exit criteria

A successful business transaction can produce an outbox event, publish it, and be consumed by the Worker. A simulated broker failure leaves the event retryable.

---

## Cycle 9 — Testing + hardening

### Deliverable

Add meaningful automated tests and cleanup.

Focus:

- service/business-rule tests
- API behavior tests where useful
- edge cases
- validation
- workflow transitions
- payment calculations
- dashboard reconciliation

### Learning goal

Turn the project from a feature demo into a maintainable learning system.

### Exit criteria

Critical business rules have automated coverage and the repository is understandable without conversational history.

---

## Cycle discipline

At the end of every cycle:

1. Run/build the system.
2. Test the cycle's behavior.
3. Review the relevant context file(s).
4. Update README/context when the design has legitimately changed.
5. Commit/tag the cycle.
6. Only then start the next cycle.

A later cycle may refactor an earlier implementation. That is intentional.
