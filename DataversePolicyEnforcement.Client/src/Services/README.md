# Services folder

## Overview

The `src/Services` folder contains the client-side service abstraction used by form handlers to fetch policy decisions. Services encapsulate external interactions (e.g., `Xrm.WebApi`) so the form logic remains testable.

## Files

- `IPolicyService.ts` — TypeScript interface describing the shape of the policy service (methods return `AttributePolicyDecision[]`).
- `PolicyService.ts` — concrete implementation that currently simulates async responses; it accepts `Xrm.WebApi` in the constructor and exposes `getDecisionsAsync` and `getAttributeDecisionsAsync`.
- `index.ts` — re-exports the public service classes.

## Behavior

- `PolicyService` methods return promises of `AttributePolicyDecision[]` and are used by `onLoad` to retrieve decisions for affected attributes.
- The production implementation should call an API (via `Xrm.WebApi`) to retrieve policy decisions; the current implementation contains simple simulated/dummy responses and delays to allow local testing.

## Usage

- Construct the service with `new PolicyService(Xrm.WebApi)` inside `onLoad` and call `getAttributeDecisionsAsync` from attribute change handlers.

## Testing

- Prefer injecting a mock `IPolicyService` when unit testing `onLoad` logic. The small interface makes mocking straightforward.
- Add unit tests in `tests` that assert correct calls and that UI updates are applied given various `AttributePolicyDecision` responses.

## Notes

- Keep service logic limited to data retrieval and mapping; do not apply UI changes here — return structured `AttributePolicyDecision` objects for callers to act on.
