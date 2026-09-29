# AutoShine Dashboard Context

The dashboard is intended for staff/management and answers operational questions, not merely database-count questions.

## Default period

Default view: **Today**.

Supported selectors:

- Today
- This Week
- This Month
- Custom Date Range

## KPI definitions

### Today's bookings

Count of bookings whose `ScheduledAt` falls within the selected period.

### Status counts

Count bookings by current status for the selected period.

### Expected revenue

Sum of current booking totals for non-cancelled bookings in the selected period.

### Collected payments

Sum of successful non-refunded payments whose payment timestamp falls within the selected period.

### Outstanding payments

For relevant bookings:

```text
OutstandingAmount = Booking.Total - valid paid amount
```

A booking with outstanding amount greater than zero contributes to the outstanding figure.

### Monthly revenue

For the selected period, aggregate successfully collected payment amounts, excluding refunded amounts.

### Jobs today

Count of bookings that reached operational stages for the selected period.

### Estimated work hours

Sum of `EstimatedMinutes` for booking items/services associated with the selected bookings, converted to hours.

This is an estimate, not actual labor time.

### Bay utilization

Simple analytical estimate:

```text
Estimated work hours / available bay-hours
```

The application should keep the formula simple and clearly label it as an estimate.

No hard scheduling constraint is derived from this metric.

### Average service duration

Based on operational timestamps captured during the workflow.

This metric may initially use:

```text
WaitingAt → CompletedAt
```

and can be refined after implementation if the time semantics need more precision.

### Service popularity

Count of booking items grouped by service for the selected period.

### Revenue by service

Sum of booking-item totals grouped by service for the selected period.

### Average booking value

```text
Revenue / non-cancelled booking count
```

### New customers

Customers whose first completed booking falls within the selected period.

### Returning customers

Customers with a completed booking in the selected period who had at least one earlier completed booking.

### Active members

Customers whose membership is active on the query date.

### VIP customers

Active customers with VIP membership.

### Cancellation rate

```text
Cancelled bookings / total bookings
```

for the selected period.

## Why Dapper is used here

Dashboard/reporting queries intentionally use Dapper because they are read-oriented SQL aggregations involving joins/grouping/date filtering.

EF Core remains the main data-access path for transactional application behavior.

## UI direction

The dashboard should be readable at a glance:

```text
KPI cards
  ↓
Operational queue/table
  ↓
Revenue + outstanding
  ↓
Service performance
  ↓
Customer/membership metrics
```

Telerik/Kendo components can be used where they improve interaction, while simple Razor/HTML is acceptable for static KPI cards.
