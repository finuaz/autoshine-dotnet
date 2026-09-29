# AutoShine Architecture

## 1. Architectural style

AutoShine is a **single business system implemented as a multi-project .NET solution with a separate background worker and asynchronous messaging**.

It is **not** being presented as microservices.

### Multi-project means

Several `.csproj` projects live in the same Git repository and are coordinated by one `.sln`:

```text
AutoShine.sln
├── AutoShine.Web.csproj
├── AutoShine.Api.csproj
├── AutoShine.Services.csproj
├── AutoShine.Data.csproj
├── AutoShine.Models.csproj
├── AutoShine.Messaging.csproj
└── AutoShine.Worker.csproj
```

These projects form dependency/build boundaries while remaining one overall application/system.

### Microservices would mean

Independent business services that can be built, deployed, scaled, and evolved separately, commonly with independently owned boundaries and often independently owned data stores.

Repo 2 does not require that complexity.

## 2. Target projects

### AutoShine.Web

Owns:

- Razor views
- jQuery
- Telerik/Kendo UI integration
- browser-side UI behavior
- calling the API

Must not directly query MySQL.

### AutoShine.Api

Owns:

- HTTP routing
- request/response handling
- authorization boundary if added later
- API error/status translation

Must not contain the bulk of business logic.

### AutoShine.Services

Owns:

- application workflows
- business rules
- status transitions
- booking pricing/deposit calculation
- orchestration of data operations
- calling validation
- initiating application events

Must not know Razor or Kendo details.

### AutoShine.Data

Owns:

- EF Core DbContext
- entity persistence configuration
- Dapper query implementation
- database-specific concerns

EF Core is preferred for transactional application operations.
Dapper is preferred for dashboard/reporting reads.

### AutoShine.Models

Owns shared model/contracts that genuinely need to cross project boundaries:

- entities where appropriate
- enums
- DTOs/request/response contracts

Avoid putting arbitrary helpers here just because multiple projects can reference them.

### AutoShine.Messaging

Owns:

- event contracts
- messaging abstractions
- RabbitMQ-specific integration details that should not be scattered through the codebase

### AutoShine.Worker

Owns:

- background execution
- RabbitMQ consumers
- asynchronous secondary work such as simulated notifications

## 3. Dependency direction

Target dependency shape:

```text
Web → API contracts / HTTP boundary
API → Services
Services → Models + Data abstractions + Messaging abstractions
Data → Models
Messaging → event contracts / RabbitMQ infrastructure
Worker → Messaging + Models where required
```

The exact references will be introduced incrementally during the relevant cycle.

## 4. Service layer rationale

The service layer is used because application workflows such as booking creation, status transitions, payment settlement, and dashboard orchestration cross multiple entities and rules.

Alternatives such as controller-heavy logic, rich domain objects, repository-only logic, or CQRS handlers are valid architectural styles, but Repo 2 uses an application/service layer because it is appropriate to the target learning goal and matches the intended workplace-style structure.

## 5. Entity vs DTO

Persistence entities are not automatically API contracts.

Example:

```text
Booking entity
    ↓ DMapper
BookingResponse DTO
    ↓
API response
```

Use separate request models when input shape and validation needs differ from response shape.

## 6. DMapper boundary

DMapper is primarily used at model boundaries, especially:

- entity → response DTO
- request DTO → entity/application model

Do not introduce mapping everywhere merely to demonstrate the mapper.

## 7. Validation

FluentValidation is used for request/input validation.

Business rules that depend on database state or workflow state remain the responsibility of the service/domain logic.

Example:

```text
FluentValidation
→ Quantity > 0

BookingService
→ Cannot complete a booking unless it is in Payment
```

## 8. EF Core vs Dapper

### EF Core

Use for:

- create/update/delete operations
- entity-oriented transactions
- normal application workflows
- relationship-aware persistence

### Dapper

Use for:

- dashboard queries
- aggregation/reporting
- read-heavy SQL where explicit SQL is clearer

This distinction is intentional and educational.

## 9. RabbitMQ boundary

RabbitMQ is not the source of truth for booking/payment state.

The database remains authoritative for transactional state.

RabbitMQ is used for asynchronous events and secondary processing.

## 10. Outbox

When transactional work produces an event:

```text
DB transaction
 ├── business data
 └── OutboxEvent
        ↓ commit
background publisher
        ↓
RabbitMQ
```

The outbox prevents a successful database transaction from silently losing its event because RabbitMQ was temporarily unavailable.

## 11. UI/API boundary

The UI communicates with the API through HTTP/JSON.

The UI should not bypass the API to reach EF Core or the MySQL database.

## 12. Implementation principle

The final target architecture is built through refactoring cycles, not generated in one large step.

Every architectural boundary should have an example in the codebase that demonstrates why that boundary exists.
