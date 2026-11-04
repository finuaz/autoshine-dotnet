# AutoShine Database Model

Database: MySQL

EF Core is the primary persistence technology. Dapper is used for selected read/reporting queries.

## Core entities

### Customer

Suggested fields:

- Id
- Name
- Phone
- Email
- Address
- Archive/deactivation state (exact representation to be decided; e.g. `IsActive` or `ArchivedAt`)
- CreatedAt
- UpdatedAt

Customers are archived/deactivated rather than physically deleted. Archival must preserve their vehicles and all related booking, payment and status history. Archived customers are not offered for new work; reactivation and the exact archive field remain workflow/schema decisions.

### Vehicle

- Id
- CustomerId
- PlateNumber
- Brand
- Model
- Year
- Color
- CreatedAt
- UpdatedAt

Constraints:

- PlateNumber unique.
- CustomerId required.

### MembershipPlan

- Id
- Name
- DiscountPercentage
- DepositPercentage
- IsVip
- IsActive

Initial plans:

```text
Member: 5% discount, 20% deposit
VIP:    10% discount, 0% deposit
```

### CustomerMembership

- Id
- CustomerId
- MembershipPlanId
- StartedAt
- ExpiresAt
- Status

This preserves membership history.

### Service

- Id
- Name
- Description
- ServiceType
- Price
- EstimatedMinutes
- IsActive
- CreatedAt
- UpdatedAt

`ServiceType`:

```text
Cleaning
AdditionalTreatment
```

### Booking

- Id
- VehicleId
- ScheduledAt
- Status
- SubTotal
- DiscountAmount
- DiscountPercentageSnapshot
- Total
- RequiredDepositAmount
- MembershipPlanSnapshot / tier data as appropriate
- Notes
- CreatedAt
- UpdatedAt
- CompletedAt nullable
- CancelledAt nullable
- CancellationReason nullable

The booking's customer is determined through its required Vehicle relationship (`Booking.Vehicle.Customer`); Booking must not have a redundant `CustomerId`. The booking stores financial snapshots that must not change when catalog or membership data changes later.

### BookingItem

- Id
- BookingId
- ServiceId (required, non-nullable)
- ServiceNameSnapshot
- ServiceTypeSnapshot
- Quantity
- UnitPrice
- Total
- Source
- ApprovalStatus / approval metadata as designed

`Source` distinguishes originally booked items from items added as additional treatment.

Services may be deactivated to prevent selection for new bookings, but a service referenced by a booking item must not be physically deleted. Configure the service foreign key to restrict/no-action deletion. Retain the service identity and snapshots for historical booking items.

### Payment

- Id
- BookingId
- Amount
- PaymentMethod
- Status
- PaidAt
- Reference
- CreatedAt

Payments are append-only financial records in the normal application flow. Refunds should be represented without rewriting historical paid amounts.

### BookingStatusHistory

- Id
- BookingId
- FromStatus
- ToStatus
- ChangedAt
- ChangedBy
- Note

### Notification

- Id
- CustomerId
- BookingId
- Type
- Message
- Status
- CreatedAt
- ProcessedAt nullable

This is initially a simulated notification store for the Worker exercise.

### OutboxEvent

- Id
- EventType
- AggregateType
- AggregateId
- Payload
- CreatedAt
- PublishedAt nullable
- RetryCount
- ErrorMessage nullable

## Relationships

```text
Customer 1 ──── * Vehicle
Customer 1 ──── * CustomerMembership
MembershipPlan 1 ──── * CustomerMembership
Vehicle 1 ──── * Booking
Booking 1 ──── * BookingItem
Service 1 ──── * BookingItem
Booking 1 ──── * Payment
Booking 1 ──── * BookingStatusHistory
Booking 1 ──── * Notification
```

Customer-to-booking access is through `Booking.Vehicle.Customer`, not a direct Booking-to-Customer relationship.

## Important historical-data rule

Booking data is a historical transaction.

At booking/item creation time, store the information needed to preserve the agreed commercial terms:

- service name snapshot
- service type snapshot
- unit price snapshot
- discount snapshot
- membership tier/benefit snapshot
- total/deposit values

Do not recompute historical booking totals using current master data.
Preserve booking, payment and booking-status history; customer archival and service deactivation must not remove those records.

## Data-access strategy

### EF Core examples

- create customer
- create/update vehicle
- create booking
- add booking item
- record payment
- transition booking status

### Dapper examples

- today's KPI summary
- service popularity
- revenue by service
- outstanding payments
- monthly revenue
- average service duration
- customer/member reporting

## Database simplicity rules

- Avoid excessive normalization where it adds no learning value.
- Avoid complex scheduling tables.
- Avoid inventory, employees, branches, suppliers, and real payment gateways.
- Preserve historical booking facts explicitly instead of relying only on mutable master tables.
