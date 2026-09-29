# AutoShine API Context

The API is the boundary between the Web UI and application services.

## Principles

- HTTP concerns live in API controllers.
- Business rules do not live in controllers.
- Incoming requests are validated before application execution.
- API responses use DTOs/contracts, not raw database entities by default.
- Workflow actions are exposed explicitly when an operation represents a business action rather than generic CRUD.

## Resource areas

- customers
- vehicles
- services
- memberships
- bookings
- payments
- dashboard

## CRUD examples

```text
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
DELETE /api/customers/{id}
```

```text
GET    /api/vehicles
GET    /api/vehicles/{id}
POST   /api/vehicles
PUT    /api/vehicles/{id}
DELETE /api/vehicles/{id}
```

```text
GET    /api/services
GET    /api/services/{id}
POST   /api/services
PUT    /api/services/{id}
```

Ordinary deletion/deactivation rules are governed by the business model. Historical services should not be destroyed in ways that break history.

## Booking workflow actions

Candidate actions:

```text
POST /api/bookings/{id}/arrive
POST /api/bookings/{id}/start-washing
POST /api/bookings/{id}/start-cleaning
POST /api/bookings/{id}/start-drying
POST /api/bookings/{id}/add-treatment
POST /api/bookings/{id}/review
POST /api/bookings/{id}/approve-treatment
POST /api/bookings/{id}/reject-treatment
POST /api/bookings/{id}/request-payment
POST /api/bookings/{id}/complete
POST /api/bookings/{id}/cancel
```

The final endpoint names may be simplified during implementation, but the underlying business actions remain explicit.

## Dashboard endpoints

Prefer purpose-oriented endpoints rather than making the browser reproduce reporting logic.

Examples:

```text
GET /api/dashboard/overview
GET /api/dashboard/operations
GET /api/dashboard/revenue
GET /api/dashboard/services
GET /api/dashboard/customers
GET /api/dashboard/memberships
GET /api/dashboard/outstanding-payments
```

Dapper-backed query services should power most of these read endpoints.

## Error behavior

The API should distinguish at least:

- validation failure
- not found
- invalid business transition
- conflict (for unique plate or duplicate booking-time cases)
- unexpected server/database failure

The exact response envelope can be standardized during the API cycle.

## API/UI rule

Kendo/jQuery sends requests to the API. It must not know EF Core details, database structure, or service-layer internals.
