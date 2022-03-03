# 2. Use Messaging Library

Date: 2021-07-13

## Status

Accepted

## Context

With more nad more use of messaging and a message broker, we've noticed a set of patterns
that have custom implementation across various services/boundaries. The Outbox Pattern,
Idempotent Consumers, Claim Check, Retries, etc.

Should we implement these patterns ourselves or use a full-featured messaging library
such as Brighter, MassTransit, or NServiceBu?

## Decision

Use a 3rd party messaging library that implements the required patterns instead of implementing the
patterns ourselves.  Most libraries are opinionated and will give a consistent usage for various
applied patterns

## Consequences

- Changing transports will be less costly as the messaging library provides an abstraction over the
underlying transport.
- It will force a consistent way of using various patterns per the opinionated way the library requires it.
- Keeping up with major version releases (breaking changes) cadence will require additional effort.
