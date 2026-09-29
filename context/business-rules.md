# AutoShine Business Rules

This file is the business guardrail. Code must implement these rules rather than inventing new behavior ad hoc.

## 1. Customer

- A customer may own multiple vehicles.
- A booking must reference a vehicle.
- A vehicle belongs to exactly one customer during its lifetime in this system.
- Vehicle ownership transfer is not supported.
- Customer deletion must not destroy historical booking/payment history.

## 2. Vehicle

- Plate number is unique.
- The same active plate cannot exist in two vehicle records.
- Vehicle data is maintained as master data; historical booking snapshots remain authoritative for historical transactions where applicable.

## 3. Service catalog

Each catalog item has:

- name
- description
- type
- current price
- estimated minutes
- active/inactive flag

Service types:

- Cleaning
- AdditionalTreatment

An inactive catalog item may still be referenced by historical booking items, but it should not be offered as a new selectable service unless the business explicitly reactivates it.

Its historical price remains preserved on booking items.

## 4. Booking lifecycle

### States

```text
Booked
Waiting
Washing
Cleaning
Drying
AdditionalTreatment
Review
Payment
Completed
Cancelled
```

### Meaning

- **Booked** — appointment created through the customer-facing booking flow; vehicle has not yet arrived.
- **Waiting** — vehicle has physically arrived at the garage and is ready for treatment.
- **Washing** — washing stage is in progress.
- **Cleaning** — cleaning/detailing stage is in progress.
- **Drying** — drying stage is in progress.
- **AdditionalTreatment** — an additional paid treatment is being performed.
- **Review** — staff checks the finished result and may accept it or request more work.
- **Payment** — final payment is being completed.
- **Completed** — work is accepted and payment is fully settled.
- **Cancelled** — booking ended before the vehicle entered the operational flow.

### Normal transitions

```text
Booked → Waiting
Waiting → Washing
Washing → Cleaning
Cleaning → Drying
Drying → Review
Review → Payment
Payment → Completed
```

### Additional treatment transitions

```text
Drying → AdditionalTreatment → Review
Review → AdditionalTreatment → Review
```

`Review → AdditionalTreatment → Review` may happen more than once when further rework is required.

### Cancellation

- Cancellation is allowed only from `Booked`.
- Cancellation is not allowed from `Waiting` or later.
- The cancellation reason must be recorded.
- Refund handling is manual; the system records refund transactions but does not integrate with a payment provider in Repo 2.

### Status transitions are server-owned

The browser may request a transition, but the service layer must validate whether that transition is allowed.

The UI must never be treated as authoritative for workflow rules.

## 5. Booking time

- Scheduled time is informational.
- AutoShine does not implement a hard capacity-based scheduler.
- A basic guard may prevent duplicate bookings for the same vehicle at the exact same scheduled time.
- Capacity is measured for dashboard/reporting purposes only.
- A booking may be rescheduled while it is still `Booked`.
- After `Waiting`, schedule changes are not treated as ordinary rescheduling.

## 6. Booking items and price snapshots

Every booking item stores the price that was agreed at booking/addition time.

```text
Service.CurrentPrice
       ↓ snapshot
BookingItem.UnitPrice
```

A later catalog price change must not modify historical booking totals.

A membership discount applied to a booking is also snapshotted and must not be recalculated from the customer's current membership later.

## 7. Adding/removing services

- Originally selected services are part of the booking without additional approval.
- Staff may add/remove eligible services before the booking reaches `Payment`.
- Financial information is considered locked once final payment processing starts.
- A newly discovered paid treatment during `Review` requires customer approval before it becomes billable.
- If a proposed treatment is rejected, the rejection remains visible in the audit trail and the existing booking may continue.

## 8. Membership

Membership states:

```text
Non-member
Member
VIP
```

Only Member and VIP are membership tiers.

### Member

- 5% discount
- 20% required booking deposit
- membership-only promotions may be modeled later, but are not required for Repo 2

### VIP

- 10% discount
- 0% mandatory booking deposit
- priority treatment among vehicles currently in `Waiting`

VIP priority must not interrupt a vehicle that is already being washed/cleaned/etc.

### Membership administration

- Staff manually assigns membership.
- Membership has `StartedAt` and `ExpiresAt`.
- Membership is not automatically renewed.
- Membership renewal is a staff action.
- Membership benefits are captured on the booking at booking/price calculation time.
- A membership expiring after booking creation does not change that booking's price.

## 9. Deposit rules

Deposit is calculated from the discounted booking total.

Example:

```text
Subtotal = 300,000
Member discount = 15,000
Total = 285,000
Required deposit = 20% of 285,000 = 57,000
```

Rules:

- Non-member: 30% deposit.
- Member: 20% deposit.
- VIP: 0% mandatory deposit.
- Customer may voluntarily pay more than the minimum deposit as long as total paid does not exceed outstanding balance.
- Payment cannot exceed outstanding balance.

## 10. Payment

Payment is independent of booking status.

Payment status:

```text
Unpaid
PartiallyPaid
Paid
Refunded
```

Calculated values:

```text
PaidAmount = sum of valid successful payments
OutstandingAmount = Booking.Total - PaidAmount
```

`Paid` means outstanding amount is zero.

The booking may enter `Payment` only after review is accepted.

The booking can reach `Completed` only when `PaymentStatus == Paid`.

A booking may have multiple payment records.

## 11. Completed bookings

For Repo 2, completed bookings are **not editable through ordinary booking CRUD**.

Corrections to historical records are out of scope for the normal UI.

This keeps financial/operational history simple and trustworthy.

## 12. Audit/history

Every booking status change must create a `BookingStatusHistory` record containing at least:

- booking id
- previous status
- new status
- changed timestamp
- actor/staff identifier when available
- optional note/reason

Treatment approval/rejection and cancellation reason should also be auditable.
