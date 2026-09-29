# AutoShine Messaging Context

RabbitMQ is included to teach asynchronous communication and background processing. It is not a replacement for MySQL and does not make AutoShine a microservices system.

## Initial events

Keep the first version small:

- `PaymentReceived`
- `BookingCompleted`

A later cycle may add `BookingCreated` if the use case proves useful.

## Example flow

```text
PaymentService
   ↓
DB transaction
   ├── Payment record
   ├── booking/payment state update
   └── OutboxEvent
          ↓ commit
Outbox publisher
   ↓
RabbitMQ
   ↓
AutoShine.Worker
   ↓
Notification
```

## Worker responsibility

Initial Worker purpose: simulated customer notifications.

Example:

```text
PaymentReceived
→ create Notification row

BookingCompleted
→ create Notification row
```

No external SMS/WhatsApp/email provider is required in Repo 2.

## Why RabbitMQ is not used for everything

The user-facing transaction should remain synchronous where the result is required immediately.

Example:

```text
Complete booking
→ validate
→ update booking
→ save transaction
→ return success
```

Secondary work can happen asynchronously:

```text
publish event
→ Worker
→ notification/reporting side effect
```

## RabbitMQ vocabulary to learn

During the messaging cycle, explicitly learn:

- producer
- consumer
- queue
- exchange
- routing key
- message
- acknowledgment
- retry
- dead-letter/failure strategy at a conceptual level

We will keep RabbitMQ topology simple rather than introducing an elaborate enterprise messaging design.

## Failure scenario

Important case:

```text
DB commit succeeds
RabbitMQ publish fails
```

The Outbox Event must remain pending so the publisher can retry later.

## Outbox

`OutboxEvent` lives in the same database transaction as the business state change.

A background publisher reads pending outbox events and attempts publication.

On success:

```text
PublishedAt = timestamp
```

On failure:

```text
RetryCount++
ErrorMessage = latest failure
```

The exact retry/backoff strategy can remain simple for Repo 2.
