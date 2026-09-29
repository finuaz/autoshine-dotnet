# AutoShine Development Rules

## 1. Learning over speed

The project exists to strengthen practical C#/.NET understanding.

Do not hide important concepts behind generated scaffolding without reviewing what the generated code means.

## 2. Build incrementally

Never create the final architecture in one huge change.

New technology should arrive in the cycle where it has a concrete reason to exist.

## 3. Keep technology boundaries meaningful

Do not use:

- EF Core and Dapper in the same operation without a reason.
- RabbitMQ when synchronous execution is sufficient.
- DMapper where manual mapping is clearer and mapping is trivial.
- Kendo components just for decoration.
- extra projects only to make the folder tree look enterprise-like.

## 4. Server-side authority

The browser is untrusted for business rules.

The service layer/API must enforce:

- valid booking transitions
- payment limits
- membership benefits
- price snapshots
- cancellation rules
- vehicle/booking consistency

## 5. Historical integrity

Do not silently rewrite historical financial data.

Completed bookings are not ordinarily editable.

Booking prices/discounts are snapshots.

Payments are historical records.

Status changes are audited.

## 6. Code style

Prefer explicit, readable C# over clever abstractions.

Before introducing a pattern, understand the problem it solves.

Avoid speculative abstractions.

## 7. Exceptions and validation

Use FluentValidation for input validation.

Use business/application logic for rules that require state or database context.

Use exceptions/results consistently once the API error-handling convention is established.

## 8. Async

Use async APIs for database and network operations.

Do not mechanically add async to purely synchronous in-memory methods.

## 9. LINQ

Learn the difference between:

- LINQ to Objects
- LINQ translated by EF Core
- SQL executed through Dapper

Do not assume they have identical execution characteristics.

## 10. Documentation as guardrail

The `context/` files represent the current design intent.

When implementation changes a business rule or architecture decision, update the relevant context file in the same cycle.

## 11. No premature microservices

The repository may contain multiple projects and a Worker, but the system remains one business application for Repo 2.

Do not split Booking, Payment, or Notification into independent microservices just to use the term.

## 12. Definition of done

A feature is not complete merely because it compiles.

For each cycle, definition of done should include:

- builds successfully
- expected workflow works
- relevant validation exists
- major edge cases considered
- code boundary is understood
- documentation matches reality
